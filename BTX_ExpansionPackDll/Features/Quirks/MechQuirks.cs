using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using global::Quirks.Tooltips;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Features.Quirks
{
    /// <summary>
    /// Implements new mech quirks and fixes existing ones:
    /// <list type="bullet">
    /// <item>Anti-Aircraft Targeting: +4 to hit airborne units</item>
    /// <item>Easy to Pilot: Gains +1 EVASIVE charge when moving, doesn't stack with Sure Footing</item>
    /// <item>Poor Performance: 'Mech can only sprint if it has moved last turn</item>
    /// <item>Poor Workmanship: 'Mech takes more critical hits</item>
    /// </list>
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
                if (chassisDef.ChassisTags.Contains(QuirkTags.ANTI_AIRCRAFT))
                {
                    __result += "\nAnti-Aircraft Targeting: +4 to hit airborne units";
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

                var quirks = mech.MechDef.GetOrSetDefaultQuirks();
                quirks.VTOL = altRep.state == AltRepState.Flying;
                if (quirks.VTOL) Main.Logger.LogDebug($"[MechQuirks] {mech.DisplayName} is now flying and counts as valid airborne target.");
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
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Ldstr && i.operand is string s && s.StartsWith("EVASIVE charge cap increased by 1")))
                    .SetOperandAndAdvance("Gains +1 EVASIVE charge when moving, doesn't stack with Sure Footing")
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Prevents the old Easy to Pilot quirk effect from being applied.
        /// </summary>
        [HarmonyPatch(typeof(global::Quirks.Quirks.MechEffects.Mech_InitStats), "Postfix")]
        public static class Mech_InitStats_Postfix
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var matcher = new CodeMatcher(instructions)
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
            [HarmonyWrapSafe]
            public static void Postfix(Mech __instance)
            {
                var pilot = __instance.GetPilot();
                if (pilot == null) return;

                bool pilotHasSureFooting = pilot.Abilities.Exists(ability => ability.Def.Id == "AbilityDefP5");
                if (__instance.MechDef.Chassis.ChassisTags.Contains(QuirkTags.EASY_TO_PILOT) && !pilotHasSureFooting)
                {
                    var effectManager = UnityGameInstance.BattleTechGame.Combat.EffectManager;
                    effectManager.CreateEffect(CustomQuirkEffects.EasyToPilotEffect, "EasyToPilot", UnityEngine.Random.Range(1, int.MaxValue), __instance, __instance, default, 0, false);
                }
            }
        }

        #endregion

        #region Exposed Actuators

        /// <summary>
        /// Ensures that BEX correctly finds quad mechs so that the Exposed Actuators quirk can be applied to them.
        /// </summary>
        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats_ExposedActuators
        {
            [HarmonyPostfix]
            [HarmonyWrapSafe]
            public static void Postfix(Mech __instance)
            {
                var mechDef = __instance.MechDef;
                if (mechDef.MechTags.Contains("unit_quad"))
                {
                    var quirks = mechDef.GetOrSetDefaultQuirks();
                    quirks.QuadMech = true;
                }
            }
        }

        #endregion

        #region Poor Performance

        /// <summary>
        /// Adds the Poor Performance quirk effect to the tooltip.
        /// </summary>
        [HarmonyPatch(typeof(QuirkToolTips), "DetailMechQuirksBad")]
        public static class QuirkToolTips_DetailMechQuirksBad_PoorPerformance
        {
            [HarmonyPostfix]
            public static void Postfix(ChassisDef chassisDef, ref string __result)
            {
                if (chassisDef.ChassisTags.Contains(QuirkTags.POOR_PERFORMANCE))
                {
                    __result += "\nPoor Performance: 'Mech can only sprint if it has moved last turn";
                }
            }
        }

        /// <summary>
        /// Initializes the Poor Performance quirk effect in the custom quirk store.
        /// </summary>
        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats_PoorPerformance
        {
            [HarmonyPrefix]
            [HarmonyWrapSafe]
            public static void Prefix(Mech __instance)
            {
                if (!__instance.MechDef.Chassis.ChassisTags.Contains(QuirkTags.POOR_PERFORMANCE)) return;

                var customQuirks = __instance.MechDef.GetOrSetCustomQuirks();
                customQuirks.PoorPerformance = true;
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
                var customQuirks = __instance.MechDef.GetOrSetCustomQuirks();
                if (customQuirks.PoorPerformance && __instance.LastMoveDistance() < 24f)
                {
                    __result = false;
                }
            }
        }

        #endregion

        #region Poor Workmanship

        /// <summary>
        /// Adds the Poor Workmanship quirk effect to the tooltip.
        /// </summary>
        [HarmonyPatch(typeof(QuirkToolTips), "DetailMechQuirksBad")]
        public static class QuirkToolTips_DetailMechQuirksBad_PoorWorkmanship
        {
            [HarmonyPostfix]
            public static void Postfix(ChassisDef chassisDef, ref string __result)
            {
                if (chassisDef.ChassisTags.Contains(QuirkTags.POOR_WORKMANSHIP))
                {
                    __result += "\nPoor Workmanship: 'Mech takes more critical hits";
                }
            }
        }

        /// <summary>
        /// Applies the Poor Workmanship quirk effect if the mech has the tag.
        /// </summary>
        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats_PoorWorkmanship
        {
            [HarmonyPrefix]
            [HarmonyWrapSafe]
            public static void Prefix(Mech __instance)
            {
                if (__instance.MechDef.Chassis.ChassisTags.Contains(QuirkTags.POOR_WORKMANSHIP))
                {
                    var effectManager = UnityGameInstance.BattleTechGame.Combat.EffectManager;
                    effectManager.CreateEffect(CustomQuirkEffects.PoorWorkmanshipEffect, "PoorWorkmanship", UnityEngine.Random.Range(1, int.MaxValue), __instance, __instance, default, 0, false);
                }
            }
        }

        #endregion
    }
}