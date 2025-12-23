using BattleTech;
using BattleTech.UI;
using BTX_ExpansionPack.Helpers;
using CustAmmoCategories;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace BTX_ExpansionPack.Fixes.Targeting
{
    internal class ArtilleryTargeting
    {
        #region Ground Attack Restrictions

        /// <summary>
        /// Prevents ground attacks with weapons that have Homing ammo or minimum/forbidden range restrictions.
        /// </summary>
        [HarmonyPatch(typeof(SelectionStateCommandAttackGround), "ProcessLeftClick")]
        public static class SelectionStateCommandAttackGround_ProcessLeftClick
        {
            [HarmonyPostfix]
            public static void Postfix(SelectionStateCommandAttackGround __instance, Vector3 worldPos, ref bool __result)
            {
                if (__result == false) return;

                var actor = __instance.SelectedActor;
                foreach (var weapon in actor.Weapons)
                {
                    if (!weapon.IsFunctional || !weapon.IsEnabled || weapon.isAMS())
                        continue;

                    // Prevent ground attack with Homing ammo
                    if (weapon.ammo()?.Id == "Ammunition_ArrowIV_Homing")
                    {
                        GenericPopupBuilder.Create(
                            $"Invalid Target",
                            $"Arrow IV homing missiles can only target enemy units directly.")
                            .AddButton("Ok")
                            .IsNestedPopupWithBuiltInFader()
                            .CancelOnEscape()
                            .Render();
                        return;
                    }

                    // Prevent ground attack within minimum or forbidden ranges
                    if (!weapon.IsOutsideSafeRange(actor.CurrentPosition, worldPos, out float unsafeRange))
                    {
                        float distance = Vector3.Distance(actor.CurrentPosition, worldPos);
                        float minRange = weapon.MinRange;
                        string message = distance < minRange
                            ? $"Your {weapon.Name} requires a minimum range of {unsafeRange:F0}m to attack."
                            : $"Your {weapon.Name} requires a safe range of {unsafeRange:F0}m to attack.";
                        message += $"\nCurrent distance: {distance:F0}m.";

                        GenericPopupBuilder.Create(
                            $"Target Too Close",
                            message)
                            .AddButton("Ok")
                            .IsNestedPopupWithBuiltInFader()
                            .CancelOnEscape()
                            .Render();
                        return;
                    }
                }
            }
        }

        #endregion

        #region Artillery AI Targeting

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
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ArtilleryTargeting), "GetBestArtilleryPosition")));
                }

                return matcher.InstructionEnumeration();
            }
        }

        public static Vector3 GetBestArtilleryPosition(Vector3 originalPosition, Weapon weapon)
        {
            var attacker = weapon.parent;

            var detectedEnemies = attacker.team.GetDetectedEnemyUnits().Where(target => !target.IsDead && weapon.IsOutsideSafeRange(attacker.CurrentPosition, target.CurrentPosition, out _)).ToList();
            if (detectedEnemies.Count < 2)
            {
                return originalPosition;
            }

            var bestPosition = FindBestClusterPosition(detectedEnemies, weapon.AOERange(), attacker.Combat.MapMetaData);
            if (bestPosition.HasValue)
            {
                Main.Log.LogDebug($"[ArtilleryAI] Retargeting {attacker.DisplayName}'s artillery strike to a cluster of enemies at {bestPosition.Value}");
                return bestPosition.Value;
            }

            return originalPosition;
        }

        private static Vector3? FindBestClusterPosition(List<AbstractActor> detectedEnemies, float aoeRange, MapMetaData mapMetaData)
        {
            Vector3? bestPosition = null;
            float bestScore = 0f;

            foreach (var primaryTarget in detectedEnemies)
            {
                var cluster = FindNearbyEnemies(primaryTarget, detectedEnemies, aoeRange);
                if (cluster.Count < 2)
                    continue;

                float currentScore = ScoreCluster(cluster);
                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestPosition = CalculateCentroid(cluster, mapMetaData);
                }
            }
            return bestPosition;
        }

        private static List<AbstractActor> FindNearbyEnemies(AbstractActor primaryTarget, IEnumerable<AbstractActor> allEnemies, float aoeRange) =>
            [.. allEnemies.Where(enemy => Vector3.Distance(primaryTarget.CurrentPosition, enemy.CurrentPosition) <= aoeRange * 2f)];

        private static float ScoreCluster(ICollection<AbstractActor> cluster)
        {
            float totalTonnage = cluster.Sum(member => member.GetTonnage());
            return totalTonnage * cluster.Count;
        }

        private static Vector3 CalculateCentroid(IEnumerable<AbstractActor> cluster, MapMetaData mapMetaData)
        {
            if (!cluster.Any()) return Vector3.zero;
            var centroid = new Vector3(cluster.Average(member => member.CurrentPosition.x), 0, cluster.Average(member => member.CurrentPosition.z));
            centroid.y = mapMetaData.GetLerpedHeightAt(centroid);
            return centroid;
        }

        #endregion
    }
}