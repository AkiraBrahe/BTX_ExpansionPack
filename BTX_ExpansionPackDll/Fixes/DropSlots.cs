using BattleTech;
using BiggerDrops.Features;
using System;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    /// <summary>
    /// Updates dropship slot and tonnage statistics based on installed upgrades.
    /// </summary>
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
                int baseTonnage = BiggerDrops.BiggerDrops.settings.defaultMaxTonnage;

                if (Main.Settings.Debug.AllDropShipUpgrades)
                {
                    UpdateStatistic(__instance, BaseMechSlotsStat, 4, ref updated);
                    UpdateStatistic(__instance, AdditionalMechSlotsStat, 4, ref updated);
                    UpdateStatistic(__instance, HotDropMechSlotsStat, 4, ref updated);
                    UpdateStatistic(__instance, MaxTonnageStat, baseTonnage + 400, ref updated);
                }
                else
                {
                    UpdateStatistic(__instance, BaseMechSlotsStat, 4, ref updated);

                    int additionalSlots = GetUpgradeStat(__instance, AdditionalMechSlotsStat);
                    UpdateStatistic(__instance, AdditionalMechSlotsStat, additionalSlots, ref updated);

                    int hotdropSlots = GetUpgradeStat(__instance, HotDropMechSlotsStat);
                    UpdateStatistic(__instance, HotDropMechSlotsStat, hotdropSlots, ref updated);

                    int maxTonnage = GetMaxTonnageStat(__instance, baseTonnage);
                    UpdateStatistic(__instance, MaxTonnageStat, maxTonnage, ref updated);
                }

                if (updated >= 1) DropManager.UpdateCULances();
                Main.Log.Log($"Dropslot stats: " +
                    $"{BaseMechSlotsStat}: {__instance.CompanyStats.GetValue<int>(BaseMechSlotsStat)}, " +
                    $"{AdditionalMechSlotsStat}: {__instance.CompanyStats.GetValue<int>(AdditionalMechSlotsStat)}, " +
                    $"{HotDropMechSlotsStat}: {__instance.CompanyStats.GetValue<int>(HotDropMechSlotsStat)}"
                );
            }

            private static int GetUpgradeStat(SimGameState simGame, string upgradeStat)
            {
                return simGame.ShipUpgrades
                    .Where(upg => simGame.HasShipUpgrade(upg.Description.Id))
                    .SelectMany(upg => upg.Stats)
                    .Where(stat => stat.name == upgradeStat && stat.set)
                    .Select(stat => stat.ToInt())
                    .DefaultIfEmpty(0)
                    .Max();
            }

            private static int GetMaxTonnageStat(SimGameState simGame, int defaultValue)
            {
                return defaultValue + simGame.ShipUpgrades
                    .Where(upg => simGame.HasShipUpgrade(upg.Description.Id))
                    .SelectMany(upg => upg.Stats)
                    .Where(stat => stat.name == MaxTonnageStat)
                    .Sum(stat => stat.ToInt());
            }

            private static void UpdateStatistic(SimGameState simGame, string statName, int value, ref int updated)
            {
                if (!simGame.CompanyStats.ContainsStatistic(statName))
                {
                    simGame.CompanyStats.AddStatistic(statName, value);
                    updated++;
                }
                else if (simGame.CompanyStats.GetValue<int>(statName) != value)
                {
                    simGame.CompanyStats.Set(statName, value);
                    updated++;
                }
            }
        }
    }
}