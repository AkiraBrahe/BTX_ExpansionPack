using System;
using System.Linq;
using BattleTech;
using BiggerDrops.Features;
using HarmonyLib;

namespace BTX_ExpansionPack.Fixes
{
    internal class DropSlots
    {
        public static int GetUpgradeStat(SimGameState simState, string upgradeStat)
        {
            int maxValue = 0;

            foreach (ShipModuleUpgrade shipModuleUpgrade in simState.ShipUpgrades)
            {
                if (simState.HasShipUpgrade(shipModuleUpgrade.Description.Id, null))
                {
                    foreach (SimGameStat simGameStat in shipModuleUpgrade.Stats)
                    {
                        if (simGameStat.name == upgradeStat && simGameStat.set)
                        {
                            int currentValue = simGameStat.ToInt();
                            if (currentValue > maxValue)
                            {
                                maxValue = currentValue;
                            }
                        }
                    }
                }
            }
            return maxValue;
        }

        public static void Postfix(SimGameState __instance)
        {
            bool flag = false;
            bool BroadswordModIsActive = AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.FullName.Equals("BroadswordDropShip"));

            if (!BroadswordModIsActive && __instance.CompanyStats.GetValue<int>("BiggerDrops_BaseMechSlots") != 4)
            {
                __instance.CompanyStats.RemoveStatistic("BiggerDrops_BaseMechSlots");
                __instance.CompanyStats.AddStatistic<int>("BiggerDrops_BaseMechSlots", 4);
                flag = true;
            }
            else if (BroadswordModIsActive && __instance.CompanyStats.GetValue<int>("BiggerDrops_BaseMechSlots") != 5)
            {
                __instance.CompanyStats.RemoveStatistic("BiggerDrops_BaseMechSlots");
                __instance.CompanyStats.AddStatistic<int>("BiggerDrops_BaseMechSlots", 5);
                flag = true;
            }

            int additionalSlots = GetUpgradeStat(__instance, "BiggerDrops_AdditionalMechSlots");
            if (__instance.CompanyStats.GetValue<int>("BiggerDrops_AdditionalMechSlots") != additionalSlots)
            {
                __instance.CompanyStats.RemoveStatistic("BiggerDrops_AdditionalMechSlots");
                __instance.CompanyStats.AddStatistic<int>("BiggerDrops_AdditionalMechSlots", additionalSlots);
                flag = true;
            }

            int hotdropSlots = GetUpgradeStat(__instance, "BiggerDrops_HotDropMechSlots");
            if (__instance.CompanyStats.GetValue<int>("BiggerDrops_HotDropMechSlots") != hotdropSlots)
            {
                __instance.CompanyStats.RemoveStatistic("BiggerDrops_HotDropMechSlots");
                __instance.CompanyStats.AddStatistic<int>("BiggerDrops_HotDropMechSlots", hotdropSlots);
                flag = true;
            }

            if (flag) { DropManager.UpdateCULances(); }
            Main.Log.Log(string.Format("dropslot stats: BiggerDrops_BaseMechSlots: {0}, BiggerDrops_AdditionalMechSlots: {1}, BiggerDrops_HotDropMechSlots: {2}",
                __instance.CompanyStats.GetValue<int>("BiggerDrops_BaseMechSlots"),
                __instance.CompanyStats.GetValue<int>("BiggerDrops_AdditionalMechSlots"),
                __instance.CompanyStats.GetValue<int>("BiggerDrops_HotDropMechSlots")
            ));
        }

        public static void Patch(Harmony harmony)
        {
            HarmonyMethod harmonyMethod = new HarmonyMethod(AccessTools.DeclaredMethod(typeof(DropSlots), "Postfix", null, null))
            {
                after = new string[] { "de.morphyum.BiggerDrops" }
            };
            harmony.Patch(AccessTools.DeclaredMethod(typeof(SimGameState), "Rehydrate", null, null), null, harmonyMethod, null, null, null);
            harmony.Patch(AccessTools.DeclaredMethod(typeof(SimGameState), "InitCompanyStats", null, null), null, harmonyMethod, null, null, null);
        }
    }
}