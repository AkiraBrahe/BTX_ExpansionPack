using BattleTech;
using BTX_ExpansionPack.Helpers;
using CustAmmoCategories;
using Quirks.Quirks.MechEffects;
using System.Linq;

namespace BTX_ExpansionPack.Fixes.Targeting
{

    public static class RapidFireTargeting
    {
        /// <summary>
        /// Restricts AMS modes on Rapid-Fire Autocannons to mechs with Anti-Aircraft Targeting.
        /// </summary>
        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats
        {
            [HarmonyPostfix]
            [HarmonyAfter("BEX.BattleTech.Extended_CE")]
            public static void Postfix(Mech __instance)
            {
                bool hasAATargeting = MechQuirkInfo.MechQuirkStore.TryGetValue(__instance.MechDef.Chassis.Description.Id, out var quirk) && quirk.AntiAircraftTargeting ||
                    (__instance.FakeVehicle() && __instance.MechDef.MechTags.Contains("unit_vehicle_airDefense"));
                if (hasAATargeting) return;

                foreach (var weapon in __instance.Weapons)
                {
                    if (weapon.IsRFAC())
                    {
                        weapon.info().DisableMode("AMS");
                        weapon.info().DisableMode("AMS_Plus");
                    }
                }
            }
        }

        /// <summary>
        /// Handles AI targeting logic for rapid fire autocannons based on missile threat.
        /// </summary>
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

                var rapidFireWeapons = attackOrder.Weapons.Where(w => w.IsRFAC());
                if (!rapidFireWeapons.Any())
                    return;

                bool isMissileThreatened = unit.IsMissileThreatened();
                foreach (Weapon weapon in rapidFireWeapons)
                {
                    if (isMissileThreatened) // && weapon.info().isModeAvailble(weapon.info().modes["AMS"], out _))
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

        /// <summary>
        /// Determines if a weapon is a rapid-fire autocannon.
        /// </summary>
        public static bool IsRFAC(this Weapon weapon)
        {
            return weapon.info()?.modes.ContainsKey("RF") ?? false;
        }
    }
}