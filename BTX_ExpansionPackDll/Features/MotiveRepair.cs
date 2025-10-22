using BattleTech;
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
                    float cruiseRepair = ReduceDebuff(__instance, "motiveSystemLoss", "CruiseSpeed");
                    float flankRepair = ReduceDebuff(__instance, "motiveSystemLossSprint", "FlankSpeed");

                    if (cruiseRepair > 0f || flankRepair > 0f)
                    {
                        Main.Log.LogDebug($"[MotiveRepair] Reduced motive damage by {cruiseRepair} meters on {__instance.DisplayName}.");
                        __instance.StatCollection.Set("MotiveRepairActive", false);
                    }
                }
            }

            private static float ReduceDebuff(AbstractActor actor, string effectId, string statName)
            {
                var effect = GetMotiveDebuffEffect(actor, effectId, statName);
                if (effect == null) return 0f;

                if (float.TryParse(effect.EffectData.statisticData.modValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float currentModValue))
                {
                    int currentStacks = CalculateDebuffStacks(currentModValue, statName);
                    int stacksToKeep = (int)Math.Floor(currentStacks / 5.0);

                    if (stacksToKeep < 4)
                    {
                        actor.Combat.EffectManager.CancelEffect(effect);
                        return Math.Abs(currentModValue);
                    }
                    float newModValue = -1 * stacksToKeep * GetSingleStackDebuff(statName);
                    effect.EffectData.statisticData.modValue = newModValue.ToString(CultureInfo.InvariantCulture);
                    return Math.Abs(currentModValue - newModValue);
                }
                return 0f;
            }
        }

        /// <summary>
        /// Forces AI vehicles to use the Motive Repair ability when their movement is significantly impaired.
        /// </summary>
        [HarmonyPatch(typeof(AITeam), "makeInvocationFromOrders")]
        public static class AITeam_makeInvocationFromOrders
        {
            [HarmonyPrefix]
            public static void Prefix(AbstractActor unit)
            {
                if (!unit.FakeVehicle() || unit.HasMovedThisRound)
                    return;

                var motiveRepairAbility = unit.ComponentAbilities.Find(x => x.Def.Id == "AbilityDef_MotiveRepair");
                if (motiveRepairAbility == null || !motiveRepairAbility.IsAvailable)
                    return;

                int cruiseStacks = GetDebuffStacks(unit, "motiveSystemLoss", "CruiseSpeed");
                int flankStacks = GetDebuffStacks(unit, "motiveSystemLossSprint", "FlankSpeed");

                if (cruiseStacks > 4 || flankStacks > 4)
                {
                    Main.Log.LogDebug($"[MotiveRepair] AI {unit.DisplayName} has {cruiseStacks} motive debuffs. Activating Motive Repair.");
                    motiveRepairAbility.Activate(unit, unit);
                }
            }

            private static int GetDebuffStacks(AbstractActor actor, string effectId, string statName)
            {
                var effect = GetMotiveDebuffEffect(actor, effectId, statName);
                if (effect == null) return 0;

                if (float.TryParse(effect.EffectData.statisticData.modValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float currentModValue))
                {
                    return CalculateDebuffStacks(currentModValue, statName);
                }
                return 0;
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