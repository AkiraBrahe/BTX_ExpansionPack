using BattleTech;
using Localize;
using System.Collections.Generic;
using System.Linq;

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
                    var mechComponentRef = mechDef.Inventory[i];
                    if (mechComponentRef.DamageLevel == ComponentDamageLevel.Functional ||
                        mechComponentRef.DamageLevel == ComponentDamageLevel.NonFunctional ||
                        MechValidationRules.MechComponentUnderMaintenance(mechComponentRef, validationLevel, baseWorkOrder))
                    {
                        if (mechComponentRef.ComponentDefType == ComponentType.Weapon)
                        {
                            hasFunctionalWeapon = true;
                            break;
                        }
                        else if (mechComponentRef.ComponentDefType == ComponentType.Upgrade &&
                                 mechComponentRef.Def.ComponentTags.Any(tag => tag == "MeleeWeapon"))
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
