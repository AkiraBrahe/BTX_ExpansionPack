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
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ArtilleryTargeting), "GetArtilleryTargetPosition")));
                }

                return matcher.InstructionEnumeration();
            }
        }

        public static Vector3 GetArtilleryTargetPosition(Vector3 originalPosition, Weapon weapon)
        {
            var attacker = weapon.parent;
            var potentialTargets = attacker.team.GetDetectedEnemyUnits()
                .Where(target => weapon.CanHitTargetPosition(attacker.CurrentPosition, target.CurrentPosition))
                .ToList();

            if (potentialTargets.Count == 0)
                return originalPosition;

            var pilot = attacker.GetPilot();
            int tactics = pilot?.Tactics ?? 4;

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

            // If allies have called strikes this round, try to use the same mode
            var teamStrikes = GetTeamStrikesThisRound(attacker);
            if (teamStrikes.Count > 0)
            {
                foreach (var strike in teamStrikes)
                {
                    var mode = strike.Mode;
                    weights = [.. weights.Select(w => w.Item1 == mode
                        ? System.Tuple.Create(w.Item1, w.Item2 * 2f)
                        : w)];
                }
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

                float aoeRange = weapon.AOERange();
                var targetPos = selectedMode switch
                {
                    ArtilleryTargetingMode.Single => GetSingleTarget(potentialTargets, teamStrikes),
                    ArtilleryTargetingMode.Cluster => GetClusterTarget(potentialTargets, attacker, aoeRange, teamStrikes),
                    ArtilleryTargetingMode.Barrage => GetBarrageTarget(potentialTargets, attacker, aoeRange, teamStrikes),
                    ArtilleryTargetingMode.CounterBattery => GetCounterBatteryTarget(potentialTargets, teamStrikes),
                    _ => null
                };

                if (targetPos.HasValue)
                {
                    var combat = UnityGameInstance.BattleTechGame.Combat;
                    int currentRound = combat.TurnDirector.CurrentRound;

                    RecordArtilleryStrike(new ArtilleryStrikeData
                    {
                        Round = currentRound,
                        TeamGUID = attacker.team.GUID,
                        TargetPosition = targetPos.Value,
                        Mode = selectedMode
                    });

                    Main.Log.LogDebug($"[ArtilleryAI] {attacker.DisplayName} (Pilot Tactics: {tactics}) selected {selectedMode} targeting mode. Targeting position: {targetPos.Value}");
                    return targetPos.Value;
                }
            }

            return originalPosition;
        }

        /// <summary>
        /// Returns a single target position, prioritizing the slowest enemy that hasn't been targeted yet.
        /// </summary>
        private static Vector3? GetSingleTarget(List<AbstractActor> potentialTargets, List<ArtilleryStrikeData> teamStrikes)
        {
            if (potentialTargets.Count == 1)
                return potentialTargets[0].CurrentPosition;

            var targetedPositions = teamStrikes.Select(s => s.TargetPosition).ToList();
            var untargetedTargets = potentialTargets.Where(t => !targetedPositions.Any(pos => Vector3.Distance(pos, t.CurrentPosition) < 1f)).ToList();

            return untargetedTargets.Count > 0
                ? untargetedTargets.OrderBy(t => t.MovementCaps != null ? t.MovementCaps.MaxWalkDistance : 0f).First().CurrentPosition
                : potentialTargets[Random.Range(0, potentialTargets.Count)].CurrentPosition;
        }

        /// <summary>
        /// Returns a cluster target position, prioritizing enemies in close proximity to each other.
        /// </summary>
        private static Vector3? GetClusterTarget(List<AbstractActor> potentialTargets, AbstractActor attacker, float aoeRange, List<ArtilleryStrikeData> teamStrikes)
        {
            if (potentialTargets.Count < 2)
                return null; // Not enough targets for cluster

            var targetedPositions = teamStrikes.Select(s => s.TargetPosition).ToList();

            return FindBestClusterPosition(potentialTargets, aoeRange, attacker.Combat.MapMetaData, targetedPositions);
        }

        /// <summary>
        /// Gets a barrage target, prioritizing enemies that are moving towards allied units.
        /// </summary>
        private static Vector3? GetBarrageTarget(List<AbstractActor> potentialTargets, AbstractActor attacker, float aoeRange, List<ArtilleryStrikeData> teamStrikes)
        {
            var allies = attacker.Combat.AllActors.Where(a => a != attacker && !a.IsDead && a.team.IsFriendly(attacker.team)).ToList();
            if (allies.Count == 0)
                return null; // No allies to defend

            int barrageStrikeCount = teamStrikes.Count(s => s.Mode == ArtilleryTargetingMode.Barrage);
            if (barrageStrikeCount >= 2)
                return null; // Already launched 2 barrages

            var barrageStrikePositions = teamStrikes.Where(s => s.Mode == ArtilleryTargetingMode.Barrage).Select(s => s.TargetPosition).ToList();
            var previousBarragePos = barrageStrikePositions.Any() ? barrageStrikePositions[0] : Vector3.zero;
            return FindBestBarragePosition(potentialTargets, aoeRange, attacker, allies, previousBarragePos);
        }

        /// <summary>
        /// Gets a counter-battery target, targeting enemy artillery units and missile boats.
        /// </summary>
        private static Vector3? GetCounterBatteryTarget(List<AbstractActor> potentialTargets, List<ArtilleryStrikeData> teamStrikes)
        {
            var targetedPositions = teamStrikes.Select(s => s.TargetPosition).ToList();
            var untargetedTargets = potentialTargets.Where(t => !targetedPositions.Any(pos => Vector3.Distance(pos, t.CurrentPosition) < 1f)).ToList();

            var artilleryUnits = untargetedTargets.Where(target => target.IsArtilleryUnit()).ToList();
            if (artilleryUnits.Count > 0)
            {
                var stationaryArtillery = artilleryUnits.Where(target => target.IsStationary() || target.IsInArtilleryMode()).ToList();
                return stationaryArtillery.Count > 0
                    ? stationaryArtillery[Random.Range(0, stationaryArtillery.Count)].CurrentPosition
                    : artilleryUnits[Random.Range(0, artilleryUnits.Count)].CurrentPosition;
            }

            var missileBoats = untargetedTargets.Where(target => target.IsDedicatedMissileBoat()).ToList();
            return missileBoats.Count > 0
                ? missileBoats[Random.Range(0, missileBoats.Count)].CurrentPosition
                : null;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Finds the best position for a cluster artillery strike to maximize enemy damage.
        /// </summary>
        private static Vector3? FindBestClusterPosition(List<AbstractActor> potentialTargets, float aoeRange, MapMetaData mapMetaData, List<Vector3> targetedPositions)
        {
            Vector3? bestPosition = null;
            float bestScore = 0f;

            foreach (var target in potentialTargets)
            {
                var cluster = FindNearbyEnemies(target, potentialTargets, aoeRange);
                if (cluster.Count < 2)
                    continue;

                var candidatePos = CalculateCentroid(cluster, mapMetaData);

                // Check if too close to recent cluster strikes
                float minDistToRecent = targetedPositions.Any()
                    ? targetedPositions.Min(pos => Vector3.Distance(pos, candidatePos))
                    : float.MaxValue;

                if (minDistToRecent < aoeRange * 0.5f)
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
            var centroid = new Vector3(cluster.Average(m => m.CurrentPosition.x), 0, cluster.Average(m => m.CurrentPosition.z));
            centroid.y = mapMetaData.GetLerpedHeightAt(centroid);
            return centroid;
        }

        /// <summary>
        /// Finds the best barrage position to block enemy movement towards allies.
        /// </summary>
        private static Vector3? FindBestBarragePosition(List<AbstractActor> potentialTargets, float aoeRange,
            AbstractActor attacker, List<AbstractActor> allies, Vector3 previousBarragePos)
        {
            var mapMetaData = attacker.Combat.MapMetaData;

            // Build list of moving targets with threat assessment
            var threateningTargets = potentialTargets
                .Select(t => new TargetMovementData
                {
                    Target = t,
                    CurrentPos = t.CurrentPosition,
                    MoveVector = t.CurrentPosition - t.PreviousPosition,
                    PredictedPos = t.CurrentPosition + (t.CurrentPosition - t.PreviousPosition),
                    ClosestAllyPos = allies.OrderBy(a => Vector3.Distance(a.CurrentPosition, t.CurrentPosition)).First().CurrentPosition
                })
                .Where(t => t.IsBlockableThreat())
                .OrderBy(t => Vector3.Distance(t.PredictedPos, t.ClosestAllyPos))
                .ToList();

            if (threateningTargets.Count == 0)
                return null;

            float minDistToAlly = threateningTargets.Min(t => Vector3.Distance(t.CurrentPos, t.ClosestAllyPos));
            if (minDistToAlly < 120f)
                return null; // Too close for effective suppression

            return previousBarragePos == Vector3.zero
                ? SelectPrimaryBarrageTarget(threateningTargets, aoeRange, mapMetaData)
                : SelectSecondaryBarrageTarget(threateningTargets, previousBarragePos, aoeRange, mapMetaData);
        }

        /// <summary>
        /// Selects the primary barrage target: strike the most threatening enemy or their cluster.
        /// </summary>
        private static Vector3? SelectPrimaryBarrageTarget(List<TargetMovementData> threateningTargets,
            float aoeRange, MapMetaData mapMetaData)
        {
            var primaryThreat = threateningTargets.First(); // Closest to ally
            var nearbyThreats = threateningTargets
                .Where(t => Vector3.Distance(t.PredictedPos, primaryThreat.PredictedPos) <= aoeRange * 1.5f)
                .ToList();

            Vector3 targetPos;
            if (nearbyThreats.Count >= 2)
                targetPos = CalculateCentroid(nearbyThreats.Select(t => t.Target), mapMetaData);
            else
                targetPos = primaryThreat.PredictedPos;

            targetPos.y = mapMetaData.GetLerpedHeightAt(targetPos);
            return targetPos;
        }

        /// <summary>
        /// Selects the secondary barrage target: loops through unsuppressed targets to find the next best position.
        /// </summary>
        private static Vector3? SelectSecondaryBarrageTarget(List<TargetMovementData> threateningTargets,
            Vector3 previousBarragePos, float aoeRange, MapMetaData mapMetaData)
        {
            var unsuppressedTargets = threateningTargets
                .Where(t => Vector3.Distance(t.PredictedPos, previousBarragePos) > aoeRange * 0.5f)
                .OrderByDescending(t => t.MoveVector.magnitude)
                .ToList();

            if (unsuppressedTargets.Count == 0)
                return null;

            foreach (var target in unsuppressedTargets)
            {
                if (Vector3.Distance(target.PredictedPos, previousBarragePos) <= aoeRange * 0.5f)
                    continue;

                var nearbyThreats = unsuppressedTargets
                    .Where(t => Vector3.Distance(t.PredictedPos, target.PredictedPos) <= aoeRange * 1.5f)
                    .ToList();

                Vector3 targetPos;
                if (nearbyThreats.Count >= 2)
                    targetPos = CalculateCentroid(nearbyThreats.Select(t => t.Target), mapMetaData);
                else
                    targetPos = target.PredictedPos;

                if (Vector3.Distance(targetPos, previousBarragePos) < aoeRange * 0.5f)
                    continue;

                targetPos.y = mapMetaData.GetLerpedHeightAt(targetPos);
                return targetPos;
            }

            return null;
        }

        #endregion
    }
}