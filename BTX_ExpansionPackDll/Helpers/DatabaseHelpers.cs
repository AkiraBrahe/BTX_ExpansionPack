using BattleTech.Data;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack
{
    public static class DatabaseHelpers
    {
        public class LanceComposition
        {
            public int Weight { get; set; }
            public List<string> UnitWeightTags { get; set; }
        }

        // --- ComStar ---
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

        // --- Clans ---
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

        // --- Dynamic Difficulty ---
        public static List<DynamicLanceDifficulty_MDD> lanceDefs =
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

        public static void ClearDynamicLanceDifficulty(this MetadataDatabase mdd)
        {
            Main.Log.LogDebug("Clearing all entries from DynamicLanceDifficulty table.");
            mdd.Execute("DELETE FROM DynamicLanceDifficulty");
        }

        public static void BulkInsertDynamicLanceDifficulty(this MetadataDatabase mdd, IEnumerable<DynamicLanceDifficulty_MDD> defs)
        {
            Main.Log.LogDebug($"Bulk inserting {defs.Count()} new entries into DynamicLanceDifficulty table.");
            mdd.Execute(
                @"INSERT INTO DynamicLanceDifficulty 
                  (DynamicLanceDifficultyID, Difficulty, NoUnitCount, LightUnitCount, MediumUnitCount, HeavyUnitCount, AssaultUnitCount, PilotTags)
                  VALUES (@DynamicLanceDifficultyID, @Difficulty, @NoUnitCount, @LightUnitCount, @MediumUnitCount, @HeavyUnitCount, @AssaultUnitCount, @PilotTags)",
                defs);
        }
    }
}