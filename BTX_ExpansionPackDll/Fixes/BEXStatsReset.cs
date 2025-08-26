using BattleTech;
using System.Collections.Generic;
using static Extended_CE.BTComponents;

namespace BTX_ExpansionPack.Fixes
{
    internal class BEXStatsReset
    {
        [HarmonyPatch(typeof(Contract), "ResetStateForRestart", [])]
        public static class Contract_ResetStateForRestart
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                foreach (KeyValuePair<string, TTRuleInfo> entry in MechTTRuleInfo.MechTTStatStore)
                {
                    TTRuleInfo ttRuleInfo = entry.Value;
                    ttRuleInfo.HipCrits = 0;
                    ttRuleInfo.EngineCrits = 0;
                    ttRuleInfo.EngineCenterCrits = 0;
                    ttRuleInfo.EngineLeftCrits = 0;
                    ttRuleInfo.EngineRightCrits = 0;
                    ttRuleInfo.GyroDestroyed = false;
                    ttRuleInfo.LifeSupportCrit = false;
                }
            }
        }
    }
}
