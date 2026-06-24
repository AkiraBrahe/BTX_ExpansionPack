using BattleTech;
using CustAmmoCategories;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BTX_ExpansionPack.Core.Helpers
{
    public static class ArtilleryHelpers
    {
        #region Coordinated Strikes

        /// <summary>
        /// Tracks active artillery strikes to enable coordinated fire.
        /// <list type="bullet">Keys: Round -> Team GUID -> Strike Data</list>
        /// </summary>
        public static Dictionary<int, Dictionary<string, List<ArtilleryStrikeData>>> _activeStrikes = [];

        /// <summary>
        /// Records a confirmed artillery strike for coordination.
        /// </summary>
        public static void RecordArtilleryStrike(ArtilleryStrikeData strike)
        {
            if (!_activeStrikes.ContainsKey(strike.Round))
                _activeStrikes[strike.Round] = [];

            string teamKey = strike.TeamGUID;
            if (!_activeStrikes[strike.Round].ContainsKey(teamKey))
                _activeStrikes[strike.Round][teamKey] = [];

            _activeStrikes[strike.Round][teamKey].Add(strike);
        }

        /// <summary>
        /// Gets all strikes recorded this round by this team.
        /// </summary>
        public static List<ArtilleryStrikeData> GetTeamStrikesThisRound(AbstractActor attacker)
        {
            var combat = UnityGameInstance.BattleTechGame.Combat;
            int currentRound = combat.TurnDirector.CurrentRound;
            string teamKey = attacker.team.GUID;

            if (_activeStrikes.TryGetValue(currentRound, out var roundStrikes))
            {
                if (roundStrikes.TryGetValue(teamKey, out var teamStrikes))
                    return teamStrikes;
            }
            return [];
        }

        /// <summary>
        /// Clears expired strike data at the start of each new round.
        /// </summary>
        [HarmonyPatch(typeof(TurnDirector), "BeginNewRound")]
        public static class TurnDirector_BeginNewRound_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(TurnDirector __instance)
            {
                int currentRound = __instance.CurrentRound;
                var expiredRounds = _activeStrikes.Keys.Where(r => r < currentRound).ToList();
                foreach (int round in expiredRounds)
                {
                    _activeStrikes.Remove(round);
                }
            }
        }

        #endregion

        #region Homing Arrow IV Tracking

        /// <summary>
        /// Tracks whether a unit has Homing Arrow IV ammo when spawning.
        /// </summary>
        [HarmonyPatch(typeof(UnitSpawnPointGameLogic), "initializeActor")]
        public static class UnitSpawnPointGameLogic_initializeActor
        {
            [HarmonyPrefix]
            public static void Prefix(AbstractActor actor)
            {
                if (actor == null || actor.ammoBoxes == null || actor.StatCollection == null) return;

                bool foundHomingAmmo = false;
                foreach (var ammoBox in actor.ammoBoxes)
                {
                    if (ammoBox.defId == "Ammunition_ArrowIV_Homing")
                    {
                        foundHomingAmmo = true;
                        break;
                    }
                }

                actor.StatCollection.GetOrCreateStatisic("HasHomingArrowIV", foundHomingAmmo);
            }
        }

        #endregion

        #region Area of Effect Targeting

        /// <summary>
        /// Finds allies within range of the artillery's area of effect.
        /// </summary>
        public static List<AbstractActor> FindAlliesWithinRange(Vector3 targetPosition, Team targetTeam, IEnumerable<AbstractActor> allActors, float aoeRange) =>
            [.. allActors.Where(actor => !actor.IsDead && actor.team.IsFriendly(targetTeam) && Vector3.Distance(targetPosition, actor.CurrentPosition) <= aoeRange * 2f)];

        /// <summary>
        /// Finds enemies within range of the artillery's area of effect.
        /// </summary>
        public static List<AbstractActor> FindEnemiesWithinRange(Vector3 targetPosition, Team targetTeam, IEnumerable<AbstractActor> allActors, float aoeRange) =>
            [.. allActors.Where(actor => !actor.IsDead && actor.team.IsEnemy(targetTeam) && Vector3.Distance(targetPosition, actor.CurrentPosition) <= aoeRange * 2f)];

        /// <summary>
        /// Finds enemy units close to the primary target, within the artillery's area of effect.
        /// </summary>
        public static List<AbstractActor> FindNearbyEnemies(AbstractActor primaryTarget, IEnumerable<AbstractActor> allEnemies, float aoeRange) =>
            [.. allEnemies.Where(enemy => Vector3.Distance(primaryTarget.CurrentPosition, enemy.CurrentPosition) <= aoeRange * 2f)];

        #endregion
    }
}