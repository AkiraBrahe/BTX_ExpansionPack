using System;
using System.Collections.Generic;
using System.Reflection;
using BattleTech;
using CustAmmoCategories;
using CustomActivatableEquipment;
using HarmonyLib;
using Localize;

namespace BTX_ExpansionPack.Fixes
{
    internal class AMSAuras
    {
        [HarmonyPatch]
        public static class AuraBubble_GetRadius
        {
            public static MethodInfo TargetMethod()
            {
                return typeof(AuraBubble).GetMethod("GetRadius", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            [HarmonyPrefix]
            public static bool Replace(AuraBubble __instance, ref float __result)
            {
                if (__instance?.Def == null) { __result = 0f; return false; }
                if (__instance.owner == null) { __result = __instance.Def.Range; return false; }

                if (__instance.source is Weapon weapon)
                {
                    if (__instance.Def.Name?.Contains("AMS") == true && __instance.source != null)
                    {
                        __result = weapon.isAMS() ? __instance.Def.Range : 0.1f;
                        return false;
                    }
                }

                if (string.IsNullOrEmpty(__instance.Def.RangeStatistic))
                {
                    __result = __instance.Def.Range;
                    return false;
                }
                __result = __instance.owner.StatCollection.GetStatistic(__instance.Def.RangeStatistic).Value<float>();
                return false;
            }
        }

        private static readonly HashSet<string> protectedAllies = new HashSet<string>();

        [HarmonyPatch(typeof(AuraActorBody), "ShowAddFloatie", new Type[] { typeof(AuraBubble), typeof(bool) })]
        public static class AuraActorBody_ShowAddFloatie
        {
            [HarmonyPrefix]
            public static bool Prefix(AuraBubble aura, bool isAlly, AuraActorBody __instance)
            {
                if (isAlly && (aura?.Def) != null && aura.Def.IsPositiveToAlly && (__instance?.owner?.Combat?.MessageCenter) != null)
                {
                    if (aura.Def.Name?.Contains("AMS") == true)
                    {
                        if (!protectedAllies.Contains(__instance.owner.GUID))
                        {
                            protectedAllies.Add(__instance.owner.GUID);
                            __instance.owner.Combat.MessageCenter.PublishMessage(
                                new FloatieMessage(
                                    aura.owner.GUID,
                                    __instance.owner.GUID,
                                    new Text("{0} PROTECTED", new object[] { aura.Def.Name }),
                                    FloatieMessage.MessageNature.Buff
                                )
                            );
                        }
                        return false;
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(AbstractActor), "OnActivationEnd")]
        public static class AbstractActor_OnActivationEnd
        {
            [HarmonyPostfix]
            public static void Postfix(AbstractActor __instance)
            {
                if (__instance?.Combat != null)
                {
                    protectedAllies.Remove(__instance.GUID);
                }
            }
        }
    }
}