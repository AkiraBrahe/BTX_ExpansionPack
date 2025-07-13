using BattleTech;
using BEXTimeline;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Utilities
{
    internal class PirateSystemLogger
    {
        private static readonly Dictionary<string, int> pirateSystemCounts = [];

        [HarmonyPatch(typeof(PirateHelper), "AddPiratesToSystem")]
        public static class PirateHelper_AddPiratesToSystem
        {
            [HarmonyPrepare]
            public static void Prepare()
            {
                if (!Main.Settings.Debug.PirateSystemLogging) return;

                pirateSystemCounts.Clear();

                foreach (string factionID in Core.Settings.PirateFactions.Keys)
                {
                    if (!pirateSystemCounts.ContainsKey(factionID))
                    {
                        pirateSystemCounts.Add(factionID, 0);
                    }
                }

                foreach (string factionID in Core.Settings.CriminalFactions.Keys)
                {
                    if (!pirateSystemCounts.ContainsKey(factionID))
                    {
                        pirateSystemCounts.Add(factionID, 0);
                    }
                }

                return;
            }

            [HarmonyPostfix]
            public static void Postfix(StarSystem theSystem)
            {
                if (!Main.Settings.Debug.PirateSystemLogging) return;

                List<string> assignedPirates = [.. theSystem.Def.ContractEmployerIDList
                    .Where(factionID => Core.Settings.PirateFactions.ContainsKey(factionID) ||
                                        Core.Settings.CriminalFactions.ContainsKey(factionID))];

                foreach (string factionID in assignedPirates)
                {
                    if (pirateSystemCounts.ContainsKey(factionID))
                    {
                        pirateSystemCounts[factionID]++;
                    }
                    else
                    {
                        pirateSystemCounts.Add(factionID, 1);
                    }
                }
            }

            [HarmonyPatch(typeof(UpdateOwnership), "UpdateTheMap")]
            public static class UpdateOwnership_UpdateTheMap
            {
                [HarmonyPostfix]
                public static void Postfix()
                {
                    if (!Main.Settings.Debug.PirateSystemLogging) return;

                    Logger.LogDebug("--- Pirate Faction System Counts ---");
                    if (pirateSystemCounts.Any())
                    {
                        foreach (var entry in pirateSystemCounts.OrderByDescending(e => e.Value))
                        {
                            Logger.LogDebug($"  {entry.Key}: {entry.Value} systems");
                        }
                    }
                    else
                    {
                        Logger.LogDebug("  No pirate system data collected yet, or no pirate systems found.");
                    }
                    Logger.LogDebug("------------------------------------");
                }
            }
        }
    }
}
