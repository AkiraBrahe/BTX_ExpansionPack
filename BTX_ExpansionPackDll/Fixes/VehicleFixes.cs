using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BTX_CAC_CompatibilityDll;
using CustAmmoCategories;
using Extended_CE;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes
{
    internal class VehicleFixes
    {
        /// <summary>
        /// Removes melee damage from vehicles (player and AI).
        /// </summary>
        [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
        public class ChassisDef_FromJSON_Patch
        {
            [HarmonyPostfix, HarmonyPriority(Priority.Last)]
            public static void Postfix(ChassisDef __instance)
            {
                if (__instance.IsVehicle())
                {
                    __instance.MeleeDamage = 0;
                }
            }
        }

        /// <summary>
        /// Prevents AI-controlled vehicles from attempting melee or DFA attacks.
        /// </summary>
        [HarmonyPatch(typeof(AttackEvaluator), "MakeAttackOrderForTarget")]
        public static class AttackEvaluator_MakeAttackOrderForTarget
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions, il)
                    .MatchForward(true,
                        new CodeMatch(OpCodes.Ldloc_S),
                        new CodeMatch(OpCodes.Ldc_I4_1),
                        new CodeMatch(OpCodes.Add),
                        new CodeMatch(OpCodes.Stloc_S))
                    .ThrowIfInvalid("Failed to find loop increment (j++)")
                    .CreateLabel(out var continueLabel)
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Ldarg_0, name: "unit"),
                        new CodeMatch(OpCodes.Ldstr, "considering attack type "),
                        new CodeMatch(i => i.IsLdloc(), name: "j"))
                    .ThrowIfInvalid("Failed to find the attack type log message");

                var skipVehicleCheckLabel = il.DefineLabel();
                matcher.Instruction.labels.Add(skipVehicleCheckLabel);

                return matcher.InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UnitUnaffectionsActorStats), nameof(UnitUnaffectionsActorStats.FakeVehicle), [typeof(ICombatant)])),
                        new CodeInstruction(OpCodes.Brfalse, skipVehicleCheckLabel),
                        new CodeInstruction(OpCodes.Ldloc, matcher.NamedMatch("j").operand),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ble, skipVehicleCheckLabel),
                        new CodeInstruction(OpCodes.Br, continueLabel))
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Prevents hand/arm actuator effects from being added to vehicles.
        /// </summary>
        [HarmonyPatch(typeof(BTComponents.Mech_InitStats), "Postfix")]
        public static class Mech_InitStats_Postfix
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                return new CodeMatcher(instructions, il)
                    .MatchForward(true,
                        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Core), nameof(Core.UsingComponents))),
                        new CodeMatch(OpCodes.Stloc_S),
                        new CodeMatch(OpCodes.Ldloc_S),
                        new CodeMatch(i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S))
                    .CreateLabel(out var skipEffectsLabel)
                    .MatchBack(false,
                        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Core), nameof(Core.UsingComponents))))
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Mech_InitStats_Postfix), nameof(IsFakeVee))),
                        new CodeInstruction(OpCodes.Brtrue_S, skipEffectsLabel)
                    )
                    .InstructionEnumeration();
            }

            public static bool IsFakeVee(Mech mech) => mech.FakeVehicle();
        }

        /// <summary>
        /// Uses actual vehicle weight instead of calculating it.
        /// </summary>
        [HarmonyPatch(typeof(MechStatisticsRules), "CalculateTonnage")]
        public class MechStatisticsRules_CalculateTonnage
        {
            [HarmonyPrefix]
            public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
            {
                maxValue = mechDef.Chassis.Tonnage;
                currentValue = mechDef.IsVehicle() ? mechDef.Chassis.Tonnage : mechDef.CalculateWeightKG() / 1000f;
                return false;
            }
        }

        /// <summary>
        /// Shows correct tonnage and remaining tonnage for vehicles in the mech lab.
        /// </summary>
        [HarmonyPatch(typeof(MechLabMechInfoWidget), "CalculateTonnage")]
        public class MechLabMechInfoWidget_CalculateTonnage
        {
            [HarmonyPrefix]
            public static bool Prefix(MechLabMechInfoWidget __instance, MechLabPanel ___mechLab, LocalizableText ___totalTonnage, UIColorRefTracker ___totalTonnageColor, LocalizableText ___remainingTonnage, UIColorRefTracker ___remainingTonnageColor)
            {
                if (___mechLab.activeMechDef == null)
                    return true;

                ChassisDef chassisDef = ___mechLab.activeMechDef.Chassis;
                int currentWeightKG = ___mechLab.CalculateWeightKG();
                int remainingWeightKG = (int)(chassisDef.Tonnage * 1000f) - currentWeightKG;

                __instance.currentTonnage = currentWeightKG / 1000f;

                if (___mechLab.activeMechDef.IsVehicle())
                {
                    ___totalTonnage.SetText("{0:0.##}", chassisDef.Tonnage);
                    ___totalTonnageColor.SetUIColor(UIColor.White);
                    ___remainingTonnage.SetText("");
                    ___remainingTonnageColor.SetUIColor(UIColor.White);
                }
                else
                {
                    ___totalTonnage.SetText("{0:0.##} / {1}",
                    [
                        __instance.currentTonnage,
                        chassisDef.Tonnage
                    ]);
                    ___totalTonnageColor.SetUIColor((remainingWeightKG < 0) ? UIColor.Red : UIColor.WhiteHalf);

                    bool isOverweight = remainingWeightKG < 0;
                    if (isOverweight)
                    {
                        float overweightTons = -remainingWeightKG / 1000f;
                        ___remainingTonnage.SetText("{0:0.##} ton{1} overweight",
                        [
                            overweightTons,
                            (remainingWeightKG == -1000) ? "" : "s"
                        ]);
                    }
                    else
                    {
                        ___remainingTonnage.SetText("{0:0.##} ton{1} remaining",
                        [
                            remainingWeightKG / 1000f,
                            (remainingWeightKG == 1000) ? "" : "s"
                        ]);
                    }
                    ___remainingTonnageColor.SetUIColor((remainingWeightKG < 0) ? UIColor.Red : ((remainingWeightKG <= 500) ? UIColor.Gold : UIColor.White));
                }

                return false;
            }
        }
    }
}
