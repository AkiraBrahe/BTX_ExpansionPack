using BattleTech;
using CustAmmoCategories;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    internal class HomingTargeting
    {
        [HarmonyPatch(typeof(BTX_CAC_CompatibilityDll.Main), "A4_Tag_Effect")]
        public static class BTX_CAC_CompatibilityDll_A4_Tag_Effect
        {
            [HarmonyPrefix]
            public static bool Prefix(Weapon wep, ICombatant target, out float __result)
            {
                var weapon = wep;
                if (weapon.mode()?.Id == "ARTY_Guided" &&
                    weapon.ammo()?.Id == "Ammunition_ArrowIV_Homing" &&
                    target.StatCollection.GetValue<float>("TAGCount") +
                    target.StatCollection.GetValue<float>("TAGCountClan") <= 0f)
                {
                    __result = 100f;
                    return false;
                }

                __result = 0f;
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
                        .Where(t =>
                            t != null &&
                           (t.StatCollection.GetValue<float>("TAGCount") +
                            t.StatCollection.GetValue<float>("TAGCountClan") > 0f))
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