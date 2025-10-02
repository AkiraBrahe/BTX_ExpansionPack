using BattleTech.Designed;

namespace BTX_ExpansionPack.Fixes
{
    internal class OccupyRegionFix
    {
        /// <summary>
        /// Forces convoy units to engage any enemies inside the extraction zone.
        /// </summary>
        [HarmonyPatch(typeof(OccupyRegionObjective), "ContractInitialize")]
        public static class OccupyRegionObjective_ContractInitialize
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.Debug.ForceConvoyToFightInZone;

            [HarmonyPostfix]
            public static void Postfix(OccupyRegionObjective __instance)
            {
                if (!__instance.DisplayName.Equals("Withdraw"))
                {
                    __instance.allowOpposingUnitsInRegion = false;
                }
            }
        }
    }
}