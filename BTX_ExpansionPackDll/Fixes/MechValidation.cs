using BattleTech;
using CustomComponents;
using Localize;
using System.Collections.Generic;
using System.Linq;
using Category = CustomComponents.Category;

namespace BTX_ExpansionPack.Fixes
{
    internal class MechValidation
    {
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
