using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using HBS.Collections;
using System;
using System.Linq;
using UnityEngine;

namespace BTX_ExpansionPack.Core
{
    public static class Extensions
    {
        #region Structure and Armor Info

        /// <summary>
        /// Retrieves the structure info of a mech.
        /// </summary>
        public static StructureInfo GetStructureInfo(this MechDef mech)
        {
            var type = StructureType.Standard;

            bool isClan = mech.Chassis.ChassisTags.Contains("chassis_clan");
            foreach (string tag in mech.Chassis.ChassisTags)
            {
                var match = StructureTypes.FirstOrDefault(st => !string.IsNullOrEmpty(st.Value.Tag) && st.Value.Tag == tag);
                if (match.Value.Tag != null)
                {
                    type = match.Key;
                    break;
                }
            }

            if (isClan && type == StructureType.EndoSteel)
                type = StructureType.ClanEndoSteel;

            return StructureTypes[type];
        }

        /// <summary>
        /// Retrieves the armor info of a mech.
        /// </summary>
        public static ArmorInfo GetArmorInfo(this MechDef mech)
        {
            if (mech.MechTags != null)
            {
                var armorType = mech.MechTags.GetArmorType();
                if (armorType != null) return ArmorTypes[(ArmorType)armorType];
            }

            var type = ArmorType.Standard;
            bool isClan = mech.Chassis.ChassisTags.Contains("chassis_clan");
            foreach (string tag in mech.Chassis.ChassisTags)
            {
                var match = ArmorTypes.FirstOrDefault(at => !string.IsNullOrEmpty(at.Value.Tag) && at.Value.Tag == tag);
                if (match.Value.Tag != null)
                {
                    type = match.Key;
                    break;
                }
            }

            if (isClan && type == ArmorType.FerroFibrous)
                type = ArmorType.ClanFerroFibrous;

            return ArmorTypes[type];
        }

        /// <summary>
        /// Retrieves the armor info of a chassis.
        /// </summary>
        public static ArmorInfo GetArmorInfo(this ChassisDef chassis)
        {
            var type = ArmorType.Standard;
            bool isClan = chassis.ChassisTags.Contains("chassis_clan");
            foreach (string tag in chassis.ChassisTags)
            {
                var match = ArmorTypes.FirstOrDefault(at => !string.IsNullOrEmpty(at.Value.Tag) && at.Value.Tag == tag);
                if (match.Value.Tag != null)
                {
                    type = match.Key;
                    break;
                }
            }

            if (isClan && type == ArmorType.FerroFibrous)
                type = ArmorType.ClanFerroFibrous;

            return ArmorTypes[type];
        }

        /// <summary>
        /// Retrieves the armor type from a tag set.
        /// </summary>
        public static ArmorType? GetArmorType(this TagSet tags)
        {
            const string ArmorPrefix = "AML_Armor_";

            string tag = tags.FirstOrDefault(t => t.StartsWith(ArmorPrefix));
            return tag != null && Enum.TryParse<ArmorType>(tag.Substring(ArmorPrefix.Length), out var type) ? type : null;
        }

        #endregion

        #region Artillery

        extension(TargetMovementData target)
        {
            /// <summary>
            /// Determines if a target is a threat that should be suppressed.
            /// Filters targets that are too close, not moving, or not moving toward allies.
            /// </summary>
            public bool IsBlockableThreat()
            {
                if (target.MoveVector.magnitude < 20f)
                    return false;

                if (target.Target.IsVTOLOrHoverTank())
                    return false;

                float distCurrentToAlly = Vector3.Distance(target.CurrentPos, target.ClosestAllyPos);
                float distPredictedToAlly = Vector3.Distance(target.PredictedPos, target.ClosestAllyPos);
                return distPredictedToAlly < distCurrentToAlly && distPredictedToAlly >= 60f;
            }
        }

