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
            public static void Postfix(OccupyRegionObjective __instance)
            {
                if (!Main.Settings.Gameplay.ForceConvoyToFightInZone)
                    return;

                //bool isConvoyObjective = __instance.requiredTagsOnUnit.Any(tag => tag.StartsWith("unit_vehicle")); // if (isConvoyObjective)
                __instance.allowOpposingUnitsInRegion = false;
                Main.Log.LogDebug($"[OccupyRegionFix] Forcing 'allowOpposingUnitsInRegion' to false for objective: {__instance.DisplayName}");
            }
        }
    }
}