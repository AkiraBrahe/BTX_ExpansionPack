using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BattleTech;
using CustAmmoCategories;
using Extended_CE;
using HarmonyLib;

namespace BTX_ExpansionPack
{
    internal class InfernoAmmoTypes
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

        [HarmonyPatch(typeof(AmmunitionBox), "DamageComponent")]
        public static class InfernoExplode
        {
            [HarmonyPrefix]
            [HarmonyBefore("com.github.mcb5637.BTX_CAC_Compatibility")]
            public static bool Prefix(AmmunitionBox __instance, ComponentDamageLevel damageLevel, bool applyEffects, WeaponHitInfo hitInfo)
            {
                if (applyEffects && damageLevel == ComponentDamageLevel.Destroyed && __instance.componentDef.CanExplode && __instance.componentDef.ComponentTags.Contains("component_infernoExplosion"))
                {
                    Main.Log.LogDebug($"[InfernoAmmoTypes] InfernoExplode triggered for {__instance.ammoDef.Description.Id}");
                    if (__instance.parent is Mech mech)
                    {
                        AmmunitionBoxDef ammunitionBoxDef = __instance.componentDef as AmmunitionBoxDef;

                        int heatDamage = (int)ammunitionBoxDef.Ammo.extDef().HeatDamagePerShot;
                        int aoeHeatDamage = (int)ammunitionBoxDef.Ammo.extDef().AOEHeatDamage;
                        int currentAmmo = __instance.StatCollection.GetValue<int>("CurrentAmmo");
                        int totalHeat = (heatDamage + aoeHeatDamage) * currentAmmo / 2;

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

        [HarmonyPatch(typeof(Heatchanges.Mech_OnActivationEnd), "Prefix")]
        public static class InfernoOverheat
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                bool found = false;
                MethodInfo hasInfernoMethod = AccessTools.Method(typeof(InfernoAmmoTypes), nameof(HasInferno), new Type[] { typeof(Mech) });

                for (int i = 0; i < codes.Count; i++)
                {
                    if (!found && i < codes.Count - 5 &&
                        codes[i].opcode.ToString().StartsWith("ldloc") &&
                        codes[i].operand is LocalBuilder listLocal && listLocal.LocalIndex == 52 && listLocal.LocalType == typeof(List<AmmunitionBox>) &&
                        codes[i + 1].opcode == OpCodes.Callvirt && codes[i + 1].operand is MethodInfo getCountMethod && getCountMethod.Name == "get_Count" &&
                        codes[i + 2].opcode == OpCodes.Ldc_I4_0 &&
                        codes[i + 3].opcode == OpCodes.Cgt &&
                        codes[i + 4].opcode.ToString().StartsWith("stloc") &&
                        codes[i + 4].operand is LocalBuilder flag36Local && flag36Local.LocalType == typeof(bool) && flag36Local.LocalIndex == 57 &&
                        codes[i + 5].opcode.ToString().StartsWith("ldloc") && codes[i + 5].operand == flag36Local)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, hasInfernoMethod);
                        found = true;
                        i += 5;
                    }
                    else
                    {
                        yield return codes[i];
                    }
                }

                if (!found)
                {
                    Main.Log.LogWarning("[InfernoAmmoTypes] Could not find the IL sequence to replace for the flag36 check.");
                }

                foreach (var code in codes)
                {
                    yield return code;
                }
            }
        }

        public static bool HasInferno(Mech __instance)
        {
            List<AmmunitionBox> list = new List<AmmunitionBox>();
            foreach (AmmunitionBox ammunitionBox in __instance.ammoBoxes)
            {
                string Id = ammunitionBox.ammoDef.Description.Id;
                if (Id == "Ammunition_SRM_Inferno" || Id == "Ammunition_LRM_Inferno" || Id == "Ammunition_ArrowIV_Inferno")
                {
                    if (ammunitionBox.StatCollection.GetValue<int>("CurrentAmmo") > 0)
                    {
                        list.Add(ammunitionBox);
                    }
                }
            }
            return list.Count > 0;
        }
    }
}