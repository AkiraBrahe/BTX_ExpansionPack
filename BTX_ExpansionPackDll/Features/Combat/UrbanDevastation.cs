using BattleTech;
using BattleTech.Framework;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace BTX_ExpansionPack.Features.Combat
{
    internal class UrbanDevastation
    {
        /// <summary>
        /// Destroys a percentage of buildings in urban maps based on contract difficulty and mission type.
        /// </summary>
        [HarmonyPatch(typeof(TurnDirector), "OnInitializeContractComplete")]
        public static class TurnDirector_OnInitializeContractComplete
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.Performance.DestroyBuildingsInUrbanBattles;

            [HarmonyPostfix]
            public static void Postfix(TurnDirector __instance)
            {
                var combat = __instance.Combat;
                var activeContract = combat?.ActiveContract;

                if (activeContract == null || activeContract.ContractBiome != Biome.BIOMESKIN.urbanHighTech) return;

                float devastationModifier = activeContract.contractTypeID.ToString() switch
                {
                    "Warzone" => 1.5f,
                    "AttackAndDefend" => 1.2f,
                    "ThreeWayBattle" => 1.1f,
                    "SimpleBattle" => 1.0f,
                    "DefendBase" => 0.8f,
                    "DestroyBase" => 0.8f,
                    _ => 0f
                };

                if (devastationModifier <= 0f) return;

                List<BattleTech.Building> candidateBuildings = [];
                foreach (var combatant in combat.GetAllCombatants())
                {
                    if (combatant is BattleTech.Building building)
                    {
                        if (building.objectiveGUIDS != null && building.objectiveGUIDS.Contains(combat.GUID))
                            continue;

                        if (building.BuildingDef != null && building.BuildingDef.Destructible &&
                            building.UrbanDestructible != null && building.UrbanDestructible.CanBeDesolation &&
                            !building.IsTabTarget)
                        {
                            candidateBuildings.Add(building);
                        }
                    }
                }

                if (candidateBuildings.Count != 0)
                {
                    candidateBuildings.Shuffle();

                    int difficulty = activeContract.Difficulty;
                    float minDevastation = Math.Min(difficulty * 0.05f * devastationModifier, 0.90f);
                    float maxDevastation = Math.Min(0.90f * devastationModifier, 0.99f);

                    float destroyPercent = (float)((Random.value * (maxDevastation - minDevastation)) + minDevastation);
                    int destroyCount = (int)Math.Floor(candidateBuildings.Count * destroyPercent);
                    destroyCount = Math.Min(destroyCount, candidateBuildings.Count);

                    Main.Logger.LogDebug($"[UrbanDevastation] Destroying {destroyCount}/{candidateBuildings.Count} ({destroyPercent:P}) buildings for {activeContract.Name}.");
                    for (int i = 0; i < destroyCount; i++)
                    {
                        var building = candidateBuildings[i];
                        building.FlagForDeath("MOD_PREMAP_DESTROY", DeathMethod.DespawnedNoMessage, DamageType.NOT_SET, 1, -1, "0", true);
                        building.HandleDeath("0");
                    }
                }
            }
        }
    }
}