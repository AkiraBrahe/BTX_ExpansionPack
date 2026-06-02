using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using System.Linq;
using UnityEngine;

namespace BTX_ExpansionPack.Core
{
    public static class Extensions
    {
        #region Artillery

        extension(AbstractActor unit)
        {
            /// <summary>
            /// Determines if a unit is essentially stationary.
            /// </summary>
            public bool IsStationary()
            {
                float positionDelta = Vector3.Distance(unit.CurrentPosition, unit.PreviousPosition);
                return positionDelta < 2f;
            }

            /// <summary>
            /// Returns a score 0-1 based on how mobile the target is.
            /// </summary>
            public float GetTargetMobility()
            {
                float positionDelta = Vector3.Distance(unit.CurrentPosition, unit.PreviousPosition);
                float maxWalkDistance = unit.MovementCaps?.MaxWalkDistance ?? 0f;
                return maxWalkDistance > 0 ? Mathf.Clamp01(positionDelta / maxWalkDistance) : 0f;
            }

            /// <summary>
            /// Determines if a unit is an artillery unit capable of firing artillery weapons.
            /// </summary>
            public bool IsArtilleryUnit()
            {
                if (unit.Weapons == null) return false;
                foreach (var w in unit.Weapons)
                {
                    if (w.IsArtillery() || (w.Description != null && w.Description.Id != null && w.Description.Id.StartsWith("Weapon_Artillery", System.StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Determines if a unit is a dedicated missile boat based on its role and tonnage.
            /// </summary>
            public bool IsDedicatedMissileBoat() => (unit.GetTonnage() >= 60 &&
                unit is Mech mech && mech.MechDef.Chassis.StockRole.StartsWith("Missile Boat")) ||
                (unit is FakeVehicleMech fakevehicle && fakevehicle.ToMechDef().MechTags.Contains("role_missileboat")) ||
                (unit is Vehicle vehicle && vehicle.VehicleDef.VehicleTags.Contains("role_missileboat"));
        }

        extension(Weapon weapon)
        {
            /// <summary>
            /// Gets the Targeting-Tracking System (TTS) level of an artillery weapon.
            /// </summary>
            public int ArtilleryTTSLevel() => (int)weapon.GetStatisticFloat("AMSAttractiveness");

            /// <summary>
            /// Determines if a target position is within range of an artillery weapon.
            /// </summary>
            public bool CanHitTargetPosition(Vector3 attackerPosition, Vector3 targetPosition)
            {
                float distance = Vector3.Distance(attackerPosition, targetPosition);
                float minRange = Mathf.Max(weapon.MinRange, weapon.ForbiddenRange());
                return distance >= minRange && distance <= weapon.MaxRange;
            }

            /// <summary>
            /// Determines if a target position is outside the minimum and forbidden ranges of an artillery weapon.
            /// </summary>
            public bool IsOutsideSafeRange(Vector3 attackerPosition, Vector3 targetPosition, out float unsafeRange)
            {
                float distance = Vector3.Distance(attackerPosition, targetPosition);
                unsafeRange = Mathf.Max(weapon.MinRange, weapon.ForbiddenRange());
                return distance >= unsafeRange;
            }
        }

        #endregion

        #region Homing Arrow IV

        extension(AbstractActor unit)
        {
            /// <summary>
            /// Determines if a unit has an active Arrow IV with homing ammo.
            /// </summary>
            public bool HasActiveHomingArrowIV()
            {
                if (unit == null || unit.StatCollection == null)
                    return false;

                if (!unit.StatCollection.GetValue<bool>("HasHomingArrowIV"))
                    return false;

                if (unit.Weapons == null) return false;
                foreach (var weapon in unit.Weapons)
                {
                    if (weapon.CanFire && weapon.IsHomingArrowIV())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Determines if a weapon is an Arrow IV with homing ammo.
        /// </summary>
        public static bool IsHomingArrowIV(this Weapon weapon)
        {
            return weapon != null &&
                   weapon.mode()?.Id == "ARTY_Guided" &&
                   weapon.ammo()?.Id == "Ammunition_ArrowIV_Homing";
        }

        /// <summary>
        /// Determines if a target is TAGed.
        /// </summary>
        public static bool IsTAGed(this ICombatant target)
        {
            return target != null && target.StatCollection != null &&
                   target.StatCollection.GetValue<float>("TAGCount") +
                   target.StatCollection.GetValue<float>("TAGCountClan") > 0f;
        }

        #endregion

        #region Special Ammo Upgrades

        /// <summary>
        /// Determines if any unit of the given team has a TAG weapon.
        /// </summary>
        public static bool AnyUnitHasTAG(this FactionValue team)
        {
            var combat = UnityGameInstance.BattleTechGame.Combat;
            if (combat != null)
            {
                foreach (var combatTeam in combat.Teams)
                {
                    if (combatTeam.FactionValue != null && combatTeam.FactionValue.Name == team.Name)
                    {
                        foreach (var actor in combatTeam.units)
                        {
                            if (actor.Weapons == null) continue;
                            foreach (var weapon in actor.Weapons)
                            {
                                if (weapon.defId != null && weapon.defId.StartsWith("Weapon_TAG"))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        #region Anti-Missile System

        extension(AbstractActor unit)
        {
            /// <summary>
            /// Determines if a unit is threatened by incoming missiles based on enemy missile weaponry and distance.
            /// </summary>
            /// <remarks>
            /// The chance of being threatened is equal to the predicted missile damage (e.g., 50 damage = 50% chance).
            /// It is purposefuly conservative to avoid overestimating threat and allow for more aggressive AI play.
            /// </remarks>
            public bool IsMissileThreatened()
            {
                float predictedMissileDamage = 0f;
                var detectedEnemies = unit.lance.team.GetDetectedEnemyUnits().Where(enemy => !enemy.IsDead).ToList();

                foreach (var enemy in detectedEnemies)
                {
                    float distance = Vector3.Distance(enemy.CurrentPosition, unit.CurrentPosition);
                    if (distance <= 60f)
                        continue;

                    foreach (var weapon in enemy.Weapons)
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
        }

        /// <summary>
        /// Determines if a mech has an Artemis IV or V system installed.
        /// </summary>
        public static bool HasArtemis(this Mech mech) => mech?.allComponents?.Any(comp => comp.defId.StartsWith("Gear_Addon_Artemis")) ?? false;

        #endregion
    }
}