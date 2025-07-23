using BattleTech;
using CustAmmoCategories;
using Extended_CE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BTX_ExpansionPack
{
    internal class InfernoAmmoTypes
    {
        private static readonly HashSet<string> InfernoAmmoIds =
        [
            "Ammunition_SRM_Inferno",
            "Ammunition_LRM_Inferno",
            "Ammunition_ArrowIV_Inferno"
        ];

        [HarmonyPatch(typeof(BTX_CAC_CompatibilityDll.RandomPatches), "GetFlexDamage")]
        public static class GetFlexDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Weapon w, ref float __result)
            {
                var weapon = w;
                ExtAmmunitionDef ammo = weapon.ammo();
                if (ammo.Id == "Ammunition_SRM_Inferno")
                {
                    float bonusDamage = weapon.weaponDef.Damage - 10f;
                    __result = Math.Max(bonusDamage, 0f);
                }
                else if (ammo.Id == "Ammunition_LRM_Inferno")
                {
                    float bonusDamage = weapon.weaponDef.Damage - 5f;
                    __result = Math.Max(bonusDamage, 0f);
                }
            }
        }

        [HarmonyPatch(typeof(AmmunitionBox), "DamageComponent")]
        public static class InfernoExplode
        {
            [HarmonyPrefix]
            [HarmonyWrapSafe]
            [HarmonyBefore("com.github.mcb5637.BTX_CAC_Compatibility")]
            public static bool Prefix(AmmunitionBox __instance, ComponentDamageLevel damageLevel, bool applyEffects, WeaponHitInfo hitInfo)
            {
                if (applyEffects && damageLevel == ComponentDamageLevel.Destroyed &&
                    __instance.componentDef.CanExplode &&
                    __instance.componentDef.ComponentTags?.Contains("component_infernoExplosion") == true)
                {
                    if (__instance.parent is Mech mech && __instance.componentDef is AmmunitionBoxDef ammunitionBoxDef)
                    {
                        var extDef = ammunitionBoxDef.Ammo?.extDef();
                        if (extDef == null) return false;

                        int heatDamage = (int)extDef.HeatDamagePerShot;
                        int aoeHeatDamage = (int)extDef.AOEHeatDamage;
                        int currentAmmo = __instance.StatCollection.GetValue<int>("CurrentAmmo");
                        int totalHeat = (heatDamage + aoeHeatDamage) * currentAmmo / 2;

                        mech.AddExternalHeat("inferno explosion", totalHeat);

                        foreach (var effectData in extDef.statusEffects?.Where(e => e.effectType == EffectType.StatisticEffect) ?? [])
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
                MethodInfo hasInfernoMethod = AccessTools.Method(typeof(InfernoAmmoTypes), nameof(HasInferno), [typeof(Mech)]);

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
                    Main.Log.LogError("[InfernoAmmoTypes] Could not find the IL sequence to replace for the flag36 check.");
                }

                foreach (var code in codes)
                {
                    yield return code;
                }
            }
        }

        public static bool HasInferno(Mech __instance) =>
            __instance.ammoBoxes.Any(ammoBox =>
                InfernoAmmoIds.Contains(ammoBox.defId) &&
                ammoBox.StatCollection.GetValue<int>("CurrentAmmo") > 0);
    }
}