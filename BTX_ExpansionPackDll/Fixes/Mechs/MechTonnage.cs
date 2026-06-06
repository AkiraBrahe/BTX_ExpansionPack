using BattleTech;
using BTX_CAC_CompatibilityDll;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Fixes.Mechs
{
    public static class MechTonnage
    {
        /// <summary>
        /// Improves the tonnage calculation to accommodate all armor types.
        /// </summary>
        [HarmonyPatch(typeof(TonnageCalculation), "Calc")]
        public static class TonnageCalculation_Calc
        {
            [HarmonyPrepare]
            public static bool Prepare() => !Main.HasAdvancedMechLab;

            [HarmonyPrefix]
            public static bool Prefix(ChassisDef c, long armorPoints, IEnumerable<MechComponentRef> inventory, ref int __result)
            {
                int kg = (int)(c.InitialTonnage * 1000.0f);
                var armor = c.GetArmorInfo();
                long kgperpoint = (long)(800 * armor.PptMultiplier); // Points per 10 Tons, i.e. 80 ppt x 10 for Standard armor

                // Original logic
                kg += (int)(armorPoints * 10L / kgperpoint);
                foreach (var i in inventory)
                {
                    kg += (int)(i.Def.Tonnage * 1000.0f);
                }
                if (kg / 10 == (int)(c.Tonnage * 100.0f))
                {
                    __result = (int)(c.Tonnage * 1000.0f);
                }
                else
                {
                    __result = kg;
                }

                return false;
            }
        }
    }
}