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
        private const string MaxTonnageStat = "BiggerDrops_MaxTonnage";

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
                int updated = 0;

                if (Main.Settings.Debug.AllDropShipUpgrades)
                {
                    UpdateStatistic(__instance, BaseMechSlotsStat, 4, ref updated);
                    UpdateStatistic(__instance, AdditionalMechSlotsStat, 4, ref updated);
                    UpdateStatistic(__instance, HotDropMechSlotsStat, 4, ref updated);
                    UpdateStatistic(__instance, MaxTonnageStat, 800, ref updated);
                }
                else
                {
                    UpdateStatistic(__instance, BaseMechSlotsStat, 4, ref updated);

                    int additionalSlots = GetUpgradeStat(__instance, AdditionalMechSlotsStat);
                    UpdateStatistic(__instance, AdditionalMechSlotsStat, additionalSlots, ref updated);

                    int hotdropSlots = GetUpgradeStat(__instance, HotDropMechSlotsStat);
                    UpdateStatistic(__instance, HotDropMechSlotsStat, hotdropSlots, ref updated);

                    int maxTonnage = GetMaxTonnageStat(__instance, BiggerDrops.BiggerDrops.settings.defaultMaxTonnage);
                    UpdateStatistic(__instance, MaxTonnageStat, maxTonnage, ref updated);
                }

                if (updated >= 1) DropManager.UpdateCULances();
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

            private static int GetMaxTonnageStat(SimGameState simState, int defaultValue)
            {
                return defaultValue + simState.ShipUpgrades
                    .Where(upg => simState.HasShipUpgrade(upg.Description.Id, null))
                    .SelectMany(upg => upg.Stats)
                    .Where(stat => stat.name == MaxTonnageStat)
                    .Sum(stat => stat.ToInt());
            }

            private static void UpdateStatistic(SimGameState simState, string statName, int value, ref int updated)
            {
                if (simState.CompanyStats.GetValue<int>(statName) != value)
                {
                    simState.CompanyStats.RemoveStatistic(statName);
                    simState.CompanyStats.AddStatistic(statName, value);
                    updated++;
                }
            }
        }
    }
}