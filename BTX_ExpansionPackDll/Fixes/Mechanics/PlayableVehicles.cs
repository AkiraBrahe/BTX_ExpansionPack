using BattleTech;
using CustAmmoCategories;
using Extended_CE.Functionality;

namespace BTX_ExpansionPack.Fixes.Mechanics
{
    internal class PlayableVehicles
    {
        /// <summary>
        /// Fixes turreted vehicles and VTOLs having incorrect max armor.
        /// </summary>
        [HarmonyPatch(typeof(Extended_CE.NewTech.ArmorRules), "MaxFrontArmor")]
        public static class BEX_ArmorRules_MaxFrontArmor
        {
            [HarmonyPrefix]
            public static bool Prefix(LocationDef locationDef, ref float __result)
            {
                if (locationDef.Location == ChassisLocations.Head ||
                    locationDef.MaxArmor <= locationDef.InternalStructure)
                {
                    __result = locationDef.MaxArmor;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Removes the self-arm-mounted weapon modifier for vehicles.
        /// </summary>
        [HarmonyPatch(typeof(ToHit), "GetSelfArmMountedModifier")]
        public static class ToHit_GetSelfArmMountedModifier
        {
            [HarmonyPrefix]
            public static bool Prefix(Weapon weapon, ref float __result)
            {
                if (weapon.parent.FakeVehicle())
                {
                    __result = 0f;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Fixes pathfinding for VTOLs and hover tanks to use the correct terrain cost modifiers.
        /// </summary>
        /// <remarks>
        /// BEX prevents mechs from running into water, but this also prevented VTOLs and hover tanks from doing so.
        /// </remarks>
        [HarmonyPatch(typeof(PathNodeGrid), "GetTerrainModifiedCost", [typeof(PathNode), typeof(PathNode), typeof(float)])]
        public static class PathNodeGrid_GetTerrainModifiedCost
        {
            [HarmonyPostfix]
            public static void Postfix(PathNodeGrid __instance, PathNode from, PathNode to, float distanceAvailable, ref float __result)
            {
                var owningActor = __instance.owningActor;
                if (owningActor == null || owningActor.FakeVehicle() || owningActor.UnaffectedDesignMasks())
                    return;

                RunRules.PathNodeGrid_GetTerrainModifiedCost.Postfix(__instance, from, to, distanceAvailable, owningActor, __instance.moveType, ref __result, __instance.mapMetaData);
            }
        }
    }
}