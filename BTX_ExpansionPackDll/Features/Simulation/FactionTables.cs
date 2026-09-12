using FullXotlTables;
using System;
using System.Collections.Generic;
using System.IO;

namespace BTX_ExpansionPack.Features.Simulation
{
    internal class FactionTables
    {
        /// <summary>
        /// Generates a custom faction table from CSV files in a specified folder.
        /// </summary>
        /// <remarks>
        /// Note: The logic is the same as the original FullXotlTables.GenerateTables class.
        /// </remarks>
        /// <returns></returns>
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
                        key = sr.ReadLine().Split(',')[0];
                        while (!sr.EndOfStream)
                        {
                            string[] line = sr.ReadLine().Split(',');
                            if (line[0].StartsWith("Collection:"))
                            {
                                factionTable.Collection.Add(line[0].Remove(0, 11), ReadWeightLine(line));
                            }
                            else if (line[0].StartsWith("Salvage:"))
                            {
                                factionTable.Salvage.Add(line[0].Remove(0, 8), ReadWeightLine(line));
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
                    weightValue.StartDate = DateTime.Parse(lineToRead[index - 1].TrimStart('J', 'u', 'm', 'p', ':'));
                    weightValue.HasStart = true;
                }
                if (index != lineToRead.Length - 1 && lineToRead[index + 1] != "")
                {
                    if (lineToRead[index + 1].StartsWith("Jump:"))
                        weightValue.StopIsJumpValue = true;
                    weightValue.StopDate = DateTime.Parse(lineToRead[index + 1].TrimStart('J', 'u', 'm', 'p', ':'));
                    weightValue.HasStop = true;
                }
                weightValueList.Add(weightValue);
            }
            return weightValueList;
        }
    }
}