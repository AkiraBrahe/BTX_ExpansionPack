using BattleTech.Framework;
using BattleTech.UI;
using System;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    internal class MaxPlayerUnits
    {
        [HarmonyPatch(typeof(ContractOverride), "FromJSONFull")]
        public static class ContractOverride_FromJSONFull
        {
            [HarmonyPostfix]
            public static void Postfix(ContractOverride __instance) => PatchContract(__instance);
        }

        [HarmonyPatch(typeof(ContractOverride), "FullRehydrate")]
        public static class ContractOverride_FullRehydrate
        {
            [HarmonyPostfix]
            public static void Postfix(ContractOverride __instance) => PatchContract(__instance);
        }

        [HarmonyPatch]
        public static void PatchContract(ContractOverride __instance)
        {
            if (Main.Settings.Gameplay.Use4LimitOnStoryMissions && IsAnyStoryContract(__instance))
                return;

            if (__instance.maxNumberOfPlayerUnits >= 4 && !IsContractLimitedTo4Units(__instance))
            {
                __instance.maxNumberOfPlayerUnits = 12;
            }
        }

        [HarmonyPatch(typeof(LanceConfiguratorPanel), "SetData")]
        public static class LanceConfiguratorPanel_SetData
        {
            [HarmonyFinalizer]
            public static void Finalizer(LanceConfiguratorPanel __instance)
            {
                bool TBDDetected = AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.GetName().Name.Equals("TBD"));
                if (TBDDetected) return;

                if (__instance.loadoutSlots != null)
                {
                    for (int i = 0; i < __instance.loadoutSlots.Length; i++)
                    {
                        bool locked = (i >= __instance.activeContract.Override.maxNumberOfPlayerUnits);
                        __instance.loadoutSlots[i].SetLockState(locked ?
                            LanceLoadoutSlot.LockState.Full :
                            LanceLoadoutSlot.LockState.Unlocked);
                    }
                }
            }
        }

        private static bool IsAnyStoryContract(ContractOverride contractOverride) =>
            contractOverride.contractDisplayStyle == ContractDisplayStyle.BaseCampaignStory ||
            contractOverride.contractDisplayStyle == ContractDisplayStyle.BaseCampaignRestoration;

        private static bool IsContractLimitedTo4Units(ContractOverride contractOverride) =>
            BTX_CAC_CompatibilityDll.Main.Sett.Use4LimitOnContractIds.Contains(contractOverride.ID);
    }
}