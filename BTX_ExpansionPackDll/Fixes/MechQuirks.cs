using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using Quirks;
using Quirks.Quirks.MechEffects;
using Quirks.Tooltips;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes
{
    /// <summary>
    /// Implements new mech quirks and fixes existing ones:
    /// <list type="bullet"><item><description>Anti-Aircraft Targeting: +4 to hit airborne units</description></item>
    /// <item><description>Easy to Pilot: Gains +1 EVASIVE charge when moving, doesn't stack with Sure Footing</description></item>
    /// <item><description>Poor Performance: 'Mech can only sprint if it has moved last turn</description></item></list>
    /// </summary>
    internal class MechQuirks
    {
        #region Anti-Aircraft Targeting

        /// <summary>
        /// Adds the Anti-Aircraft Targeting quirk effect to the tooltip.
        /// </summary>
        [HarmonyPatch(typeof(QuirkToolTips), "DetailMechQuirksGood")]
        public static class QuirkToolTips_DetailMechQuirksGood_AntiAircraft
        {
            [HarmonyPostfix]
            public static void Postfix(ChassisDef chassisDef, ref string __result)
            {
                if (chassisDef.ChassisTags.Contains("mech_quirk_antiaircraft"))
                {
                    __result = __result.Replace("<color=#ffcc00><b>", "<color=#ffcc00><b>\nAnti-Aircraft Targeting: +4 to hit airborne units");
                }
            }
        }

        /// <summary>
        /// Makes LAMs in air mode count as airborne targets when targeted by anti-air mechs.
        /// </summary>
        [HarmonyPatch(typeof(AbstractActor), "OnActivationEnd")]
        public static class AbstractActor_OnActivationEnd
        {
            [HarmonyPostfix]
            public static void Postfix(AbstractActor __instance)
            {
                if (__instance is not Mech mech || mech.GameRep == null) return;

                var altRep = mech.GameRep.GetComponent<AlternateMechRepresentation>();
                if (altRep == null) return;

                string chassisId = mech.MechDef.ChassisID;
                if (!MechQuirkInfo.MechQuirkStore.TryGetValue(chassisId, out var quirk))
                {
                    quirk = new QuirkList(); MechQuirkInfo.MechQuirkStore.Add(chassisId, quirk);
                }
                quirk.VTOL = altRep.state == AltRepState.Flying;
                if (quirk.VTOL) Main.Log.LogDebug($"[MechQuirks] {mech.DisplayName} is now flying and counts as valid airborne target.");
            }
        }

        #endregion

        #region Easy to Pilot

        /// <summary>
        /// Changes the Easy to Pilot quirk effect in the tooltip.
        /// </summary>
        [HarmonyPatch(typeof(QuirkToolTips), "DetailMechQuirksGood")]
        public static class QuirkToolTips_DetailMechQuirksGood_EasyToPilot
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                return new CodeMatcher(instructions, il)
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Ldstr && i.operand is string s && s.StartsWith("EVASIVE charge cap increased by 1")))
                    .SetOperandAndAdvance("Gains +1 EVASIVE charge when moving, doesn't stack with Sure Footing")
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Prevents the old Easy to Pilot quirk effect from being applied.
        /// </summary>
        [HarmonyPatch(typeof(Quirks.Quirks.MechEffects.Mech_InitStats), "Postfix")]
        public static class Mech_InitStats_Postfix
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions, il)
                    .MatchForward(false, new CodeMatch(i => i.opcode == OpCodes.Call && i.operand is MethodInfo mi && mi.Name == "get_EasyToPilotEffect"))
                    .MatchBack(false, new CodeMatch(i => i.opcode.FlowControl == FlowControl.Cond_Branch));

                object jumpTarget = matcher.Operand;
                return matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Br, jumpTarget))
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Applies the new Easy to Pilot quirk effect unless the pilot has Sure Footing.
        /// </summary>
        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats_EasyToPilot
        {
            [HarmonyPostfix]
            public static void Postfix(Mech __instance)
            {
                var pilot = __instance.GetPilot();
                if (pilot == null) return;

                bool pilotHasSureFooting = pilot.Abilities.Exists(ability => ability.Def.Id == "AbilityDefP5");
                if (__instance.MechDef.Chassis.ChassisTags.Contains("mech_quirk_easytopilot") && !pilotHasSureFooting)
                {
                    var effectManager = UnityGameInstance.BattleTechGame.Combat.EffectManager;
                    effectManager.CreateEffect(EasyToPilotEffect, "EasyToPilot", UnityEngine.Random.Range(1, int.MaxValue), __instance, __instance, default, 0, false);
                }
            }

            internal static EffectData EasyToPilotEffect => new()
            {
                effectType = EffectType.StatisticEffect,
                targetingData = QuirkStatusEffects.OnActivation,
                Description = new DescriptionDef("TraitDefEvasiveChargeAddOne", "Increased Evasion", "Gains +[AMT] EVASIVE charge when moving", "uixSvgIcon_ability_mastertactician", 0, 0f, false, null, null, null),
                durationData = QuirkStatusEffects.Duration,
                statisticData = new StatisticEffectData
                {
                    statName = "EvasivePipsGainedAdditional",
                    operation = StatCollection.StatOperation.Int_Add,
                    modValue = "1",
                    modType = "System.Int32"
                },
                nature = EffectNature.Buff
            };
        }

        #endregion

        #region Poor Performance

        /// <summary>
        /// Adds the Poor Performance quirk effect to the tooltip.
        /// </summary>
        [HarmonyPatch(typeof(QuirkToolTips), "DetailMechQuirksBad")]
        public static class QuirkToolTips_DetailMechQuirksBad
        {
            [HarmonyPostfix]
            public static void Postfix(ChassisDef chassisDef, ref string __result)
            {
                if (chassisDef.ChassisTags.Contains("quirk_poor_performance"))
                {
                    __result += "\nPoor Performance: 'Mech can only sprint if it has moved last turn";
                }
            }
        }

        /// <summary>
        /// Stores custom mech quirks that need to track state.
        /// </summary>
        private static readonly Dictionary<string, CustomQuirkList> CustomQuirkStore = [];
        private class CustomQuirkList
        {
            public bool PoorPerformance = false;
        }

        /// <summary>
        /// Marks mechs with the Poor Performance quirk in the custom quirk store.
        /// </summary>
        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats_PoorPerformance
        {
            [HarmonyPostfix]
            public static void Postfix(Mech __instance)
            {
                if (__instance.MechDef.Chassis.ChassisTags.Contains("quirk_poor_performance"))
                {
                    if (!CustomQuirkStore.ContainsKey(__instance.GUID))
                        CustomQuirkStore[__instance.GUID] = new CustomQuirkList();
                    CustomQuirkStore[__instance.GUID].PoorPerformance = true;
                }
            }
        }

        /// <summary>
        /// Makes the Poor Performance quirk limit the max sprint distance based on whether the mech moved last turn.
        /// </summary>
        [HarmonyPatch(typeof(Mech), "CanSprint", MethodType.Getter)]
        public static class Mech_CanSprint
        {
            [HarmonyPostfix]
            public static void Postfix(Mech __instance, ref bool __result)
            {
                if (CustomQuirkStore.TryGetValue(__instance.GUID, out var customQuirks) &&
                    customQuirks.PoorPerformance && __instance.LastMoveDistance() < 1f)
                {
                    __result = false;
                }
            }
        }

        #endregion
    }
}