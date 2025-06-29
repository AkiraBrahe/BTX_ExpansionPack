using BattleTech.Framework;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    internal class MaxPlayerUnits
    {

        [HarmonyPatch(typeof(ContractOverride), "FromJSONFull")]
        public static class ContractOverride_FromJSONFull
        {
            [HarmonyPostfix]
            public static void Postfix(ContractOverride __instance)
            {
                PatchContract(__instance);
            }
        }

        [HarmonyPatch(typeof(ContractOverride), "FullRehydrate")]
        public static class ContractOverride_FullRehydrate
        {
            [HarmonyPostfix]
            public static void Postfix(ContractOverride __instance)
            {
                PatchContract(__instance);
            }
        }

        [HarmonyPatch]
        public static void PatchContract(ContractOverride __instance)
        {
            if (Main.Settings.Gameplay.Use4LimitOnStoryMissions && IsAnyStoryContract(__instance))
            {
                return;
            }

            if (__instance.maxNumberOfPlayerUnits >= 4 && !IsContractLimitedTo4Units(__instance))
            {
                __instance.maxNumberOfPlayerUnits = 12;
            }
        }

        private static bool IsAnyStoryContract(ContractOverride contractOverride) =>
            contractOverride.contractDisplayStyle == ContractDisplayStyle.BaseCampaignStory ||
            contractOverride.contractDisplayStyle == ContractDisplayStyle.BaseCampaignRestoration;

        private static bool IsContractLimitedTo4Units(ContractOverride contractOverride) =>
            BTX_CAC_CompatibilityDll.Main.Sett.Use4LimitOnContractIds.Contains(contractOverride.ID);
    }
}