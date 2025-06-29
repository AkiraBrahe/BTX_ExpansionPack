using BattleTech;
using CustomAmmoCategoriesPatches;
using System.Collections.Concurrent;
using System.Linq;

namespace BTX_ExpansionPack
{
    internal class SelfKnockdown
    {
        private static readonly ConcurrentDictionary<int, float> InstabilityBySequence = new();

        [HarmonyPatch(typeof(Weapon), "ProcessOnFiredFloatieEffects")]
        public static class Weapon_ProcessOnFiredFloatieEffects
        {
            [HarmonyPrefix]
            public static void Prefix(Weapon __instance)
            {
                if (__instance?.parent is not Mech mech || !mech.isHasStability())
                    return;

                var attackDirector = mech.Combat?.AttackDirector;
                if (attackDirector?.allAttackSequences is { Count: > 0 } sequences)
                {
                    var lastSeqId = sequences.Keys.Max();
                    InstabilityBySequence[lastSeqId] = __instance.tonnage;
                    Main.Log.LogDebug($"[SelfKnockdown] {__instance.Name} has SelfKnockdown tag. Flag set for sequence {lastSeqId}.");
                }
            }
        }

        [HarmonyPatch(typeof(AttackDirector), "OnAttackComplete")]
        public static class AttackDirector_OnAttackComplete
        {
            [HarmonyPrefix]
            public static void Prefix(AttackDirector __instance, MessageCenterMessage message)
            {
                if (message is not AttackCompleteMessage attackCompleteMessage)
                    return;

                var sequenceId = attackCompleteMessage.sequenceId;
                if (InstabilityBySequence.TryRemove(sequenceId, out var tonnage))
                {
                    var attackSequence = __instance.GetAttackSequence(sequenceId);
                    if (attackSequence?.attacker is Mech mech && mech.isHasStability())
                    {
                        var selfInstability = tonnage * 1.5f;
                        var instabilityToApply = (!mech.BracedLastRound || mech.DistMovedThisRound > 20f)
                            ? selfInstability
                            : selfInstability / 2f;

                        Main.Log.LogDebug($"[SelfKnockdown] Applying {instabilityToApply} instability to {mech.DisplayName} (sequence {sequenceId}).");
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