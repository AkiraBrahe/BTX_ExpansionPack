using BattleTech;
using BattleTech.UI;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    public static class AbilityResourceEffects
    {
        public static bool GetAbilityUsedFiring(this AbstractActor actor)
        {
            return actor.StatCollection.GetValue<bool>("ActorConsumedFiring");
        }

        public static void DisableAbilitiesUsingResource(this CombatSelectionHandler handler, AbilityDef.ResourceConsumed resource)
        {
            for (int i = handler.ActivatedAbilityButtons.Count - 1; i >= 0; i--)
            {
                if (resource == AbilityDef.ResourceConsumed.ConsumesFiring)
                {
                    if (handler.ActivatedAbilityButtons[i].Ability.Def.ActivationTime == AbilityDef.ActivationTiming.ConsumedByFiring)
                    {
                        handler.ActivatedAbilityButtons[i].DisableButton();
                    }
                }
                else if (resource == AbilityDef.ResourceConsumed.ConsumesMovement)
                {
                    if (handler.ActivatedAbilityButtons[i].Ability.Def.ActivationTime == AbilityDef.ActivationTiming.ConsumedByMovement)
                    {
                        handler.ActivatedAbilityButtons[i].DisableButton();
                    }
                }
            }
        }
        public static EffectDurationData Duration => new() { duration = 1, stackLimit = 1 };

        public static EffectTargetingData TargetingData =>
            new()
            {
                effectTriggerType = EffectTriggerType.Passive,
                triggerLimit = 0,
                extendDurationOnTrigger = 0,
                specialRules = AbilityDef.SpecialRules.NotSet,
                auraEffectType = AuraEffectType.NotSet,
                effectTargetType = EffectTargetType.Creator,
                alsoAffectCreator = false,
                range = 0f,
                forcePathRebuild = true,
                forceVisRebuild = false,
                showInTargetPreview = false,
                showInStatusPanel = false
            };

        public static EffectData ImmobileEffectData =>
            new()
            {
                effectType = EffectType.StatisticEffect,
                targetingData = AbilityResourceEffects.TargetingData,
                Description = new DescriptionDef("ConsumesMovementFix", "Ability Used Movement", "Ability Used Movement", "uixSvgIcon_action_multitarget", 0, 0f, false, null, null, null),
                durationData = AbilityResourceEffects.Duration,
                statisticData = new StatisticEffectData
                {
                    statName = "irbtmu_immobile_unit",
                    operation = StatCollection.StatOperation.Set,
                    modValue = "true",
                    modType = "System.Boolean"
                }
            };

        public static EffectData AbilityUsedFiringData =>
            new()
            {
                effectType = EffectType.StatisticEffect,
                targetingData = AbilityResourceEffects.TargetingData,
                Description = new DescriptionDef("ConsumesFiringFix", "Ability Used Firing", "Ability Used Firing", "uixSvgIcon_action_multitarget", 0, 0f, false, null, null, null),
                durationData = AbilityResourceEffects.Duration,
                statisticData = new StatisticEffectData
                {
                    statName = "ActorConsumedFiring",
                    operation = StatCollection.StatOperation.Set,
                    modValue = "true",
                    modType = "System.Boolean"
                }
            };
    }

    [HarmonyPatch(typeof(AbstractActor), "InitEffectStats")]
    public static class AbstractActor_InitEffectStats_AbilityResourceFix
    {
        [HarmonyPostfix]
        public static void Postfix(AbstractActor __instance)
        {
            __instance.StatCollection.AddStatistic("ActorConsumedFiring", false);
        }
    }

    [HarmonyPatch(typeof(AbstractActor), "OnNewRound", typeof(int))]
    public static class AbstractActor_OnNewRound
    {
        [HarmonyPostfix]
        public static void Postfix(AbstractActor __instance)
        {
            __instance.StatCollection.Set("ActorConsumedFiring", false);
        }
    }

    [HarmonyPatch(typeof(SelectionStateMove), "GetAllMeleeTargets", [])]
    public static class SelectionStateMove_GetAllMeleeTargets
    {
        [HarmonyPrefix]
        public static bool Prefix(SelectionStateMove __instance, out List<ICombatant> __result)
        {
            if (__instance.SelectedActor.GetAbilityUsedFiring())
            {
                __result = [];
                return false;
            }

            __result = null;
            return true;
        }
    }

    [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "ResetAbilityButton", [typeof(AbstractActor), typeof(CombatHUDActionButton), typeof(Ability), typeof(bool)])]
    public static class CombatHUDMechwarriorTray_ResetAbilityButton
    {
        [HarmonyPostfix]
        public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor, CombatHUDActionButton button, Ability ability, bool forceInactive)
        {
            if (actor != null && ability != null && actor.GetAbilityUsedFiring())
            {
                if (ability.Def.ActivationTime == AbilityDef.ActivationTiming.ConsumedByFiring) button.DisableButton();
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDWeaponPanel), "ResetAbilityButton", [typeof(AbstractActor), typeof(CombatHUDActionButton), typeof(Ability), typeof(bool)])]
    public static class CombatHUDWeaponPanel_ResetAbilityButton
    {
        [HarmonyPostfix]
        public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor, CombatHUDActionButton button, Ability ability, bool forceInactive)
        {
            if (actor != null && ability != null && actor.GetAbilityUsedFiring())
            {
                if (ability.Def.ActivationTime == AbilityDef.ActivationTiming.ConsumedByFiring) button.DisableButton();
            }
        }
    }

    [HarmonyPatch(typeof(CombatSelectionHandler), "AddFireState", [typeof(AbstractActor)])]
    public static class CombatSelectionHandler_AddFireState
    {
        [HarmonyPrefix]
        public static void Prefix(ref bool __runOriginal, CombatSelectionHandler __instance, AbstractActor actor)
        {
            if (!__runOriginal) return;

            if (actor != null && actor.GetAbilityUsedFiring())
            {
                var HUD = __instance.HUD;
                HUD.MechWarriorTray.FireButton.DisableButton();
                var selectionStack = HUD.SelectionHandler.SelectionStack;
                if (!selectionStack.Any(x => x is SelectionStateDoneWithMech) && actor.HasMovedThisRound)
                {
                    // Main.Log.LogDebug($"[CombatSelectionHandler_AddFireState] Adding SelectionStateDoneWithMech.");
                    var doneState = new SelectionStateDoneWithMech(actor.Combat, HUD,
                        HUD.MechWarriorTray.DoneWithMechButton, actor);
                    HUD.SelectionHandler.addNewState(doneState);
                }

                __runOriginal = false;
                return;
            }

        }
    }

    [HarmonyPatch(typeof(CombatSelectionHandler), "AddFireState", [typeof(AbstractActor), typeof(ICombatant), typeof(CombatHUDAttackModeSelector.SelectedButton)])]
    static class CombatSelectionHandler_AddFireState2
    {
        [HarmonyPrefix]
        public static void Prefix(ref bool __runOriginal, CombatSelectionHandler __instance, AbstractActor actor)
        {
            if (!__runOriginal) return;

            if (actor != null && actor.GetAbilityUsedFiring())
            {
                var HUD = __instance.HUD;
                HUD.MechWarriorTray.FireButton.DisableButton();
                __runOriginal = false;
            }

        }
    }

    [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "ResetMechwarriorButtons", [typeof(AbstractActor)])]
    public static class CombatHUDMechwarriorTray_ResetMechwarriorButtons
    {
        [HarmonyPostfix]
        public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor)
        {
            if (actor != null && actor.GetAbilityUsedFiring())
            {
                __instance.FireButton.DisableButton();
            }
        }
    }



    [HarmonyPatch(typeof(CombatHUDActionButton), "ActivateAbility", [typeof(string), typeof(string)])]
    public static class CombatHUDActionButton_ActivateAbility_Confirmed
    {
        [HarmonyPostfix]
        public static void Postfix(CombatHUDActionButton __instance, string creatorGUID, string targetGUID)
        {
            var HUD = __instance.HUD;
            var selectedActor = HUD.SelectedActor;
            var selectionStack = HUD.SelectionHandler.SelectionStack;
            if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesFiring)
            {
                if (selectedActor.HasBegunActivation || selectedActor.HasMovedThisRound && !selectedActor.HasActivatedThisRound)
                {
                    if (selectedActor is Mech mech)
                    {
                        mech.GenerateAndPublishHeatSequence(-1, true, false, selectedActor.GUID);
                        // Main.Log.LogDebug($"Generated and Published Heat Sequence for {mech.Description.UIName}.");
                    }

                    selectedActor.DoneWithActor();
                    selectedActor.OnActivationEnd(selectedActor.GUID, __instance.GetInstanceID());
                    return;
                }

                selectedActor.CreateEffect(AbilityResourceEffects.AbilityUsedFiringData, null, AbilityResourceEffects.AbilityUsedFiringData.Description.Id, -1, selectedActor);
                for (int i = selectionStack.Count - 1; i >= 0; i--)
                {
                    if (selectionStack[i] is SelectionStateFire selectionStateFire)
                    {
                        selectionStateFire.OnInactivate();
                        selectionStateFire.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateFire);
                    }
                    else switch (selectionStack[i])
                        {
                            case SelectionStateFireMulti selectionStateFireMulti:
                                selectionStateFireMulti.OnInactivate();
                                selectionStateFireMulti.OnRemoveFromStack();
                                selectionStack.Remove(selectionStateFireMulti);
                                break;
                            case SelectionStateMoraleAttack selectionStateMoraleAttack:
                                selectionStateMoraleAttack.OnInactivate();
                                selectionStateMoraleAttack.OnRemoveFromStack();
                                selectionStack.Remove(selectionStateMoraleAttack);
                                break;
                            case SelectionStateMove selectionStateMove:
                                selectionStateMove.RefreshPossibleTargets();
                                break;
                            case SelectionStateJump selectionStateJump:
                                selectionStateJump.RefreshPossibleTargets();
                                break;
                        }
                }
                HUD.MechWarriorTray.FireButton.DisableButton();
                if (!selectionStack.Any(x => x is SelectionStateDoneWithMech) && selectedActor.HasMovedThisRound)
                {
                    // Main.Log.LogDebug($"[CombatHUDActionButton_ActivateAbility_Confirmed] Adding SelectionStateDoneWithMech.");
                    var doneState = new SelectionStateDoneWithMech(selectedActor.Combat, HUD,
                        HUD.MechWarriorTray.DoneWithMechButton, selectedActor);
                    HUD.SelectionHandler.addNewState(doneState);
                }
                HUD.SelectionHandler.DisableAbilitiesUsingResource(AbilityDef.ResourceConsumed.ConsumesFiring);
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesMovement)
            {
                selectedActor.CreateEffect(AbilityResourceEffects.ImmobileEffectData, null, AbilityResourceEffects.ImmobileEffectData.Description.Id, -1, selectedActor);
                HUD.SelectionHandler.DisableAbilitiesUsingResource(AbilityDef.ResourceConsumed.ConsumesMovement);
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesActivation)
            {
                if (selectedActor is Mech mech)
                {
                    mech.GenerateAndPublishHeatSequence(-1, true, false, selectedActor.GUID);
                    // Main.Log.LogDebug($"Generated and Published Heat Sequence for {mech.Description.UIName}.");
                }

                selectedActor.DoneWithActor();
                selectedActor.OnActivationEnd(selectedActor.GUID, __instance.GetInstanceID());
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDEquipmentSlot), "ActivateAbility", [typeof(string), typeof(string)])]
    public static class CombatHUDEquipmentSlot_ActivateAbility_Confirmed
    {
        [HarmonyPostfix]
        public static void Postfix(CombatHUDEquipmentSlot __instance, string creatorGUID, string targetGUID)
        {
            var HUD = __instance.HUD;
            var selectedActor = HUD.SelectedActor;
            var selectionStack = HUD.SelectionHandler.SelectionStack;
            if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesFiring)
            {
                if (selectedActor.HasBegunActivation || selectedActor.HasMovedThisRound && !selectedActor.HasActivatedThisRound)
                {
                    if (selectedActor is Mech mech)
                    {
                        mech.GenerateAndPublishHeatSequence(-1, true, false, selectedActor.GUID);
                        // Main.Log.LogDebug($"Generated and Published Heat Sequence for {mech.Description.UIName}.");
                    }

                    selectedActor.DoneWithActor();
                    selectedActor.OnActivationEnd(selectedActor.GUID, __instance.GetInstanceID());
                    return;
                }

                selectedActor.CreateEffect(AbilityResourceEffects.AbilityUsedFiringData, null, AbilityResourceEffects.AbilityUsedFiringData.Description.Id, -1, selectedActor);
                for (int i = selectionStack.Count - 1; i >= 0; i--)
                {
                    if (selectionStack[i] is SelectionStateFire selectionStateFire)
                    {
                        selectionStateFire.OnInactivate();
                        selectionStateFire.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateFire);
                    }
                    else if (selectionStack[i] is SelectionStateFireMulti selectionStateFireMulti)
                    {
                        selectionStateFireMulti.OnInactivate();
                        selectionStateFireMulti.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateFireMulti);
                    }
                    else if (selectionStack[i] is SelectionStateMoraleAttack selectionStateMoraleAttack)
                    {
                        selectionStateMoraleAttack.OnInactivate();
                        selectionStateMoraleAttack.OnRemoveFromStack();
                        selectionStack.Remove(selectionStateMoraleAttack);
                    }
                }
                HUD.MechWarriorTray.FireButton.DisableButton();
                if (!selectionStack.Any(x => x is SelectionStateDoneWithMech) && selectedActor.HasMovedThisRound)
                {
                    // Main.Log.LogDebug($"[CombatHUDEquipmentSlot_ActivateAbility_Confirmed] Adding SelectionStateDoneWithMech.");
                    var doneState = new SelectionStateDoneWithMech(selectedActor.Combat, HUD,
                        HUD.MechWarriorTray.DoneWithMechButton, selectedActor);
                    // addState.GetValue(doneState);
                    HUD.SelectionHandler.addNewState(doneState);
                }
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesMovement)
            {
                // selectedActor.HasMovedThisRound = true;
                selectedActor.CreateEffect(AbilityResourceEffects.ImmobileEffectData, null, AbilityResourceEffects.ImmobileEffectData.Description.Id, -1, selectedActor);
                // selectedActor.Combat.EffectManager.CreateEffect(ImmobileEffect.ImmobileEffectData,"IRTweaks_Immobilized", -1, selectedActor, selectedActor, default(WeaponHitInfo), 1, false);
            }
            else if (__instance.Ability.Def.Resource == AbilityDef.ResourceConsumed.ConsumesActivation)
            {
                if (selectedActor is Mech mech)
                {
                    mech.GenerateAndPublishHeatSequence(-1, true, false, selectedActor.GUID);
                    // Main.Log.LogDebug($"Generated and Published Heat Sequence for {mech.Description.UIName}.");
                }

                selectedActor.DoneWithActor();
                selectedActor.OnActivationEnd(selectedActor.GUID, __instance.GetInstanceID());
            }
        }
    }

    [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "ResetAbilityButtons", [typeof(AbstractActor)])]
    public static class CombatHUDMechwarriorTray_ResetAbilityButtons_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor)
        {
            var forceInactive = actor.HasMovedThisRound || actor.HasFiredThisRound; // need to figure this part out; do other checks? this is still disabling the butons. integrat with CU?
            var abilityButtons = __instance.AbilityButtons;
            foreach (var button in abilityButtons)
            {
                // Main.Log.LogDebug($"Processing button for {button?.Ability?.Def?.Description?.Name}.");
                if (button?.Ability?.Def?.Resource == AbilityDef.ResourceConsumed.ConsumesActivation && forceInactive)
                {
                    // Main.Log.LogDebug($"Disabling button for {button.Ability.Def.Description?.Name}.");
                    button.DisableButton();
                }
            }
        }
    }
}