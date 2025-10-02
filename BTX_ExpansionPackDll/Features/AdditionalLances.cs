using BattleTech.Data;
using BattleTech.Framework;
using FullXotlTables;
using HBS.Collections;
using System;
using Random = UnityEngine.Random;

namespace BTX_ExpansionPack.Features
{
    internal partial class AdditionalLances
    {
        /// <summary>
        /// Ensures that certain lances spawn with appropriate weight classes and types.
        /// </summary>
        [HarmonyPatch(typeof(UnitSpawnPointOverride), "RequestUnit")]
        public static class UnitSpawnPointOverride_RequestUnit_PrePatch
        {
            [HarmonyPrefix]
            [HarmonyBefore("BattleTech.Haree.FullXotlTables")]
            public static void Prefix(UnitSpawnPointOverride __instance, string lanceDefId, DateTime? currentDate, TagSet companyTags)
            {
                if (currentDate == null) return;
                switch (lanceDefId)
                {
                    case "lancedef_apc_dynamic_battle1" when !__instance.unitTagSet.Contains("unit_light"):
                        string lightWeightClass = Random.Range(0, 100) < 80
                            ? "unit_medium"
                            : "unit_light";

                        __instance.unitTagSet.RemoveRange(weightClassTags);
                        __instance.unitTagSet.Add(lightWeightClass);
                        break;

                    case "lancedef_arty_dynamic_battle1" when !__instance.unitTagSet.Contains("unit_vehicle_spotter"):
                        if (__instance.unitTagSet.Contains("unit_light") || __instance.unitTagSet.Contains("unit_assault"))
                        {
                            string middleWeightClass = Random.Range(0, 100) < 60
                                    ? "unit_heavy"
                                    : "unit_medium";

                            __instance.unitTagSet.RemoveRange(weightClassTags);
                            __instance.unitTagSet.Add(middleWeightClass);
                        }
                        break;

                    case "lancedef_arty_dynamic_battle1" when __instance.unitTagSet.Contains("unit_vehicle_spotter"):
                        string peekedUnitId = Core.xotlTables.RequestUnit(currentDate.Value, __instance.unitTagSet, __instance.unitExcludedTagSet, companyTags);
                        if (!spotterVehicles.Contains(peekedUnitId))
                        {
                            __instance.unitTagSet.Remove("unit_vehicle_spotter");
                            __instance.unitExcludedTagSet.Add("unit_speed_low");
                        }
                        break;

                    case "lancedef_vtol_dynamic_battle1" when !__instance.unitTagSet.Contains("unit_light"):
                        __instance.unitTagSet.RemoveRange(weightClassTags);
                        __instance.unitTagSet.Add("unit_light");
                        break;
                }
            }
        }

        /// <summary>
        /// Adjusts lance difficulty by a small random variance to increase lance variety.
        /// </summary>
        [HarmonyPatch(typeof(UnitsAndLances_MDDExtensions), "GetDynamicLanceDifficultyListByDifficulty")]
        public static class UnitsAndLances_MDDExtensions_GetDynamicLanceDifficultyListByDifficulty
        {
            [HarmonyPrefix]
            public static void Prefix(ref long difficulty)
            {
                int variance = difficulty <= 3 ? Random.Range(0, 1) : Random.Range(0, 2);
                if (variance == 0) return;
                long originalDifficulty = difficulty;
                difficulty += variance;
                Logger.Log($"Varied lance difficulty from {originalDifficulty} to {difficulty}.");
            }
        }
    }
}