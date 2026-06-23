using BattleTech.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BTX_ExpansionPack.Core.Data
{
    public static class LanceData
    {
        public struct LanceComposition
        {
            public int Weight { get; set; }
            public List<string> UnitWeightTags { get; set; }
        }

        #region ComStar Level IIs

        public static readonly List<LanceComposition> ComstarLightLevelIIs =
        [
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_light", "unit_light", "unit_light", "unit_light", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 4, UnitWeightTags = ["unit_medium", "unit_light", "unit_light", "unit_light", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_medium", "unit_medium", "unit_light", "unit_light", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_medium", "unit_medium", "unit_light", "unit_light", "unit_light"] }
        ];

        public static readonly List<LanceComposition> ComstarMediumLevelIIs =
        [
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_medium", "unit_medium", "unit_medium", "unit_medium", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_medium", "unit_medium", "unit_medium", "unit_medium", "unit_medium", "unit_light"] },
            new LanceComposition { Weight = 4, UnitWeightTags = ["unit_heavy", "unit_medium", "unit_medium", "unit_medium", "unit_medium", "unit_medium"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_heavy", "unit_medium", "unit_medium", "unit_medium", "unit_medium", "unit_light"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_medium", "unit_medium", "unit_medium", "unit_medium"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_medium", "unit_medium", "unit_medium"] }
        ];

        public static readonly List<LanceComposition> ComstarHeavyLevelIIs =
        [
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_medium", "unit_medium"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_medium"] },
            new LanceComposition { Weight = 4, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_assault", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_medium"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_assault", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_assault", "unit_assault", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy"] }
        ];

        public static readonly List<LanceComposition> ComstarAssaultLevelIIs =
        [
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_assault", "unit_assault", "unit_assault", "unit_heavy", "unit_heavy", "unit_medium"] },
            new LanceComposition { Weight = 4, UnitWeightTags = ["unit_assault", "unit_assault", "unit_assault", "unit_assault", "unit_heavy", "unit_heavy"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_assault", "unit_assault", "unit_assault", "unit_assault", "unit_assault", "unit_heavy"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_assault", "unit_assault", "unit_assault", "unit_assault", "unit_assault", "unit_assault"] }
        ];

        #endregion

        #region Clan Stars

        public static readonly List<LanceComposition> ClanLightStars =
        [
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_light", "unit_light", "unit_light", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_medium", "unit_light", "unit_light", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_medium", "unit_medium", "unit_light", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_light", "unit_light", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_medium", "unit_medium", "unit_medium", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_medium", "unit_medium", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_medium", "unit_medium", "unit_medium", "unit_medium", "unit_light"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_medium", "unit_medium", "unit_medium", "unit_medium", "unit_medium"] }
        ];

        public static readonly List<LanceComposition> ClanMediumStars =
        [
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_medium", "unit_medium", "unit_medium", "unit_medium", "unit_light"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_medium", "unit_medium", "unit_medium", "unit_medium", "unit_medium"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_heavy", "unit_medium", "unit_medium", "unit_medium", "unit_medium"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_medium", "unit_light", "unit_light"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_medium", "unit_medium", "unit_light"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_medium", "unit_medium", "unit_medium"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_medium", "unit_light"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_medium", "unit_medium"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_medium"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy"] }
        ];

        public static readonly List<LanceComposition> ClanHeavyStars =
        [
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_medium"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_assault", "unit_assault", "unit_heavy", "unit_medium", "unit_medium"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_assault", "unit_heavy", "unit_heavy", "unit_heavy", "unit_medium"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_assault", "unit_assault", "unit_heavy", "unit_heavy", "unit_medium"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_assault", "unit_heavy", "unit_heavy", "unit_heavy", "unit_heavy"] },
            new LanceComposition { Weight = 2, UnitWeightTags = ["unit_assault", "unit_assault", "unit_heavy", "unit_heavy", "unit_heavy"] },
            new LanceComposition { Weight = 1, UnitWeightTags = ["unit_assault", "unit_assault", "unit_assault", "unit_heavy", "unit_heavy"] }
        ];

        #endregion

        #region Dynamic Lances

        public static readonly List<DynamicLanceDifficulty_MDD> dynamicLanceDefs =
        [
            new() { DynamicLanceDifficultyID = 0, Difficulty = 0, NoUnitCount = 3, LightUnitCount = 1, PilotTags = "pilot_npc_d1" },
            new() { DynamicLanceDifficultyID = 1, Difficulty = 1, NoUnitCount = 2, LightUnitCount = 2, PilotTags = "pilot_npc_d1" },
            new() { DynamicLanceDifficultyID = 2, Difficulty = 1, NoUnitCount = 1, LightUnitCount = 3, PilotTags = "pilot_npc_d1" },
            new() { DynamicLanceDifficultyID = 3, Difficulty = 2, NoUnitCount = 1, LightUnitCount = 2, MediumUnitCount = 1, PilotTags = "pilot_npc_d2" },
            new() { DynamicLanceDifficultyID = 4, Difficulty = 2, NoUnitCount = 0, LightUnitCount = 4, PilotTags = "pilot_npc_d2" },
            new() { DynamicLanceDifficultyID = 5, Difficulty = 3, NoUnitCount = 0, LightUnitCount = 3, MediumUnitCount = 1, PilotTags = "pilot_npc_d3" },
            new() { DynamicLanceDifficultyID = 6, Difficulty = 3, NoUnitCount = 0, LightUnitCount = 2, MediumUnitCount = 2, PilotTags = "pilot_npc_d3" },
            new() { DynamicLanceDifficultyID = 7, Difficulty = 4, NoUnitCount = 0, LightUnitCount = 2, MediumUnitCount = 1, HeavyUnitCount = 1, PilotTags = "pilot_npc_d4" },
            new() { DynamicLanceDifficultyID = 8, Difficulty = 4, NoUnitCount = 0, LightUnitCount = 1, MediumUnitCount = 2, HeavyUnitCount = 1, PilotTags = "pilot_npc_d4" },
            new() { DynamicLanceDifficultyID = 9, Difficulty = 5, NoUnitCount = 0, MediumUnitCount = 4, PilotTags = "pilot_npc_d5" },
            new() { DynamicLanceDifficultyID = 10, Difficulty = 5, NoUnitCount = 0, LightUnitCount = 2, HeavyUnitCount = 2, PilotTags = "pilot_npc_d5" },
            new() { DynamicLanceDifficultyID = 11, Difficulty = 6, NoUnitCount = 0, LightUnitCount = 1, MediumUnitCount = 1, HeavyUnitCount = 2, PilotTags = "pilot_npc_d6" },
            new() { DynamicLanceDifficultyID = 12, Difficulty = 6, NoUnitCount = 0, MediumUnitCount = 3, HeavyUnitCount = 1, PilotTags = "pilot_npc_d6" },
            new() { DynamicLanceDifficultyID = 13, Difficulty = 7, NoUnitCount = 0, LightUnitCount = 1, HeavyUnitCount = 3, PilotTags = "pilot_npc_d7" },
            new() { DynamicLanceDifficultyID = 14, Difficulty = 7, NoUnitCount = 0, MediumUnitCount = 2, HeavyUnitCount = 2, PilotTags = "pilot_npc_d7" },
            new() { DynamicLanceDifficultyID = 15, Difficulty = 8, NoUnitCount = 0, MediumUnitCount = 1, HeavyUnitCount = 3, PilotTags = "pilot_npc_d8" },
            new() { DynamicLanceDifficultyID = 16, Difficulty = 8, NoUnitCount = 0, HeavyUnitCount = 4, PilotTags = "pilot_npc_d8" },
            new() { DynamicLanceDifficultyID = 17, Difficulty = 9, NoUnitCount = 0, MediumUnitCount = 1, HeavyUnitCount = 2, AssaultUnitCount = 1, PilotTags = "pilot_npc_d9" },
            new() { DynamicLanceDifficultyID = 18, Difficulty = 9, NoUnitCount = 0, HeavyUnitCount = 3, AssaultUnitCount = 1, PilotTags = "pilot_npc_d9" },
            new() { DynamicLanceDifficultyID = 19, Difficulty = 10, NoUnitCount = 0, MediumUnitCount = 1, HeavyUnitCount = 1, AssaultUnitCount = 2, PilotTags = "pilot_npc_d10" },
            new() { DynamicLanceDifficultyID = 20, Difficulty = 10, NoUnitCount = 0, HeavyUnitCount = 2, AssaultUnitCount = 2, PilotTags = "pilot_npc_d10" },
            new() { DynamicLanceDifficultyID = 21, Difficulty = 11, NoUnitCount = 0, HeavyUnitCount = 1, AssaultUnitCount = 3, PilotTags = "pilot_npc_d10" },
            new() { DynamicLanceDifficultyID = 22, Difficulty = 12, NoUnitCount = 0, AssaultUnitCount = 4, PilotTags = "pilot_npc_d10" }
        ];

        #endregion

        #region Artillery Vehicles & Spotters

        public class ArtilleryVehicleDef
        {
            private static readonly int[] ReferenceYears = [3025, 3039, 3049, 3058, 3067, 3075];

            public string DefId { get; set; }
            public int IntroYear { get; set; }

            /// <summary>
            /// Availability as arrays: [faction] => [availability at each reference year]
            /// </summary>
            public Dictionary<string, int[]> FactionAvailabilityByYear { get; set; }

            /// <summary>
            /// Checks if the unit is available for a given faction and year, returning the availability weight if so.
            /// </summary>
            public bool IsAvailable(string faction, int year, out int weight, bool isClan = false, bool isPeriphery = false)
            {
                if (year < IntroYear)
                {
                    weight = 0;
                    return false;
                }

                weight = GetAvailability(faction, year, isClan, isPeriphery);
                return weight > 0;
            }

            /// <summary>
            /// Gets the availability weight for a faction in a given year using linear interpolation between nearest reference years.
            /// </summary>
            public int GetAvailability(string faction, int year, bool isClan, bool isPeriphery)
            {
                var fallbackFaction = "ALL";
                if (isClan) fallbackFaction = "CLAN";
                else if (isPeriphery) fallbackFaction = "PERIPHERY";

                var availArray = FactionAvailabilityByYear.ContainsKey(faction)
                    ? FactionAvailabilityByYear[faction] : FactionAvailabilityByYear[fallbackFaction];
                if (availArray == null) return 0;

                int index = Array.FindLastIndex(ReferenceYears, y => y <= year);
                if (index < 0) return 0;

                if (index >= ReferenceYears.Length - 1 || ReferenceYears[index] == year)
                    return availArray[index];

                // Interpolate between nearest reference years
                int yearSpan = ReferenceYears[index + 1] - ReferenceYears[index];
                float lerpFactor = yearSpan > 0 ? (float)(year - ReferenceYears[index]) / yearSpan : 0f;
                return Mathf.RoundToInt(Mathf.Lerp(availArray[index], availArray[index + 1], lerpFactor));
            }
        }

        public static readonly List<ArtilleryVehicleDef> ArtilleryVehicles =
        [
            new() {
                DefId = "vehicledef_BALLISTA",
                IntroYear = 2480,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 7, 6, 0, 0, 0, 0 } },
                    { "PERIPHERY", new[] { 19, 18, 18, 25, 20, 3 } },
                    { "Davion", new[] { 9, 13, 0,  0,  0,  0 } },
                    { "Kurita", new[] { 11, 7, 0, 0, 0, 0 } },
                    { "Liao", new[] { 10, 16, 0, 0, 0, 0 } },
                    { "Marik", new[] { 7, 15, 0, 0, 0, 0 } },
                    { "Steiner", new[] { 11, 7, 0, 0, 0, 0 } },
                    { "Niops", new[] { 13, 13, 11, 13, 11, 6 } },
                }
            },
            new() {
                DefId = "vehicledef_CARRIER_ARROWIV",
                IntroYear = 3058,
                FactionAvailabilityByYear = new() {
                    { "Kurita", new[] { 0, 0, 0, 50, 50, 50 } },
                }
            },
            new() {
                DefId = "vehicledef_CHAPARRAL",
                IntroYear = 2611,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 0, 0, 9, 8, 3, 2 } },
                    { "Davion", new[] { 0,  0,  20, 20, 3,  2 } },
                    { "Kurita", new[] { 0, 0, 10, 9, 4, 2 } },
                    { "Liao", new[] { 0, 0, 27, 16, 3, 2 } },
                    { "Marik", new[] { 0, 0, 9, 13, 2, 3 } },
                    { "Steiner", new[] { 0, 0, 10, 9, 3, 3 } },
                    { "ComStar", new[] { 18, 18, 19, 23, 6, 5 } },
                    { "WordOfBlake", new[] { 0, 0, 19, 18, 52, 20 } },
                    { "Niops", new[] { 18, 18, 22, 26, 12, 7 } },
                }
            },
            new() {
                DefId = "vehicledef_CHAPARRAL_CASE",
                IntroYear = 3071,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "Davion", new[] { 0,  0,  0,  0,  0,  1 } },
                    { "Kurita", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "Liao", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "Marik", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "Steiner", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "ComStar", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "WordOfBlake", new[] { 0, 0, 0, 0, 0, 5 } },
                    { "Niops", new[] { 0, 0, 0, 0, 0, 2 } },
                }
            },
            new() {
                DefId = "vehicledef_CHAPARRAL_ERML",
                IntroYear = 3074,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "Davion", new[] { 0,  0,  0,  0,  0,  1 } },
                    { "Kurita", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "Liao", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "Marik", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "Steiner", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "ComStar", new[] { 0, 0, 0, 0, 0, 1 } },
                    { "WordOfBlake", new[] { 0, 0, 0, 0, 0, 5 } },
                    { "Niops", new[] { 0, 0, 0, 0, 0, 2 } },
                }
            },
            new() {
                DefId = "vehicledef_CHAPARRAL_MG",
                IntroYear = 3062,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "Davion", new[] { 0 , 0,  0,  0,  1,  1 } },
                    { "Kurita", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "Liao", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "Marik", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "Steiner", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "ComStar", new[] { 0, 0, 0, 0, 2, 1 } },
                    { "WordOfBlake", new[] { 0, 0, 0, 0, 13, 5 } },
                    { "Niops", new[] { 0, 0, 0, 0, 3, 2 } },
                }
            },
            new() {
                DefId = "vehicledef_CHAPARRAL_SRM",
                IntroYear = 2614,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "Davion", new[] { 0,  0,  0,  0,  1,  1 } },
                    { "Kurita", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "Liao", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "Marik", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "Steiner", new[] { 0, 0, 0, 0, 1, 1 } },
                    { "ComStar", new[] { 0, 0, 0, 0, 2, 1 } },
                    { "WordOfBlake", new[] { 0, 0, 0, 0, 13, 5 } },
                    { "Niops", new[] { 0, 0, 0, 0, 6, 3 } },
                }
            },
            new() {
                DefId = "vehicledef_DEMOLISHER_ARROWIV",
                IntroYear = 3062,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 0, 0, 0, 0, 31, 34 } },
                    { "PERIPHERY", new[] { 0, 0, 0, 0, 0, 77 } },
                    { "Davion", new[] { 0,  0,  0,  0,  80, 64 } },
                    { "Kurita", new[] { 0, 0, 0, 0, 51, 32 } },
                    { "Liao", new[] { 0, 0, 0, 0, 40, 32 } },
                    { "Marik", new[] { 0, 0, 0, 0, 36, 60 } },
                    { "Steiner", new[] { 0, 0, 0, 0, 29, 23 } },
                    { "ComStar", new[] { 0, 0, 0, 0, 19, 18 } },
                    { "WordOfBlake", new[] { 0, 0, 0, 0, 79, 41 } },
                    { "Niops", new[] { 0, 0, 0, 0, 0, 31 } },
                }
            },
            new() {
                DefId = "vehicledef_HUITZILOPOCHTLI",
                IntroYear = 2845,
                FactionAvailabilityByYear = new() {
                    { "CLAN", new[] { 45, 45, 45, 45, 45, 45 } },
                }
            },
            new() {
                DefId = "vehicledef_KARNOV_ARTILLERY",
                IntroYear = 3039,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 0, 25, 27, 23, 16, 17 } },
                    { "Davion", new[] { 0,  53, 58, 58, 20, 16 } },
                    { "Kurita", new[] { 0, 19, 20, 17, 18, 11 } },
                    { "Liao", new[] { 0, 65, 78, 45, 20, 16 } },
                    { "Marik", new[] { 0, 61, 27, 36, 13, 21    } },
                    { "Steiner", new[] { 0, 19, 20, 18, 14, 16 } },
                    { "ComStar", new[] { 0, 18, 19, 23, 13, 13 } },
                    { "WordOfBlake", new[] { 0, 0, 19, 18, 56, 29 } },
                }
            },
            new() {
                DefId = "vehicledef_LONGTOM-LT-MOB-25",
                IntroYear = 2602,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 12, 18, 28, 46, 19, 14 } },
                    { "PERIPHERY", new[] { 7, 9, 12, 25, 34, 8 } },
                    { "CLAN", new[] { 10, 10, 10, 10, 10, 10 } },
                    { "Davion", new[] { 17, 37, 58, 115, 24, 14 } },
                    { "Kurita", new[] { 19, 19, 29, 49, 30, 14 } },
                    { "Liao", new[] { 18, 46, 78, 91, 24, 14 } },
                    { "Marik", new[] { 12, 43, 27, 73, 15, 18 } },
                    { "Steiner", new[] { 20, 19, 29, 52, 25, 20 } },
                    { "ComStar", new[] { 6, 9, 11, 20, 11, 7 } },
                    { "WordOfBlake", new[] { 0, 0, 14, 19, 33, 12 } },
                    { "Niops", new[] { 5, 6, 8, 13, 18, 13 } },
                }
            },
            new() {
                DefId = "vehicledef_LONGTOM-LT-MOB-25F",
                IntroYear = 2695,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 0, 0, 0, 0, 3, 3 } },
                    { "PERIPHERY", new[] { 0, 0, 0, 0, 6, 1 } },
                    { "Davion", new[] { 0, 0, 0, 0, 4, 2 } },
                    { "Kurita", new[] { 0, 0, 0, 0, 5, 2 } },
                    { "Liao", new[] { 0, 0, 0, 0, 4, 2 } },
                    { "Marik", new[] { 0, 0, 0, 0, 3, 3 } },
                    { "Steiner", new[] { 0, 0, 0, 0, 4, 3 } },
                    { "ComStar", new[] { 0, 0, 2, 3, 3, 2 } },
                    { "WordOfBlake", new[] { 0, 0, 2, 7, 23, 8 } },
                    { "Niops", new[] { 0, 0, 0, 0, 3, 2 } },
                }
            },
            new() {
                DefId = "vehicledef_MARKSMAN",
                IntroYear = 2702,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 5, 0, 0, 0, 0, 0 } },
                    { "Davion", new[] { 7, 0, 0, 0, 0, 0 } },
                    { "Kurita", new[] { 9, 0, 0, 0, 0, 0 } },
                    { "Liao", new[] { 8, 0, 0, 0, 0, 0 } },
                    { "Marik", new[] { 5, 0, 0, 0, 0, 0 } },
                    { "Steiner", new[] { 9, 0, 0, 0, 0, 0 } },
                    { "ComStar", new[] { 4, 0, 0, 0, 0, 0 } },
                }
            },
            new() {
                DefId = "vehicledef_MARKSMAN_LPPC",
                IntroYear = 3068,
                FactionAvailabilityByYear = new() {
                    { "Steiner", new[] { 0, 0, 0, 0, 14, 23 } },
                }
            },
            new() {
                DefId = "vehicledef_PADILLA",
                IntroYear = 2620,
                FactionAvailabilityByYear = new() {
                    { "Liao", new[] { 0, 0, 27, 32, 20, 16 } },
                    { "ComStar", new[] { 13, 12, 13, 16, 9, 9 } },
                    { "WordOfBlake", new[] { 0, 0, 14, 13, 39, 20 } },
                    { "Niops", new[] { 13, 13, 15, 18, 21, 16 } },
                }
            },
            new() {
                DefId = "vehicledef_PILUM_ARROWIV",
                IntroYear = 3059,
                FactionAvailabilityByYear = new() {
                    { "Davion", new[] { 0, 0, 0, 41, 40, 32 } },
                    { "Steiner", new[] { 0, 0, 0, 9, 14, 16 } },
                }
            },
            new() {
                DefId = "vehicledef_REGULATOR_ARROWIV",
                IntroYear = 3064,
                FactionAvailabilityByYear = new() {
                    { "Liao", new[] { 0, 0, 0, 0, 81, 64 } },
                }
            },
            new() {
                DefId = "vehicledef_SCHILTRON-PRIME",
                IntroYear = 3059,
                FactionAvailabilityByYear = new() {
                    { "Davion", new[] { 0, 0, 0, 0, 14, 11 } },
                    { "Kurita", new[] { 0, 0, 0, 0, 18, 11 } },
                    { "Marik", new[] { 0, 0, 0, 0, 9, 15 } },
                    { "WordOfBlake", new[] { 0, 0, 0, 0, 39, 20 } },
                }
            },
            new() {
                DefId = "vehicledef_STURMFEUR_KALKI",
                IntroYear = 3068,
                FactionAvailabilityByYear = []
            },
            new() {
                DefId = "vehicledef_THOR",
                IntroYear = 2680,
                FactionAvailabilityByYear = new() {
                    { "ComStar", new[] { 9, 9, 9, 12, 4, 2 } },
                    { "WordOfBlake", new[] { 0, 0, 10, 6, 7, 3 } },
                    { "Niops", new[] { 9, 9, 11, 13, 15, 11 } },
                }
            },
            new() {
                DefId = "vehicledef_THOR_C3i",
                IntroYear = 3066,
                FactionAvailabilityByYear = new() {
                    { "ComStar", new[] { 0, 0, 0, 0, 3, 4 } },
                    { "WordOfBlake", new[] { 0, 0, 0, 0, 21, 12 } },
                }
            },
            new() {
                DefId = "vehicledef_THOR_CLAN",
                IntroYear = 2870,
                FactionAvailabilityByYear = new() {
                    { "CLAN", new[] { 45, 45, 45, 45, 45, 45 } },
                }
            },
            new() {
                DefId = "vehicledef_THUMPER",
                IntroYear = 2734,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 76, 51, 36, 23, 11, 12 } },
                    { "PERIPHERY", new[] { 75, 73, 70, 50, 40, 10 } },
                    { "Davion", new[] { 106, 105, 81, 58, 14, 11 } },
                    { "Kurita", new[] { 122, 55, 41, 25, 18, 11 } },
                    { "Liao", new[] { 114, 131, 110, 45, 14, 11 } },
                    { "Marik", new[] { 76, 121, 38, 36, 9, 9 } },
                    { "Steiner", new[] { 124, 55, 41, 26, 14, 16 } },
                    { "ComStar", new[] { 51, 35, 27, 23, 9, 9 } },
                    { "WordOfBlake", new[] { 0, 0, 23, 18, 39, 9 } },
                    { "Niops", new[] { 52, 51, 43, 26, 21, 16 } },
                }
            },
            new() {
                DefId = "vehicledef_THUMPER_TAV-1",
                IntroYear = 3072,
                FactionAvailabilityByYear = new() {
                    { "Marik", new[] { 0, 0, 0, 0, 0, 6 } },
                    { "WordOfBlake", new[] { 0, 0, 0, 0, 0, 12 } },
                }
            },
            new() {
                DefId = "vehicledef_YELLOWJACKET_ARROWIV",
                IntroYear = 3067,
                FactionAvailabilityByYear = new() {
                    { "ALL", new[] { 0, 0, 0, 0, 16, 17 } },
                    { "Davion", new[] { 0, 0, 0, 0, 57, 45 } },
                    { "Kurita", new[] { 0, 0, 0, 0, 25, 16 } },
                    { "Liao", new[] { 0, 0, 0, 0, 20, 16 } },
                    { "Marik", new[] { 0, 0, 0, 0, 13, 21 } },
                    { "Steiner", new[] { 0, 0, 0, 0, 0, 0 } },
                    { "ComStar", new[] { 0, 0, 0, 0, 19, 18 } },
                    { "WordOfBlake", new[] { 0, 0, 0, 0, 56, 29 } },
                }
            }
        ];

        public static readonly HashSet<string> SpotterVehicles =
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

        #endregion
    }
}