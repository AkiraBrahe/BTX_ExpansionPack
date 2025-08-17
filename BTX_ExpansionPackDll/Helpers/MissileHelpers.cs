using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using UnityEngine;

namespace BTX_ExpansionPack
{
    public static class MissileHelpers
    {
        public static bool IsMissileThreatened(this AbstractActor unit)
        {
            float predictedMissileDamage = 0f;

            foreach (var enemy in unit.lance.team.GetDetectedEnemyUnits())
            {
                float distance = Vector3.Distance(enemy.CurrentPosition, unit.CurrentPosition);
                if (distance <= 60f)
                    continue;

                foreach (Weapon weapon in enemy.Weapons)
                {
                    if (!weapon.CanFire || weapon.AMSImmune())
                        continue;

                    var missileEffect = weapon.getWeaponEffect() as MissileLauncherEffect;
                    if (missileEffect != null)
                    {
                        if (distance <= weapon.MaxRange)
                        {
                            float toHit = weapon.GetToHitFromPosition(unit, 1, enemy.CurrentPosition, unit.CurrentPosition, true, unit.IsEvasive, false);
                            float damage = weapon.ShotsWhenFired * toHit * (weapon.DamagePerShot + weapon.HeatDamagePerShot);
                            predictedMissileDamage += damage;
                        }
                    }
                }
            }

            return Random.Range(0f, 100f) < predictedMissileDamage;
        }

        public static bool IsDedicatedMissileBoat(this AbstractActor unit) =>
            unit is Mech mech && mech.MechDef.Chassis.StockRole.StartsWith("Missile Boat") ||
            unit is FakeVehicleMech fakevehicle && fakevehicle.ToMechDef().MechTags.Contains("role_missileboat") ||
            unit is Vehicle vehicle && vehicle.VehicleDef.VehicleTags.Contains("role_missileboat");

        public static bool HasArtemis(this Mech mech)
        {
            foreach (MechComponent mechComponent in mech.allComponents)
            {
                if (mechComponent.defId.StartsWith("Gear_Addon_Artemis"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
