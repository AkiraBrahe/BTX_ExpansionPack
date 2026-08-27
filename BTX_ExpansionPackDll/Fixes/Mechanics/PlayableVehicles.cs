using BattleTech;
using BattleTech.Save.SaveGameStructure;
using CustAmmoCategories;
using Extended_CE.Functionality;
using System;

namespace BTX_ExpansionPack.Fixes.Mechanics
{
    internal class PlayableVehicles
    {
        /// <summary>
        /// Fixes turreted vehicles having incorrect max turret armor.
        /// </summary>
        [HarmonyPatch(typeof(Extended_CE.NewTech.ArmorRules), "MaxFrontArmor")]
        public static class ArmorRules_MaxFrontArmor
        {
            [HarmonyPrefix]
            public static bool Prefix(LocationDef locationDef, ref float __result)
            {
                if (locationDef.Location != ChassisLocations.Head) return true;
                __result = locationDef.MaxArmor;
                return false;
            }
        }

        /// <summary>
        /// Fixes pathfinding for VTOLs and hover tanks to use the correct terrain cost modifiers.
        /// </summary>
        /// <remarks>
        /// BEX prevents mechs from running over water, but this also prevented VTOLs and hover tanks from doing so.
        /// </remarks>
        [HarmonyPatch(typeof(PathNodeGrid), "GetTerrainModifiedCost", [typeof(PathNode), typeof(PathNode), typeof(float)])]
        public static class PathNodeGrid_GetTerrainModifiedCost
        {
            public static void Postfix(PathNodeGrid __instance, PathNode from, PathNode to, float distanceAvailable, ref float __result)
            {
                if (TacticalGameChanges.tutorialMission)
                    return;

                var owningActor = __instance.owningActor;
                if (owningActor == null || owningActor is Vehicle || owningActor.FakeVehicle())
                    return;

                // Original BEX logic
                var mapMetaData = __instance.mapMetaData;
                if (Extended_CE.Core.Settings.UsingRunMovementRules && __instance.moveType == MoveType.Walking && (double)__result <= (double)distanceAvailable && (double)from.CostToThisNode >= 10.0)
                {
                    if ((double)from.CostToThisNode + (double)__result > (double)__instance.MaxDistance * 0.66666668653488159)
                    {
                        DesignMaskDef priorityDesignMask = __instance.owningActor.Combat.MapMetaData.GetPriorityDesignMask(to.MapTerrainDataCell);
                        if (priorityDesignMask != null && priorityDesignMask.moveCostSprintMultiplier > 1.3200000524520874)
                            __result = 99999.9f;
                    }
                }
                else if (__instance.moveType == MoveType.Sprinting && (double)__result > 99000.0 && (double)from.CostToThisNode < 10.0 && mapMetaData != null)
                {
                    Point startPoint = mapMetaData.GetIndex(from.Position);
                    Point endPoint = mapMetaData.GetIndex(to.Position);
                    if (mapMetaData.IsWithinBounds(startPoint) && mapMetaData.IsWithinBounds(endPoint))
                        __result = (float)((double)__instance.MaxDistance - (double)from.CostToThisNode - -3.4028234663852886E+38);
                }
            }
        }
    }
}