using BattleTech;
using BTX_CAC_CompatibilityDll;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
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
                long kgperpoint = 800; // Points per 10 Tons, i.e. 80 ppt x 10 for Standard armor

                if (c.ChassisTags.Contains("chassis_ferro"))
                {
                    kgperpoint = c.ChassisTags.Contains("chassis_clan") ? 960 : 896;
                }
                else
                {
                    var armor = ArmorTypes.Values.FirstOrDefault(a => c.ChassisTags.Contains(a.Tag));
                    if (armor != null)
                    {
                        kgperpoint = (long)(800 * armor.PptMultiplier);
                    }
                }

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