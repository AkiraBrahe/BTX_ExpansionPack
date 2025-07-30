using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using UnityEngine;

namespace BTX_ExpansionPack.Fixes
{
    internal class AttackGround
    {
        [HarmonyPatch(typeof(SelectionStateCommandAttackGround), "ProcessLeftClick")]
        internal class SelectionStateCommandAttackGround_ProcessLeftClick
        {
            [HarmonyPrefix]
            public static bool Prefix(SelectionStateCommandAttackGround __instance, Vector3 worldPos)
            {
                if (__instance.NumPositionsLocked != 0) return false;

                AbstractActor selectedActor = __instance.SelectedActor;
                foreach (Weapon weapon in selectedActor.Weapons)
                {
                    if (!weapon.IsFunctional || !weapon.IsEnabled || weapon.isAMS())
                        continue;

                    // Prevent ground attack with Homing ammo
                    if (weapon.ammo()?.Id == "Ammunition_ArrowIV_Homing")
                    {
                        GenericPopupBuilder.Create(
                            $"Invalid Target",
                            $"Arrow IV homing missiles can only target enemy units directly.")
                            .AddButton("Ok")
                            .IsNestedPopupWithBuiltInFader()
                            .CancelOnEscape()
                            .Render();
                        return false;
                    }

                    // Prevent ground attack within minimum or forbidden ranges
                    float distance = Vector3.Distance(selectedActor.CurrentPosition, worldPos);
                    float minRange = weapon.MinRange;
                    float forbiddenRange = weapon.ForbiddenRange() > 0f ?
                        weapon.ForbiddenRange() :
                        weapon.AOERange() / 2f;

                    if (distance < minRange)
                    {
                        GenericPopupBuilder.Create(
                            $"Target Too Close",
                            $"Your {weapon.Name} requires a minimum range of {minRange:F0}m to attack.\nCurrent distance: {distance:F0}m.")
                            .AddButton("Ok")
                            .IsNestedPopupWithBuiltInFader()
                            .CancelOnEscape()
                            .Render();
                        return false;
                    }
                    else if (distance < forbiddenRange)
                    {
                        GenericPopupBuilder.Create(
                            $"Target Too Close",
                            $"Your {weapon.Name} requires a safe range of {forbiddenRange:F0}m to attack.\nCurrent distance: {distance:F0}m.")
                            .AddButton("Ok")
                            .IsNestedPopupWithBuiltInFader()
                            .CancelOnEscape()
                            .Render();
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
