using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using HarmonyLib;

namespace BTX_ExpansionPack
{
    internal class VehiclePilotInjuries
    {
        [HarmonyPatch(typeof(Vehicle), "DamageLocation")]
        public static class Vehicle_DamageLocation
        {
            [HarmonyPrefix]
            public static void Prefix(Vehicle __instance, WeaponHitInfo hitInfo, VehicleChassisLocations vLoc, float totalArmorDamage, float directStructureDamage)
            {
                var currentStructure = __instance.GetCurrentStructure(vLoc);
                if (currentStructure <= 0f) return;

                var currentArmor = __instance.GetCurrentArmor(vLoc);
                var incomingStructureDamage = directStructureDamage + totalArmorDamage - currentArmor;

                if (currentStructure - incomingStructureDamage <= 0f)
                {
                    var pilot = __instance.GetPilot();
                    if (pilot != null)
                    {
                        pilot.SetNeedsInjury(InjuryReason.ActorDestroyed);
                        __instance.CheckPilotStatusFromAttack(hitInfo.attackerId, hitInfo.attackSequenceId, hitInfo.stackItemUID);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Mech), "DamageLocation")]
        public static class Mech_DamageLocation
        {
            [HarmonyPrefix]
            public static void Prefix(Mech __instance, WeaponHitInfo hitInfo, ArmorLocation aLoc, float totalArmorDamage, float directStructureDamage)
            {
                if (!__instance.FakeVehicle()) return;

                var cLoc = MechStructureRules.GetChassisLocationFromArmorLocation(aLoc);
                if (cLoc == ChassisLocations.None || cLoc == ChassisLocations.Arms || cLoc == ChassisLocations.MainBody) return;

                var structureStat = __instance.GetStringForStructureLocation(cLoc);
                if (structureStat == null) return;

                var currentStructure = __instance.GetCurrentStructure(cLoc);
                if (currentStructure <= 0f) return;

                var currentArmor = __instance.GetCurrentArmor(aLoc);
                var incomingStructureDamage = directStructureDamage + totalArmorDamage - currentArmor;

                if (currentStructure - incomingStructureDamage <= 0f)
                {
                    var pilot = __instance.GetPilot();
                    if (pilot != null)
                    {
                        pilot.SetNeedsInjury(InjuryReason.ActorDestroyed);
                        __instance.CheckPilotStatusFromAttack(hitInfo.attackerId, hitInfo.attackSequenceId, hitInfo.stackItemUID);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AmmunitionBox), "DamageComponent")]
        public static class AmmunitionBox_DamageComponent_NoCASE
        {
            [HarmonyPrefix]
            public static bool Prefix(AmmunitionBox __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
            {
                if (applyEffects && damageLevel == ComponentDamageLevel.Destroyed && __instance.componentDef.CanExplode)
                {
                    if (__instance.parent is FakeVehicleMech vehicle)
                    {
                        VehicleDef vehicleDef = vehicle.MechDef?.toVehicleDef(vehicle.MechDef.DataManager);
                        if (vehicleDef != null)
                        {
                            bool hasClanCASE = vehicleDef.VehicleTags.Contains("unit_clan");
                            bool hasISCASE = vehicleDef.VehicleTags.Contains("unit_vehicle_case");
                            if (!hasClanCASE || !hasISCASE)
                            {
                                if (vehicle.Combat.Constants.PilotingConstants.InjuryFromAmmoExplosion)
                                {
                                    Pilot pilot = vehicle.GetPilot();
                                    pilot?.SetNeedsInjury(InjuryReason.AmmoExplosion);
                                    __instance.parent.CheckPilotStatusFromAttack(hitInfo.attackerId, hitInfo.attackSequenceId, hitInfo.stackItemUID);
                                }
                                return true;
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(AmmunitionBox), "DamageComponent")]
        public static class MechComponent_DamageComponent_NoCASE
        {
            [HarmonyPrefix]
            public static bool Prefix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
            {
                if (applyEffects && damageLevel == ComponentDamageLevel.Destroyed && __instance.componentDef.CanExplode && __instance.componentDef.ComponentType != ComponentType.AmmunitionBox)
                {
                    if (__instance.parent is FakeVehicleMech vehicle)
                    {
                        VehicleDef vehicleDef = vehicle.MechDef?.toVehicleDef(vehicle.MechDef.DataManager);
                        if (vehicleDef != null)
                        {
                            bool hasClanCASE = vehicleDef.VehicleTags.Contains("unit_clan");
                            bool hasISCASE = vehicleDef.VehicleTags.Contains("unit_vehicle_case");
                            if (!hasClanCASE || !hasISCASE)
                            {
                                if (vehicle.Combat.Constants.PilotingConstants.InjuryFromAmmoExplosion)
                                {
                                    Pilot pilot = vehicle.GetPilot();
                                    pilot?.SetNeedsInjury(InjuryReason.ComponentExplosion);
                                    __instance.parent.CheckPilotStatusFromAttack(hitInfo.attackerId, hitInfo.attackSequenceId, hitInfo.stackItemUID);
                                }
                                return true;
                            }
                        }
                    }
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
                if (__instance.InjuryReason == InjuryReason.ActorDestroyed && __instance.ParentActor != null)
                {
                    if (__instance.ParentActor is FakeVehicleMech)
                    {
                        __result = "VEHICLE DESTROYED";
                    }
                }
            }
        }
    }
}