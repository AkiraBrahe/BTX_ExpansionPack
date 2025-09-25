using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using Quirks;
using Quirks.Quirks.MechEffects;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes
{
    /// <summary> 
    /// Implements new mech quirks and fixes existing ones:
    /// <list type="bullet">
    /// <item><description>Anti-Aircraft Targeting: +4 to hit airborne units</description></item>
    /// <item><description>Easy to Pilot: Gains +1 EVASIVE charge when moving, doesn't stack with Sure Footing</description></item>
    /// <item><description>Poor Performance: 'Mech can only sprint if it has moved last turn</description></item>
    /// </list>
    /// </summary>
    internal class MechQuirks
    {
        #region Anti-Aircraft Targeting

        /// <summary>
        /// Adds the Anti-Aircraft Targeting quirk effect to the tooltip.
        /// </summary>
        [HarmonyPatch(typeof(Quirks.Tooltips.QuirkToolTips), "DetailMechQuirksGood")]
        public static class QuirkToolTips_DetailMechQuirksGood
        {
            [HarmonyPostfix]
            public static void Postfix(MechDef mechDef, ref string __result)
            {
                if (mechDef.Chassis.ChassisTags.Contains("mech_quirk_antiaircraft"))
                {
                    __result = __result.Replace("<color=#ffcc00><b>", "<color=#ffcc00><b>\nAnti-Aircraft Targeting: +4 to hit airborne units");
                }
            }
        }

        /// <summary>
        /// Makes LAMs in air mode count as airborne targets when targeted by anti-air mechs.
        /// </summary>
        [HarmonyPatch(typeof(AlternatesRepresentation), "AddCurrentTags")]
        public static class AlternatesRepresentation_AddCurrentTags
        {
            [HarmonyPostfix]
            public static void Postfix(AlternatesRepresentation __instance)
            {
                if (__instance?.parentMech == null || __instance.CurrentRepresentation?.altDef == null)
                    return;

                var tags = __instance.CurrentRepresentation.altDef.additionalEncounterTags;
                MechQuirkInfo.MechQuirkStore[__instance.parentMech.MechDef.Chassis.Description.Id].VTOL = tags.Contains("unit_lam");
            }
        }

        #endregion

        #region Easy to Pilot

        /// <summary>
        /// Modifies the Easy to Pilot quirk effect in the tooltip.
        /// </summary>
        [HarmonyPatch(typeof(Quirks.Tooltips.QuirkToolTips), "DetailMechQuirksGood")]
        public static class QuirkToolTips_DetailMechQuirksGood_Transpiler
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                return new CodeMatcher(instructions, il)
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Ldstr && i.operand is string s && s.Contains("EVASIVE charge increased by 1")))
                    .SetOperandAndAdvance("Gains +1 EVASIVE charge when moving, doesn't stack with Sure Footing")
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Changes the Easy to Pilot quirk to give +1 EVASIVE charge when moving, and prevents it from stacking with Sure Footing.
        /// </summary>
        [HarmonyPatch(typeof(Quirks.Quirks.MechEffects.Mech_InitStats), "Postfix")]
        public static class Mech_InitStats_Postfix
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                return new CodeMatcher(instructions, il)
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Call && i.operand is MethodInfo mi && mi.Name == "get_EasyToPilotEffect"))
                    .MatchBack(false, new CodeMatch(OpCodes.Brfalse_S))
                    .SetOpcodeAndAdvance(OpCodes.Br_S)
                    .InstructionEnumeration();
            }
        }

        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats_EasyToPilot
        {
            [HarmonyPostfix]
            public static void Postfix(Mech __instance)
            {
                var pilot = __instance.GetPilot();
                if (pilot == null) return;

                bool sureFooting = pilot.Abilities.Exists(ability => ability.Def.Id == "AbilityDefP5");
                if (__instance.MechDef.Chassis.ChassisTags.Contains("mech_quirk_easytopilot") && !sureFooting)
                {
                    EffectManager effectManager = UnityGameInstance.BattleTechGame.Combat.EffectManager;
                    effectManager.CreateEffect(EasyToPilotEffect, "EasyToPilot", UnityEngine.Random.Range(1, int.MaxValue), __instance, __instance, default, 0, false);
                    Main.Log.LogDebug($"Applied Easy to Pilot effect to {__instance.DisplayName}.");
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
        [HarmonyPatch(typeof(Quirks.Tooltips.QuirkToolTips), "DetailMechQuirksBad")]
        public static class QuirkToolTips_DetailMechQuirksBad
        {
            [HarmonyPostfix]
            public static void Postfix(MechDef mechDef, ref string __result)
            {
                if (mechDef.Chassis.ChassisTags.Contains("quirk_poor_performance"))
                {
                    __result += "\nPoor Performance: 'Mech can only sprint if it has moved last turn";
                }
            }
        }

        /// <summary>
        /// Implements the Poor Performance quirk, which prevents a 'Mech from sprinting if it did not move last turn.
        /// </summary>
        private static readonly Dictionary<string, CustomQuirkList> CustomQuirkStore = [];
        private class CustomQuirkList
        {
            public bool PoorPerformance = false;
        }

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

        [HarmonyPatch(typeof(Mech), "MaxSprintDistance", MethodType.Getter)]
        [HarmonyPriority(Priority.Last)]
        [HarmonyWrapSafe]
        public static class Mech_MaxSprintDistance
        {
            [HarmonyPostfix]
            public static void Postfix(Mech __instance, ref float __result)
            {
                if (CustomQuirkStore[__instance.GUID].PoorPerformance == true && __instance.LastMoveDistance() < 1f)
                {
                    float walkDistance = __instance.MaxWalkDistance;
                    if (__result > walkDistance)
                        __result = walkDistance;
                }
            }
        }

        #endregion
    }
}