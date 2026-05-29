using BattleTech;
using CustomComponents;
using Localize;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Category = CustomComponents.Category;

namespace BTX_ExpansionPack.Fixes
{
    internal class MechValidation
    {
        /// <summary>
        /// Prevents CAC-C from auto-fixing inventory blockers on mechs.
        /// </summary>
        [HarmonyPatch(typeof(BTX_CAC_CompatibilityDll.MechAutoFixer), "HandleMech")]
        public static class MechAutoFixer_HandleMech
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var targetMethod = AccessTools.Method(typeof(BTX_CAC_CompatibilityDll.MovableBlockers), nameof(BTX_CAC_CompatibilityDll.MovableBlockers.FixMechInventory));

                return new CodeMatcher(instructions)
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode.ToString().StartsWith("ldloc")),
                        new CodeMatch(i => i.opcode.ToString().StartsWith("ldloc")),
                        new CodeMatch(OpCodes.Call, targetMethod))
                    .RemoveInstructions(3)
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Ensures that melee weapons count as valid weapons when validating mech loadouts.
        /// </summary>
        [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
        public static class MechValidationRules_ValidateMechPosessesWeapons
        {
            [HarmonyPrefix]
            public static bool Prefix(MechDef mechDef, MechValidationLevel validationLevel, WorkOrderEntry_MechLab baseWorkOrder, ref Dictionary<MechValidationType, List<Text>> errorMessages)
            {
                bool hasFunctionalWeapon = false;
                for (int i = 0; i < mechDef.Inventory.Length; i++)
                {
                    var componentRef = mechDef.Inventory[i];
                    if (componentRef.DamageLevel == ComponentDamageLevel.Functional ||
                        componentRef.DamageLevel == ComponentDamageLevel.NonFunctional ||
                        MechValidationRules.MechComponentUnderMaintenance(componentRef, validationLevel, baseWorkOrder))
                    {
                        if (componentRef.ComponentDefType == ComponentType.Weapon)
                        {
                            hasFunctionalWeapon = true;
                            break;
                        }
                        else if (componentRef.ComponentDefType == ComponentType.Upgrade &&
                                 componentRef.GetComponents<Category>().Any(c => c.CategoryID is "Handheld" or "Industrial"))
                        {
                            hasFunctionalWeapon = true;
                            break;
                        }
                    }
                }

                if (!hasFunctionalWeapon)
                {
                    MechValidationRules.AddErrorMessage(ref errorMessages, MechValidationType.WeaponsMissing, new Text("MISSING WEAPONS: This 'Mech must mount at least one functional Weapon", []));
                }

                return false;
            }
        }
    }
}