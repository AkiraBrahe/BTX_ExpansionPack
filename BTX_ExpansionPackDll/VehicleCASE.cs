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
                    if (__instance.parent is Mech parentMech && parentMech.FakeVehicle())
                    {
                        Main.Log.LogDebug($"[VehicleCASE] Parent is a fake vehicle: {parentMech.DisplayName} (ID: {parentMech.GUID})");
                        VehicleDef vehicleDef = parentMech.MechDef?.toVehicleDef(parentMech.MechDef.DataManager);
                        if (vehicleDef != null)
                        {
                            bool hasClanCASE = vehicleDef.VehicleTags.Contains("unit_clan");
                            bool hasISCASE = vehicleDef.VehicleTags.Contains("unit_vehicle_case");
                            if (hasClanCASE || hasISCASE)
                            {
                                Main.Log.LogDebug($"[VehicleCASE] CASE detected for {__instance.Name}");
                                float currentAmmo = (float)___statCollection.GetValue<int>("CurrentAmmo");
                                int capacity = __instance.ammunitionBoxDef.Capacity;
                                if (currentAmmo / capacity >= 0.5f)
                                {
                                    float currentRearArmor = parentMech.GetCurrentArmor(ArmorLocation.RightArm);
                                    float totalRearArmor = parentMech.GetMaxArmor(ArmorLocation.RightArm);
                                    float damageToApply = Mathf.Min(currentRearArmor, totalRearArmor / 2f);

                                    if (currentRearArmor > 0)
                                    {
                                        Main.Log.LogDebug($"[VehicleCASE] Ammo box is at least 50% full, applied {damageToApply} rear damage to {parentMech.DisplayName}.");
                                        parentMech.ApplyArmorStatDamage(ArmorLocation.RightArm, damageToApply, hitInfo);
                                    }

                                    string floatieText = $"{__instance.Name} EXPLOSION CASE PROTECTED";
                                    parentMech.Combat.MessageCenter.PublishMessage(new FloatieMessage(parentMech.GUID, parentMech.GUID, floatieText, FloatieMessage.MessageNature.CriticalHit));
                                }
                                else
                                {
                                    Main.Log.LogDebug($"[VehicleCASE] Ammo box is less than 50% full, no rear damage applied to {parentMech.DisplayName}.");
                                    string floatieText = $"{__instance.Name} EXPLOSION CASE PROTECTED";
                                    parentMech.Combat.MessageCenter.PublishMessage(new FloatieMessage(parentMech.GUID, parentMech.GUID, floatieText, FloatieMessage.MessageNature.Neutral));
                                }
                                return false;
                            }
                            else
                            {
                                Main.Log.LogDebug($"[VehicleCASE] No CASE detected for {parentMech.DisplayName}, allowing standard explosion and injury.");
                            }
                        }
                        else
                        {
                            Main.Log.LogWarning($"[VehicleCASE] Could not convert MechDef to VehicleDef for {parentMech.DisplayName}");
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
                    if (__instance.parent is Mech parentMech && parentMech.FakeVehicle())
                    {
                        Main.Log.LogDebug($"[VehicleCASE] Parent is a fake vehicle: {parentMech.DisplayName} (ID: {parentMech.GUID})");
                        VehicleDef vehicleDef = parentMech.MechDef?.toVehicleDef(parentMech.MechDef.DataManager);
                        if (vehicleDef != null)
                        {
                            bool hasClanCASE = vehicleDef.VehicleTags.Contains("unit_clan");
                            bool hasISCASE = vehicleDef.VehicleTags.Contains("unit_vehicle_case");
                            if (hasClanCASE || hasISCASE)
                            {
                                Main.Log.LogDebug($"[VehicleCASE] CASE detected for {__instance.Name}");
                                ArmorLocation rearArmorLocation = ArmorLocation.RightArm;
                                float currentRearArmor = parentMech.GetCurrentArmor(rearArmorLocation);
                                float totalRearArmor = parentMech.GetMaxArmor(rearArmorLocation);
                                float damageToApply = Mathf.Min(currentRearArmor, totalRearArmor / 2f);

                                if (currentRearArmor > 0)
                                {
                                    Main.Log.LogDebug($"[VehicleCASE] Applied {damageToApply} rear damage to {parentMech.DisplayName}.");
                                    parentMech.ApplyArmorStatDamage(ArmorLocation.RightArm, damageToApply, hitInfo);
                                }

                                string floatieText = $"{__instance.Name} EXPLOSION CASE PROTECTED";
                                parentMech.Combat.MessageCenter.PublishMessage(new FloatieMessage(parentMech.GUID, parentMech.GUID, floatieText, FloatieMessage.MessageNature.CriticalHit));
                                return false;
                            }
                            else
                            {
                                Main.Log.LogDebug($"[VehicleCASE] No CASE detected for {parentMech.DisplayName}, allowing standard explosion and injury.");
                            }
                        }
                        else
                        {
                            Main.Log.LogWarning($"[VehicleCASE] Could not convert MechDef to VehicleDef for {parentMech.DisplayName}");
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