using BattleTech;
using BattleTech.Data;
using BattleTech.Framework;
using HBS.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using static BTX_ExpansionPack.Core.Helpers.LanceHelpers;
using Random = UnityEngine.Random;

namespace BTX_ExpansionPack.Features.Lances
{
    internal class LanceOverrides
    {
        /// <summary>
        /// Allows Snord's Irregulars to spawn as enemy in Search Denial contracts against ComStar.
        /// </summary>
        [HarmonyPatch(typeof(SimGameState), "PrepContract")]
        public static class SimGameState_PrepContract
        {
            public static void Prefix(SimGameState __instance, Contract contract, ref FactionValue target, StarSystem system)
            {
                if (contract.Override.ID.StartsWith("ThreeWayBattle_SearchDenialCS") && Random.Range(0f, 1f) < 0.05f)
                {
                    Main.Logger.LogDebug($"[LanceOverrides] Replacing target with Snord's Irregulars for {contract.Name} contract on {system.Name} system.");
                    target = __instance.DataManager.Factions.Get("faction_Merc28").FactionValue;
                }
            }
        }

        /// <summary>
        /// Allows elite Capellan units to use augmented lances following the Clan invasion.
        /// </summary>
        [HarmonyPatch(typeof(MissionControl.Config.ExtendedLancesSettings), "GetFactionLanceSize")]
        [HarmonyPatch(typeof(MissionControl.Config.ExtendedLancesSettings), "GetFactionLanceDifficulty")]
        public static class AdditionalLances_Patches
        {
            private static int CurrentYear => BEXTimeline.UpdateOwnership.LastDayUpdated.Year;

            [HarmonyPrefix]
            public static void Prefix(ref string factionKey)
            {
                if (!string.IsNullOrEmpty(factionKey) && CurrentYear >= 3052 && factionKey.StartsWith("LiaoA"))
                {
                    Main.Logger.LogDebug($"[LanceOverrides] Forcing augmented lance formation for Capellan faction '{factionKey}' in year {CurrentYear}.");
                    factionKey = "AugmentedLance";
                }
            }
        }

        /// <summary>
        /// Intercepts pilot spawns to assign elite pilots to ComStar and Clan units.
        /// </summary>
        [HarmonyPatch(typeof(UnitSpawnPointOverride), "RequestPilot")]
        public static class UnitSpawnPointOverride_RequestPilot
        {
            [HarmonyPrefix]
            [HarmonyBefore("BEX.BattleTech.Extended_CE")]
            public static bool Prefix(UnitSpawnPointOverride __instance, string lanceName)
            {
                var context = LanceGenerationContext.GetContext(lanceName);
                if (context == null)
                    return true;

                string lanceDefId = context.LanceDefId;
                if (lanceDefId.StartsWith("lancedef_comstar") || lanceDefId.StartsWith("lancedef_clan"))
                {
                    int difficulty = context.Difficulty;
                    string factionId = context.FactionId;
                    __instance.pilotTagSet.ForceEliteDifficulty(difficulty, factionId);
                }

                return true;
            }
        }

        /// <summary>
        /// Intercepts lance spawns to enforce more diverse and lore-accurate lance compositions.
        /// </summary>
        [HarmonyPatch(typeof(UnitSpawnPointOverride), "RequestUnit")]
        public static class UnitSpawnPointOverride_RequestUnit
        {
            private static readonly Dictionary<string, List<string>> lanceCompositionAssignments = [];
            private static readonly Dictionary<string, List<string>> artilleryLanceAssignments = [];

            [HarmonyPatch(typeof(Contract), "BeginRequestResources")]
            [HarmonyPatch(typeof(Contract), "ResetStateForRestart")]
            public static class Contract_ClearLanceAssignments
            {
                [HarmonyPrefix]
                public static void Prefix()
                {
                    LanceGenerationContext.ClearAllContexts();
                    lanceCompositionAssignments.Clear();
                    artilleryLanceAssignments.Clear();
                }
            }

