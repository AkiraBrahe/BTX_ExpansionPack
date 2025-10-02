using BattleTech.UI;
using CustAmmoCategories;
using UnityEngine;

namespace BTX_ExpansionPack.Fixes.Targeting
{
    /// <summary>
    /// Prevents ground attacks with weapons that have Homing ammo or minimum/forbidden range restrictions.
    /// </summary>
    internal class AttackGround
    {
        [HarmonyPatch(typeof(SelectionStateCommandAttackGround), "ProcessLeftClick")]
        internal class SelectionStateCommandAttackGround_ProcessLeftClick
        {
            [HarmonyPostfix]
            public static void Postfix(SelectionStateCommandAttackGround __instance, Vector3 worldPos, ref bool __result)
            {
                if (__result == false) return;

                var actor = __instance.SelectedActor;
                foreach (var weapon in actor.Weapons)
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
                        return;
                    }

                    // Prevent ground attack within minimum or forbidden ranges
                    float distance = Vector3.Distance(actor.CurrentPosition, worldPos);
                    float minRange = weapon.MinRange;
                    float forbiddenRange = weapon.ForbiddenRange() > 0f
                        ? weapon.ForbiddenRange() : weapon.AOERange() > 0f
                            ? weapon.AOERange() : 0f; //blast radius

                    if (distance < minRange)
                    {
                        GenericPopupBuilder.Create(
                            $"Target Too Close",
                            $"Your {weapon.Name} requires a minimum range of {minRange:F0}m to attack.\nCurrent distance: {distance:F0}m.")
                            .AddButton("Ok")
                            .IsNestedPopupWithBuiltInFader()
                            .CancelOnEscape()
                            .Render();
                        return;
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
                        return;
                    }
                }
            }
        }
    }
}
