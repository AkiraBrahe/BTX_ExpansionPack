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
        /// Allows elite Capellan units to use augmented lances following the Clan invasion.
        /// </summary>
        [HarmonyPatch(typeof(MissionControl.Config.AdditionalLances), "GetLanceSize", typeof(string))]
        [HarmonyPatch(typeof(MissionControl.Config.AdditionalLances), "GetFactionLanceDifficulty", typeof(string), typeof(LanceOverride))]
        public static class AdditionalLances_CapellanAugmentedLances
        {
            private static int CurrentYear => BEXTimeline.UpdateOwnership.LastDayUpdated.Year;

            [HarmonyPrefix]
            public static void Prefix(ref string factionKey)
            {
                if (string.IsNullOrEmpty(factionKey)) return;

                if (CurrentYear >= 3052 && factionKey.StartsWith("LiaoA"))
                {
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
            public static bool Prefix(UnitSpawnPointOverride __instance)
            {
                if (LanceGenerationContext.Current == null)
                    return true;

                string lanceDefId = LanceGenerationContext.Current.LanceDefId;
                if (lanceDefId.StartsWith("lancedef_comstar") || lanceDefId.StartsWith("lancedef_clan"))
                {
                    int difficulty = LanceGenerationContext.Current.Difficulty;
                    string factionId = LanceGenerationContext.Current.FactionId;
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
                    lanceCompositionAssignments.Clear();
                    artilleryLanceAssignments.Clear();
                }
            }

            [HarmonyPrefix]
            [HarmonyBefore("BattleTech.Haree.FullXotlTables")]
            public static bool Prefix(UnitSpawnPointOverride __instance, LoadRequest request, string lanceDefId, string lanceName, int unitIndex, DateTime? currentDate, TagSet companyTags)
            {
                int year = currentDate?.Year ?? 3025;
                int difficulty = LanceGenerationContext.Current?.Difficulty ?? 5;
                string factionId = LanceGenerationContext.Current?.FactionId ?? "General";

                if (unitIndex == 0)
                {
                    factionId = __instance.unitTagSet.FirstOrDefault(tag => tag.StartsWith("unit_none_"))?.Substring("unit_none_".Length);
                    LanceGenerationContext.SetContext(difficulty, lanceDefId, factionId);
                }

                if (unitIndex >= 4)
                    EnforceAugmentedLance(__instance, unitIndex, year, factionId);
                if (lanceDefId == "lancedef_arty_dynamic_battle1")
                    return HandleArtilleryLance(__instance, request, lanceName, unitIndex, year, factionId, difficulty, companyTags);
                else if (lanceDefId.StartsWith("lancedef_comstar") || lanceDefId.StartsWith("lancedef_clan"))
                    HandleComStarClanLance(__instance, lanceDefId, lanceName, unitIndex, difficulty);

                return true;
            }

            #region Augmented Lance Helpers

            /// <summary>
            /// Enforces the augmented lance formation of the Capellan Confederation.
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
                        Main.Log.LogDebug($"[AugmentedLanceOverride] Forced Capellan unit {unitIndex} to Vehicle.");
                    }
                    else if (__instance.unitType == UnitType.Vehicle)
                    {
                        __instance.unitType = UnitType.Mech;
                        __instance.unitTagSet.Remove("unit_vehicle");
                        __instance.unitTagSet.Add("unit_mech");
                        __instance.unitExcludedTagSet.Remove("unit_vtol");
                        __instance.unitExcludedTagSet.Add("unit_noncombatant");
                        Main.Log.LogDebug($"[AugmentedLanceOverride] Forced Capellan unit {unitIndex} to Mech.");
                    }
                }
            }

            #endregion

            #region Artillery Helpers

            /// <summary>
            /// Handles dedicated artillery lances for better unit variety.
            /// </summary>
            /// <remarks>
            /// Artillery vehicles now have their own availability, separate from Xotl's unit tables to prevent artillery vehicles from spawning in standard lances. The method handles:
            /// <list type="bullet">
            /// <item><description><b>Command artillery lances:</b> lances with one command artillery unit and three escorts.</description></item>
            /// <item><description><b>Standard artillery lances:</b> lances with only artillery vehicles or three artillery vehicles and one spotter vehicle.</description></item>
            /// </list>
            /// </remarks>
            private static bool HandleArtilleryLance(UnitSpawnPointOverride instance, LoadRequest request, string lanceName, int unitIndex, int year, string factionId, int difficulty, TagSet companyTags)
            {
                if (unitIndex == 0)
                {
                    string selected = SelectArtillery(factionId, year, out var available);
                    var composition = BuildArtilleryComposition(selected, available);

                    artilleryLanceAssignments[lanceName] = composition;
                    Main.Log.LogDebug($"[ArtilleryOverride] Selected artillery composition for '{lanceName}': {string.Join(", ", composition)}");
                }

                if (!artilleryLanceAssignments.TryGetValue(lanceName, out var artList))
                    return true;

                bool isCommand = IsCommandArtillery(artList[0]);
                if (isCommand)
                {
                    if (unitIndex == 0)
                    {
                        ApplyArtilleryUnit(instance, request, artList[0], lanceName, unitIndex);
                        return false;
                    }

                    ApplyCommandArtilleryEscort(instance, lanceName, unitIndex);
                    return true;
                }

                if (unitIndex < artList.Count)
                {
                    if (unitIndex == 4 && difficulty < 7)
                    {
                        // Chance for a spotter vehicle to spawn
                        int chance = 75 - (difficulty * 10);
                        if (Random.Range(0, 100) < chance)
                            ApplyArtillerySpotter(instance, request, artList[unitIndex], lanceName, unitIndex, year, companyTags);
                    }

                    ApplyArtilleryUnit(instance, request, artList[unitIndex], lanceName, unitIndex);
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Selects a random artillery vehicle available to the specified faction in the given year.
            /// </summary>
            private static string SelectArtillery(string factionId, int year, out Dictionary<string, int> available)
            {
                string parentFaction = GetParentFaction(factionId);
                var factionValue = FactionEnumeration.GetFactionByName(parentFaction);
                bool isClan = factionValue != null && factionValue.IsClan;
                bool isPeriphery = factionValue != null && factionValue.IsPeriphery();

                available = ArtilleryVehicles
                    .Select(v => (v.DefId, Available: v.IsAvailable(parentFaction, year, out int w, isClan, isPeriphery), Weight: w))
                    .Where(x => x.Available)
                    .ToDictionary(x => x.DefId, x => x.Weight);

                if (available.Any())
                {
                    return WeightedRandomSelect(available);
                }

                Main.Log.LogWarning($"[ArtilleryOverride] No available artillery found for faction '{parentFaction}' in {year}. Using default.");
                return "vehicledef_THUMPER";
            }

            /// <summary>
            /// Builds a list of artillery vehicles to be assigned to a lance.
            /// </summary>
            private static List<string> BuildArtilleryComposition(string selectedArtillery, Dictionary<string, int> available)
            {
                if (IsCommandArtillery(selectedArtillery))
                    return [selectedArtillery];

                List<string> composition = [selectedArtillery];
                Dictionary<string, int> variantPool = [];

                if (selectedArtillery.StartsWith("vehicledef_CHAPARRAL"))
                {
                    variantPool = available.Where(kv => kv.Key.StartsWith("vehicledef_CHAPARRAL")).ToDictionary(kv => kv.Key, kv => kv.Value);
                }

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
            private static void ApplyArtilleryUnit(UnitSpawnPointOverride instance, LoadRequest request, string defId, string lanceName, int unitIndex)
            {
                instance.selectedUnitDefId = defId;
                instance.selectedUnitType = UnitType.Vehicle;
                request.AddBlindLoadRequest(BattleTechResourceType.VehicleDef, defId);
                Main.Log.LogDebug($"[ArtilleryOverride] Assigned '{defId}' to '{lanceName}' unit {unitIndex}");
            }

            /// <summary>
            /// Changes unit tags of the fourth artillery spawn point to allow BEX to spawn a random spotter vehicle.
            /// </summary>
            private static void ApplyArtillerySpotter(UnitSpawnPointOverride instance, LoadRequest request, string defId, string lanceName, int unitIndex, int year, TagSet companyTags)
            {

                instance.unitExcludedTagSet.Add("unit_vehicle_artillery");
                instance.unitTagSet.Remove("unit_vehicle_artillery");
                instance.unitTagSet.Add("unit_vehicle_spotter");
                instance.unitTagSet.ClampToWeightClass("unit_medium", "unit_light", 0.6f);

                var currentDate = new DateTime(year, 1, 1);
                string peekedUnitId = FullXotlTables.Core.xotlTables.RequestUnit(currentDate, instance.unitTagSet, instance.unitExcludedTagSet, companyTags);
                if (SpotterVehicles.Contains(peekedUnitId))
                    request.AddBlindLoadRequest(BattleTechResourceType.VehicleDef, peekedUnitId);
                else
                    request.AddBlindLoadRequest(BattleTechResourceType.VehicleDef, defId);
                Main.Log.LogDebug($"[ArtilleryOverride] Letting BEX spawn random spotter vehicle for '{lanceName}' unit {unitIndex}");
            }

            /// <summary>
            /// Changes unit tags for command artillery spawn points to allow BEX to spawn random escort units.
            /// </summary>
            private static void ApplyCommandArtilleryEscort(UnitSpawnPointOverride instance, string lanceName, int unitIndex)
            {
                instance.unitTagSet.Add("xotl_min_0.3333");
                instance.unitTagSet.Remove("unit_vehicle_artillery");
                instance.unitExcludedTagSet.Add("unit_vehicle_artillery");
                Main.Log.LogDebug($"[ArtilleryOverride] Letting BEX spawn random escort vehicle for '{lanceName}' unit {unitIndex}");
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
            /// Instead of duplicating the first unit for the fifth and sixth spawn points (Mission Control logic), a random lance composition is selected and applied for the entire lance.
            /// </remarks>
            private static void HandleComStarClanLance(UnitSpawnPointOverride instance, string lanceDefId, string lanceName, int unitIndex, int difficulty)
            {
                List<string> selectedComposition;

                if (unitIndex == 0)
                {
                    selectedComposition = SelectComStarClanComposition(lanceDefId, lanceName, difficulty);
                    lanceCompositionAssignments[lanceName] = selectedComposition;
                    Main.Log.LogDebug($"[ComstarClanOverride] Selected composition for lance '{lanceName}': {string.Join(", ", selectedComposition)}");
                }
                else
                {
                    lanceCompositionAssignments.TryGetValue(lanceName, out selectedComposition);
                }

                if (selectedComposition != null && unitIndex < selectedComposition.Count)
                {
                    ApplyComStarClanOverride(instance, selectedComposition[unitIndex], lanceName, unitIndex);
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
                    var comstarList = difficulty switch
                    {
                        <= 3 => ComstarLightLevelIIs,
                        <= 6 => ComstarMediumLevelIIs,
                        <= 9 => ComstarHeavyLevelIIs,
                        _ => ComstarAssaultLevelIIs
                    };

                    return GetRandomComposition(comstarList);
                }

                // 2. Clan Stars
                var clanList = difficulty switch
                {
                    <= 3 => ClanLightStars,
                    <= 6 => ClanMediumStars,
                    _ => ClanHeavyStars
                };

                // Select from next-lighter tier list for an ambusher or secondary lance (25% chance)
                if (lanceName.Contains("_Ambushers") ||
                   (lanceName.Contains("_Secondary") && Random.Range(0f, 1f) < 0.25f))
                {
                    if (clanList == ClanHeavyStars)
                        clanList = ClanMediumStars;
                    else if (clanList == ClanMediumStars)
                        clanList = ClanLightStars;
                }

                return GetRandomComposition(clanList);
            }

            /// <summary>
            /// Applies the selected lance composition to a spawn point.
            /// </summary>
            private static void ApplyComStarClanOverride(UnitSpawnPointOverride instance, string weightTag, string lanceName, int unitIndex)
            {
                instance.unitTagSet.ForceWeightClass(weightTag);
                Main.Log.LogDebug($"[ComstarClanOverride] Applied tag '{weightTag}' to unit {unitIndex} in lance '{lanceName}'.");
            }

            #endregion
        }
    }
}