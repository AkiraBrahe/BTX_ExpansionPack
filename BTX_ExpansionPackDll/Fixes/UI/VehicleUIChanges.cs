using BattleTech;
using CustomUnits;
using Localize;

namespace BTX_ExpansionPack.Fixes
{
    internal class VehicleUIChanges
    {
        [HarmonyPatch(typeof(ReducedComponentRefInfoHelper), "description")]
        public static class ReducedComponentRefInfoHelper_description
        {
            [HarmonyPrefix]
            public static bool Prefix(Strings.Culture culture, ref string __result)
            {
                if (culture != Strings.Culture.CULTURE_RU_RU)
                {
                    __result = "Enabling this option allows for limited customization of vehicles.\n\n" +
                        "• Limited means you can replace existing weapons and ammo, but cannot add or remove any equipment. To replace or repair a component, simply drag a new one over it. The new component must be of the same type and have equal or lesser size, tonnage, and heat generation.\n\n" +
                        "• With this option active, your vehicles won't be automatically repaired after each battle. Additionally, you will be able to store vehicles, with all weapons and ammo being automatically moved to your storage. When you restore a stored vehicle, it will be equipped with destroyed versions of the original weapons and ammo.";

                    return false;
                }

                return true;
            }
        }

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
    }
}