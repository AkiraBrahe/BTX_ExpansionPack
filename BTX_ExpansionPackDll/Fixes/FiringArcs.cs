﻿using BattleTech;
using CustomUnits;
using UnityEngine;

namespace BTX_ExpansionPack.Fixes
{
    /// <summary>
    /// Overrides firing arc logic to correctly handle both vehicles and mechs with quirks.
    /// </summary>
    internal class FiringArcs
    {
        [HarmonyPatch(typeof(Mech), "IsTargetPositionInFiringArc")]
        public static class Mech_IsTargetPositionInFiringArc
        {
            [HarmonyPrefix]
            [HarmonyPriority(Priority.First)]
            public static bool Prefix(ref bool __runOriginal, Mech __instance, ICombatant targetUnit, Vector3 attackPosition, Quaternion attackRotation, Vector3 targetPosition, ref bool __result)
            {
                // Special case: Standing on building
                if (targetUnit.Type == TaggedObjectType.Building &&
                    targetUnit is BattleTech.Building building &&
                    !string.IsNullOrEmpty(__instance.standingOnBuildingGuid))
                {
                    string text = __instance.standingOnBuildingGuid + ".Building";
                    if (__instance.standingOnBuildingGuid == building.GUID || text == building.GUID)
                    {
                        __result = true;
                        __runOriginal = false;
                        return false;
                    }
                }

                // Special case: Directional torso mount
                if (__instance.MechDef?.Chassis?.ChassisTags?.Contains("mech_quirk_directionaltorsomount") == true)
                {
                    __result = true;
                    __runOriginal = false;
                    return false;
                }

                float firingArc = GetFiringArc(__instance, targetUnit, attackPosition);
                float currentAngle = Mathf.Abs(Mathf.DeltaAngle(
                    PathingUtil.GetAngle(attackRotation * Vector3.forward),
                    PathingUtil.GetAngle(targetPosition - attackPosition)));
                __result = currentAngle < firingArc;
                __runOriginal = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(Vehicle), "IsTargetPositionInFiringArc")]
        public static class Vehicle_IsTargetPositionInFiringArc
        {
            [HarmonyPrefix]
            [HarmonyPriority(Priority.First)]
            public static bool Prefix(ref bool __runOriginal, Vehicle __instance, ICombatant targetUnit, Vector3 attackPosition, Quaternion attackRotation, Vector3 targetPosition, ref bool __result)
            {
                float firingArc = GetFiringArc(__instance, targetUnit, attackPosition);
                float currentAngle = Mathf.Abs(Mathf.DeltaAngle(
                    PathingUtil.GetAngle(attackRotation * Vector3.forward),
                    PathingUtil.GetAngle(targetPosition - attackPosition)));
                __result = currentAngle < firingArc;
                __runOriginal = false;
                return false;
            }
        }

        private static float GetFiringArc(AbstractActor actor, ICombatant targetUnit, Vector3 attackPosition)
        {
            if (actor is Vehicle vehicle)
            {
                return vehicle.VehicleDef.Chassis.HasTurret ? 360f : 90f;
            }

            if (actor is Mech mech)
            {
                if (mech is FakeVehicleMech fakeVehicle)
                {
                    var vehicleDef = fakeVehicle.MechDef?.toVehicleDef(fakeVehicle.MechDef.DataManager);
                    return vehicleDef?.Chassis != null && vehicleDef.Chassis.HasTurret ? 360f : 90f;
                }

                return GetMechFiringArc(mech, targetUnit, attackPosition);
            }

            return actor.Combat.Constants.ToHit.FiringArcDegrees;
        }

        private static float GetMechFiringArc(Mech mech, ICombatant targetUnit, Vector3 attackPosition)
        {
            float firingArc = mech.Combat.Constants.ToHit.FiringArcDegrees;

            if (mech is (Mech or QuadMech) and not TrooperSquad)
            {
                float distance = Vector3.Distance(attackPosition, targetUnit.CurrentPosition);
                if (distance < Core.Settings.CloseRangeFiringArcDistance)
                    return Core.Settings.CloseRangeFiringArc;

                var tags = mech.MechDef?.Chassis?.ChassisTags;
                if (tags?.Contains("mech_quirk_notorsotwist") == true)
                    return firingArc / 2f;
                if (tags?.Contains("mech_quirk_extendedtorsotwist") == true)
                    return firingArc * 2f;
            }

            return firingArc;
        }
    }
}