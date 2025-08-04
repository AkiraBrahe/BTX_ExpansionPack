using BattleTech;
using BiggerDrops.Features;
using System;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    internal class DropSlots
    {
        private const string BaseMechSlotsStat = "BiggerDrops_BaseMechSlots";
        private const string AdditionalMechSlotsStat = "BiggerDrops_AdditionalMechSlots";
        private const string HotDropMechSlotsStat = "BiggerDrops_HotDropMechSlots";

        [HarmonyPatch(typeof(SimGameState), "InitCompanyStats")]
        [HarmonyPatch(typeof(SimGameState), "Rehydrate")]
        [HarmonyAfter("de.morphyum.BiggerDrops")]
        public static class SimGameState_Patches
        {
            [HarmonyPrepare]
            public static bool Prepare() => !AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.GetName().Name.Equals("BroadswordDropShip"));

            [HarmonyPostfix]
            public static void Postfix(SimGameState __instance)
            {
                bool updated = false;

                if (Main.Settings.Debug.AllDropShipUpgrades &&
                    (__instance.CompanyStats.GetValue<int>(BaseMechSlotsStat) != 4 ||
                    __instance.CompanyStats.GetValue<int>(AdditionalMechSlotsStat) != 4 ||
                    __instance.CompanyStats.GetValue<int>(HotDropMechSlotsStat) != 4))
                {
                    UpdateStatistic(__instance, BaseMechSlotsStat, 4);
                    UpdateStatistic(__instance, AdditionalMechSlotsStat, 4);
                    UpdateStatistic(__instance, HotDropMechSlotsStat, 4);
                    updated = true;
                }
                else
                {
                    if (__instance.CompanyStats.GetValue<int>(BaseMechSlotsStat) != 4)
                    {
                        UpdateStatistic(__instance, BaseMechSlotsStat, 4);
                        updated = true;
                    }

                    int additionalSlots = GetUpgradeStat(__instance, AdditionalMechSlotsStat);
                    if (__instance.CompanyStats.GetValue<int>(AdditionalMechSlotsStat) != additionalSlots)
                    {
                        UpdateStatistic(__instance, AdditionalMechSlotsStat, additionalSlots);
                        updated = true;
                    }

                    int hotdropSlots = GetUpgradeStat(__instance, HotDropMechSlotsStat);
                    if (__instance.CompanyStats.GetValue<int>(HotDropMechSlotsStat) != hotdropSlots)
                    {
                        UpdateStatistic(__instance, HotDropMechSlotsStat, hotdropSlots);
                        updated = true;
                    }
                }

                if (updated) DropManager.UpdateCULances();
                Main.Log.Log($"Dropslot stats: " +
                    $"{BaseMechSlotsStat}: {__instance.CompanyStats.GetValue<int>(BaseMechSlotsStat)}, " +
                    $"{AdditionalMechSlotsStat}: {__instance.CompanyStats.GetValue<int>(AdditionalMechSlotsStat)}, " +
                    $"{HotDropMechSlotsStat}: {__instance.CompanyStats.GetValue<int>(HotDropMechSlotsStat)}"
                );
            }

            private static int GetUpgradeStat(SimGameState simState, string upgradeStat)
            {
                return simState.ShipUpgrades
                    .Where(upg => simState.HasShipUpgrade(upg.Description.Id, null))
                    .SelectMany(upg => upg.Stats)
                    .Where(stat => stat.name == upgradeStat && stat.set)
                    .Select(stat => stat.ToInt())
                    .DefaultIfEmpty(0)
                    .Max();
            }

            private static void UpdateStatistic(SimGameState simState, string statName, int value)
            {
                if (simState.CompanyStats.GetValue<int>(statName) != value)
                {
                    simState.CompanyStats.RemoveStatistic(statName);
                    simState.CompanyStats.AddStatistic(statName, value);
                }
            }
        }
    }
}