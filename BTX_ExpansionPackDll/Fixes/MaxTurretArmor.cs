using BattleTech;

namespace BTX_ExpansionPack.Fixes
{
    internal class MaxTurretArmor
    {
        [HarmonyPatch(typeof(Extended_CE.NewTech.ArmorRules), "MaxFrontArmor")]
        public static class ArmorRules_MaxFrontArmor
        {
            [HarmonyPrefix]
            public static bool Prefix(LocationDef locationDef, ref float __result)
            {
                if (locationDef.Location != ChassisLocations.Head) return true;
                __result = locationDef.MaxArmor;
                return false;
            }
        }
    }
}

