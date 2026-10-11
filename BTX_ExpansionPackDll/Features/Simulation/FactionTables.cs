using FullXotlTables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Features.Simulation
{
    internal class FactionTables
    {
        /// <summary>
        /// Generates a custom faction table from CSV files in a specified folder.
        /// </summary>
        /// <remarks>
        /// Same logic as BEX except that CSVs can have the Battle Value (BV) as the first column for sorting.
        /// </remarks>
        public static XotlTable GenerateFromCustomFolder(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath, "*.csv");
            var customTables = new XotlTable();
            foreach (string file in files)
            {
                var factionTable = new FactionTable();
                string key = null;
                using (var sr = new StreamReader(file))
                {
                    if (sr != null)
                    {
                        string[] firstLine = sr.ReadLine().Split(',');
                        bool hasBV = firstLine.Length > 1 && (string.IsNullOrEmpty(firstLine[0]) || firstLine[0] == "BV" || int.TryParse(firstLine[0], out _));
                        key = hasBV ? firstLine[1] : firstLine[0];

                        string[] ReadCsvLine()
                        {
                            string[] parts = sr.ReadLine().Split(',');
                            return hasBV && parts.Length > 1 ? [.. parts.Skip(1)] : parts;
                        }

                        string[] line = ReadCsvLine();
                        while (!sr.EndOfStream)
                        {
                            line = ReadCsvLine();
                            if (line[0].StartsWith("Collection:"))
                            {
                                factionTable.Collection.Add(line[0].Split(':')[1], ReadWeightLine(line));
                            }
                            else if (line[0].StartsWith("Salvage:"))
                            {
                                factionTable.Salvage.Add(line[0].Split(':')[1], ReadWeightLine(line));
                            }
                            else if (line[0].Contains("Dates"))
                            {
                                for (int index = 1; index < line.Length; index += 2)
                                    factionTable.Dates.Add(DateTime.Parse(line[index]));
                            }
                            else if (line[0].Contains("Lights"))
                            {
                                while (!sr.EndOfStream && !line[0].Contains("Mediums"))
                                {
                                    line = sr.ReadLine().Split(',');
                                    if (line[0] != "" && !line[0].Contains("Mediums"))
                                        factionTable.Mechs.Lights.Add(line[0], ReadWeightLine(line));
                                }
                                while (!sr.EndOfStream && !line[0].Contains("Heavies"))
                                {
                                    line = sr.ReadLine().Split(',');
                                    if (line[0] != "" && !line[0].Contains("Heavies"))
                                        factionTable.Mechs.Mediums.Add(line[0], ReadWeightLine(line));
                                }
                                while (!sr.EndOfStream && !line[0].Contains("Assaults"))
                                {
                                    line = sr.ReadLine().Split(',');
                                    if (line[0] != "" && !line[0].Contains("Assaults"))
                                        factionTable.Mechs.Heavies.Add(line[0], ReadWeightLine(line));
                                }
                                while (!sr.EndOfStream)
                                {
                                    line = sr.ReadLine().Split(',');
                                    if (line[0] != "")
                                        factionTable.Mechs.Assaults.Add(line[0], ReadWeightLine(line));
                                }
                            }
                        }
                    }
                }

                if (key != null)
                    customTables.Factions.Add(key, factionTable);
            }

            return customTables;
        }

        private static List<WeightValue> ReadWeightLine(string[] lineToRead)
        {
            List<WeightValue> weightValueList = [];
            for (int index = 1; index < lineToRead.Length; index += 2)
            {
                var weightValue = new WeightValue
                {
                    Value = int.Parse(lineToRead[index])
                };
                if (index != 1 && lineToRead[index - 1] != "")
                {
                    if (lineToRead[index - 1].StartsWith("Jump:"))
                        weightValue.StartIsJumpValue = true;
                    weightValue.StartDate = DateTime.Parse(lineToRead[index - 1].Replace("Jump:", ""));
                    weightValue.HasStart = true;
                }
                if (index != lineToRead.Length - 1 && lineToRead[index + 1] != "")
                {
                    if (lineToRead[index + 1].StartsWith("Jump:"))
                        weightValue.StopIsJumpValue = true;
                    weightValue.StopDate = DateTime.Parse(lineToRead[index + 1].Replace("Jump:", ""));
                    weightValue.HasStop = true;
                }
                weightValueList.Add(weightValue);
            }
            return weightValueList;
        }
    }

    /// <summary>
    /// Skips vehicle rarity doubling and shifts salvage requests ten years earlier.
    /// </summary>
    /// <remarks>
    /// BEX uses fixed 3028 and 3025 dates for salvage rolls. Shifting both relative to the request date allows newer units in initial and chained salvage rolls.
    /// </remarks>
    [HarmonyPatch(typeof(XotlTable), "RequestUnit")]
    public static class XotlTable_RequestUnit
    {
        private static DateTime GetSalvageDate(DateTime currentDate) => currentDate.AddYears(-10);

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            // Skip the logic that doubles the rarity of certain vehicles.
            var matcher = new CodeMatcher(instructions, il)
                .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "vehicledef_APC_Maxim_3052AP"))
                .MatchBack(false, new CodeMatch(i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S));

            object jumpTarget = matcher.Operand;
            var patchedInstructions = matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Br, jumpTarget))
                .InstructionEnumeration();

            // Patch both salvage dates to use the request date minus ten years.
            int[] salvageYears = [3025, 3028];
            var currentInstructions = patchedInstructions;

            foreach (int year in salvageYears)
            {
                var salvageDateMatcher = new CodeMatcher(currentInstructions, il)
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Ldc_I4 && Equals(i.operand, year)),
                        new CodeMatch(OpCodes.Ldc_I4_1),
                        new CodeMatch(OpCodes.Ldc_I4_1),
                        new CodeMatch(i => i.opcode == OpCodes.Newobj &&
                            i.operand is ConstructorInfo constructor &&
                            constructor.DeclaringType == typeof(DateTime)));

                currentInstructions = salvageDateMatcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
                    .RemoveInstructions(3)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XotlTable_RequestUnit), nameof(GetSalvageDate))))
                    .InstructionEnumeration();
            }

            return currentInstructions;
        }
    }

    /// <summary>
    /// Clamps the current date to the first table date to avoid negative weights.
    /// </summary>
    /// <remarks>
    /// This is a workaround for a bug in BEX that causes InterpolateWeight() to return a negative fraction when called with a date earlier than the first table date.
    /// </remarks>
    [HarmonyPatch(typeof(WeightedContainer), "InterpolateWeight")]
    public static class WeightedContainer_InterpolateWeight
    {
        [HarmonyPrefix]
        public static void Prefix(ref DateTime currentDate, List<DateTime> dates)
        {
            if (dates.Count > 0 && currentDate < dates[0])
                currentDate = dates[0];
        }
    }
}