        extension(AbstractActor unit)
        {
            /// <summary>
            /// Determines if a unit is essentially stationary.
            /// </summary>
            public bool IsStationary()
            {
                if (unit.IsShutDown || unit.IsProne)
                    return true;

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
            /// Determines if a unit is a VTOL or hover tank.
            /// </summary>
            public bool IsVTOLOrHoverTank()
            {
                return unit is Mech mech && (mech.MechDef.MechTags.Contains("unit_vtol") || mech.MechDef.MechTags.Contains("unit_hover")) ||
                       unit is Vehicle vehicle && (vehicle.VehicleDef.VehicleTags.Contains("unit_vtol") || vehicle.VehicleDef.VehicleTags.Contains("unit_hover"));
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

                return UnityEngine.Random.Range(0f, 100f) < predictedMissileDamage;
            }
        }

        /// <summary>
        /// Determines if a mech has an Artemis IV or V system installed.
        /// </summary>
        public static bool HasArtemis(this Mech mech) => mech?.allComponents?.Any(comp => comp.defId.StartsWith("Gear_Addon_Artemis")) ?? false;

        #endregion

        #region Lance Composition

        public static readonly string[] difficultyTags = ["pilot_npc_d1", "pilot_npc_d2", "pilot_npc_d3", "pilot_npc_d4", "pilot_npc_d5", "pilot_npc_d6", "pilot_npc_d7", "pilot_npc_d8", "pilot_npc_d9", "pilot_npc_d10"];

        public static readonly string[] weightClassTags = ["unit_light", "unit_medium", "unit_heavy", "unit_assault"];

        extension(TagSet tagSet)
        {
            /// <summary>
            /// Forces a pilot tag set to an elite difficulty level range.
            /// </summary>
            public TagSet ForceEliteDifficulty(int difficulty, string factionId)
            {
                bool isEliteDivision = factionId is "ComStarA" or "WordOfBlakeA";
                difficulty = isEliteDivision ? 10 : Mathf.Clamp(difficulty, 6, 10);

                var currentDiffTag = tagSet.FirstOrDefault(tag => tag.StartsWith("pilot_npc_d"));
                if (currentDiffTag == null || difficultyTags.IndexOf(currentDiffTag) < 5)
                {
                    if (currentDiffTag != null)
                        tagSet.Remove(currentDiffTag);

                    tagSet.Add($"pilot_npc_d{difficulty}");
                }

                return tagSet;
            }

            /// <summary>
            /// Clamps a tag set to a specific weight class range.
            /// </summary>
            public TagSet ClampToWeightClass(string weightClass1, string weightClass2, float chance)
            {
                if (!tagSet.Contains(weightClass1) && !tagSet.Contains(weightClass2))
                {
                    if (Random.Range(0, 100) < chance)
                    {
                        tagSet.RemoveRange(weightClassTags);
                        tagSet.Add(weightClass1);
                    }
                    else
                    {
                        tagSet.RemoveRange(weightClassTags);
                        tagSet.Add(weightClass2);
                    }
                }

                return tagSet;
            }

            /// <summary>
            /// Forces a tag set to a specific weight class.
            /// </summary>
            public TagSet ForceWeightClass(string weightClass)
            {
                tagSet.RemoveRange(weightClassTags);
                tagSet.Add(weightClass);
                return tagSet;
            }

            /// <summary>
            /// Forces a tag set to a specific unit type.
            /// </summary>
            public TagSet ForceUnitType(UnitType unitType)
            {
                switch (unitType)
                {
                    case UnitType.Mech:
                        tagSet.Remove("unit_vehicle");
                        tagSet.Add("unit_mech");
                        break;
                    case UnitType.Vehicle:
                        tagSet.Remove("unit_mech");
                        tagSet.Add("unit_vehicle");
                        break;
                }
                return tagSet;
            }
        }

        #endregion

        #region Faction Info

        /// <summary>
        /// Checks if a faction is a periphery faction.
        /// </summary>
        public static bool IsPeriphery(this FactionValue faction) => PeripheryFactions.Contains(faction.Name);

        public static readonly HashSet<string> PeripheryFactions = [
            "AuriganDirectorate", "AuriganRestoration", "Calderon", "Circinus", "Elysia",
            "Illyrian", "Lothian", "MagistracyOfCanopus", "Marian", "NewColonyRegion",
            "Oberon", "Outworld", "Rim", "TaurianConcordat", "Tortuga", "Valkyrate"
        ];

        #endregion
    }
}