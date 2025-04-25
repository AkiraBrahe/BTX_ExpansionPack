using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustAmmoCategories;
using HarmonyLib;
using Localize;
using UnityEngine;

namespace BTX_ExpansionPack
{
    internal class InfernoAmmoPatches
    {
        [HarmonyPatch(typeof(BTX_CAC_CompatibilityDll.RandomPatches), "GetFlexDamage")]
        public static class GetFlexDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Weapon w, ref float __result)
            {
                ExtAmmunitionDef ammunitionDef = w.ammo();
                if (ammunitionDef.Id == "Ammunition_SRM_Inferno")
                {
                    float bonusDamage = w.weaponDef.Damage - 10.0f;
                    if (bonusDamage > 0)
                    {
                        __result = bonusDamage;
                    }
                }
                else if (ammunitionDef.Id == "Ammunition_LRM_Inferno")
                {
                    float bonusDamage = w.weaponDef.Damage - 5.0f;
                    if (bonusDamage > 0)
                    {
                        __result = bonusDamage;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BattleTech.AmmunitionBox), "DamageComponent")]
        public static class InfernoExplode
        {
            [HarmonyPrefix]
            [HarmonyBefore("com.github.mcb5637.BTX_CAC_Compatibility")]
            public static bool Prefix(AmmunitionBox __instance, ComponentDamageLevel damageLevel, bool applyEffects, WeaponHitInfo hitInfo)
            {
                if (applyEffects && damageLevel == ComponentDamageLevel.Destroyed && __instance.componentDef.CanExplode && __instance.componentDef.ComponentTags.Contains("component_infernoExplosion"))
                {
                    Main.Log.LogDebug($"[InfernoAmmoPatches] InfernoExplode triggered for {__instance.ammoDef.Description.Id}");
                    if (__instance.parent is Mech mech)
                    {
                        AmmunitionBoxDef ammunitionBoxDef = __instance.componentDef as AmmunitionBoxDef;

                        int heatPerShot = (int)ammunitionBoxDef.Ammo.extDef().HeatDamagePerShot;
                        int aoeHeatDamage = (int)ammunitionBoxDef.Ammo.extDef().AOEHeatDamage;
                        int currentAmmo = __instance.StatCollection.GetValue<int>("CurrentAmmo");
                        int totalHeat = (heatPerShot + aoeHeatDamage) * currentAmmo / 2;

                        mech.AddExternalHeat("inferno explosion", totalHeat);

                        foreach (EffectData effectData in ammunitionBoxDef.Ammo.extDef().statusEffects.Where((effectData) => effectData.effectType == EffectType.StatisticEffect))
                        {
                            mech.Combat.EffectManager.CreateEffect(effectData, effectData.Description.Id, hitInfo.attackSequenceId, mech, mech, default, -1, false);
                        }

                        mech.Combat.AttackDirector.GetAttackSequence(hitInfo.attackSequenceId)?.FlagAttackDidHeatDamage(mech.GUID);
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Mech), "OnActivationEnd")]
        public static class Mech_OnActivationEnd
        {
            [HarmonyPrefix]
            [HarmonyBefore("BEX.BattleTech.Extended_CE")]
            public static void Prefix(Mech __instance, int stackItemID)
            {
                if (__instance.IsOverheated)
                Main.Log.LogDebug($"[InfernoAmmoPatches] Mech_OnActivationEnd triggered for {__instance.Description.UIName}");
                {
                    bool hasInfernoAmmo = __instance.ammoBoxes.Any(box =>
                        (box.ammoDef.Description.Id == "Ammunition_SRM_Inferno" || box.ammoDef.Description.Id == "Ammunition_LRM_Inferno" || box.ammoDef.Description.Id == "Ammunition_ArrowIV_Inferno") &&
                        box.StatCollection.GetValue<int>("CurrentAmmo") > 0
                    );

                    if (hasInfernoAmmo)
                    {
                        MultiSequence multiSequence = new MultiSequence(__instance.Combat);
                        multiSequence.SetCamera(CameraControl.Instance.ShowDeathCam(__instance, false, -1f), 0);

                        float overheatLevel = __instance.CurrentHeat - __instance.OverheatLevel;
                        float explosionChance = overheatLevel * 2f;
                        float roll = __instance.Combat.NetworkRandom.Float(0f, 100f);

                        multiSequence.AddChildSequence(new ShowActorInfoSequence(__instance, $"Inferno Ammo Explosion Chance: {(int)explosionChance}%", FloatieMessage.MessageNature.Buff, true), multiSequence.ChildSequenceCount - 1);

                        if (roll < explosionChance)
                        {
                            List<AmmunitionBox> infernoAmmoBoxes = __instance.ammoBoxes.Where(box =>
                                box.ammoDef.Description.Id == "Ammunition_SRM_Inferno" || box.ammoDef.Description.Id == "Ammunition_LRM_Inferno" || box.ammoDef.Description.Id == "Ammunition_ArrowIV_Inferno"
                            ).ToList();

                            if (infernoAmmoBoxes.Count > 0)
                            {
                                infernoAmmoBoxes.Shuffle<AmmunitionBox>();
                                AmmunitionBox ammoBoxToExplode = infernoAmmoBoxes[0];

                                if (__instance.GameRep != null)
                                {
                                    WwiseManager.PostEvent<AudioEventList_explosion>(AudioEventList_explosion.explosion_small, __instance.GameRep.audioObject, null, null);
                                    __instance.GameRep.PlayVFX(ammoBoxToExplode.Location, __instance.Combat.Constants.VFXNames.componentDestruction_AmmoExplosion, true, Vector3.zero, true, -1f);
                                    __instance.Combat.MessageCenter.PublishMessage(new AddSequenceToStackMessage(new ShowActorInfoSequence(__instance, new Text("{0} DESTROYED", new object[] { ammoBoxToExplode.UIName }), FloatieMessage.MessageNature.ComponentDestroyed, true)));
                                    WeaponHitInfo weaponHitInfo = new WeaponHitInfo(stackItemID, -1, -1, -1, "", "", -1, null, null, null, null, null, null, null, new AttackDirection[] { AttackDirection.FromFront }, null, null, null);
                                    ammoBoxToExplode.DamageComponent(weaponHitInfo, ComponentDamageLevel.Destroyed, true);
                                }
                            }
                        }
                        else
                        {
                            multiSequence.AddChildSequence(new ShowActorInfoSequence(__instance, "Inferno Ammo Explosion Averted!", FloatieMessage.MessageNature.Buff, true), multiSequence.ChildSequenceCount - 1);
                        }

                        multiSequence.AddChildSequence(new DelaySequence(__instance.Combat, 2f), multiSequence.ChildSequenceCount - 1);
                        __instance.Combat.MessageCenter.PublishMessage(new AddSequenceToStackMessage(multiSequence));
                    }
                }
            }
        }
    }
}