using BattleTech;
using CustomUnits;
using UnityEngine;

namespace BTX_ExpansionPack.Fixes
{
    internal class FiringArcs
    {
        [HarmonyPatch(typeof(Mech), "IsTargetPositionInFiringArc")]
        public static class FiringArcOverrideLogic
        {
            [HarmonyPrefix]
            [HarmonyWrapSafe]
            [HarmonyBefore("io.mission.customunits")]
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

            private static float GetFiringArc(Mech mech, ICombatant targetUnit, Vector3 attackPosition)
            {
                float firingArc = mech.Combat.Constants.ToHit.FiringArcDegrees; // Default

                if (mech is FakeVehicleMech)
                {
                    var vehicleDef = mech.MechDef?.toVehicleDef(mech.MechDef.DataManager);
                    if (vehicleDef?.Chassis != null)
                        return vehicleDef.Chassis.HasTurret ? 360f : 90f;
                    return 90f;
                }

                if (mech is (Mech or QuadMech) && mech is not TrooperSquad)
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
}