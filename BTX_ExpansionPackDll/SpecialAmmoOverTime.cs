using System;
using System.Collections.Generic;
using BattleTech;
using HarmonyLib;
using Random = UnityEngine.Random;

namespace BTX_ExpansionPack
{
    internal class SpawningActorInfo
    {
        public static bool IsPlayerTeam { get; set; } = false;
        public static bool IsClan { get; set; } = false;
        public static string FactionShortName { get; set; } = "";

        [HarmonyPatch(typeof(UnitSpawnPointGameLogic), "initializeActor")]
        public static class UnitSpawnPointGameLogic_initializeActor
        {
            public static void Prefix(Team team)
            {
                if (team != null && team.FactionValue != null && team.FactionValue.FactionDef != null && !team.IsLocalPlayer)
                {
                    IsPlayerTeam = team.FactionValue.IsPlayer1sMercUnit || team.FactionValue.IsPlayer2sMercUnit;
                    IsClan = team.FactionValue.IsClan;
                    FactionShortName = team.FactionValue.FactionDef.ShortName;
                }
                else
                {
                    IsPlayerTeam = true;
                    FactionShortName = "";
                }
            }
        }
    }

    internal class SpecialAmmoOverTime
    {
        private static string FactionShortName => SpawningActorInfo.FactionShortName;
        private static bool IsClan => SpawningActorInfo.IsClan;

        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats
        {
            [HarmonyPostfix]
            public static void Postfix(Mech __instance)
            {
                Main.Log.LogDebug("[SpecialAmmoOverTime] Mech.InitStats Postfix called for: " + __instance.DisplayName + " (ID: " + __instance.GUID + ")");
                Main.Log.LogDebug($"[SpecialAmmoOverTime] SpawningActorInfo.IsPlayerTeam: {SpawningActorInfo.IsPlayerTeam}, SpawningActorInfo.IsClan: {SpawningActorInfo.IsClan}, SpawningActorInfo.FactionShortName: '{SpawningActorInfo.FactionShortName}'");

                SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
                bool isInvalidTarget = SpawningActorInfo.IsPlayerTeam || __instance.StatCollection.GetValue<bool>("IsTargetingDummy") || __instance.StatCollection.GetValue<bool>("GuaranteeUnhittable");
                bool isInvalidState = simulation == null || __instance.Combat == null || __instance.Combat.ActiveContract == null;

                if (!isInvalidTarget && !isInvalidState)
                {
                    if (FactionShortName == "Kurita")
                    {
                        if (simulation.CurrentDate < new DateTime(3058, 1, 1))
                        {
                            ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_SRM", "Ammo_AmmunitionBox_Generic_SRM_DF", new DateTime(3052, 1, 1), new DateTime(3058, 1, 1), 0.15f);
                        }
                        else
                        {
                            ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_SRM", "Ammo_AmmunitionBox_Generic_SRM_DF", 0.05f);
                        }
                    }
                    else if (FactionShortName == "Liao")
                    {
                        Main.Log.LogDebug("[SpecialAmmoOverTime] Faction identified as Liao.");
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_SRM", "Ammo_AmmunitionBox_Generic_SRM_Inferno", 1.00f);
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Inferno", new DateTime(3056, 1, 1), new DateTime(3062, 1, 1), 0.10f);
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_ArrowIV", "Ammo_AmmunitionBox_Generic_ArrowIV_Inferno", 0.01f);
                        Main.Log.LogDebug("[SpecialAmmoOverTime] Liao got their infernos!");
                    }
                    else if (FactionShortName == "Marik")
                    {
                        if (simulation.CurrentDate < new DateTime(3057, 1, 1))
                        {
                            ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm", new DateTime(3053, 1, 1), new DateTime(3058, 1, 1), 0.10f);
                        }
                        else
                        {
                            ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm-I", new DateTime(3057, 1, 1), new DateTime(3066, 1, 1), 0.10f);
                        }
                    }
                    else if (FactionShortName == "ComStar")
                    {
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm", new DateTime(3053, 1, 1), new DateTime(3058, 1, 1), 0.15f);
                    }
                    else if (FactionShortName == "Word of Blake")
                    {
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm-I", new DateTime(3057, 1, 1), new DateTime(3066, 1, 1), 0.15f);
                    }
                    else if (IsClan == true || FactionShortName == "Black Widow Company")
                    {
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm", 0.10f);
                    }
                    else
                    {
                        Main.Log.LogDebug($"[SpecialAmmoOverTime] Faction is '{FactionShortName}'. No special ammo logic for this faction in this patch.");
                    }
                }
                else
                {
                    Main.Log.LogDebug("[SpecialAmmoOverTime] Skipping special ammo application due to invalid target or state.");
                }
            }
        }

