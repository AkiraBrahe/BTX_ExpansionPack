using BattleTech.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace BTX_ExpansionPack.Core.Helpers
{
    public static class LanceHelpers
    {
        #region Context Propagation

        internal static class LanceGenerationContext
        {
            [field: ThreadStatic]
            public static GenerationContext Current { get; private set; }

            public class GenerationContext(int difficulty, string lanceDefId, string factionId)
            {
                public int Difficulty { get; } = difficulty;
                public string LanceDefId { get; } = lanceDefId;
                public string FactionId { get; } = factionId;

            }

            public static void SetContext(int difficulty, string lanceDefId, string factionId)
            {
                Current = new GenerationContext(difficulty, lanceDefId, factionId);
            }

            public static void ClearContext()
            {
                Current = null;
            }
        }

        /// <summary>
        /// Stores information about the current lance generation.
        /// </summary>
        [HarmonyPatch(typeof(UnitSpawnPointOverride), "GenerateUnit")]
        public static class UnitSpawnPointOverride_GenerateUnit
        {
            [HarmonyPrefix]
            public static void Prefix(int contractDifficulty, string lanceDefId)
            {
                LanceGenerationContext.SetContext(contractDifficulty, lanceDefId, null);
            }

            [HarmonyPostfix]
            public static void Postfix()
            {
                LanceGenerationContext.ClearContext();
            }
        }

        #endregion

        #region Lance Generation

        /// <summary>
        /// Gets the parent faction of the given faction identifier.
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