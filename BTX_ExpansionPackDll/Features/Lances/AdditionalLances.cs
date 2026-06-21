using BattleTech.Framework;
using HBS.Collections;
using System;

namespace BTX_ExpansionPack.Features.Lances
{
    internal class AdditionalLances
    {
        /// <summary>
        /// Ensures that additional lances spawn with appropriate weight classes.
        /// </summary>
        [HarmonyPatch(typeof(UnitSpawnPointOverride), "RequestUnit", typeof(string), typeof(DateTime?), typeof(TagSet))]
        public static class UnitSpawnPointOverride_RequestUnit_PrePatch
        {
            [HarmonyPrefix]
            [HarmonyBefore("BattleTech.Haree.FullXotlTables")]
            public static void Prefix(UnitSpawnPointOverride __instance, string lanceDefId, DateTime? currentDate)
            {
                if (currentDate == null) return;
                switch (lanceDefId)
                {
                    // APC Lance: Clamp weight to Medium/Light
                    case "lancedef_apc_dynamic_battle1":
                        __instance.unitTagSet.ClampToWeightClass("unit_medium", "unit_light", 0.8f);
                        break;

                    // VTOL Lance: Force weight to Light
                    case "lancedef_vtol_dynamic_battle1":
                        __instance.unitTagSet.ForceWeightClass("unit_light");
                        break;

                    // Heavy VTOL Lance: Force weight to Heavy
                    case "lancedef_vtol_heavy_dynamic_battle1":
                        __instance.unitTagSet.ForceWeightClass("unit_heavy");
                        break;
                }
            }
        }

        /// <summary>
        /// Remaps sub-units to their parent factions so they can spawn appropriate lances.
        /// </summary>
        [HarmonyPatch(typeof(MissionControl.Config.AdditionalLances), "GetLancePoolKeys", typeof(string), typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(int))]
        public static class AdditionalLances_GetLancePoolKeys
        {
            private static readonly string[] FactionsWithSubunits =
            [
                "Davion", "Kurita", "Liao", "Marik", "Steiner",
                "ChaosMarch", "FedCom", "Ives", "Rasalhague", "Merc",
                "TaurianConcordat", "Calderon", "MagistracyOfCanopus", "Marian", "Outworld"
            ];

            [HarmonyPrefix]
            public static void Prefix(ref string faction)
            {
                if (string.IsNullOrEmpty(faction)) return;

                foreach (var baseFaction in FactionsWithSubunits)
                {
                    if (faction.StartsWith(baseFaction) && faction.Length > baseFaction.Length)
                    {
                        faction = baseFaction;
                        break;
                    }
                }
            }
        }
    }
}