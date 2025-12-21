﻿using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using System;
using System.Globalization;
using System.Linq;

namespace BTX_ExpansionPack.Features
{
    internal class MotiveRepair
    {
        /// <summary>
        /// Initializes the Motive Repair related statistics for vehicles.
        /// </summary>
        [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
        public static class AbstractActor_InitEffectStats
        {
            [HarmonyPostfix]
            public static void Postfix(AbstractActor __instance)
            {
                if (__instance.FakeVehicle())
                {
                    __instance.StatCollection.AddStatistic("MotiveRepairActive", false);
                }
            }
        }

        /// <summary>
        /// Disables the Motive Repair ability button for non-vehicles.
        /// </summary>
        [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "ResetAbilityButton", [typeof(AbstractActor), typeof(CombatHUDActionButton), typeof(Ability), typeof(bool)])]
        public static class CombatHUDMechwarriorTray_ResetAbilityButton
        {
            [HarmonyPostfix]
            public static void Postfix(AbstractActor actor, CombatHUDActionButton button, Ability ability)
            {
                if (ability != null && ability.Def.Id == "AbilityDef_MotiveRepair")
                {
                    if (actor != null && (actor is not Mech mech || !mech.MechDef.IsVehicle()))
                    {
                        button.DisableButton();
                    }
                }
            }
        }

        /// <summary>
        /// Reduces motive system loss debuffs at the end of the turn whenever the Motive Repair ability is active.
        /// </summary>
        [HarmonyPatch(typeof(AbstractActor), "OnActivationEnd")]
        public static class AbstractActor_OnActivationEnd
        {
            [HarmonyPrefix]
            public static void Prefix(AbstractActor __instance)
            {
                if (!__instance.FakeVehicle())
                    return;

                bool isRepairActive = __instance.StatCollection.GetValue<bool>("MotiveRepairActive");
                if (isRepairActive)
                {
                    var cruiseEffect = GetMotiveDebuffEffect(__instance, "motiveSystemLoss", "CruiseSpeed");
                    if (cruiseEffect == null) return;

                    if (float.TryParse(cruiseEffect.EffectData.statisticData.modValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float currentModValue))
                    {
                        int currentStacks = CalculateDebuffStacks(currentModValue, "CruiseSpeed");
                        int stacksToKeep = (int)Math.Floor(currentStacks / 5.0);

                        float repairedAmount = 0f;
                        if (stacksToKeep < 4)
                        {
                            repairedAmount = Math.Abs(currentModValue);
                            __instance.Combat.EffectManager.CancelEffect(cruiseEffect);

                            var flankEffect = GetMotiveDebuffEffect(__instance, "motiveSystemLossSprint", "FlankSpeed");
                            if (flankEffect != null)
                                __instance.Combat.EffectManager.CancelEffect(flankEffect);
                        }
                        else
                        {
                            float newCruiseModValue = -1 * stacksToKeep * GetSingleStackDebuff("CruiseSpeed");
                            repairedAmount = Math.Abs(currentModValue - newCruiseModValue);
                            cruiseEffect.EffectData.statisticData.modValue = newCruiseModValue.ToString(CultureInfo.InvariantCulture);

                            var flankEffect = GetMotiveDebuffEffect(__instance, "motiveSystemLossSprint", "FlankSpeed");
                            if (flankEffect != null)
                            {
                                float newFlankModValue = -1 * stacksToKeep * GetSingleStackDebuff("FlankSpeed");
                                flankEffect.EffectData.statisticData.modValue = newFlankModValue.ToString(CultureInfo.InvariantCulture);
                            }
                        }

                        if (repairedAmount > 0f)
                        {
                            Main.Log.LogDebug($"[MotiveRepair] Reduced motive damage by {repairedAmount:F1} meters on {__instance.DisplayName}.");
                            __instance.StatCollection.Set("MotiveRepairActive", false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Improves AI decision-making for movement-related abilities.
        /// </summary>
        [HarmonyPatch(typeof(AITeam), "getInvocationForCurrentUnit")]
        public static class AITeam_getInvocationForCurrentUnit
        {
            [HarmonyPrefix]
            public static void Prefix(AITeam __instance)
            {
                var unit = __instance.currentUnit;
                if (unit == null || unit.HasMovedThisRound || unit.IsDead || unit.GetPilot() == null)
                    return;

                // Handle vehicle-specific motive damage repair
                if (unit.FakeVehicle() && IsMovementImpaired(unit, out int motiveStacks) && motiveStacks > 4)
                {
                    var motiveRepair = unit.ComponentAbilities.FirstOrDefault(x => x.Def.Id == "AbilityDef_MotiveRepair");
                    if (motiveRepair != null && motiveRepair.IsAvailable)
                    {
                        Main.Log.LogDebug($"[AI_Movement] AI {unit.DisplayName} has {motiveStacks} motive debuffs. Activating Motive Repair.");
                        motiveRepair.Activate(unit, unit);
                        return;
                    }
                }

                // Handle any unit being physically stuck
                if (IsStuck(unit))
                {
                    var carefulManeuvers = unit.GetPilot().Abilities.FirstOrDefault(x => x.Def.Id == "AbilityDef_CarefulManeuvers");
                    if (carefulManeuvers != null && carefulManeuvers.IsAvailable)
                    {
                        Main.Log.LogDebug($"[AI_Movement] AI {unit.DisplayName} is stuck. Activating Careful Maneuvers.");
                        carefulManeuvers.Activate(unit, unit);
                        return;
                    }
                }
            }

            private static bool IsMovementImpaired(AbstractActor unit, out int motiveStacks)
            {
                var motiveEffect = GetMotiveDebuffEffect(unit, "motiveSystemLoss", "CruiseSpeed");
                if (motiveEffect != null && float.TryParse(motiveEffect.EffectData.statisticData.modValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float modValue))
                {
                    motiveStacks = CalculateDebuffStacks(modValue, "CruiseSpeed");
                    return motiveStacks > 0;
                }

                motiveStacks = 0;
                return false;
            }

            private static bool IsStuck(AbstractActor unit)
            {
                var pathing = unit.Pathing;
                var walkGrid = pathing?.getGrid(MoveType.Walking);
                return walkGrid != null && walkGrid.GetSampledPathNodes().Count <= 1;
            }
        }

        private static Effect GetMotiveDebuffEffect(AbstractActor actor, string effectId, string statName) =>
            actor.Combat.EffectManager.GetAllEffectsTargeting(actor)
                .FirstOrDefault(e => e.EffectData.Description.Id == effectId &&
                                     e.EffectData.statisticData.statName == statName);

        private static int CalculateDebuffStacks(float currentModValue, string statName)
        {
            float singleStackDebuff = GetSingleStackDebuff(statName);
            return singleStackDebuff <= 0f ? 0 : (int)Math.Round(Math.Abs(currentModValue) / singleStackDebuff);
        }

        private static float GetSingleStackDebuff(string statName) =>
            statName == "CruiseSpeed"
                ? Extended_CE.Core.Settings.MotiveCritMovementLoss
                : Extended_CE.Core.Settings.MotiveCritMovementLoss * 1.5f;
    }
}