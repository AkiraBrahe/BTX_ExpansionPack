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
        /// Applies the correct bonus damage with LRM inferno ammo.
        /// </summary>
        [HarmonyPatch(typeof(BTX_CAC_CompatibilityDll.RandomPatches), "GetFlexDamage")]
        [Obsolete("Move to CAC-C when possible")]
        public static class GetFlexDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Weapon w, ref float __result)
            {
                var weapon = w;
                var ammo = weapon.ammo();
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
        [Obsolete("Move to CAC-C when possible")]
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
                        int currentAmmo = __instance.CurrentAmmo;
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
        [Obsolete("Move to CAC-C when possible")]
        public static class InfernoOverheat
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode.ToString().StartsWith("ldloc")),
                        new CodeMatch(i => i.opcode == OpCodes.Callvirt && i.operand is MethodInfo mi && mi.Name == "get_Count"),
                        new CodeMatch(OpCodes.Ldc_I4_0),
                        new CodeMatch(OpCodes.Cgt))
                    .ThrowIfInvalid("Failed to find inferno ammo check")
                    .SetOpcodeAndAdvance(OpCodes.Ldarg_0)
                    .SetOperandAndAdvance(AccessTools.Method(typeof(InfernoOverheat), nameof(HasInferno)))
                    .InstructionEnumeration();
            }

            public static bool HasInferno(Mech mech) =>
                mech.ammoBoxes.Any(ammoBox => ammoBox.defId.EndsWith("Inferno") && ammoBox.CurrentAmmo > 0);
        }
    }
}