using System;
using BattleTech;
using CustomAmmoCategoriesPatches;
using HarmonyLib;

namespace BTX_ExpansionPack
{
    internal class SelfKnockdown
    {
        public static class WeaponInfo
        {
            public static bool ShouldApplySelfInstability { get; set; } = false;
            public static float SelfInstabilityWeaponTonnage { get; set; } = 0f;
        }

        [HarmonyPatch(typeof(Weapon), "ProcessOnFiredFloatieEffects", new Type[] { })]
        public static class Weapon_ProcessOnFiredFloatieEffects
        {
            [HarmonyPrefix]
            public static void Prefix(Weapon __instance)
            {
                if (__instance.parent is Mech mech && mech.isHasStability())
                {
                    if (__instance?.weaponDef?.ComponentTags?.Contains("SelfKnockdown") == true)
                    {
                        WeaponInfo.ShouldApplySelfInstability = true;
                        WeaponInfo.SelfInstabilityWeaponTonnage = __instance.tonnage;
                        Main.Log.LogDebug($"Weapon '{__instance.Name}' has SelfKnockdown tag. Flag set.");
                    }
                    else
                    {
                        WeaponInfo.ShouldApplySelfInstability = false;
                        WeaponInfo.SelfInstabilityWeaponTonnage = 0f;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AttackDirector), "OnAttackComplete", new Type[] { typeof(MessageCenterMessage) })]
        public static class AttackDirector_OnAttackComplete
        {
            [HarmonyPrefix]
            public static void Prefix(ref bool __runOriginal, AttackDirector __instance, MessageCenterMessage message)
            {
                if (!__runOriginal) return;

                if (WeaponInfo.ShouldApplySelfInstability)
                {
                    WeaponInfo.ShouldApplySelfInstability = false;

                    if (message is AttackCompleteMessage attackCompleteMessage)
                    {
                        int sequenceId = attackCompleteMessage.sequenceId;
                        AttackDirector.AttackSequence attackSequence = __instance.GetAttackSequence(sequenceId);
                        if (attackSequence?.attacker is Mech mech && mech.isHasStability())
                        {
                            float selfInstability = WeaponInfo.SelfInstabilityWeaponTonnage * 1.5f;

                            if (!mech.BracedLastRound || mech.DistMovedThisRound > 20f)
                            {
                                Main.Log.LogDebug($"[SelfKnockdown] Applying {selfInstability} instability to {mech.DisplayName}.");
                                mech.AddAbsoluteInstability(selfInstability, StabilityChangeSource.Effect, mech.GUID);
                            }
                            else
                            {
                                Main.Log.LogDebug($"[SelfKnockdown] Applying only {selfInstability} instability, {mech.DisplayName} has braced last turn.");
                                mech.AddAbsoluteInstability(selfInstability / 2f, StabilityChangeSource.Effect, mech.GUID);

                            }
                            if (mech.NeedsInstabilityCheck)
                            {
                                mech.CheckForInstability();
                                if (mech.IsFlaggedForKnockdown)
                                {
                                    mech.HandleKnockdown(-1,
                                        $"{mech.DisplayName}_FromSelfInstability",
                                        mech.CurrentPosition, null);
                                    mech.DoneWithActor();
                                    mech.OnActivationEnd(mech.GUID, -1);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}