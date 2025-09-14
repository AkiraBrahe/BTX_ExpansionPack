using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using System.Linq;

namespace BTX_ExpansionPack.Features
{
    internal class MotiveRepair
    {
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
        /// Removes up to three motive system loss debuffs at the end of the turn if the Motive Repair ability is active.
        /// </summary>
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
                    int removedCruise = RemoveDebuffs(__instance, "motiveSystemLoss", "CruiseSpeed", 3);
                    int removedFlank = RemoveDebuffs(__instance, "motiveSystemLossSprint", "FlankSpeed", 3);

                    if (removedCruise > 0 || removedFlank > 0)
                    {
                        __instance.StatCollection.Set("MotiveRepairActive", false);
                        Main.Log.LogDebug($"[MotiveRepair] Removed {removedCruise} cruise and {removedFlank} flank debuffs from {__instance.DisplayName}.");
                    }
                }
            }

            private static int RemoveDebuffs(AbstractActor actor, string effectId, string statName, int maxToRemove)
            {
                int removed = 0;
                var effects = actor.Combat.EffectManager.GetAllEffectsTargeting(actor)
                    .Where(effect => (effect.EffectData?.Description?.Id == effectId) &&
                                     (effect.EffectData?.statisticData?.statName == statName));
                foreach (var effect in effects)
                {
                    if (removed >= maxToRemove) break;
                    actor.Combat.EffectManager.CancelEffect(effect, true);
                    removed++;
                }
                return removed;
            }
        }

        /// <summary>
        /// Removes all motive system components from vehicles when a contract is completed.
        /// </summary>
        [HarmonyPatch(typeof(Contract), "CompleteContract")]
        public static class Contract_CompleteContract
        {
            [HarmonyPrefix]
            public static void Prefix(Contract __instance)
            {
                if (__instance.State == Contract.ContractState.InProgress)
                {
                    var allActors = __instance.BattleTechGame.Combat.AllActors.ToList();
                    foreach (var actor in allActors)
                    {
                        if (actor is Mech mech && mech.MechDef.IsVehicle())
                        {
                            mech.allComponents.RemoveAll(mechComponent => mechComponent.defId == "Gear_BEX_MotiveSystem");
                            mech.miscComponents.RemoveAll(mechComponent => mechComponent.defId == "Gear_BEX_MotiveSystem");
                        }
                    }
                }
            }
        }

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