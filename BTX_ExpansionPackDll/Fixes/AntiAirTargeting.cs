using BattleTech;
using CustomUnits;
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
            public static void Postfix(ref string __result, AbstractActor attacker, Weapon weapon, ICombatant target)
            {
                Mech attackingMech = attacker as Mech;
                bool hasAntiAirQuirk = MechQuirkInfo.MechQuirkStore[attackingMech.MechDef.Chassis.Description.Id].AntiAircraftTargeting;
                if (attackingMech == null || !hasAntiAirQuirk) return;

                bool isAirborneTarget = false;

                FakeVehicleMech vtolTarget = target as FakeVehicleMech;
                if (vtolTarget != null && vtolTarget.MechDef != null && vtolTarget.MechDef.MechTags.Contains("unit_vtol"))
                {
                    isAirborneTarget = true;
                }

                Mech lamTarget = target as Mech;
                if (lamTarget != null && lamTarget.EncounterTags != null && lamTarget.EncounterTags.Contains("unit_lam"))
                {
                    isAirborneTarget = true;
                }

                if (isAirborneTarget)
                {
                    __result = string.Format("{0}ANTI-AIR {1:+#;-#}; ", __result, MechQuirks.modSettings.AntiAircraftTargetingToHit);
                }
            }
        }
    }
}