            [HarmonyPrefix]
            public static bool Prefix(UnitSpawnPointOverride __instance, LoadRequest request, string lanceDefId, string lanceName, int unitIndex, DateTime? currentDate, TagSet companyTags)
            {
                var context = LanceGenerationContext.GetContext(lanceName);
                if (context == null)
                    return true;

                int year = currentDate?.Year ?? 3025;
                int difficulty = context.Difficulty;
                string factionId = context.FactionId;

                bool unitWasSelected = false;

                if (unitIndex >= 4)
                    EnforceAugmentedLance(__instance, unitIndex, year, factionId);
                if (lanceDefId == "lancedef_arty_dynamic_battle1")
                    HandleArtilleryLance(__instance, request, lanceName, unitIndex, year, factionId, difficulty, companyTags, ref unitWasSelected);
                else if (lanceDefId.StartsWith("lancedef_comstar") || lanceDefId.StartsWith("lancedef_clan"))
                    HandleComStarClanLance(__instance, lanceDefId, lanceName, unitIndex, difficulty);

                if (unitWasSelected)
                    return false;

                // Simplified BEX logic for unit selection
                bool isMech = __instance.unitTagSet.Contains("unit_mech"); bool isVehicle = __instance.unitTagSet.Contains("unit_vehicle");
                bool isTaggedUnit = __instance.IsUnitDefTagged && (isMech || isVehicle) && currentDate != null;

                if (isTaggedUnit)
                {
                    __instance.selectedUnitDefId = FullXotlTables.Core.xotlTables.RequestUnit(currentDate.Value, __instance.unitTagSet, __instance.unitExcludedTagSet, companyTags);
                    __instance.selectedUnitType = isMech ? UnitType.Mech : UnitType.Vehicle;
                    request.AddBlindLoadRequest(isMech ? BattleTechResourceType.MechDef : BattleTechResourceType.VehicleDef, __instance.selectedUnitDefId);
                    return false;
                }

                return true;
            }

            #region Augmented Lance Helpers

            /// <summary>
            /// Enforces the Capellan Confederation's augmented lance formation post-Clan invasion.
            /// </summary>
            private static void EnforceAugmentedLance(UnitSpawnPointOverride __instance, int unitIndex, int year, string factionId)
            {
                if (factionId.StartsWith("LiaoA") && year >= 3052)
                {
                    if (__instance.unitType == UnitType.Mech)
                    {
                        __instance.unitType = UnitType.Vehicle;
                        __instance.unitTagSet.Remove("unit_mech");
                        __instance.unitTagSet.Add("unit_vehicle");
                        __instance.unitExcludedTagSet.Add("unit_vtol");
                        __instance.unitExcludedTagSet.Add("unit_noncombatant");
                        Main.Logger.LogDebug($"[AugmentedLanceOverride] Forced Capellan unit {unitIndex} to Vehicle.");
                    }
                    else if (__instance.unitType == UnitType.Vehicle)
                    {
                        __instance.unitType = UnitType.Mech;
                        __instance.unitTagSet.Remove("unit_vehicle");
                        __instance.unitTagSet.Add("unit_mech");
                        __instance.unitExcludedTagSet.Remove("unit_vtol");
                        __instance.unitExcludedTagSet.Add("unit_noncombatant");
                        Main.Logger.LogDebug($"[AugmentedLanceOverride] Forced Capellan unit {unitIndex} to Mech.");
                    }
                }
            }

            #endregion

            #region Artillery Helpers

            /// <summary>
            /// Handles dedicated artillery lances for more lore-accurate compositions.
            /// Sets <paramref name="unitAssigned"/> to indicate if a unit was directly assigned.
            /// </summary>
            /// <remarks>
            /// Artillery vehicles are managed separately from Xotl's unit tables to prevent artillery from spawning in standard lances.
            /// <br/>Lances can be either a command artillery unit with escorts or a standard artillery unit with an optional spotter vehicle.
            /// </remarks>
            private static void HandleArtilleryLance(UnitSpawnPointOverride instance, LoadRequest request, string lanceName, int unitIndex, int year, string factionId, int difficulty, TagSet companyTags, ref bool unitAssigned)
            {
                unitAssigned = false;

                // 1. Initialize artillery composition on first unit of this lance
                if (unitIndex == 0)
                {
                    string selected = SelectArtillery(factionId, year, out var available);
                    var composition = BuildArtilleryComposition(selected, available);

                    artilleryLanceAssignments[lanceName] = composition;
                }

                // 2. Retrieve the pre-built composition for this lance
                if (!artilleryLanceAssignments.TryGetValue(lanceName, out var artList))
                {
                    Main.Logger.LogWarning($"[ArtilleryOverride] No composition found for '{lanceName}'.");
                    return;
                }

                // 3a. Handle command artillery
                bool isCommand = IsCommandArtillery(artList[0]);
                if (isCommand)
                {
                    if (unitIndex == 0)
                    {
                        AssignArtilleryUnit(instance, request, artList[0]);
                        unitAssigned = true;
                        return;
                    }

                    AssignCommandArtilleryEscort(instance, request, year, companyTags);
                    unitAssigned = true;
                    return;
                }

                // 3b. Handle standard artillery
                if (unitIndex < artList.Count)
                {
                    if (unitIndex == 4 && difficulty < 7)
                    {
                        // Chance for a spotter vehicle to spawn
                        int chance = 75 - (difficulty * 10);
                        if (Random.Range(0, 100) < chance)
                        {
                            AssignArtillerySpotter(instance, request, year, companyTags);
                            unitAssigned = true;
                            return;
                        }
                    }

                    AssignArtilleryUnit(instance, request, artList[unitIndex]);
                    unitAssigned = true;
                    return;
                }
            }

