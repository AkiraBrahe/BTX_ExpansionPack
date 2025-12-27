using BattleTech;
using BEXTimeline;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace BTX_ExpansionPack.Features
{
    internal class FactionStores
    {
        private static readonly Dictionary<string, string> StartingFactionStores = new()
        {
            { "starsystemdef_Addicks", "Davion" },
            { "starsystemdef_Andurien", "Marik" },
            { "starsystemdef_Belladonna", "Davion" },
            { "starsystemdef_BrokenWheel", "Davion" },
            { "starsystemdef_Carver(Liberty3063+)", "Liao" },
            { "starsystemdef_Inarcs", "Steiner" },
            { "starsystemdef_Indicass", "Liao" },
            { "starsystemdef_Irece", "Kurita" },
            { "starsystemdef_Johnsondale", "Davion" },
            { "starsystemdef_Kirklin", "Davion" },
            { "starsystemdef_Layover", "Davion" },
            { "starsystemdef_Loyalty", "Marik" },
            { "starsystemdef_Menke", "Liao" },
            { "starsystemdef_Mitchella", "Outworld" },
            { "starsystemdef_Northwind", "Davion" },
            { "starsystemdef_Proserpina", "Kurita" },
            { "starsystemdef_Richvale", "Steiner" },
            { "starsystemdef_Salem", "Davion" },
            { "starsystemdef_Skye", "Steiner" },
            { "starsystemdef_Sterope(NewTaurus)", "TaurianConcordat" },
            { "starsystemdef_TauCeti(NewEarth2116+)", "Steiner" }
        };

        private static readonly HashSet<string> MechOnlyStartingFactionStores =
        [
            "starsystemdef_Inarcs",
            "starsystemdef_Irece",
            "starsystemdef_Menke",
            "starsystemdef_Northwind"
        ];

        private static readonly Dictionary<string, List<string>> VehicleOnlyTimelineStores = new()
        {
            { "3032-01-01T00:00:00", new List<string> { "starsystemdef_Betelgeuse" } },
            { "3036-01-01T00:00:00", new List<string> { "starsystemdef_Spittal" } },
            { "3042-01-01T00:00:00", new List<string> { "starsystemdef_Alphard(MH)" } },
            { "3050-01-01T00:00:00", new List<string> { "starsystemdef_Orestes" } },
            { "3052-01-01T00:00:00", new List<string> { "starsystemdef_Ruchbah" } },
            { "3064-01-01T00:00:00", new List<string> { "starsystemdef_Bristol" } },
            { "3068-01-01T00:00:00", new List<string> { "starsystemdef_Arcturus", "starsystemdef_Benet", "starsystemdef_Melissia" } }
        };

        /// <summary>
        /// Adds custom faction stores after Inner Sphere Map has finished updating the StarSystemDefs files.
        /// Also handles missing shop switch entries and conditionally removes vehicle stores if vehicles are not playable.
        /// </summary>
        [HarmonyPatch(typeof(SimGameState), "InitializeDataFromDefs")]
        public static class SimGameState_InitializeDataFromDefs
        {
            [HarmonyPostfix]
            public static void Postfix(SimGameState __instance)
            {
                var dataManager = __instance.DataManager;

                foreach (var shopEntry in StartingFactionStores)
                {
                    string systemId = shopEntry.Key;

                    if (!Main.HasPlayableVehicles && !MechOnlyStartingFactionStores.Contains(systemId))
                        continue;

                    if (dataManager.SystemDefs.TryGet(systemId, out var systemDef))
                    {
                        string itemCollectionId = $"itemCollection_factoryHolder_{SanitizeSystemDefId(systemId)}";
                        systemDef.FactionShopOwner = systemDef.Owner;
                        systemDef.FactionShopItems ??= [];

                        if (!systemDef.FactionShopItems.Contains(itemCollectionId))
                        {
                            systemDef.FactionShopItems.Add(itemCollectionId);
                        }
                    }
                }

                if (!Main.HasPlayableVehicles)
                {
                    var factionShops = Core.Settings.FactionShopCreation;
                    if (factionShops != null)
                    {
                        foreach (var entry in VehicleOnlyTimelineStores)
                        {
                            if (DateTime.TryParse(entry.Key, out var date))
                            {
                                if (factionShops.TryGetValue(date, out var shopsOnDate))
                                {
                                    foreach (string systemId in entry.Value)
                                    {
                                        shopsOnDate.Remove(systemId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes vehicles from shops if they are not playable.
        /// </summary>
        [HarmonyPatch(typeof(Shop), "RefreshShop")]
        public static class Shop_RefreshShop
        {
            [HarmonyPrepare]
            public static bool HarmonyPrepare() => !Main.HasPlayableVehicles;

            [HarmonyPostfix]
            public static void Postfix(Shop __instance) => __instance.ActiveInventory?.RemoveAll(x => x.ID.StartsWith("vehicledef_"));
        }

        /// <summary>
        /// Correctly handles systemdef IDs with parenthetical suffixes when creating faction stores.
        /// </summary>
        [HarmonyPatch(typeof(UpdateOwnership), "UpdateTheMap")]
        public static class UpdateOwnership_UpdateTheMap
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchStartForward(new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(string), "Substring", [typeof(int)])))
                    .Advance(-1).SetAndAdvance(OpCodes.Nop, null)
                    .SetAndAdvance(OpCodes.Call, AccessTools.Method(typeof(FactionStores), "SanitizeSystemDefId"))
                    .InstructionEnumeration();
            }
        }

        public static string SanitizeSystemDefId(string systemDefId)
        {
            if (string.IsNullOrEmpty(systemDefId))
                return String.Empty;

            if (systemDefId.Equals("starsystemdef_TauCeti(NewEarth2116+)"))
                return "NewEarth";

            string systemName = systemDefId.Substring(14);
            return Regex.Replace(systemName, @"\s*\(.*\)", "");
        }
    }
}