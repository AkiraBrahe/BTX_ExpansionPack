using BattleTech;
using CustAmmoCategories;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BTX_ExpansionPack.Core.Helpers
{
    internal class ArtilleryHelpers
    {
        /// <summary>
        /// Tracks whether a unit has Homing Arrow IV ammo when spawning.
        /// </summary>
        [HarmonyPatch(typeof(UnitSpawnPointGameLogic), "initializeActor")]
        public static class UnitSpawnPointGameLogic_initializeActor
        {
            [HarmonyPrefix]
            public static void Prefix(AbstractActor unit)
            {
                if (unit == null || unit.ammoBoxes == null || unit.StatCollection == null) return;

                bool foundHomingAmmo = false;
                foreach (var ammoBox in unit.ammoBoxes)
                {
                    if (ammoBox.defId == "Ammunition_ArrowIV_Homing")
                    {
                        foundHomingAmmo = true;
                        break;
                    }
                }

                unit.StatCollection.GetOrCreateStatisic("HasHomingArrowIV", foundHomingAmmo);
            }
        }

        /// <summary>
        /// Finds allies within range of the artillery's area of effect.
        /// </summary>
        public static List<AbstractActor> FindAlliesWithinRange(Vector3 targetPosition, Team targetTeam, IEnumerable<AbstractActor> allActors, float aoeRange) =>
            [.. allActors.Where(actor => !actor.IsDead && actor.team.IsFriendly(targetTeam) && Vector3.Distance(targetPosition, actor.CurrentPosition) <= aoeRange * 2f)];

        /// <summary>
        /// Finds enemy units close to the primary target, within the artillery's area of effect.
        /// </summary>
        public static List<AbstractActor> FindNearbyEnemies(AbstractActor primaryTarget, IEnumerable<AbstractActor> allEnemies, float aoeRange) =>
            [.. allEnemies.Where(enemy => Vector3.Distance(primaryTarget.CurrentPosition, enemy.CurrentPosition) <= aoeRange * 2f)];
    }
}
