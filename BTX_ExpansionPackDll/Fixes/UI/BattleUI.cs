using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using IRBTModUtils;
using System.Threading;

namespace BTX_ExpansionPack.Fixes.UI
{
    public static class BattleUI
    {
        [HarmonyPatch(typeof(Pilot), "InjuryReasonDescription", MethodType.Getter)]
        public static class PilotInjury_InjuryReasonDescription
        {
            [HarmonyPostfix]
            public static void Postfix(Pilot __instance, ref string __result)
            {
                if (__instance.InjuryReason == InjuryReason.ActorDestroyed &&
                    __instance.ParentActor is FakeVehicleMech)
                {
                    __result = "VEHICLE DESTROYED";
                }
            }
        }

        [HarmonyPatch(typeof(ToHitModifiersHelper), "GetAbbreviatedChassisLocation", typeof(VehicleChassisLocations))]
        public static class ToHitModifiersHelper_GetAbbreviatedChassisLocation
        {
            [HarmonyPrefix]
            public static bool Prefix(VehicleChassisLocations location, ref string __result)
            {
                __result = LocationNamingHelper.GetLocationName(["fake_vehicle_chassis"], location.toFakeChassis(), false);
                return false;
            }
        }

        [HarmonyPatch(typeof(ToHitModifiersHelper), "GetToHitModifierName", [typeof(Mech), typeof(int)])]
        public static class ToHitModifiersHelper_GetToHitModifierName_Mech
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.UI.Battle.ShowFullLocationName;

            [HarmonyPrefix]
            public static bool Prefix(ref bool __runOriginal, Mech unit, int location, ref string __result)
            {
                if (unit == null)
                {
                    __result = string.Empty;
                    __runOriginal = false;
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
                __runOriginal = false;
                return false;
            }

            public static string GetAbbreviatedChassisLocation(Mech unit, ChassisLocations cLoc)
            {
                var tags = unit.MechDef.MechTags;
                var location = cLoc;
                var locationName = LocationNamingHelper.GetLocationName(tags, location, true);
                return !string.IsNullOrEmpty(locationName) ? locationName : string.Empty;
            }
        }

        [HarmonyPatch(typeof(ToHitModifiersHelper), "GetToHitModifierName", [typeof(Vehicle), typeof(int)])]
        public static class ToHitModifiersHelper_GetToHitModifierName_Vehicle
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.UI.Battle.ShowFullLocationName;

            [HarmonyPrefix]
            public static bool Prefix(ref bool __runOriginal, Vehicle unit, int location, ref string __result)
            {
                if (unit == null)
                {
                    __result = string.Empty;
                    __runOriginal = false;
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
                __runOriginal = false;
                return false;
            }

            public static string GetAbbreviatedChassisLocation(Vehicle unit, VehicleChassisLocations vLoc)
            {
                var tags = unit.VehicleDef.VehicleTags;
                var location = vLoc.toFakeChassis();
                var locationName = LocationNamingHelper.GetLocationName(tags, location, true);
                return !string.IsNullOrEmpty(locationName) ? locationName : string.Empty;
            }
        }
    }
}