using BattleTech;
using CustAmmoCategories;
using Extended_CE.Functionality;
using FullXotlTables;
using System.Collections.Generic;
using System.Reflection.Emit;

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

        /// <summary>
        /// Skips the logic that doubles the rarity of certain vehicles.
        /// </summary>
        [HarmonyPatch(typeof(XotlTable), "RequestUnit")]
        public static class XotlTable_RequestUnit
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions, il)
                    .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "vehicledef_APC_Maxim_3052AP"))
                    .MatchBack(false, new CodeMatch(i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S));

                var jumpTarget = matcher.Operand;
                return matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Br, jumpTarget))
                    .InstructionEnumeration();
            }
        }
    }
}