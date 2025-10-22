using BattleTech;
using CustAmmoCategories;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace BTX_ExpansionPack.Fixes.Targeting
{
    internal class ArtilleryTargeting
    {
        /// <summary>
        /// Improves AI decision-making for artillery strikes by targeting clusters of enemies.
        /// </summary>
        [HarmonyPatch(typeof(AITeam_makeInvocationFromOrders), "Postfix")]
        public static class AITeam_makeInvocationFromOrders_Postfix
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions, il);

                matcher.End();
                for (int i = 0; i < 2; i++)
                {
                    matcher.MatchBack(false,
                        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(WeaponArtilleryHelper), "AddArtilleryStrike")));
                    matcher.Advance(-1);
                    matcher.Insert(
                        new CodeInstruction(OpCodes.Ldloc_S, 7),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AITeam_makeInvocationFromOrders_Postfix), "GetBestArtilleryPosition")));
                }

                return matcher.InstructionEnumeration();
            }

            public static Vector3 GetBestArtilleryPosition(Vector3 originalPosition, Weapon weapon)
            {
                var attacker = weapon.parent;
                float AOERange = weapon.AOERange();

                var detectedEnemies = attacker.team.GetDetectedEnemyUnits().ToList();
                if (detectedEnemies.Count < 2) return originalPosition;

                Vector3? bestPosition = null;
                float bestScore = 0f;

                // Iterate through all detected enemies to find the center of the best cluster
                foreach (var primaryTarget in detectedEnemies)
                {
                    var cluster = new List<AbstractActor> { primaryTarget };
                    foreach (var secondaryTarget in detectedEnemies)
                    {
                        if (primaryTarget == secondaryTarget) continue;

                        float distance = Vector3.Distance(primaryTarget.CurrentPosition, secondaryTarget.CurrentPosition);
                        if (distance <= AOERange * 2f)
                            cluster.Add(secondaryTarget);
                    }

                    if (cluster.Count < 2) continue;

                    // Calculate the geometric center and total value of the cluster
                    var clusterCenter = Vector3.zero;
                    float totalTonnage = 0f;
                    foreach (var member in cluster)
                    {
                        clusterCenter += member.CurrentPosition;
                        totalTonnage += member.GetTonnage(); // Using tonnage as a proxy for threat level
                    }
                    clusterCenter /= cluster.Count;
                    clusterCenter.y = attacker.Combat.MapMetaData.GetLerpedHeightAt(clusterCenter);

                    float currentScore = totalTonnage * cluster.Count;
                    if (currentScore > bestScore)
                    {
                        bestScore = currentScore;
                        bestPosition = clusterCenter;
                    }
                }

                if (bestPosition != null)
                {
                    Main.Log.LogDebug($"[ArtilleryAI] Retargeting {attacker.DisplayName}'s artillery strike to a cluster of enemies at {bestPosition.Value}");
                    return bestPosition.Value;
                }

                return originalPosition;
            }
        }
    }
}