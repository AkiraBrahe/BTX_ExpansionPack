using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using UnityEngine;

namespace BTX_ExpansionPack
{
    internal class VehicleCASE
    {
        [HarmonyPatch(typeof(AmmunitionBox), "DamageComponent")]
        public static class AmmunitionBox_DamageComponent
        {
            [HarmonyPrefix]
            public static bool Prefix(AmmunitionBox __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
            {
                if (!applyEffects || damageLevel != ComponentDamageLevel.Destroyed || !__instance.componentDef.CanExplode)
                    return true;

                if (__instance.parent is FakeVehicleMech vehicle)
                {
                    var vehicleDef = vehicle.MechDef?.toVehicleDef(vehicle.MechDef.DataManager);
                    if (vehicleDef == null)
                        return true;

                    bool hasCASE = HasCASE(vehicleDef);
                    if (hasCASE)
                    {
                        float currentAmmo = __instance.CurrentAmmo;
                        float capacity = __instance.AmmoCapacity;
                        if (capacity > 0 && currentAmmo / capacity >= 0.5f)
                        {
                            Main.Log.LogDebug($"[VehicleCASE] Ammo box is 50% or more full, applying rear damage to {vehicle.DisplayName}.");
                            ApplyCASEProtection(vehicle, __instance.Name, hitInfo, true);
                        }
                        else
                        {
                            Main.Log.LogDebug($"[VehicleCASE] Ammo box is less than 50% full, no rear damage applied to {vehicle.DisplayName}.");
                            ApplyCASEProtection(vehicle, __instance.Name, hitInfo, false);
                        }
                        return false;
                    }
                    else
                    {
                        if (vehicle.Combat.Constants.PilotingConstants.InjuryFromAmmoExplosion)
                        {
                            Pilot pilot = vehicle.GetPilot();
                            pilot?.SetNeedsInjury(InjuryReason.AmmoExplosion);
                            vehicle.CheckPilotStatusFromAttack(hitInfo.attackerId, hitInfo.attackSequenceId, hitInfo.stackItemUID);
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
        public static class MechComponent_DamageComponent
        {
            [HarmonyPrefix]
            public static bool Prefix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
            {
                if (!applyEffects || damageLevel != ComponentDamageLevel.Destroyed || !__instance.componentDef.CanExplode || __instance.componentDef.ComponentType == ComponentType.AmmunitionBox)
                    return true;

                if (__instance.parent is FakeVehicleMech vehicle)
                {
                    var vehicleDef = vehicle.MechDef?.toVehicleDef(vehicle.MechDef.DataManager);
                    if (vehicleDef == null)
                        return true;

                    bool hasCASE = HasCASE(vehicleDef);
                    if (hasCASE)
                    {
                        Main.Log.LogDebug($"[VehicleCASE] CASE detected for {__instance.Name}");
                        ApplyCASEProtection(vehicle, __instance.Name, hitInfo, true);
                        return false;
                    }
                    else
                    {
                        if (vehicle.Combat.Constants.PilotingConstants.InjuryFromAmmoExplosion)
                        {
                            Pilot pilot = vehicle.GetPilot();
                            pilot?.SetNeedsInjury(InjuryReason.ComponentExplosion);
                            vehicle.CheckPilotStatusFromAttack(hitInfo.attackerId, hitInfo.attackSequenceId, hitInfo.stackItemUID);
                        }
                    }
                }
                return true;
            }
        }

        private static bool HasCASE(VehicleDef vehicleDef) =>
            vehicleDef.VehicleTags.Contains("unit_clan") ||
            vehicleDef.VehicleTags.Contains("unit_vehicle_case");

        private static void ApplyCASEProtection(FakeVehicleMech vehicle, string componentName, WeaponHitInfo hitInfo, bool applyDamage)
        {
            ArmorLocation rearArmorLocation = VehicleChassisLocations.Rear.toFakeArmor();
            float currentRearArmor = vehicle.GetCurrentArmor(rearArmorLocation);
            float totalRearArmor = vehicle.GetMaxArmor(rearArmorLocation);
            float damageToApply = Mathf.Min(currentRearArmor, totalRearArmor / 2f);

            if (applyDamage && currentRearArmor > 0)
            {
                Main.Log.LogDebug($"[VehicleCASE] Applied {damageToApply} rear damage to {vehicle.DisplayName}.");
                vehicle.ApplyArmorStatDamage(rearArmorLocation, damageToApply, hitInfo);
            }

            string floatieText = $"{componentName} EXPLOSION CASE PROTECTED";
            var nature = applyDamage ? FloatieMessage.MessageNature.CriticalHit : FloatieMessage.MessageNature.Neutral;
            vehicle.Combat.MessageCenter.PublishMessage(new FloatieMessage(vehicle.GUID, vehicle.GUID, floatieText, nature));
        }
    }
}