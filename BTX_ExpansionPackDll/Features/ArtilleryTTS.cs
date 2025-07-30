using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using System.Linq;
using UnityEngine;

namespace BTX_ExpansionPack
{
    internal class ArtilleryTTS
    {
        [HarmonyPatch(typeof(WeaponArtilleryHelper), "CreateStrikeInvocation")]
        public static class WeaponArtilleryHelper_CreateStrikeInvocation
        {
            [HarmonyPrefix]
            [HarmonyWrapSafe]
            public static void Prefix(AbstractActor unit)
            {
                var weapons = unit.GetArtilleryStrike(out Vector3 position);
                if (weapons.Count == 0) return;

                AbstractActor closestTarget = FindClosestEnemy(unit, position, out float distanceToTarget);
                if (closestTarget != null && distanceToTarget > 0f)
                {
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        var weapon = weapons[i];

                        float ttsLevel = GetWeaponTTSLevel(weapon);
                        if (ttsLevel == 0f)
                            continue;

                        float minMissRadius = weapon.MinMissRadius();
                        float maxMissRadius = weapon.MaxMissRadius();

                        Vector3 newPos = CalculateAdjustedStrikePosition(position, closestTarget.CurrentPosition, ttsLevel, minMissRadius, maxMissRadius);
                        Main.Log.LogDebug($"[ArtilleryTTS] Adjusted strike position for {unit.DisplayName}'s {weapon.Name} towards {closestTarget.DisplayName}." +
                            $"Original strike position:{position}; Distance to target: {distanceToTarget}; New position {newPos}");
                        weapon.AddArtilleryStrike(newPos, i + 1);
                    }
                }
            }

            private static AbstractActor FindClosestEnemy(AbstractActor unit, Vector3 strikePosition, out float distanceToTarget)
            {
                AbstractActor closestTarget = null;
                float minDist = float.MaxValue;

                foreach (AbstractActor enemy in unit.Combat.AllEnemies)
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

            private static Vector3 CalculateAdjustedStrikePosition(Vector3 initialPosition, Vector3 targetPosition, float ttsLevel, float minMissRadius, float maxMissRadius)
            {
                // Calculate the pull factor based on TTS level
                float pullFactor = 0f;
                if (ttsLevel == 1f) pullFactor = 0.30f; // 30% closer
                else if (ttsLevel == 2f) pullFactor = 0.50f; // 50% closer
                else if (ttsLevel >= 3f) pullFactor = 0.70f; // 70% closer

                Vector3 directionToTarget = targetPosition - initialPosition;
                Vector3 adjustedPosition = initialPosition + (directionToTarget * pullFactor);

                // Apply random spread based on miss radius and TTS level
                float scatterReductionFactor = 0f;
                if (ttsLevel == 1f) scatterReductionFactor = 0.30f;
                else if (ttsLevel == 2f) scatterReductionFactor = 0.50f;
                else if (ttsLevel >= 3f) scatterReductionFactor = 0.70f;

                float scatterRadius = Mathf.Lerp(maxMissRadius, minMissRadius, scatterReductionFactor);
                Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * scatterRadius;
                Vector3 randomSpreadOffset = new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);

                return adjustedPosition + randomSpreadOffset;
            }
        }

        [HarmonyPatch(typeof(SelectionStateArtilleryStrike), "ShowButton")]
        public static class SelectionStateArtilleryStrike_ShowButton
        {
            [HarmonyPostfix]
            public static void Postfix(SelectionStateArtilleryStrike __instance)
            {
                var actor = Traverse.Create(__instance).Property("SelectedActor").GetValue<AbstractActor>();
                if (actor == null) return;

                var weapons = actor.GetArtilleryStrike(out _);
                if (weapons == null || weapons.Count == 0) return;

                bool allHaveTTS = weapons.All(w => GetWeaponTTSLevel(w) > 0f);
                if (allHaveTTS)
                {
                    __instance.HUD.ShowFireButton(CombatHUDFireButton.FireMode.Fire, "TTS-ADJUSTED ARTILLERY STRIKE", __instance.showHeatWarnings);
                    return;
                }

                bool anyHasTTS = weapons.Any(w => GetWeaponTTSLevel(w) > 0f);
                if (anyHasTTS)
                {
                    __instance.HUD.ShowFireButton(CombatHUDFireButton.FireMode.Fire, "PARTIALLY TTS-ADJUSTED ARTILLERY STRIKE", __instance.showHeatWarnings);
                    return;
                }
            }
        }

        public static float GetWeaponTTSLevel(Weapon weapon)
        {
            return weapon == null ? 0f : weapon.mode().AIHitChanceCap;
        }
    }
}