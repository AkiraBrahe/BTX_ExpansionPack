using BattleTech;
using CustAmmoCategories;
using Extended_CE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes
{
    internal class InfernoAmmo
    {
        /// <summary>
        /// Applies the correct bonus damage with inferno ammo.
        /// </summary>
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

        /// <summary>
        /// Accounts for AoE heat damage when an inferno ammo box is destroyed.
        /// </summary>
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

        /// <summary>
        /// Checks for all inferno ammo types when a mech overheats with inferno ammo.
        /// </summary>
        [HarmonyPatch(typeof(Heatchanges.Mech_OnActivationEnd), "Prefix")]
        public static class InfernoOverheat
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var hasInfernoMethod = AccessTools.Method(typeof(InfernoOverheat), nameof(HasInferno), [typeof(Mech)]);
                bool found = false;

                for (int i = 0; i < codes.Count; i++)
                {
                    if (!found && i < codes.Count - 3 &&
                        codes[i].opcode.ToString().StartsWith("ldloc") &&
                        codes[i + 1].opcode == OpCodes.Callvirt &&
                        codes[i + 1].operand is MethodInfo mi && mi.Name == "get_Count" &&
                        codes[i + 2].opcode == OpCodes.Ldc_I4_0 &&
                        codes[i + 3].opcode == OpCodes.Cgt)
                    {
                        var labels = codes[i].labels;
                        var ldarg0 = new CodeInstruction(OpCodes.Ldarg_0);
                        ldarg0.labels.AddRange(labels);
                        yield return ldarg0;
                        yield return new CodeInstruction(OpCodes.Call, hasInfernoMethod);
                        found = true;
                        i += 3;
                        continue;
                    }
                    yield return codes[i];
                }

                if (!found)
                {
                    Main.Log.LogWarning("Could not find the IL sequence to replace for inferno ammo check.");
                }
            }

            public static bool HasInferno(Mech __instance) =>
                __instance.ammoBoxes.Any(ammoBox => ammoBox.defId.EndsWith("Inferno") && ammoBox.CurrentAmmo > 0);
        }
    }
}