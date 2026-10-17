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


        {


        /// <summary>
        /// Various patches to the XotlTable.RequestUnit method:
        /// - Renames locals tables with the "F" affix for consistency with other faction tables.
        /// - Skips vehicle rarity doubling so we can use A tables where available.
        /// - Replaces salvage dates to allow newer units in salvage rolls.
        /// </summary>
        [HarmonyPatch(typeof(XotlTable), "RequestUnit")]
        public static class XotlTable_RequestUnit
        {
            private static DateTime GetSalvageDate(DateTime currentDate) => currentDate.AddYears(-10);

            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions, il);

                // 1. Rename locals tables: change "Locals" to "F" in the EndsWith check
                matcher.MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Ldstr && i.operand is string s && s == "Locals"),
                        new CodeMatch(OpCodes.Callvirt, typeof(string).GetMethod("EndsWith", [typeof(string)])))
                       .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldstr, "F"))
                       .MatchForward(false, new CodeMatch(i => i.LoadsConstant(6)))
                       .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4, 1));

                // 2. Skip vehicle rarity doubling: replace conditional branch with unconditional jump
                matcher.MatchForward(false, new CodeMatch(OpCodes.Ldstr, "vehicledef_APC_Maxim_3052AP"))
                       .MatchBack(false, new CodeMatch(i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S));

                object jumpTarget = matcher.Operand;
                matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))
                       .InsertAndAdvance(new CodeInstruction(OpCodes.Br, jumpTarget));

                // 3. Replace salvage dates: change fixed dates with call method to get the current date minus ten years
                int patchedDates = 0;
                while (true)
                {
                    matcher.Start().MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Ldloca_S || i.opcode == OpCodes.Ldloca),
                        new CodeMatch(i => i.opcode == OpCodes.Ldc_I4 && i.operand is int year && (year == 3025 || year == 3028)),
                        new CodeMatch(OpCodes.Ldc_I4_1),
                        new CodeMatch(OpCodes.Ldc_I4_1),
                        new CodeMatch(i => i.opcode == OpCodes.Call &&
                            i.operand is ConstructorInfo constructor &&
                            constructor.DeclaringType == typeof(DateTime)));

                    if (!matcher.IsValid)
                        break;

                    var localAddress = matcher.Instruction;
                    var replacement = new CodeInstruction(OpCodes.Ldarg_1);
                    replacement.labels.AddRange(localAddress.labels);
                    replacement.blocks.AddRange(localAddress.blocks);

                    var storeOpcode = localAddress.opcode == OpCodes.Ldloca_S ? OpCodes.Stloc_S : OpCodes.Stloc;
                    object local = localAddress.operand;

                    matcher.SetInstructionAndAdvance(replacement)
                           .RemoveInstructions(4)
                           .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XotlTable_RequestUnit), nameof(GetSalvageDate))),
                                   new CodeInstruction(storeOpcode, local));
                    patchedDates++;
                }

                return patchedDates != 2
                    ? throw new InvalidOperationException($"Expected to patch 2 salvage dates in XotlTable.RequestUnit, but patched {patchedDates}.")
                    : matcher.InstructionEnumeration();
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
}