using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using static BTX_ExpansionPack.Core.Helpers.ArtilleryHelpers;

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
        /// Improves AI decision-making for artillery strikes by adding new targeting behaviors.
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

        public enum ArtilleryTargetingMode
        {
            Single,
            Cluster,
            Barrage,
            CounterBattery
        }

        public static Vector3 GetBestArtilleryPosition(Vector3 originalPosition, Weapon weapon)
        {
            var attacker = weapon.parent;

            int tactics = 4; // Default if pilot is null
            var pilot = attacker.GetPilot();
            if (pilot != null)
            {
                tactics = pilot.Tactics;
            }

            var potentialTargets = attacker.team.GetDetectedEnemyUnits()
                .Where(target => weapon.CanHitTargetPosition(attacker.CurrentPosition, target.CurrentPosition))
                .ToList();

            if (potentialTargets.Count == 0)
            {
                return originalPosition;
            }

            // Determine available targeting modes based on Tactics level
            var weights = new List<System.Tuple<ArtilleryTargetingMode, float>>();
            if (tactics < 4) // Basic tactics
            {
                weights.Add(System.Tuple.Create(ArtilleryTargetingMode.Single, 40f));
                weights.Add(System.Tuple.Create(ArtilleryTargetingMode.Cluster, 60f));
            }
            else if (tactics < 7) // Intermediate tactics
            {
                weights.Add(System.Tuple.Create(ArtilleryTargetingMode.Single, 30f));
                weights.Add(System.Tuple.Create(ArtilleryTargetingMode.Cluster, 45f));
                weights.Add(System.Tuple.Create(ArtilleryTargetingMode.Barrage, 25f));
            }
            else // Advanced tactics
            {
                weights.Add(System.Tuple.Create(ArtilleryTargetingMode.Single, 20f));
                weights.Add(System.Tuple.Create(ArtilleryTargetingMode.Cluster, 30f));
                weights.Add(System.Tuple.Create(ArtilleryTargetingMode.Barrage, 25f));
                weights.Add(System.Tuple.Create(ArtilleryTargetingMode.CounterBattery, 25f));
            }

            // Shuffle/draw modes according to relative weights and try them
            var triedModes = new HashSet<ArtilleryTargetingMode>();
            while (triedModes.Count < weights.Count)
            {
                var activeWeights = weights.Where(w => !triedModes.Contains(w.Item1)).ToList();
                float activeTotalWeight = activeWeights.Sum(w => w.Item2);
                if (activeTotalWeight <= 0f) break;

                float roll = (float)(new System.Random().NextDouble() * activeTotalWeight);
                float accum = 0f;
                var selectedMode = activeWeights.Last().Item1;
                foreach (var w in activeWeights)
                {
                    accum += w.Item2;
                    if (roll <= accum)
                    {
                        selectedMode = w.Item1;
                        break;
                    }
                }

                triedModes.Add(selectedMode);

                Vector3? targetPos = null;
                switch (selectedMode)
                {
                    case ArtilleryTargetingMode.Single:
                        targetPos = GetSingleTarget(potentialTargets);
                        break;
                    case ArtilleryTargetingMode.Cluster:
                        targetPos = GetClusterTarget(potentialTargets, weapon, attacker);
                        break;
                    case ArtilleryTargetingMode.Barrage:
                        targetPos = GetBarrageTarget(potentialTargets, weapon, attacker);
                        break;
                    case ArtilleryTargetingMode.CounterBattery:
                        targetPos = GetCounterBatteryTarget(potentialTargets);
                        break;
                }

                if (targetPos.HasValue)
                {
                    Main.Log.LogDebug($"[ArtilleryAI] {attacker.DisplayName} (Pilot Tactics: {tactics}) selected {selectedMode} targeting mode. Targeting position: {targetPos.Value}");
                    return targetPos.Value;
                }
            }

            return originalPosition;
        }

        /// <summary>
        /// Gets a single target, prioritizing stationary or slow enemies.
        /// </summary>
        private static Vector3? GetSingleTarget(List<AbstractActor> potentialTargets)
        {
            if (potentialTargets.Count == 1)
                return potentialTargets[0].CurrentPosition;

            var stationaryTargets = potentialTargets.Where(t => t.IsStationary()).ToList();
            return stationaryTargets.Count > 0
                ? stationaryTargets[Random.Range(0, stationaryTargets.Count)].CurrentPosition
                : potentialTargets.OrderBy(t => t.MovementCaps.MaxWalkDistance).First().CurrentPosition;
        }

        /// <summary>
        /// Gets a cluster target, prioritizing enemy units in close proximity.
        /// </summary>
        private static Vector3? GetClusterTarget(List<AbstractActor> potentialTargets, Weapon weapon, AbstractActor attacker) => potentialTargets.Count < 2 ? null : FindBestClusterPosition(potentialTargets, weapon.AOERange(), attacker.Combat.MapMetaData);

        /// <summary>
        /// Gets a barrage target, prioritizing enemies that are moving towards allied units.
        /// </summary>
        private static Vector3? GetBarrageTarget(List<AbstractActor> potentialTargets, Weapon weapon, AbstractActor attacker)
        {
            var allies = attacker.Combat.AllActors.Where(actor => !actor.IsDead && actor.team.IsFriendly(attacker.team)).ToList();
            var barrageCandidates = new Dictionary<AbstractActor, Vector3>();

            foreach (var target in potentialTargets)
            {
                if (target.IsStationary())
                    continue;

                // Project future position based on their movement vector
                var moveVec = target.CurrentPosition - target.PreviousPosition;
                if (moveVec.magnitude < 4f)
                    continue; // Didn't move enough to establish a direction
                var predictedPos = target.CurrentPosition + moveVec;
                predictedPos.y = attacker.Combat.MapMetaData.GetLerpedHeightAt(predictedPos);

                // Friendly fire check
                var alliesAtRisk = FindAlliesWithinRange(predictedPos, attacker.team, attacker.Combat.AllActors, weapon.AOERange());
                if (alliesAtRisk.Count > 0)
                    continue;

                barrageCandidates.Add(target, predictedPos);
            }

            if (barrageCandidates.Count == 0)
                return null;

            // Choose closest to an allied unit (defensive barrage)
            return barrageCandidates.OrderBy(kvp => Vector3.Distance(kvp.Value, allies.Min(a => a.CurrentPosition))).First().Value;
        }

        /// <summary>
        /// Gets a counter-battery target, targeting enemy artillery units and missile boats.
        /// </summary>
        private static Vector3? GetCounterBatteryTarget(List<AbstractActor> potentialTargets)
        {
            var artilleryUnits = potentialTargets.Where(target => target.IsArtilleryUnit()).ToList();
            if (artilleryUnits.Count > 0)
            {
                // Prioritize stationary artillery (likely aiming)
                var stationaryArtillery = artilleryUnits.Where(target => target.IsStationary()).ToList();
                return stationaryArtillery.Count > 0
                    ? stationaryArtillery[Random.Range(0, stationaryArtillery.Count)].CurrentPosition
                    : artilleryUnits[Random.Range(0, artilleryUnits.Count)].CurrentPosition;
            }

            var missileBoats = potentialTargets.Where(target => target.IsDedicatedMissileBoat()).ToList();
            return missileBoats.Count > 0
                ? missileBoats[Random.Range(0, missileBoats.Count)].CurrentPosition
                : null;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculates the best position for an artillery strike to maximize enemy damage.
        /// </summary>
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

        /// <summary>
        /// Calculates the score for a cluster of enemies based on their tonnage, count, and mobility.
        /// </summary>
        private static float ScoreCluster(ICollection<AbstractActor> cluster)
        {
            float totalTonnage = cluster.Sum(member => member.GetTonnage());
            float mobilityModifier = cluster.Average(m => m.GetTargetMobility());
            float clusterValue = totalTonnage * cluster.Count;

            return clusterValue * (1f + mobilityModifier);
        }

        /// <summary>
        /// Calculates the centroid (average position) of a cluster of enemies.
        /// </summary>
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