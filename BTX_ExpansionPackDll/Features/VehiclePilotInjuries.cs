using BattleTech;
using CustAmmoCategories;
using CustomUnits;

namespace BTX_ExpansionPack
{
    internal class VehiclePilotInjuries
    {
        [HarmonyPatch(typeof(Pilot), "InjuryReasonDescription", MethodType.Getter)]
        public static class PilotInjury_InjuryReasonDescription
        {
            [HarmonyPostfix]
            public static void Postfix(Pilot __instance, ref string __result)
            {
                if (__instance.InjuryReason == InjuryReason.ActorDestroyed && __instance.ParentActor is FakeVehicleMech)
                {
                    __result = "VEHICLE DESTROYED";
                }
            }
        }

        [HarmonyPatch(typeof(Mech), "DamageLocation")]
        public static class Mech_DamageLocation
        {
            [HarmonyPrefix]
            public static void Prefix(Mech __instance, WeaponHitInfo hitInfo, ArmorLocation aLoc, float totalArmorDamage, float directStructureDamage)
            {
                if (!__instance.FakeVehicle())
                    return;

                var cLoc = MechStructureRules.GetChassisLocationFromArmorLocation(aLoc);
                if (cLoc == ChassisLocations.None || cLoc == ChassisLocations.Arms || cLoc == ChassisLocations.MainBody)
                    return;

                TryApplyPilotInjury(
                    __instance,
                    cLoc,
                    totalArmorDamage,
                    directStructureDamage,
                    hitInfo,
                    InjuryReason.ActorDestroyed
                );
            }
        }

        [HarmonyPatch(typeof(Vehicle), "DamageLocation")]
        public static class Vehicle_DamageLocation
        {
            [HarmonyPrefix]
            public static void Prefix(Vehicle __instance, WeaponHitInfo hitInfo, VehicleChassisLocations vLoc, float totalArmorDamage, float directStructureDamage)
            {
                TryApplyPilotInjury(
                    __instance,
                    vLoc,
                    totalArmorDamage,
                    directStructureDamage,
                    hitInfo,
                    InjuryReason.ActorDestroyed
                );
            }
        }

        private static void TryApplyPilotInjury(AbstractActor actor, object loc, float totalArmorDamage, float directStructureDamage, WeaponHitInfo hitInfo, InjuryReason reason)
        {
            float currentStructure = loc switch
            {
                ChassisLocations cLoc => (actor as Mech)?.GetCurrentStructure(cLoc) ?? 0f,
                VehicleChassisLocations vLoc => (actor as Vehicle)?.GetCurrentStructure(vLoc) ?? 0f,
                _ => 0f
            };
            if (currentStructure <= 0f) return;

            float currentArmor = loc switch
            {
                ArmorLocation aLoc => (actor as Mech)?.GetCurrentArmor(aLoc) ?? 0f,
                VehicleChassisLocations vLoc => (actor as Vehicle)?.GetCurrentArmor(vLoc) ?? 0f,
                _ => 0f
            };

            float incomingStructureDamage = directStructureDamage + totalArmorDamage - currentArmor;
            if (currentStructure - incomingStructureDamage > 0f) return;

            var pilot = actor.GetPilot();
            if (pilot == null) return;

            pilot.SetNeedsInjury(reason);
            actor.CheckPilotStatusFromAttack(hitInfo.attackerId, hitInfo.attackSequenceId, hitInfo.stackItemUID);
        }
    }
}