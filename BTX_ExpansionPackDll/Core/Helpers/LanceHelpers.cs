using BattleTech.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BTX_ExpansionPack.Core.Helpers
{
    public static class LanceHelpers
    {
        #region Context Propagation

        internal static class LanceGenerationContext
        {
            // Dictionary to store contexts per lance name, protected by a lock for thread safety.
            private static readonly Dictionary<string, GenerationContext> contextDictionary = [];
            private static readonly object contextLock = new();

            public class GenerationContext(int difficulty, string lanceDefId, string factionId)
            {
                public int Difficulty { get; } = difficulty;
                public string LanceDefId { get; } = lanceDefId;
                public string FactionId { get; } = factionId;

            }

            public static void StoreContext(string lanceName, int difficulty, string lanceDefId, string factionId)
            {
                lock (contextLock)
                {
                    contextDictionary[lanceName] = new GenerationContext(difficulty, lanceDefId, factionId);
                }
            }

            public static GenerationContext GetContext(string lanceName)
            {
                lock (contextLock)
                {
                    return contextDictionary.TryGetValue(lanceName, out var context) ? context : null;
                }
            }

            public static void ClearAllContexts()
            {
                lock (contextLock)
                {
                    contextDictionary.Clear();
                }
            }
        }

        /// <summary>
        /// Stores information about the lance being generated.
        /// </summary>
        [HarmonyPatch(typeof(LanceOverride), "RequestLance")]
        public static class LanceOverride_RequestLance_ContextPatch
        {
            [HarmonyPrefix]
            public static void Prefix(LanceOverride __instance, int requestedDifficulty)
            {
                int adjustedDifficulty = requestedDifficulty + __instance.lanceDifficultyAdjustment;
                __instance.selectedLanceDifficulty = adjustedDifficulty;

                LanceGenerationContext.StoreContext(
                    __instance.name,
                    adjustedDifficulty,
                    __instance.lanceDefId,
                    __instance.teamOverride.FactionValue.Name
                );
            }
        }

        #endregion

        #region Lance Generation

        /// <summary>
        /// Gets the parent faction of the given faction identifier. Example: "MarikC25" -> "Marik".
        /// </summary>
        public static string GetParentFaction(string factionIdentifier)
        {
            if (string.IsNullOrEmpty(factionIdentifier))
                return "General";

            try
            {
                var unitTableRefs = FullXotlTables.Core.Settings.UnitTableReferences;
                if (unitTableRefs != null && unitTableRefs.TryGetValue(factionIdentifier, out var refData) && refData != null)
                {
                    return refData.Vehicles;
                }
            }
            catch (Exception)
            {
                return factionIdentifier;
            }

            return factionIdentifier;
        }

        /// <summary>
        /// Selects a random unit from a pool of units using weighted selection.
        /// </summary>
        public static string WeightedRandomSelect(Dictionary<string, int> pool)
        {
            int totalWeight = pool.Sum(kv => kv.Value);
            int roll = Random.Range(0, totalWeight);

            foreach (var kv in pool)
            {
                if (roll < kv.Value)
                    return kv.Key;
                roll -= kv.Value;
            }

            return pool.Keys.Last();
        }

        /// <summary>
        /// Gets the appropriate composition list and its difficulty range for ComStar lances.
        /// </summary>
        public static (List<LanceComposition> list, int min, int max) GetComStarCompositionTier(int difficulty)
        {
            return difficulty switch
            {
                <= 3 => (ComstarLightLevelIIs, 0, 3),
                <= 6 => (ComstarMediumLevelIIs, 4, 6),
                <= 9 => (ComstarHeavyLevelIIs, 7, 9),
                _ => (ComstarAssaultLevelIIs, 10, 12)
            };
        }

        /// <summary>
        /// Gets the appropriate composition list and its difficulty range for Clan lances.
        /// </summary>
        public static (List<LanceComposition> list, int min, int max) GetClanCompositionTier(int difficulty)
        {
            return difficulty switch
            {
                <= 3 => (ClanLightStars, 0, 3),
                <= 7 => (ClanMediumStars, 4, 7),
                _ => (ClanHeavyStars, 8, 12)
            };
        }

        /// <summary>
        /// Downgrades the given list of lance compositions to a lower tier based on the faction type.
        /// </summary>
        public static List<LanceComposition> DowngradeCompositions(List<LanceComposition> compositions, bool isComStar = false)
        {
            if (compositions == null || compositions.Count == 0) return [];

            if (isComStar)
            {
                if (compositions == ComstarAssaultLevelIIs)
                    compositions = ComstarHeavyLevelIIs;
                else if (compositions == ComstarHeavyLevelIIs)
                    compositions = ComstarMediumLevelIIs;
                else if (compositions == ComstarMediumLevelIIs)
                    compositions = ComstarLightLevelIIs;
            }
            else
            {
                if (compositions == ClanHeavyStars)
                    compositions = ClanMediumStars;
                else if (compositions == ClanMediumStars)
                    compositions = ClanLightStars;
            }

            return compositions;
        }

        /// <summary>
        /// Applies a difficulty-based weight boost to compositions, favoring those closest to the requested difficulty.
        /// </summary>
        public static List<LanceComposition> ApplyDifficultyWeighting(
            List<LanceComposition> compositions,
            int requestedDifficulty,
            int difficultyMin,
            int difficultyMax)
        {
            if (compositions == null || compositions.Count == 0) return compositions;

            // Normalize requested difficulty to 0-1 range
            float normalizedDifficulty = (float)(requestedDifficulty - difficultyMin) / (difficultyMax - difficultyMin);
            normalizedDifficulty = Mathf.Clamp01(normalizedDifficulty);

            // Calculate ideal index position
            float idealIndex = normalizedDifficulty * (compositions.Count - 1);

            // Apply weight multipliers based on proximity using Gaussian-like distribution
            var weightedCompositions = new List<LanceComposition>();
            for (int i = 0; i < compositions.Count; i++)
            {
                var comp = compositions[i];
                float distance = Mathf.Abs(i - idealIndex);
                float proximityMultiplier = 1f + (Mathf.Exp(-distance * distance / 0.5f) / 2f); // Up to +50% boost for closest compositions
                int boostedWeight = Mathf.Max(comp.Weight, Mathf.RoundToInt(comp.Weight * proximityMultiplier));

                weightedCompositions.Add(new LanceComposition
                {
                    Weight = boostedWeight,
                    UnitWeightTags = comp.UnitWeightTags
                });
            }

            return weightedCompositions;
        }

        /// <summary>
        /// Selects a random composition from a list of lance compositions using weighted selection.
        /// </summary>
        public static List<string> GetRandomComposition(List<LanceComposition> compositions)
        {
            if (compositions == null || compositions.Count == 0) return [];

            int totalWeight = compositions.Sum(c => c.Weight);
            int roll = Random.Range(0, totalWeight);

            foreach (var composition in compositions)
            {
                if (roll < composition.Weight)
                    return composition.UnitWeightTags;
                roll -= composition.Weight;
            }
            return compositions.Last().UnitWeightTags;
        }

        #endregion
    }
}