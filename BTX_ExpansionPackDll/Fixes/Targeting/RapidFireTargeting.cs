using BattleTech;
using CustAmmoCategories;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    /// <summary>
    /// Handles AI targeting logic for rapid fire autocannons based on missile threat.
    /// </summary>
    internal class RapidFireTargeting
    {
        [HarmonyPatch(typeof(AttackEvaluator), "MakeAttackOrder")]
        public static class AttackEvaluator_MakeAttackOrder
        {
            [HarmonyPostfix]
            public static void Postfix(AbstractActor unit, BehaviorTreeResults __result)
            {
                if (__result.nodeState == BehaviorNodeState.Failure ||
                    __result.orderInfo is not AttackOrderInfo attackOrder)
                {
                    return;
                }

                var rapidFireWeapons = attackOrder.Weapons.Where(w => w.weaponDef.ComponentTags.Contains("RapidFireAutocannon"));
                if (!rapidFireWeapons.Any())
                    return;

                bool isMissileThreatened = unit.IsMissileThreatened();
                foreach (Weapon weapon in rapidFireWeapons)
                {
                    if (weapon.weaponDef.ComponentTags.Contains("RapidFireAutocannon"))
                    {
                        if (isMissileThreatened)
                        {
                            weapon.setCantAMSFire(false);
                            weapon.forceMode("AMS");
                        }
                        else
                        {
                            weapon.setCantNormalFire(false);
                            weapon.forceMode("RF");
                        }
                    }
                }
            }
        }
    }
}