            /// <summary>
            /// Selects a random artillery vehicle available to the specified faction and year.
            /// </summary>
            private static string SelectArtillery(string factionId, int year, out Dictionary<string, int> available)
            {
                string parentFaction = GetParentFaction(factionId);
                var factionValue = FactionEnumeration.GetFactionByName(parentFaction);
                bool isClan = factionValue != null && factionValue.IsClan;
                bool isPeriphery = factionValue != null && factionValue.IsPeriphery();

                Main.Logger.LogDebug($"[SelectArtillery] faction={factionId}, parent={parentFaction}, year={year}, isClan={isClan}, isPeriphery={isPeriphery}.");

                // Weighted dictionary of available artillery units for this faction
                available = ArtilleryVehicles
                    .Select(v => (v.DefId, Available: v.IsAvailable(parentFaction, year, out int w, isClan, isPeriphery), Weight: w))
                    .Where(x => x.Available)
                    .ToDictionary(x => x.DefId, x => x.Weight);

                if (available.Any())
                {
                    string selected = WeightedRandomSelect(available);
                    Main.Logger.LogDebug($"[SelectArtillery] Selected {selected} from {available.Count} options.");
                    return selected;
                }

                Main.Logger.LogWarning($"[SelectArtillery] No artillery available for '{parentFaction}' in {year}. Using default.");
                return "vehicledef_THUMPER";
            }

            /// <summary>
            /// Builds a 4-unit artillery composition from the selected artillery type.
            /// </summary>
            /// <remarks>
            /// Command artillery returns only one unit. Escorts are added later by separate tag-based selection.
            /// <br/>Chaparral artillery lances are mixed; other artillery types repeat the same DefId.
            /// </remarks>
            private static List<string> BuildArtilleryComposition(string selectedArtillery, Dictionary<string, int> available)
            {
                if (IsCommandArtillery(selectedArtillery))
                    return [selectedArtillery];

                List<string> composition = [selectedArtillery];
                var variantPool = selectedArtillery.StartsWith("vehicledef_CHAPARRAL")
                    ? available.Where(kv => kv.Key.StartsWith("vehicledef_CHAPARRAL")).ToDictionary(kv => kv.Key, kv => kv.Value)
                    : [];

                // Add three more units to complete the composition
                for (int i = 1; i < 4; i++)
                {
                    composition.Add(variantPool.Count > 1
                        ? WeightedRandomSelect(variantPool)
                        : selectedArtillery);
                }

                return composition;
            }

            /// <summary>
            /// Assigns the selected artillery unit to a spawn point, bypassing BEX unit selection.
            /// </summary>
            private static void AssignArtilleryUnit(UnitSpawnPointOverride instance, LoadRequest request, string defId)
            {
                instance.selectedUnitDefId = defId;
                instance.selectedUnitType = UnitType.Vehicle;
                request.AddBlindLoadRequest(BattleTechResourceType.VehicleDef, defId);
            }

            /// <summary>
            /// Assigns a random artillery spotter vehicle to accompany an artillery lance.
            /// </summary>
            private static void AssignArtillerySpotter(UnitSpawnPointOverride instance, LoadRequest request, int year, TagSet companyTags)
            {
                // Modify unit tags to let BEX select a spotter vehicle instead of artillery
                instance.unitExcludedTagSet.Add("unit_vehicle_artillery");
                instance.unitTagSet.Remove("unit_vehicle_artillery");
                instance.unitTagSet.Add("unit_vehicle_spotter");
                instance.unitTagSet.ClampToWeightClass("unit_medium", "unit_light", 0.6f);

                // Request unit with modified tags
                var currentDate = new DateTime(year, 1, 1);
                string spotterUnitId = FullXotlTables.Core.xotlTables.RequestUnit(currentDate, instance.unitTagSet, instance.unitExcludedTagSet, companyTags);
                instance.selectedUnitDefId = spotterUnitId;
                instance.selectedUnitType = UnitType.Vehicle;
                request.AddBlindLoadRequest(BattleTechResourceType.VehicleDef, spotterUnitId);
                Main.Logger.LogDebug($"[ArtillerySpotter] Assigned spotter unit '{spotterUnitId}' for artillery lance.");
            }

