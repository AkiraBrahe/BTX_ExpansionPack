using BattleTech;
using MechAffinity;

namespace BTX_ExpansionPack.Utilities
{
    [HarmonyPatch(typeof(PilotAffinityManager), "addToChassisPrefabLut")]
    public class PilotAffinityManager_DebugPrefix
    {
        [HarmonyPrefix]
        public static void Prefix(MechDef mech)
        {
            if (mech == null)
            {
                Main.Log.LogDebug("MechAffinity Debug: Found a NULL MechDef in the loop!");
                return;
            }

            Main.Log.LogDebug($"MechAffinity Debug: Processing Mech: {mech.Description?.Id ?? "Unknown ID"}");

            if (mech.Chassis == null)
            {
                Main.Log.LogDebug($"MechAffinity Debug: CRITICAL - Mech {mech.Description?.Id} has no ChassisDef!");
            }
        }
    }
}