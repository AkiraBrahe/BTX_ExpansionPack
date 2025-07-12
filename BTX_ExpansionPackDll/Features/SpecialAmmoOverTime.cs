using BattleTech;
using BEXTimeline;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace BTX_ExpansionPack
{
    internal class SpawningActorInfo
    {
        public static bool IsClan { get; set; } = false;
        public static string FactionShortName { get; set; } = "";

        [HarmonyPatch(typeof(UnitSpawnPointGameLogic), "initializeActor")]
        public static class UnitSpawnPointGameLogic_initializeActor
        {
            [HarmonyPrefix]
            public static void Prefix(Team team)
            {
                if (team != null && team.FactionValue != null && team.FactionValue.FactionDef != null && !team.IsLocalPlayer)
                {
                    IsClan = team.FactionValue.IsClan;
                    FactionShortName = team.FactionValue.FactionDef.ShortName;
                }
                else
                {
                    IsClan = false;
                    FactionShortName = "";
                }
            }
        }
    }

    internal class SpecialAmmoOverTime
    {
        private static bool IsClan => SpawningActorInfo.IsClan;
        private static string FactionShortName => SpawningActorInfo.FactionShortName;

        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats
        {
            [HarmonyPostfix]
            public static void Postfix(Mech __instance)
            {
                SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
                bool isInvalidState = simulation == null || __instance.Combat == null || __instance.Combat.ActiveContract == null;
                bool IsPlayerTeam = MechComponentsOverTime.spawningActorIsPlayerTeam;
                bool isInvalidTarget = IsPlayerTeam || __instance.StatCollection.GetValue<bool>("IsTargetingDummy") || __instance.StatCollection.GetValue<bool>("GuaranteeUnhittable");

                if (isInvalidState || IsPlayerTeam || isInvalidTarget) return;

                switch (FactionShortName)
                {
                    case "Kurita":
                        if (simulation.CurrentDate < new DateTime(3058, 1, 1))
                        {
                            ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_SRM", "Ammo_AmmunitionBox_Generic_SRM_DF", 0.15f, new DateTime(3052, 1, 1), new DateTime(3058, 1, 1));
                        }
                        else
                        {
                            ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_SRM", "Ammo_AmmunitionBox_Generic_SRM_DF", 0.05f);
                        }
                        break;

                    case "Liao":
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_SRM", "Ammo_AmmunitionBox_Generic_SRM_Inferno", 1.00f);
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Inferno", 0.10f, new DateTime(3056, 1, 1), new DateTime(3062, 1, 1));
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_ArrowIV", "Ammo_AmmunitionBox_Generic_ArrowIV_Inferno", 0.01f);
                        Main.Log.LogDebug("[SpecialAmmoOverTime] The Capellans got their infernos!");
                        break;

                    case "Marik":
                        if (simulation.CurrentDate < new DateTime(3057, 1, 1))
                        {
                            ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm", 0.10f, new DateTime(3053, 1, 1), new DateTime(3058, 1, 1));
                        }
                        else
                        {
                            ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm-I", 0.10f, new DateTime(3057, 1, 1), new DateTime(3066, 1, 1));
                        }
                        break;

                    case "ComStar":
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm", 0.15f, new DateTime(3053, 1, 1), new DateTime(3058, 1, 1));
                        break;

                    case "Word of Blake":
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm-I", 0.15f, new DateTime(3057, 1, 1), new DateTime(3066, 1, 1));
                        break;
                }

                if (IsClan || FactionShortName == "Black Widow Company" || FactionShortName == "Wolf's Dragoons")
                {
                    ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm", 0.10f);
                }

                if (!IsClan && Helpers.AnyAllyHasTAG(__instance))
                {
                    ReplaceAmmo(__instance, "Ammunition_ArrowIV", "Ammunition_ArrowIV_Homing", 0.25f);
                }
            }

            private static void ReplaceAmmo(
                Mech mech,
                string originalAmmoID,
                string replacementAmmoID,
                float chance,
                DateTime? productionDate = null,
                DateTime? commonDate = null,
                float minChance = 0.05f)
            {
                float actualChance = chance;
                SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;

                if (productionDate.HasValue && commonDate.HasValue && simulation != null)
                {
                    actualChance = InterpolateChance(simulation.CurrentDate, productionDate.Value, commonDate.Value, chance, minChance);
                }

                Main.Log.LogDebug($"[SpecialAmmoOverTime] Attempting to replace '{originalAmmoID}' with '{replacementAmmoID}' with {actualChance:P0} chance for {mech.DisplayName}.");

                List<AmmunitionBox> ammoBoxesToReplace = [];
                foreach (AmmunitionBox ammoBox in mech.ammoBoxes)
                {
                    if (ammoBox.ammoDef.Description.Id == originalAmmoID && Random.Range(0f, 1f) < actualChance)
                    {
                        ammoBoxesToReplace.Add(ammoBox);
                    }
                }
                foreach (AmmunitionBox ammoBoxToRemove in ammoBoxesToReplace)
                {
                    mech.allComponents.Remove(ammoBoxToRemove);
                    mech.ammoBoxes.Remove(ammoBoxToRemove);
                    MechComponentsOverTime.Mech_InitStats.AddOurAmmo(mech, replacementAmmoID, (ChassisLocations)ammoBoxToRemove.Location);
                }
            }

            private static float InterpolateChance(DateTime currentDate, DateTime productionDate, DateTime commonDate, float maxChance, float minChance = 0.05f)
            {
                if (currentDate < productionDate) return 0.00f;
                if (currentDate >= commonDate) return maxChance;

                double currentDays = (currentDate - productionDate).TotalDays;
                double totalDays = (commonDate - productionDate).TotalDays;

                float interpolationFactor = (float)(currentDays / totalDays);
                return minChance + (maxChance - minChance) * interpolationFactor;
            }
        }
    }
}