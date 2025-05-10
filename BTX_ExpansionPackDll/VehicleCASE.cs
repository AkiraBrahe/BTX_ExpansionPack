using BattleTech;
using CustAmmoCategories;
using CustomUnits;
using HarmonyLib;
using UnityEngine;

namespace BTX_ExpansionPack
{
    internal class VehicleCASE
    {
        [HarmonyPatch(typeof(AmmunitionBox), "DamageComponent")]
        public static class AmmunitionBox_DamageComponent_CASE
        {
            [HarmonyPrefix]
            public static bool Prefix(AmmunitionBox __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects, StatCollection ___statCollection)
            {
                if (applyEffects && damageLevel == ComponentDamageLevel.Destroyed && __instance.componentDef.CanExplode)
                {
                    if (__instance.parent is FakeVehicleMech vehicle)
                    {
                        Main.Log.LogDebug($"[VehicleCASE] Parent is a fake vehicle: {vehicle.DisplayName}");
                        VehicleDef vehicleDef = vehicle.MechDef?.toVehicleDef(vehicle.MechDef.DataManager);
                        if (vehicleDef != null)
                        {
                            bool hasClanCASE = vehicleDef.VehicleTags.Contains("unit_clan");
                            bool hasISCASE = vehicleDef.VehicleTags.Contains("unit_vehicle_case");
                            if (hasClanCASE || hasISCASE)
                            {
                                Main.Log.LogDebug($"[VehicleCASE] CASE detected for {__instance.Name}");
                                float currentAmmo = __instance.CurrentAmmo;
                                int capacity = __instance.AmmoCapacity;
                                if (currentAmmo / capacity >= 0.5f)
                                {
                                    ArmorLocation rearArmorLocation = VehicleChassisLocations.Rear.toFakeArmor();
                                    float currentRearArmor = vehicle.GetCurrentArmor(rearArmorLocation);
                                    float totalRearArmor = vehicle.GetMaxArmor(rearArmorLocation);
                                    float damageToApply = Mathf.Min(currentRearArmor, totalRearArmor / 2f);

                                    if (currentRearArmor > 0)
                                    {
                                        Main.Log.LogDebug($"[VehicleCASE] Ammo box is at least 50% full, applied {damageToApply} rear damage to {vehicle.DisplayName}.");
                                        vehicle.ApplyArmorStatDamage(rearArmorLocation, damageToApply, hitInfo);
                                    }

                                    string floatieText = $"{__instance.Name} EXPLOSION CASE PROTECTED";
                                    vehicle.Combat.MessageCenter.PublishMessage(new FloatieMessage(vehicle.GUID, vehicle.GUID, floatieText, FloatieMessage.MessageNature.CriticalHit));
                                }
                                else
                                {
                                    Main.Log.LogDebug($"[VehicleCASE] Ammo box is less than 50% full, no rear damage applied to {vehicle.DisplayName}.");
                                    string floatieText = $"{__instance.Name} EXPLOSION CASE PROTECTED";
                                    vehicle.Combat.MessageCenter.PublishMessage(new FloatieMessage(vehicle.GUID, vehicle.GUID, floatieText, FloatieMessage.MessageNature.Neutral));
                                }
                                return false;
                            }
                            else
                            {
                                Main.Log.LogDebug($"[VehicleCASE] No CASE detected for {vehicle.DisplayName}, allowing standard explosion and injury.");
                            }
                        }
                        else
                        {
                            Main.Log.LogWarning($"[VehicleCASE] Could not convert MechDef to VehicleDef for {vehicle.DisplayName}");
                        }
                    }
                    else
                    {
                        Main.Log.LogDebug($"[VehicleCASE] Parent is not a fake vehicle: {__instance.parent?.GetType().Name}");
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
        public static class MechComponent_DamageComponent_CASE
        {
            [HarmonyPrefix]
            public static bool Prefix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
            {
                if (applyEffects && damageLevel == ComponentDamageLevel.Destroyed && __instance.componentDef.CanExplode && __instance.componentDef.ComponentType != ComponentType.AmmunitionBox)
                {
                    if (__instance.parent is FakeVehicleMech vehicle)
                    {
                        Main.Log.LogDebug($"[VehicleCASE] Parent is a fake vehicle: {vehicle.DisplayName}");
                        VehicleDef vehicleDef = vehicle.MechDef?.toVehicleDef(vehicle.MechDef.DataManager);
                        if (vehicleDef != null)
                        {
                            bool hasClanCASE = vehicleDef.VehicleTags.Contains("unit_clan");
                            bool hasISCASE = vehicleDef.VehicleTags.Contains("unit_vehicle_case");
                            if (hasClanCASE || hasISCASE)
                            {
                                Main.Log.LogDebug($"[VehicleCASE] CASE detected for {__instance.Name}");
                                ArmorLocation rearArmorLocation = VehicleChassisLocations.Rear.toFakeArmor();
                                float currentRearArmor = vehicle.GetCurrentArmor(rearArmorLocation);
                                float totalRearArmor = vehicle.GetMaxArmor(rearArmorLocation);
                                float damageToApply = Mathf.Min(currentRearArmor, totalRearArmor / 2f);

                                if (currentRearArmor > 0)
                                {
                                    Main.Log.LogDebug($"[VehicleCASE] Applied {damageToApply} rear damage to {vehicle.DisplayName}.");
                                    vehicle.ApplyArmorStatDamage(rearArmorLocation, damageToApply, hitInfo);
                                }

                                string floatieText = $"{__instance.Name} EXPLOSION CASE PROTECTED";
                                vehicle.Combat.MessageCenter.PublishMessage(new FloatieMessage(vehicle.GUID, vehicle.GUID, floatieText, FloatieMessage.MessageNature.CriticalHit));
                                return false;
                            }
                            else
                            {
                                Main.Log.LogDebug($"[VehicleCASE] No CASE detected for {vehicle.DisplayName}, allowing standard explosion and injury.");
                            }
                        }
                        else
                        {
                            Main.Log.LogWarning($"[VehicleCASE] Could not convert MechDef to VehicleDef for {vehicle.DisplayName}");
                        }
                    }
                    else
                    {
                        Main.Log.LogDebug($"[VehicleCASE] Parent is not a fake vehicle: {__instance.parent?.GetType().Name}");
                    }
                }
                return true;
            }
        }
    }
}