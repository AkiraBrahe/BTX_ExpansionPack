using BattleTech;
using BEXTimeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace BTX_ExpansionPack.Features.Simulation
{
    internal class FactionStores
    {
        #region Faction Store Data

        internal static readonly Dictionary<string, (string Faction, bool VehicleOnly)> StartingFactionStores = new()
        {
            { "starsystemdef_Addicks", ("Davion", true) },
            { "starsystemdef_Andurien", ("Marik", true) },
            { "starsystemdef_Belladonna", ("Davion", true) },
            { "starsystemdef_BrokenWheel", ("Davion", true) },
            { "starsystemdef_Cahokia", ("Davion", false) },
            { "starsystemdef_Carver(Liberty3063+)", ("Liao", true) },
            { "starsystemdef_Dieron", ("Kurita", false) },
            { "starsystemdef_Inarcs", ("Steiner", true) },     // Vehicle-only until 3056
            { "starsystemdef_Indicass", ("Liao", true) },
            { "starsystemdef_Irece", ("Kurita", false) },
            { "starsystemdef_Johnsondale", ("Davion", true) },
            { "starsystemdef_Kirklin", ("Davion", true) },
            { "starsystemdef_Layover", ("Davion", true) },
            { "starsystemdef_Loyalty", ("Marik", true) },
            { "starsystemdef_Menke", ("Liao", true) },         // Vehicle-only until 3057
            { "starsystemdef_Mitchella", ("Outworld", true) },
            { "starsystemdef_Northwind", ("Davion", false) },
            { "starsystemdef_Proserpina", ("Kurita", true) },
            { "starsystemdef_Richvale", ("Steiner", true) },
            { "starsystemdef_Salem", ("Davion", true) },
            { "starsystemdef_Sevon", ("Outworld", false) },
            { "starsystemdef_Skye", ("Steiner", false) },
            { "starsystemdef_SonHoa", ("Steiner", false) },
            { "starsystemdef_Sterope(NewTaurus)", ("TaurianConcordat", true) },
            { "starsystemdef_TauCeti(NewEarth2116+)", ("Steiner", true) },
            { "starsystemdef_Togura", ("Kurita", false) },
            { "starsystemdef_Vega", ("Kurita", false) }
        };

        internal static readonly Dictionary<string, List<string>> MechOnlyStoresByDate = new()
        {
            { "3056-01-01T00:00:00", [ "starsystemdef_Inarcs" ] },
            { "3057-01-01T00:00:00", [ "starsystemdef_Menke" ] }
        };

        internal static readonly Dictionary<string, List<string>> VehicleOnlyStoresByDate = new()
        {
            { "3032-01-01T00:00:00", [ "starsystemdef_Betelgeuse" ] },
            { "3036-01-01T00:00:00", [ "starsystemdef_Spittal" ] },
            { "3042-01-01T00:00:00", [ "starsystemdef_Alphard(MH)" ] },
            { "3050-01-01T00:00:00", [ "starsystemdef_Orestes" ] },
            { "3052-01-01T00:00:00", [ "starsystemdef_Ruchbah" ] },
            { "3064-01-01T00:00:00", [ "starsystemdef_Bristol" ] },
            { "3068-01-01T00:00:00", [ "starsystemdef_Arcturus", "starsystemdef_Benet", "starsystemdef_Irece", "starsystemdef_Melissia" ] }
        };

        #endregion

        /// <summary>
        /// Checks for missing item collections and logs a warning if any are missing.
        /// </summary>
        [HarmonyPatch(typeof(SimGameState), "InitializeDataFromDefs")]
        public static class SimGameState_InitializeDataFromDefs
        {
            [HarmonyPostfix]
            public static void Postfix(SimGameState __instance)
            {
                // Validate ShopSwitch collections
                var shopSwitch = BEXTimeline.Core.Settings.ShopSwitch;
                if (shopSwitch != null)
                {
                    foreach (var kvp in shopSwitch)
                    {
                        string baseId = kvp.Key;
                        kvp.Value.RemoveAll(year =>
                        {
                            bool missing = !__instance.DataManager.Exists(BattleTechResourceType.ItemCollectionDef, baseId + year);
                            if (missing) Main.Logger.LogWarning($"Missing item collection for ShopSwitch: {baseId}{year}");
                            return missing;
                        });
                    }
                }

                // Validate FactionShopItems across all systems
                foreach (var systemDef in __instance.StarSystems.Select(s => s.Def))
                {
                    if (systemDef?.FactionShopItems != null)
                    {
                        systemDef.FactionShopItems.RemoveAll(itemId =>
                        {
                            bool missing = !__instance.DataManager.Exists(BattleTechResourceType.ItemCollectionDef, itemId);
                            if (missing) Main.Logger.LogWarning($"Missing FactionShopItem collection for {systemDef.Description.Id}: {itemId}");
                            return missing;
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Fix the faction store on the Alpheratz system so that it updates correctly over time.
        /// </summary>
        [HarmonyPatch(typeof(StarSystem), "Rehydrate")]
        public static class StarSystem_Rehydrate
        {
            [HarmonyPostfix]
            public static void Postfix(StarSystem __instance)
            {
                if (__instance.SystemID.Equals("starsystemdef_Alpheratz"))
                {
                    __instance.Def.FactionShopItems = [.. __instance.Def.FactionShopItems.Select(id => id == "itemCollection_factory_Alpheratz" ? "itemCollection_factoryHolder_Alpheratz" : id)];
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