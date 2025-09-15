using CustomUnits;
using Quirks.Quirks.MechEffects;

namespace BTX_ExpansionPack.Fixes
{
    /// <summary>
    /// Makes LAMs in air mode count as airborne targets when targeted by anti-air mechs.
    /// </summary>
    internal class AntiAirTargeting
    {
        [HarmonyPatch(typeof(AlternatesRepresentation), "AddCurrentTags")]
        public static class AlternatesRepresentation_AddCurrentTags
        {
            [HarmonyPostfix]
            public static void Postfix(AlternatesRepresentation __instance)
            {
                if (__instance?.parentMech == null || __instance.CurrentRepresentation?.altDef == null)
                    return;

                var tags = __instance.CurrentRepresentation.altDef.additionalEncounterTags;
                MechQuirkInfo.MechQuirkStore[__instance.parentMech.MechDef.Chassis.Description.Id].VTOL = tags.Contains("unit_lam");
            }
        }
    }
}