            /// <summary>
            /// Assigns a random escort vehicle to accompany the command artillery unit.
            /// </summary>
            private static void AssignCommandArtilleryEscort(UnitSpawnPointOverride instance, LoadRequest request, int year, TagSet companyTags)
            {
                // Modify units tags to let BEX select non-artillery escorts
                instance.unitTagSet.Add("xotl_min_0.3333");
                instance.unitTagSet.Remove("unit_vehicle_artillery");
                instance.unitExcludedTagSet.Add("unit_vehicle_artillery");

                // Request unit with modified tags
                var currentDate = new DateTime(year, 1, 1);
                string escortUnitId = FullXotlTables.Core.xotlTables.RequestUnit(currentDate, instance.unitTagSet, instance.unitExcludedTagSet, companyTags);
                instance.selectedUnitDefId = escortUnitId;
                instance.selectedUnitType = UnitType.Vehicle;
                request.AddBlindLoadRequest(BattleTechResourceType.VehicleDef, escortUnitId);
                Main.Logger.LogDebug($"[CommandArtilleryEscort] Assigned escort unit '{escortUnitId}' for command artillery.");
            }

            /// <summary>
            /// Checks if the given unit is a command artillery vehicle.
            /// Currently, only the Mobile Long Tom and Schiltron Prime qualify as command artillery.
            /// </summary>
            private static bool IsCommandArtillery(string defId) => defId.StartsWith("vehicledef_LONGTOM-LT-MOB") || defId.StartsWith("vehicledef_SCHILTRON");

            #endregion

            #region ComStar / Clan Helpers

            /// <summary>
            /// Handles ComStar Level II and Clan Star compositions for better unit variety.
            /// </summary>
            /// <remarks>
            /// Instead of duplicating a unit for the fifth and sixth spawn points (Mission Control logic), a random lance composition is selected and applied for the entire lance.
            /// </remarks>
            private static void HandleComStarClanLance(UnitSpawnPointOverride instance, string lanceDefId, string lanceName, int unitIndex, int difficulty)
            {
                List<string> selectedComposition;

                if (unitIndex == 0)
                {
                    selectedComposition = SelectComStarClanComposition(lanceDefId, lanceName, difficulty);
                    lanceCompositionAssignments[lanceName] = selectedComposition;
                    Main.Logger.LogDebug($"[ComstarClanOverride] Selected composition for lance '{lanceName}' (difficulty: {difficulty}): {string.Join(", ", selectedComposition)}");
                }
                else
                {
                    lanceCompositionAssignments.TryGetValue(lanceName, out selectedComposition);
                }

                if (selectedComposition != null && unitIndex < selectedComposition.Count)
                {
                    ApplyComStarClanOverride(instance, selectedComposition[unitIndex]);
                }
            }

            /// <summary>
            /// Selects a random lance composition for a ComStar Level II or Clan Star.
            /// </summary>
            private static List<string> SelectComStarClanComposition(string lanceDefId, string lanceName, int difficulty)
            {
                // 1. ComStar Level IIs
                if (lanceDefId.StartsWith("lancedef_comstar"))
                {
                    var (comstarList, diffMin, diffMax) = GetComStarCompositionTier(difficulty);

                    // Select from next-lighter tier list for reinforcements
                    if (lanceDefId == "lancedef_comstar_dynamic_battle2")
                    {
                        DowngradeCompositions(comstarList, isComStar: true);
                        return GetRandomComposition(comstarList);
                    }

                    comstarList = ApplyDifficultyWeighting(comstarList, difficulty, diffMin, diffMax);
                    return GetRandomComposition(comstarList);
                }

                // 2. Clan Stars
                var (clanList, clanDiffMin, clanDiffMax) = GetClanCompositionTier(difficulty);

                // Select from next-lighter tier list for an ambusher or secondary lance (25% chance)
                if (lanceName.Contains("_Ambushers") ||
                   (lanceName.Contains("_Secondary") && Random.Range(0f, 1f) < 0.25f))
                {
                    DowngradeCompositions(clanList);
                    return GetRandomComposition(clanList);
                }

                clanList = ApplyDifficultyWeighting(clanList, difficulty, clanDiffMin, clanDiffMax);
                return GetRandomComposition(clanList);
            }

            /// <summary>
            /// Applies the selected lance composition to a spawn point.
            /// </summary>
            private static void ApplyComStarClanOverride(UnitSpawnPointOverride instance, string weightTag) => instance.unitTagSet.ForceWeightClass(weightTag);

            #endregion
        }
    }
}