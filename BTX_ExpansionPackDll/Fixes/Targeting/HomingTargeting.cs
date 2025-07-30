using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using System.Linq;

namespace BTX_ExpansionPack
{
    internal class HomingTargeting
    {
        [HarmonyPatch(typeof(CombatHUDFireButton), "OnClick")]
        public static class CombatHUDFireButton_OnClick
        {
            [HarmonyPrefix]
            [HarmonyWrapSafe]
            public static bool Prefix(CombatHUDFireButton __instance)
            {
                CombatHUD HUD = __instance.HUD;
                CombatHUDFireButton.FireMode fireMode = __instance.currentFireMode;
                if (fireMode != CombatHUDFireButton.FireMode.Fire)
                    return true;

                if (HUD.SelectedActor.HasActiveHomingArrowIV())
                {
                    ICombatant target = HUD.SelectedTarget;
                    if (target == null || !target.IsTAGed())
                    {
                        GenericPopupBuilder.Create(
                            "Invalid Target",
                            "Arrow IV homing missiles need TAG guidance to attack this unit.")
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

        [HarmonyPatch(typeof(BTX_CAC_CompatibilityDll.Main), "A4_Tag_Effect")]
        public static class BTX_CAC_CompatibilityDll_A4_Tag_Effect
        {
            [HarmonyPrefix]
            public static bool Prefix(Weapon wep, ICombatant target, out float __result)
            {
                __result = wep.IsHomingArrowIV() && (target == null || !target.IsTAGed()) ? 100f : 0f;
                return false;
            }
        }

        [HarmonyPatch(typeof(AttackEvaluator), "MakeAttackOrder")]
        public static class AttackEvaluator_MakeAttackOrder
        {
            [HarmonyPrefix]
            [HarmonyWrapSafe]
            public static void Prefix(AbstractActor unit)
            {
                if (unit == null || unit.BehaviorTree == null || unit.BehaviorTree.enemyUnits == null)
                    return;

                bool hasHoming = unit.Weapons.Any(w =>
                    w.CanFire &&
                    w.ammo() != null &&
                    w.ammo().Id == "Ammunition_ArrowIV_Homing");

                if (hasHoming)
                {
                    var tagged = unit.BehaviorTree.enemyUnits
                        .Where(t => t
                        .IsTAGed())
                        .ToList();

                    if (tagged.Count == 0)
                        return;

                    var untagged = unit.BehaviorTree.enemyUnits.Except(tagged).ToList();
                    unit.BehaviorTree.enemyUnits.Clear();
                    unit.BehaviorTree.enemyUnits.AddRange(tagged);
                    unit.BehaviorTree.enemyUnits.AddRange(untagged);
                }
            }
        }
    }
}