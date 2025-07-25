using BattleTech;
using Quirks;
using Quirks.Quirks.MechEffects;

namespace BTX_ExpansionPack.Fixes
{
    internal class AntiAirTargeting
    {
        [HarmonyPatch(typeof(ToHit), "GetAllModifiersDescription")]
        public static class ToHit_GetAllModifiersDescription
        {
            [HarmonyPostfix]
            [HarmonyAfter("BEX.BattleTech.MechQuirks")]
            public static void Postfix(ref string __result, AbstractActor attacker, ICombatant target)
            {
                var attackingMech = attacker as Mech;
                bool hasAntiAirQuirk = MechQuirkInfo.MechQuirkStore[attackingMech.MechDef.chassisID].AntiAircraftTargeting;
                if (attackingMech == null || !hasAntiAirQuirk) return;

                bool isAirborneTarget = false;
                if (target is AbstractActor unit)
                {
                    if (unit.GetTags().Contains("unit_vtol") || unit.GetTags().Contains("unit_lam"))
                    {
                        isAirborneTarget = true;
                    }
                }

                if (isAirborneTarget)
                {
                    __result = string.Format("{0}ANTI-AIR {1:+#;-#}; ", __result, MechQuirks.modSettings.AntiAircraftTargetingToHit);
                }
            }
        }
    }
}