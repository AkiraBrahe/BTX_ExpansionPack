using BattleTech;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    /// <summary>
    /// Allows buildings to be targeted when a mech is standing on them.
    /// </summary>
    internal class BuildingTargeting
    {
        // [HarmonyPatch(typeof(AttackEvaluator), "MakeAttackOrder")]
        // public static class AttackEvaluator_MakeAttackOrder
        // {
        //     [HarmonyPostfix]
        //     public static void Postfix(BehaviorTreeResults __result)
        //     {
        //         if (__result.nodeState == BehaviorNodeState.Failure ||
        //             __result.orderInfo is not AttackOrderInfo attackOrder)
        //         {
        //             return;
        //         }
        // 
        //         if (attackOrder.TargetUnit is Mech mech && !mech.IsDead && !string.IsNullOrEmpty(mech.standingOnBuildingGuid))
        //         {
        //             var building = mech.Combat.FindCombatantByGUID(mech.standingOnBuildingGuid, true);
        //             if (building != null)
        //             {
        //                 attackOrder.TargetUnit = building;
        //             }
        //         }
        //     }
        // }

        [HarmonyPatch(typeof(AIThreatUtil), "SortHostileUnitsByThreat")]
        public static class AIThreatUtil_SortHostileUnitsByThreat
        {
            [HarmonyPostfix]
            [HarmonyWrapSafe]
            public static void Postfix(List<ICombatant> units)
            {
                var mechBuildingPairs = units
                    .OfType<Mech>()
                    .Where(m => !m.IsDead && !string.IsNullOrEmpty(m.standingOnBuildingGuid))
                    .Select(m => new
                    {
                        Mech = m,
                        Building = m.Combat.FindCombatantByGUID(m.standingOnBuildingGuid, true)
                    })
                    .Where(pair => pair.Building != null)
                    .GroupBy(pair => pair.Building)
                    .Select(g => new { g.First().Mech, Building = g.Key })
                    .ToList();

                foreach (var pair in mechBuildingPairs)
                {
                    int mechIndex = units.IndexOf(pair.Mech);
                    if (mechIndex != -1)
                    {
                        units.Insert(mechIndex + 1, pair.Building);
                    }
                }
            }
        }
    }
}