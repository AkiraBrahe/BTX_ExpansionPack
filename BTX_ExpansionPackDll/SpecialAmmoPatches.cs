using System;
using System.Collections.Generic;
using BattleTech;
using HarmonyLib;
using Random = UnityEngine.Random;

namespace BTX_ExpansionPack
{
    internal class SpecialAmmoPatches
    {
        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats
        {
            [HarmonyPostfix]
            public static void Postfix(Mech __instance)
            {
                SimGameState simulation = UnityGameInstance.BattleTechGame.Simulation;
                bool isPlayerTeam = __instance.team.IsLocalPlayer || __instance.team.FactionValue.IsPlayer1sMercUnit || __instance.team.FactionValue.IsPlayer2sMercUnit;
                bool isInvalidTarget = __instance.StatCollection.GetValue<bool>("IsTargetingDummy") || __instance.StatCollection.GetValue<bool>("GuaranteeUnhittable");
                bool isInvalidState = simulation == null || __instance.Combat == null || __instance.Combat.ActiveContract == null;

                if (!isPlayerTeam && !isInvalidTarget && !isInvalidState)
                {
                    if (__instance.team?.FactionValue?.FactionDef?.ShortName == "Kurita")
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
                    else if (__instance.team?.FactionValue?.FactionDef?.ShortName == "Liao")
                    {
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_SRM", "Ammo_AmmunitionBox_Generic_SRM_Inferno", 0.10f);
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Inferno", new DateTime(3056, 1, 1), new DateTime(3062, 1, 1), 0.10f);
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_ArrowIV", "Ammo_AmmunitionBox_Generic_ArrowIV_Inferno", 0.01f);
                    }
                    else if (__instance.team?.FactionValue?.FactionDef?.ShortName == "Marik")
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
                    else if (__instance.team?.FactionValue?.Name == "ComStar")
                    {
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm", new DateTime(3053, 1, 1), new DateTime(3058, 1, 1), 0.15f);
                    }

                    else if (__instance.team?.FactionValue?.Name == "WordOfBlake")
                    {
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm-I", new DateTime(3057, 1, 1), new DateTime(3066, 1, 1), 0.15f);
                    }
                    else if (__instance.team?.FactionValue?.IsClan == true || __instance.team?.FactionValue?.Name == "BlackWidowCompany")
                    {
                        ReplaceAmmo(__instance, "Ammo_AmmunitionBox_Generic_LRM", "Ammo_AmmunitionBox_Generic_LRM_Swarm", 0.10f);
                    }
                }
            }

            private static void ReplaceAmmo(Mech mech, string originalAmmoID, string replacementAmmoID, float chance)
            {
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

                double totalDays = (commonDate - productionDate).TotalDays;
                double currentDays = (currentDate - productionDate).TotalDays;

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
    }
}