        private static void ReplaceAmmo(Mech mech, string originalAmmoID, string replacementAmmoID, float chance)
        {
            Main.Log.LogDebug($"[SpecialAmmoOverTime] Attempting to replace '{originalAmmoID}' with '{replacementAmmoID}' with {chance}% chance for {mech.DisplayName}.");

            List<AmmunitionBox> ammoBoxesToReplace = new List<AmmunitionBox>();
            foreach (AmmunitionBox ammunitionBox in mech.ammoBoxes)
            {
                if (ammunitionBox.ammoDef.Description.Id == originalAmmoID)
                {
                    if (Random.Range(0f, 1f) < chance)
                    {
                        ammoBoxesToReplace.Add(ammunitionBox);
                    }
                }
            }
            foreach (AmmunitionBox ammoBoxToRemove in ammoBoxesToReplace)
            {
                mech.allComponents.Remove(ammoBoxToRemove);
                mech.ammoBoxes.Remove(ammoBoxToRemove);
                AddOurAmmo(mech, replacementAmmoID, (ChassisLocations)ammoBoxToRemove.Location);
            }
        }

        private static void ReplaceAmmo(Mech mech, string originalAmmoID, string replacementAmmoID, DateTime productionDate, DateTime commonDate, float maxChance)
        {
            SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
            float currentChance = InterpolateChance(simulation.CurrentDate, productionDate, commonDate, maxChance);

            Main.Log.LogDebug($"[SpecialAmmoOverTime] Attempting to replace '{originalAmmoID}' with '{replacementAmmoID}' with {maxChance}% chance for {mech.DisplayName}.");

            List<AmmunitionBox> ammoBoxesToReplace = new List<AmmunitionBox>();
            foreach (AmmunitionBox ammunitionBox in mech.ammoBoxes)
            {
                if (ammunitionBox.ammoDef.Description.Id == originalAmmoID)
                {
                    if (Random.Range(0f, 1f) < currentChance)
                    {
                        ammoBoxesToReplace.Add(ammunitionBox);
                    }
                }
            }
            foreach (AmmunitionBox ammoBoxToRemove in ammoBoxesToReplace)
            {
                mech.allComponents.Remove(ammoBoxToRemove);
                mech.ammoBoxes.Remove(ammoBoxToRemove);
                AddOurAmmo(mech, replacementAmmoID, (ChassisLocations)ammoBoxToRemove.Location);
            }
        }

        private static float InterpolateChance(DateTime currentDate, DateTime productionDate, DateTime commonDate, float maxChance)
        {
            if (currentDate < productionDate) return 0.00f;
            if (currentDate >= commonDate) return maxChance;

            double currentDays = (currentDate - productionDate).TotalDays;
            double totalDays = (commonDate - productionDate).TotalDays;

            float interpolationFactor = (float)(currentDays / totalDays);
            return 0.05f + (maxChance - 0.05f) * interpolationFactor;
        }

        private static void AddOurAmmo(Mech mech, string componentId, ChassisLocations location)
        {
            MechComponentRef mechComponentRef = new MechComponentRef(componentId, Guid.NewGuid().ToString(), ComponentType.AmmunitionBox, location, -1, ComponentDamageLevel.Functional, false) { DataManager = mech.MechDef.DataManager };
            mechComponentRef.RefreshComponentDef();
            AmmunitionBox ammunitionBox = new AmmunitionBox(mech, mechComponentRef, Guid.NewGuid().ToString());
            mech.allComponents.Add(ammunitionBox);
            mech.ammoBoxes.Add(ammunitionBox);
        }
    }

    //internal static class InfantryOverTime
    //{
    //    private static string FactionShortName => SpawningActorInfo.FactionShortName;
    //
    //    [HarmonyPatch(typeof(Mech), "InitStats")]
    //    public static class Mech_InitStats
    //    {
    //        [HarmonyPostfix]
    //        public static void Postfix(Mech __instance)
    //        {
    //            SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
    //            bool isInvalidTarget = SpawningActorInfo.SpawningActorIsPlayerTeam || __instance.StatCollection.GetValue<bool>("IsTargetingDummy") || __instance.StatCollection.GetValue<bool>("GuaranteeUnhittable");
    //            bool isInvalidState = simulation == null || __instance.Combat == null || __instance.Combat.ActiveContract == null;
    //            if (FactionShortName == "Liao" && simulation.CurrentDate < new DateTime(3058, 1, 1))
    //            {
    //                // Infantry replacement code logic goes here.
    //            }
    //        }
    //    }
    //}
}