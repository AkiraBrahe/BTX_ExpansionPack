using BattleTech;
using CustAmmoCategories;
using IRBTModUtils;
using System.Threading;

namespace BTX_ExpansionPack.Fixes.UI
{
    public static class BattleUI
    {
        [HarmonyPatch(typeof(ToHitModifiersHelper), "GetToHitModifierName", [typeof(Mech), typeof(int)])]
        public static class ToHitModifiersHelper_GetToHitModifierName_Mech
        {
            [HarmonyPrefix]
            public static bool Prefix(Mech unit, int location, ref string __result)
            {
                if (unit == null)
                {
                    __result = string.Empty;
                    return false;
                }

                var cLoc = (ChassisLocations)location;
                if (string.IsNullOrEmpty(unit.GetStringForStructureDamageLevel(cLoc)))
                {
                    cLoc = ChassisLocations.CenterTorso;
                }

                Thread.CurrentThread.pushActor(unit);
                var locationDamageLevel = unit.GetLocationDamageLevel(cLoc);
                Thread.CurrentThread.clearActor();

                string text = locationDamageLevel switch
                {
                    LocationDamageLevel.Penalized => string.Format("{0} DAMAGED", GetAbbreviatedChassisLocation(unit, cLoc)),
                    LocationDamageLevel.NonFunctional => string.Format("{0} DESTROYED", GetAbbreviatedChassisLocation(unit, cLoc)),
                    _ => string.Empty,
                };

                __result = text;
                return false;
            }

            public static string GetAbbreviatedChassisLocation(Mech unit, ChassisLocations cLoc)
            {
                var tags = unit.MechDef.MechTags;
                var location = cLoc;
                var locationName = LocationNamingHelper.GetLocationName(tags, location, Main.Settings.UI.Battle.ShowFullLocationName);
                return !string.IsNullOrEmpty(locationName) ? locationName : string.Empty;
            }
        }

        [HarmonyPatch(typeof(ToHitModifiersHelper), "GetToHitModifierName", [typeof(Vehicle), typeof(int)])]
        public static class ToHitModifiersHelper_GetToHitModifierName_Vehicle
        {
            [HarmonyPrefix]
            public static bool Prefix(Vehicle unit, int location, ref string __result)
            {
                if (unit == null)
                {
                    __result = string.Empty;
                    return false;
                }

                var vLoc = (VehicleChassisLocations)location;
                if (string.IsNullOrEmpty(unit.GetStringForStructureDamageLevel(vLoc)))
                {
                    vLoc = VehicleChassisLocations.Front;
                }

                var locationDamageLevel = unit.GetLocationDamageLevel(vLoc);

                string text = locationDamageLevel switch
                {
                    LocationDamageLevel.Penalized => string.Format("{0} DAMAGED", GetAbbreviatedChassisLocation(unit, vLoc)),
                    LocationDamageLevel.NonFunctional => string.Format("{0} DESTROYED", GetAbbreviatedChassisLocation(unit, vLoc)),
                    _ => string.Empty,
                };

                __result = text;
                return false;
            }

            public static string GetAbbreviatedChassisLocation(Vehicle unit, VehicleChassisLocations vLoc)
            {
                var tags = unit.VehicleDef.VehicleTags;
                var location = vLoc.toFakeChassis();
                var locationName = LocationNamingHelper.GetLocationName(tags, location, Main.Settings.UI.Battle.ShowFullLocationName);
                return !string.IsNullOrEmpty(locationName) ? locationName : string.Empty;
            }
        }
    }
}