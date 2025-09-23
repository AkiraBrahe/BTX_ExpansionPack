using BattleTech;
using CustAmmoCategories;

namespace BTX_ExpansionPack.Fixes
{
    /// <summary>
    /// Reduces move distance if the mech with Poor Performance quirk has not moved last turn.
    /// </summary>
    internal class PoorPerformance
    {
        [HarmonyPatch(typeof(Mech), "MaxSprintDistance", MethodType.Getter)]
        [HarmonyPriority(Priority.Last)]
        [HarmonyWrapSafe]
        public static class Mech_MaxSprintDistance
        {
            [HarmonyPostfix]
            public static void Postfix(Mech __instance, ref float __result)
            {
                if (__instance.MechDef.Chassis.ChassisTags.Contains("quirk_poor_performance") && __instance.LastMoveDistance() < 1f)
                {
                    float walkDistance = __instance.MaxWalkDistance;
                    if (__result > walkDistance)
                        __result = walkDistance;
                }
            }
        }
    }
}
