using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using System.Linq;
using UnityEngine;

namespace BTX_ExpansionPack.Helpers
{
    /// <summary>
    /// Helpers for missile-related functionality.
    /// </summary>
    public static class MissileHelpers
    {
        /// <summary>
        /// Determines if a unit is threatened by incoming missiles based on enemy missile weaponry and distance.
        /// </summary>
        /// <remarks>
        /// The chance of being threatened is equal to the predicted missile damage (e.g., 50 damage = 50% chance).
        /// It is purposefuly conservative to avoid overestimating threat and allow for more aggressive AI play.
        /// </remarks>
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

        /// <summary>
        /// Determines if a unit is a dedicated missile boat based on its chassis role or tags.
        /// </summary>
        public static bool IsDedicatedMissileBoat(this AbstractActor unit) =>
            unit is Mech mech && mech.MechDef.Chassis.StockRole.StartsWith("Missile Boat") ||
            unit is FakeVehicleMech fakevehicle && fakevehicle.ToMechDef().MechTags.Contains("role_missileboat") ||
            unit is Vehicle vehicle && vehicle.VehicleDef.VehicleTags.Contains("role_missileboat");

        /// <summary>
        /// Determines if a mech has an Artemis IV or V system installed.
        /// </summary>
        public static bool HasArtemis(this Mech mech)
        {
            return mech?.allComponents?.Any(comp => comp.defId.StartsWith("Gear_Addon_Artemis")) ?? false;
        }
    }
}
