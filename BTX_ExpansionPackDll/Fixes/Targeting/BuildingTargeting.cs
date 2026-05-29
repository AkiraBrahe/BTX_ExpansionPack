using BattleTech;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Fixes.Targeting
{
    internal class BuildingTargeting
    {
        /// <summary>
        /// Allows buildings to be targeted when a mech is standing on them.
        /// </summary>
        [HarmonyPatch(typeof(AIThreatUtil), "SortHostileUnitsByThreat")]
        public static class AIThreatUtil_SortHostileUnitsByThreat
        {
            [HarmonyPostfix]
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