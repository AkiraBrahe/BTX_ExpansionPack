using BattleTech;
using BattleTech.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BTX_ExpansionPack.DatabaseHelpers;

namespace BTX_ExpansionPack.Features
{
    internal partial class AdditionalLances
    {
        /// <summary>
        /// Intercepts ComStar lance spawns to enforce specific lore-based compositions.
        /// </summary>
        [HarmonyPatch(typeof(UnitSpawnPointOverride), "RequestUnit")]
        public static class UnitSpawnPointOverride_RequestUnit_Comstar
        {
            private const string PrimaryComStarLance = "Lance_HostileAll_Primary";
            private const string SecondaryComStarLance = "Lance_HostileAll_Secondary";

            private static readonly Dictionary<string, List<string>> lanceCompositionAssignments = [];

            [HarmonyPatch(typeof(Contract), "BeginRequestResources")]
            [HarmonyPatch(typeof(Contract), "ResetStateForRestart", [])]
            public static class Contract_ClearLanceAssignments
            {
                [HarmonyPrefix]
                public static void Prefix() => lanceCompositionAssignments.Clear();
            }

            [HarmonyPrefix]
            public static void Prefix(UnitSpawnPointOverride __instance, string lanceDefId, string lanceName, int unitIndex)
            {
                if (!lanceDefId.StartsWith("lancedef_comstar"))
                    return;

                if (lanceName is not PrimaryComStarLance and not SecondaryComStarLance)
                    return;

                List<string> selectedComposition;

                if (unitIndex == 0)
                {
                    List<LanceComposition> targetList = [.. ComstarLightLevelIIs, .. ComstarMediumLevelIIs];
                    selectedComposition = GetRandomComposition(targetList);
                    lanceCompositionAssignments[lanceName] = selectedComposition;
                    Main.Log.LogDebug($"Selected ComStar Level II composition for lance '{lanceName}': {string.Join(", ", selectedComposition)}");
                }
                else
                {
                    if (!lanceCompositionAssignments.TryGetValue(lanceName, out selectedComposition))
                    {
                        Main.Log.LogDebug($"Error: No composition found for lance '{lanceName}' at unitIndex {unitIndex}.");
                        return;
                    }
                }

                if (unitIndex >= selectedComposition.Count) return;
                string newWeightTag = selectedComposition[unitIndex];
                __instance.unitTagSet.RemoveRange(weightClassTags);
                __instance.unitTagSet.Add(newWeightTag);
                Main.Log.LogDebug($"Applied tag '{newWeightTag}' to unit {unitIndex} in lance '{lanceName}'.");
            }

            private static List<string> GetRandomComposition(List<LanceComposition> compositions)
            {
                if (compositions == null || compositions.Count == 0) return [];

                int totalWeight = compositions.Sum(c => c.Weight);
                int randomNumber = Random.Range(0, totalWeight);

                foreach (var composition in compositions)
                {
                    if (randomNumber < composition.Weight)
                    {
                        return composition.UnitWeightTags;
                    }
                    randomNumber -= composition.Weight;
                }
                return compositions.Last().UnitWeightTags;
            }
        }
    }
}