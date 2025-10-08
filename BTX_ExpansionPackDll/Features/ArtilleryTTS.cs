using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using System.Linq;
using UnityEngine;

namespace BTX_ExpansionPack.Features
{
    public static class ArtilleryTTS
    {
        /// <summary>
        /// Adjusts the artillery strike's target position based on the weapon's TTS level.
        /// </summary>
        [HarmonyPatch(typeof(WeaponArtilleryHelper), "CreateStrikeInvocation")]
        public static class WeaponArtilleryHelper_CreateStrikeInvocation
        {
            [HarmonyPrefix]
            [HarmonyWrapSafe]
            public static void Prefix(AbstractActor unit)
            {
                var weapons = unit.GetArtilleryStrike(out var position);
                if (weapons.Count == 0) return;

                var closestTarget = FindClosestEnemy(unit, position, out float distanceToTarget);
                if (closestTarget != null && distanceToTarget > 0f)
                {
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        var weapon = weapons[i];
                        int ttsLevel = weapon.ArtilleryTTSLevel();
                        if (ttsLevel == 0)
                            continue;

                        float minMissRadius = weapon.MinMissRadius();
                        var newPos = CalculateAdjustedStrikePosition(position, closestTarget.CurrentPosition, distanceToTarget, ttsLevel, minMissRadius);
                        weapon.AddArtilleryStrike(newPos, i + 1);

                        float newDistanceToTarget = Vector3.Distance(newPos, closestTarget.CurrentPosition);
                        Main.Log.LogDebug($"[ArtilleryTTS] Adjusted strike for {unit.DisplayName}'s {weapon.Name} towards {closestTarget.DisplayName} (TTS Level: {ttsLevel})." +
                            $"\nOriginal strike position: {position}, distance: {distanceToTarget:F1}m." +
                            $"\nNew strike position: {newPos}, distance: {newDistanceToTarget:F1}m.");
                    }
                }
            }

            private static AbstractActor FindClosestEnemy(AbstractActor unit, Vector3 strikePosition, out float distanceToTarget)
            {
                AbstractActor closestTarget = null;
                float minDist = float.MaxValue;

                foreach (var enemy in unit.Combat.AllEnemies)
                {
                    float dist = Vector3.Distance(strikePosition, enemy.CurrentPosition);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestTarget = enemy;
                    }
                }

                distanceToTarget = minDist;
                return closestTarget;
            }

            private static Vector3 CalculateAdjustedStrikePosition(Vector3 initialPosition, Vector3 targetPosition, float distanceToTarget, int ttsLevel, float minMissRadius)
            {
                // 1. Determine pull factor and scatter reduction based on TTS level
                float pullFactor = 0f, scatterReductionFactor = 0f;
                if (ttsLevel == 1) { pullFactor = 0.30f; scatterReductionFactor = 0.30f; }
                else if (ttsLevel == 2) { pullFactor = 0.50f; scatterReductionFactor = 0.50f; }
                else if (ttsLevel >= 3) { pullFactor = 0.70f; scatterReductionFactor = 0.70f; }

                // 2. Cap the maximum pull distance to prevent large, unrealistic adjustments
                float maxPullDistance = 75f;
                float pullDistance = Mathf.Min(distanceToTarget * pullFactor, maxPullDistance);

                // 3. Calculate the adjusted strike position
                var directionToTarget = targetPosition - initialPosition;
                var adjustedPosition = initialPosition + (directionToTarget.normalized * pullDistance);

                // 4. Apply random spread, scaling it by the initial distance to the target.
                float maxScatterRadius = Mathf.Lerp(minMissRadius, 0f, scatterReductionFactor);
                float scatterRadius = Mathf.Min(maxScatterRadius, distanceToTarget * (1f - pullFactor));
                var randomCirclePoint = Random.insideUnitCircle * scatterRadius;
                var randomSpreadOffset = new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);

                return adjustedPosition + randomSpreadOffset;
            }
        }

        /// <summary>
        /// Modifies the artillery strike button text to indicate that a TTS is active.
        /// </summary>
        [HarmonyPatch(typeof(SelectionStateArtilleryStrike), "ShowButton")]
        public static class SelectionStateArtilleryStrike_ShowButton
        {
            [HarmonyPostfix]
            public static void Postfix(SelectionStateArtilleryStrike __instance)
            {
                var actor = __instance.SelectedActor;
                if (actor == null) return;

                var weapons = actor.GetArtilleryStrike(out _);
                if (weapons == null || weapons.Count == 0) return;

                bool allHaveTTS = weapons.All(w => w.ArtilleryTTSLevel() > 0f);
                if (allHaveTTS)
                {
                    __instance.HUD.ShowFireButton(CombatHUDFireButton.FireMode.Fire, "TTS-ADJUSTED\nARTILLERY STRIKE", __instance.showHeatWarnings);
                    return;
                }

                bool anyHasTTS = weapons.Any(w => w.ArtilleryTTSLevel() > 0f);
                if (anyHasTTS)
                {
                    __instance.HUD.ShowFireButton(CombatHUDFireButton.FireMode.Fire, "PARTIALLY TTS-ADJUSTED\nARTILLERY STRIKE", __instance.showHeatWarnings);
                    return;
                }
            }
        }

        public static int ArtilleryTTSLevel(this Weapon weapon) => (int)weapon.GetStatisticFloat("AMSAttractiveness");
    }
}