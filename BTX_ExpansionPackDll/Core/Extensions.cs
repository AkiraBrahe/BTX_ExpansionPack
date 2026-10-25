using BattleTech;
using BattleTech.Data;
using CustAmmoCategories;
using CustomComponents;
using CustomUnits;
using HBS.Collections;
using Quirks.Quirks.MechEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BTX_ExpansionPack.Core
{
    public static class Extensions
    {
        extension(TagSet tags)
        {
            #region Armor Tags

            /// <summary>
            /// Retrieves the armor type from a tag set.
            /// </summary>
            public ArmorType? GetArmorType()
            {
                string tag = tags.FirstOrDefault(t => t.StartsWith(ArmorPrefix));
                return tag != null && Enum.TryParse<ArmorType>(tag.Substring(ArmorPrefix.Length), out var type) ? type : null;
            }

            #endregion

            #region Difficulty and Weight Class

            /// <summary>
            /// Aplies a specific difficulty tag to a pilot tag set.
            /// </summary>
            public TagSet ApplyDifficulty(string difficultyTag)
            {
                tags.RemoveRange(DifficultyTags);
                tags.Add(difficultyTag);
                return tags;
            }

            /// <summary>
            /// Clamps a pilot tag set to an elite difficulty level range.
            /// </summary>
            public TagSet ClampToEliteDifficultyRange(int difficulty, string factionId)
            {
                bool isEliteDivision = factionId is "ComStarA" or "WordOfBlakeA";
                difficulty = isEliteDivision ? 10 : Mathf.Clamp(difficulty, 6, 10);

                string currentDiffTag = tags.FirstOrDefault(tag => tag.StartsWith("pilot_npc_d"));
                if (currentDiffTag == null || DifficultyTags.IndexOf(currentDiffTag) < 5)
                {
                    if (currentDiffTag != null)
                        tags.Remove(currentDiffTag);

                    tags.Add($"pilot_npc_d{difficulty}");
                }

                return tags;
            }

            /// <summary>
            /// Promotes a pilot to a higher difficulty level.
            /// </summary>
            public TagSet PromotePilot(int step = 1)
            {
                if (step <= 0) return tags;

                for (int i = 0; i < DifficultyTags.Length - step; i++)
                {
                    if (tags.Contains(WeightClassTags[i]))
                    {
                        return tags.ApplyWeightClass(WeightClassTags[i + step]);
                    }
                }

                return tags;
            }

            /// <summary>
            /// Demotes a pilot to a lower difficulty level.
            /// </summary>
            public TagSet DemotePilot(int step = -1)
            {
                if (step >= 0) return tags;

                for (int i = DifficultyTags.Length - 1; i > 0; i--)
                {
                    if (tags.Contains(DifficultyTags[i]))
                    {
                        return tags.ApplyDifficulty(DifficultyTags[i + step]);
                    }
                }
                return tags;
            }

            /// <summary>
            /// Aplies a specific weight class to a unit tag set.
            /// </summary>
            public TagSet ApplyWeightClass(string weightClass)
            {
                tags.RemoveRange(WeightClassTags);
                tags.Add(weightClass);
                return tags;
            }

            /// <summary>
            /// Clamps a unit tag set to a specific weight class range.
            /// </summary>
            public TagSet ClampToWeightClass(string weightClass1, string weightClass2, float chance)
            {
                if (!tags.Contains(weightClass1) && !tags.Contains(weightClass2))
                {
                    if (Random.Range(0, 100) < chance)
                    {
                        tags.RemoveRange(WeightClassTags);
                        tags.Add(weightClass1);
                    }
                    else
                    {
                        tags.RemoveRange(WeightClassTags);
                        tags.Add(weightClass2);
                    }
                }

                return tags;
            }

            /// <summary>
            /// Upgrades a unit to the next higher weight class.
            /// </summary>
            public TagSet UpgradeToNextWeightClass()
            {
                for (int i = 0; i < WeightClassTags.Length - 1; i++)
                {
                    if (tags.Contains(WeightClassTags[i]))
                    {
                        return tags.ApplyWeightClass(WeightClassTags[i + 1]);
                    }
                }

                return tags;
            }

            /// <summary>
            /// Downgrades a unit to the previous lower weight class.
            /// </summary>
            public TagSet DowngradeToPreviousWeightClass()
            {
                for (int i = WeightClassTags.Length - 1; i > 0; i--)
                {
                    if (tags.Contains(WeightClassTags[i]))
                    {
                        return tags.ApplyWeightClass(WeightClassTags[i - 1]);
                    }
                }

                return tags;
            }

            #endregion
        }

        extension(MechDef mech)
        {
            #region Structure and Armor

            /// <summary>
            /// Retrieves the structure info of a mech.
            /// </summary>
            public StructureInfo GetStructureInfo()
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
            public ArmorInfo GetArmorInfo(bool checkMechTags = true)
            {
                if (checkMechTags && mech.MechTags != null)
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

            #endregion

            #region Quirks

            /// <summary>
            /// Retrieves the default quirks of a mech from BEX, or creates them if they don't exist.
            /// </summary>
            public QuirkList GetOrSetDefaultQuirks()
            {
                string chassisId = mech.ChassisID;

                if (!MechQuirkInfo.MechQuirkStore.TryGetValue(chassisId, out var quirks))
                {
                    quirks = new QuirkList();
                    MechQuirkInfo.MechQuirkStore[chassisId] = quirks;
                }

                return quirks;
            }

            /// <summary>
            /// Retrieves the custom quirks of a mech, or creates them if they don't exist.
            /// </summary>
            public CustomQuirkList GetOrSetCustomQuirks()
            {
                string chassisId = mech.ChassisID;

                if (!CustomQuirkStore.TryGetValue(chassisId, out var customQuirks))
                {
                    customQuirks = new CustomQuirkList();
                    CustomQuirkStore[chassisId] = customQuirks;
                }

                return customQuirks;
            }

            #endregion

            #region Inventory Slots

            /// <summary>
            /// Gets the number of free inventory slots in a mech.
            /// </summary>
            public int GetFreeSlots(IEnumerable<MechComponentRef> inventory) =>
                mech.Chassis.Locations.Sum(location => mech.GetFreeSlotsInLoc(inventory, location.Location));

            /// <summary>
            /// Gets the number of free inventory slots in a mech, excluding a specific category.
            /// </summary>
            public int GetFreeSlots(IEnumerable<MechComponentRef> inventory, string excludedCategory) =>
                mech.Chassis.Locations.Sum(location => mech.GetFreeSlotsInLoc(inventory, location.Location, excludedCategory));

            /// <summary>
            /// Determines the number of free inventory slots in a location.
            /// </summary>
            public int GetFreeSlotsInLoc(IEnumerable<MechComponentRef> inventory, ChassisLocations location) =>
                mech.GetChassisLocationDef(location).InventorySlots - inventory.Where(i => i.Def != null && i.MountedLocation == location).Sum(i => i.Def.InventorySize);

            /// <summary>
            /// Determines the number of free inventory slots in a location, taking into account the size of the item being added.
            /// </summary>
            public int GetFreeSlotsInLoc(IEnumerable<MechComponentRef> inventory, ChassisLocations location, int size) =>
                (mech.GetChassisLocationDef(location).InventorySlots - inventory.Where(i => i.Def != null && i.MountedLocation == location).Sum(i => i.Def.InventorySize)) / size;

            /// <summary>
            /// Determines the number of free inventory slots in a location, excluding a specific category.
            /// </summary>
            public int GetFreeSlotsInLoc(IEnumerable<MechComponentRef> inventory, ChassisLocations location, string excludedCategory) =>
                mech.GetChassisLocationDef(location).InventorySlots - inventory.Where(i => i.Def != null && i.MountedLocation == location && !i.IsCategory(excludedCategory)).Sum(i => i.Def.InventorySize);

            #endregion
        }

        extension(ChassisDef chassis)
        {
            #region Armor

            /// <summary>
            /// Retrieves the armor info of a chassis.
            /// </summary>
            public ArmorInfo GetArmorInfo()
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

            #endregion
        }

        extension(AbstractActor unit)
        {
            #region Movement and Pathfinding

            /// <summary>
            /// Checks if a unit is immobile based on the irbtmu_immobile_unit stat.
            /// </summary>
            public bool IsImmobile() =>
                unit.StatCollection.ContainsStatistic("irbtmu_immobile_unit") &&
                unit.StatCollection.GetValue<bool>("irbtmu_immobile_unit");

            /// <summary>
            /// Checks if a unit is stuck (cannot move via walking or jumping).
            /// </summary>
            public bool IsStuck() => !unit.HasNonStationaryWalkingDestination() && !unit.CanJump();

            /// <summary>
            /// Checks if a unit has a non-stationary walking destination.
            /// </summary>
            public bool HasNonStationaryWalkingDestination()
            {
                if (unit.Pathing == null)
                    return false;

                var pathNodes = unit.Pathing.getGrid(MoveType.Walking)?.GetSampledPathNodes();
                return pathNodes != null && pathNodes.Count > 1;
            }

            /// <summary>
            /// Checks if a unit can jump.
            /// </summary>
            public bool CanJump() => unit is Mech mech && mech.JumpPathing != null && mech.JumpDistance > 0f;

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

            #endregion

            #region Support Roles

            /// <summary>
            /// Determines if a unit is an artillery unit capable of firing artillery weapons.
            /// </summary>
            public bool IsArtilleryUnit()
            {
                if (unit.Weapons == null) return false;
                foreach (var w in unit.Weapons)
                {
                    if (w.IsArtillery() || (w.Description?.Id != null && w.Description.Id.StartsWith("Weapon_Artillery", StringComparison.OrdinalIgnoreCase)))
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

            #endregion

            #region Missile Threat Detection

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
                var detectedEnemies = unit.lance.team.GetDetectedEnemyUnits().Where(enemy => enemy.CanMove);

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

                return Random.Range(0, 100) < predictedMissileDamage;
            }

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

            #endregion

            #region Unit Classification

            /// <summary>
            /// Determines if a unit is a VTOL or hover tank from its tags.
            /// </summary>
            public bool IsVTOLOrHoverTank() =>
                (unit is Mech mech && (mech.MechDef.MechTags.Contains("unit_vtol") || mech.MechDef.MechTags.Contains("unit_hover")))
                || (unit is Vehicle vehicle && (vehicle.VehicleDef.VehicleTags.Contains("unit_vtol") || vehicle.VehicleDef.VehicleTags.Contains("unit_hover")));

            #endregion
        }

        extension(TargetMovementData target)
        {
            #region Threat Evaluation

            /// <summary>
            /// Determines if a target is a threat that should be suppressed.
            /// Filters targets that are too fast, too close, not moving, or not moving toward allies.
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

            #endregion
        }

        extension(Weapon weapon)
        {
            #region Artillery

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

            #endregion

            #region Homing Arrow IV

            /// <summary>
            /// Determines if a weapon is an Arrow IV with homing ammo.
            /// </summary>
            public bool IsHomingArrowIV() => weapon != null
                && weapon.mode()?.Id == "ARTY_Guided"
                && weapon.ammo()?.Id == "Ammunition_ArrowIV_Homing";

            #endregion
        }

        extension(ICombatant target)
        {
            #region TAG

            /// <summary>
            /// Determines if a target is TAGed.
            /// </summary>
            public bool IsTAGed() => target?.StatCollection != null
                && target.StatCollection.GetValue<float>("TAGCount") + target.StatCollection.GetValue<float>("TAGCountClan") > 0f;

            #endregion
        }

        extension(FactionValue faction)
        {
            #region Team Equipment

            /// <summary>
            /// Determines if any unit of the given team has a TAG weapon.
            /// </summary>
            public bool AnyUnitHasTAG()
            {
                var combat = UnityGameInstance.BattleTechGame.Combat;
                if (combat == null) return false;

                foreach (var combatTeam in combat.Teams)
                {
                    if (combatTeam.FactionValue != null && combatTeam.FactionValue.Name == faction.Name)
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

                return false;
            }

            #endregion

            #region Faction Classification

            /// <summary>
            /// Checks if a faction is a periphery faction.
            /// </summary>
            public bool IsPeriphery() => PeripheryFactions.Contains(faction.Name);

            #endregion
        }

        extension(MetadataDatabase mdd)
        {
            #region Dynamic Lance Difficulty

            /// <summary>
            /// Clear all entries from the Dynamic Lance Difficulty database.
            /// </summary>
            public void ClearDynamicLanceDifficulty() => mdd.Execute("DELETE FROM DynamicLanceDifficulty");

            /// <summary>
            /// Bulk insert multiple Dynamic Lance Difficulty entries into the database.
            /// </summary>
            public void BulkInsertDynamicLanceDifficulty(IEnumerable<DynamicLanceDifficulty_MDD> defs) =>
                mdd.Execute(@"INSERT INTO DynamicLanceDifficulty (DynamicLanceDifficultyID, Difficulty, NoUnitCount, LightUnitCount, MediumUnitCount, HeavyUnitCount, AssaultUnitCount, PilotTags)
                  VALUES (@DynamicLanceDifficultyID, @Difficulty, @NoUnitCount, @LightUnitCount, @MediumUnitCount, @HeavyUnitCount, @AssaultUnitCount, @PilotTags)", defs);

            #endregion
        }

        #region Other Extensions

        /// <summary>
        /// Determines if a unit is a VTOL from its tags.
        /// </summary>
        public static bool IsVTOL(this VehicleDef vehicle) => vehicle.VehicleTags.Contains("unit_vtol");

        /// <summary>
        /// Determines if a mech has an Artemis IV or V system installed.
        /// </summary>
        public static bool HasArtemis(this Mech mech) => mech?.allComponents?.Any(comp => comp.defId.StartsWith("Gear_Addon_Artemis")) ?? false;

        #endregion
    }
}