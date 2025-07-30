using BattleTech;
using CustomAmmoCategoriesPatches;

namespace BTX_ExpansionPack
{
    internal class SelfKnockdown
    {
        public static class WeaponInfo
        {
            public static bool ShouldApplySelfInstability { get; set; } = false;
            public static float SelfInstabilityWeaponTonnage { get; set; } = 0f;
        }

        [HarmonyPatch(typeof(Weapon), "ProcessOnFiredFloatieEffects")]
        public static class Weapon_ProcessOnFiredFloatieEffects
        {
            [HarmonyPrefix]
            public static void Prefix(Weapon __instance)
            {
                if (__instance?.parent is not Mech mech || !mech.isHasStability())
                    return;

                bool hasSelfKnockdown = __instance.defId.StartsWith("Weapon_Artillery") ||
                                        __instance.defId.StartsWith("Weapon_Gauss");

                WeaponInfo.ShouldApplySelfInstability = hasSelfKnockdown;
                WeaponInfo.SelfInstabilityWeaponTonnage = hasSelfKnockdown ? __instance.tonnage : 0f;
            }
        }

        [HarmonyPatch(typeof(AttackDirector), "OnAttackComplete")]
        public static class AttackDirector_OnAttackComplete
        {
            [HarmonyPrefix]
            public static void Prefix(AttackDirector __instance, MessageCenterMessage message)
            {
                if (WeaponInfo.ShouldApplySelfInstability)
                {
                    WeaponInfo.ShouldApplySelfInstability = false;

                    if (message is not AttackCompleteMessage attackCompleteMessage)
                        return;

                    int sequenceId = attackCompleteMessage.sequenceId;
                    AttackDirector.AttackSequence attackSequence = __instance.GetAttackSequence(sequenceId);
                    if (attackSequence?.attacker is Mech mech && mech.isHasStability())
                    {
                        float selfInstability = WeaponInfo.SelfInstabilityWeaponTonnage * 1.5f;
                        float instabilityToApply = (!mech.BracedLastRound || mech.DistMovedThisRound > 20f) ?
                            selfInstability :
                            selfInstability / 2f;

                        // Main.Log.LogDebug($"[SelfKnockdown] Applied {instabilityToApply} instability to {mech.DisplayName}.");
                        mech.AddAbsoluteInstability(instabilityToApply, StabilityChangeSource.Effect, mech.GUID);

                        if (!mech.NeedsInstabilityCheck) return;
                        mech.CheckForInstability();

                        if (!mech.IsFlaggedForKnockdown) return;
                        mech.HandleKnockdown(-1, $"{mech.DisplayName}_FromSelfInstability", mech.CurrentPosition, null);

                        mech.DoneWithActor();
                        mech.OnActivationEnd(mech.GUID, -1);
                    }
                }
            }
        }
    }
}