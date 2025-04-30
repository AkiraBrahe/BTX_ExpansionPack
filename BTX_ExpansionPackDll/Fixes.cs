using System;
using System.Collections.Generic;
using System.Reflection;
using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using CustomActivatableEquipment;
using CustomUnits;
using HarmonyLib;
using Localize;

namespace BTX_ExpansionPack
{
    internal class AMSAuraFix
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
                    if (__instance.Def.Id == "AMS" || __instance.Def.Id == "LAMS" || __instance.Def.Id == "RFAMS")
                    {
                        if (weapon.isAMS())
                        {
                            __result = __instance.Def.Range;
                        }
                        else
                        {
                            __result = 0.1f;
                        }
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
    }
    }
}