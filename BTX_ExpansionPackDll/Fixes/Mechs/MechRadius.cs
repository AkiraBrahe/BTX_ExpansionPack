using BattleTech;

namespace BTX_ExpansionPack.Fixes.Mechs
{
    internal class MechRadius
    {
        /// <summary>
        /// Adjusts mech radius based on weight class to make it more progressive.
        /// </summary>
        [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
        public static class ChassisDef_FromJSON_RadiusFix
        {
            [HarmonyPostfix]
            public static void Postfix(ChassisDef __instance)
            {
                if (__instance == null) return;

                __instance.Radius = __instance.Tonnage switch
                {
                    < 20 => 3,              // Ultralight
                    >= 20 and <= 39 => 4,   // Light
                    >= 40 and <= 59 => 5,   // Medium
                    >= 60 and <= 79 => 6,   // Heavy
                    >= 80 and <= 100 => 7,  // Assault
                    > 100 => 8,             // Superheavy
                    _ => throw new System.NotImplementedException(),
                };
            }
        }
    }
}
