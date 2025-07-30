using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using System;

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
                if (cLoc is ChassisLocations.None or ChassisLocations.Arms or ChassisLocations.MainBody)
                    return;

                float currentStructure = __instance.GetCurrentStructure(cLoc);
                if (currentStructure <= 0f)
                    return;

                float currentArmor = __instance.GetCurrentArmor(aLoc);
                float damageSpillover = Math.Max(0f, totalArmorDamage - currentArmor);
                float effectiveDamage = damageSpillover + directStructureDamage;

                // If the damage is enough to destroy the location, apply pilot injury.
                if (effectiveDamage > 0f && currentStructure <= effectiveDamage)
                {
                    var pilot = __instance.GetPilot();
                    if (pilot != null)
                    {
                        pilot.SetNeedsInjury(InjuryReason.ActorDestroyed);
                        __instance.CheckPilotStatusFromAttack(hitInfo.attackerId, hitInfo.attackSequenceId, hitInfo.stackItemUID);
                    }
                }
            }

            [HarmonyPatch(typeof(Vehicle), "DamageLocation")]
            public static class Vehicle_DamageLocation
            {
                [HarmonyPrefix]
                public static void Prefix(Vehicle __instance, WeaponHitInfo hitInfo, VehicleChassisLocations vLoc, float totalArmorDamage, float directStructureDamage)
                {
                    float currentStructure = __instance.GetCurrentStructure(vLoc);
                    if (currentStructure <= 0f)
                        return;

                    float currentArmor = __instance.GetCurrentArmor(vLoc);
                    float damageSpillover = Math.Max(0f, totalArmorDamage - currentArmor);
                    float effectiveDamage = damageSpillover + directStructureDamage;

                    // If the damage is enough to destroy the location, apply pilot injury.
                    if (effectiveDamage > 0f && currentStructure <= effectiveDamage)
                    {
                        var pilot = __instance.GetPilot();
                        if (pilot != null && !pilot.IsIncapacitated)
                        {
                            pilot.SetNeedsInjury(InjuryReason.ActorDestroyed);
                            __instance.CheckPilotStatusFromAttack(hitInfo.attackerId, hitInfo.attackSequenceId, hitInfo.stackItemUID);
                        }
                    }
                }
            }
        }
    }
}