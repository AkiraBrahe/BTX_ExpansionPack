using BattleTech;
using System;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Features
{
    internal class UrbanDevastation
    {
        [HarmonyPatch(typeof(TurnDirector), "OnInitializeContractComplete")]
        public static class TurnDirector_OnInitializeContractComplete
        {
            private static readonly Random rng = new();

            [HarmonyPostfix]
            public static void Postfix(TurnDirector __instance)
            {
                CombatGameState combat = __instance.Combat;
                if (combat == null || combat.ActiveContract.ContractBiome != Biome.BIOMESKIN.urbanHighTech)
                    return;

                List<BattleTech.Building> candidateBuildings = [];
                foreach (ICombatant combatant in combat.GetAllCombatants())
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

                if (candidateBuildings.Count == 0)
                    return;

                Shuffle(candidateBuildings);

                float minDevastation = 0.60f; float maxDevastation = 0.90f;
                float destroyPercent = (float)((rng.NextDouble() * (maxDevastation - minDevastation)) + minDevastation);
                int destroyCount = (int)Math.Floor(candidateBuildings.Count * destroyPercent);
                destroyCount = Math.Min(destroyCount, candidateBuildings.Count);

                for (int i = 0; i < destroyCount; i++)
                {
                    var building = candidateBuildings[i];
                    building.FlagForDeath("MOD_PREMAP_DESTROY", DeathMethod.DespawnedNoMessage, DamageType.NOT_SET, 1, -1, "0", true);
                    building.HandleDeath("0");
                }
            }

            private static void Shuffle<T>(IList<T> list)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    (list[n], list[k]) = (list[k], list[n]);
                }
            }
        }
    }
}