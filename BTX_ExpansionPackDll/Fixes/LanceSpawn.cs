using BattleTech.Data;
using BattleTech.Framework;
using FullXotlTables;
using HBS.Collections;
using System;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Fixes
{
    internal class LanceSpawn
    {
        private static readonly Random random = new();

        [HarmonyPatch(typeof(UnitSpawnPointOverride), "RequestUnit")]
        public static class UnitSpawnPointOverride_RequestUnit
        {

            private static readonly HashSet<string> weightClassTags =
            [
                "unit_light",
                "unit_medium",
                "unit_heavy",
                "unit_assault"
            ];

            private static readonly HashSet<string> spotterVehicles =
            [
                "vehicledef_ALSVIN",
                "vehicledef_ARVAKR",
                "vehicledef_ASSHUR",
                "vehicledef_CAVALRY_TAG",
                "vehicledef_CENTIPEDE_TAG",
                "vehicledef_CYRANO_ROYAL",
                "vehicledef_EPONA-A",
                "vehicledef_EPONA-C",
                "vehicledef_EPONA-D",
                "vehicledef_FULCRUM",
                "vehicledef_FULCRUM_II",
                "vehicledef_GALLEON_TAG",
                "vehicledef_HEPHAESTUS-PRIME",
                "vehicledef_JEdgar_TAG",
                "vehicledef_LIGHTNING_ROYAL",
                "vehicledef_MAXIM_3052",
                "vehicledef_MAXIM_AP",
                "vehicledef_MAXIM_C3S",
                "vehicledef_MAXIM_I",
                "vehicledef_MAXIM_I_CC",
                "vehicledef_MINION_TAG",
                "vehicledef_MUSKETEER",
                "vehicledef_MUSKETEER_ARMOR",
                "vehicledef_PEGASUS_3058",
                "vehicledef_SCIMITAR_TAG",
                "vehicledef_SPRINT",
                "vehicledef_ZEPHYR",
                "vehicledef_ZEPHYR_C3i",
                "vehicledef_ZEPHYR_ROYAL",
            ];

            [HarmonyPrefix]
            [HarmonyBefore("BattleTech.Haree.FullXotlTables")]
            public static void Prefix(UnitSpawnPointOverride __instance, string lanceDefId, DateTime? currentDate, TagSet companyTags)
            {
                if (currentDate == null) return;
                switch (lanceDefId)
                {
                    case "lancedef_apc_dynamic_battle1" when !__instance.unitTagSet.Contains("unit_light"):
                        string lightWeightClass = random.Next(0, 100) < 80
                            ? "unit_medium"
                            : "unit_light";

                        __instance.unitTagSet.RemoveRange([.. weightClassTags]);
                        __instance.unitTagSet.Add(lightWeightClass);
                        break;

                    case "lancedef_arty_dynamic_battle1" when !__instance.unitTagSet.Contains("unit_vehicle_spotter"):
                        if (__instance.unitTagSet.Contains("unit_light") || __instance.unitTagSet.Contains("unit_assault"))
                        {
                            string middleWeightClass = random.Next(0, 100) < 60
                                    ? "unit_heavy"
                                    : "unit_medium";

                            __instance.unitTagSet.RemoveRange([.. weightClassTags]);
                            __instance.unitTagSet.Add(middleWeightClass);
                        }
                        break;

                    case "lancedef_arty_dynamic_battle1" when __instance.unitTagSet.Contains("unit_vehicle_spotter"):
                        var peekedUnitId = Core.xotlTables.RequestUnit(currentDate.Value, __instance.unitTagSet, __instance.unitExcludedTagSet, companyTags);
                        if (!spotterVehicles.Contains(peekedUnitId))
                        {
                            __instance.unitTagSet.Remove("unit_vehicle_spotter");
                            __instance.unitExcludedTagSet.Add("unit_speed_low");
                        }
                        break;

                    case "lancedef_vtol_dynamic_battle1" when !__instance.unitTagSet.Contains("unit_light"):
                        __instance.unitTagSet.RemoveRange([.. weightClassTags]);
                        __instance.unitTagSet.Add("unit_light");
                        break;
                }
            }

            [HarmonyPatch(typeof(UnitsAndLances_MDDExtensions), "GetDynamicLanceDifficultyListByDifficulty")]
            public static class UnitsAndLances_MDDExtensions_GetDynamicLanceDifficultyListByDifficulty
            {
                [HarmonyPrefix]
                public static void Prefix(ref long difficulty)
                {
                    int variance = difficulty <= 3 ? random.Next(0, 2) : random.Next(0, 3);
                    if (variance == 0) return;
                    long originalDifficulty = difficulty;
                    difficulty += variance;
                    Logger.Log($"Varied lance difficulty from {originalDifficulty} to {difficulty}.");
                }
            }
        }
    }
}