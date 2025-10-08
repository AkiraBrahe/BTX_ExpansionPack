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
        /// Reduces motive system loss debuffs at the end of the turn whenever the Motive Repair ability is active.
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

        [HarmonyPatch(typeof(AbstractActor), "OnActivationEnd")]
        public static class AbstractActor_OnActivationEnd
        {
            [HarmonyPrefix]
            public static void Prefix(AbstractActor __instance)
            {
                if (!__instance.FakeVehicle() || !__instance.StatCollection.ContainsStatistic("MotiveRepairActive"))
                    return;

                bool isRepairActive = __instance.StatCollection.GetStatistic("MotiveRepairActive").Value<bool>();
                if (isRepairActive)
                {
                    float cruiseRepair = ReduceDebuff(__instance, "motiveSystemLoss", "CruiseSpeed");
                    float flankRepair = ReduceDebuff(__instance, "motiveSystemLossSprint", "FlankSpeed");

                    if (cruiseRepair > 0f || flankRepair > 0f)
                    {
                        __instance.StatCollection.Set("MotiveRepairActive", false);
                        Main.Log.LogDebug($"[MotiveRepair] Reduced motive damage by {cruiseRepair} meters on {__instance.DisplayName}.");
                    }
                }
            }

            private static float ReduceDebuff(AbstractActor actor, string effectId, string statName)
            {
                var effect = actor.Combat.EffectManager.GetAllEffectsTargeting(actor)
                    .FirstOrDefault(e => e.EffectData?.Description?.Id == effectId &&
                                         e.EffectData?.statisticData?.statName == statName);

                if (effect == null) return 0f;

                if (float.TryParse(effect.EffectData.statisticData.modValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float currentModValue))
                {
                    float singleStackDebuff = statName == "CruiseSpeed"
                        ? Extended_CE.Core.Settings.MotiveCritMovementLoss
                        : Extended_CE.Core.Settings.MotiveCritMovementLoss * 1.5f;

                    if (singleStackDebuff <= 0f) return 0f;

                    int currentStacks = (int)Math.Round(Math.Abs(currentModValue) / singleStackDebuff);
                    int stacksToKeep = (int)Math.Floor(currentStacks / 5.0);

                    if (stacksToKeep < 4)
                    {
                        actor.Combat.EffectManager.CancelEffect(effect);
                        return Math.Abs(currentModValue);
                    }

                    float newModValue = -1 * stacksToKeep * singleStackDebuff;
                    effect.EffectData.statisticData.modValue = newModValue.ToString(CultureInfo.InvariantCulture);
                    return Math.Abs(currentModValue - newModValue);
                }

                return 0f;
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
    }
}