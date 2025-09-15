using BattleTech;
using CustAmmoCategories;
using CustomActivatableEquipment;
using Localize;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes
{
    internal class AMSAuras
    {
        [HarmonyPatch(typeof(AuraBubble), nameof(AuraBubble.GetRadius), MethodType.Normal)]
        public static class AuraBubble_GetRadius
        {
            [HarmonyPrefix]
            public static bool Prefix(AuraBubble __instance, ref float __result)
            {
                if (__instance?.Def == null) { __result = 0f; return false; }
                if (__instance.owner == null) { __result = __instance.Def.Range; return false; }

                if (__instance.source is Weapon weapon)
                {
                    if (!string.IsNullOrEmpty(__instance.Def.Name) &&
                        __instance.Def.Name.Contains("AMS"))
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

                __result = __instance.owner.StatCollection
                    .GetStatistic(__instance.Def.RangeStatistic)
                    .Value<float>();
                return false;
            }
        }

        private static readonly HashSet<string> protectedAllies = [];

        [HarmonyPatch(typeof(AuraActorBody), "ShowAddFloatie", [typeof(AuraBubble), typeof(bool)])]
        public static class AuraActorBody_ShowAddFloatie
        {
            [HarmonyPrefix]
            public static bool Prefix(AuraBubble aura, bool isAlly, AuraActorBody __instance)
            {
                if (!isAlly || aura?.Def == null || !aura.Def.IsPositiveToAlly ||
                    __instance?.owner?.Combat?.MessageCenter == null)
                {
                    return true;
                }

                if (!string.IsNullOrEmpty(aura.Def.Name) &&
                    aura.Def.Name.Contains("AMS"))
                {
                    if (protectedAllies.Add(__instance.owner.GUID))
                    {
                        __instance.owner.Combat.MessageCenter.PublishMessage(
                            new FloatieMessage(
                                aura.owner.GUID,
                                __instance.owner.GUID,
                                new Text("{0} PROTECTED", [aura.Def.Name]),
                                FloatieMessage.MessageNature.Buff
                            )
                        );
                    }
                    return false;
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

        [HarmonyPatch(typeof(CustomAmmoCategories), "CalcAMSAIDamageCoeff")]
        public static class CustomAmmoCategories_CalcAMSAIDamageCoeff
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool found = false;
                var codes = new List<CodeInstruction>(instructions);
                var targetMethod = AccessTools.Method(typeof(Weapon), "get_MaxRange");
                var replacementMethod = AccessTools.Method(typeof(CustomAmmoCategories_CalcAMSAIDamageCoeff), nameof(GetAMSRange));

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == targetMethod)
                    {
                        if (i > 0 && codes[i - 1].opcode == OpCodes.Ldarg_0)
                        {
                            codes[i] = new CodeInstruction(OpCodes.Call, replacementMethod);

                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    Main.Log.LogWarning("Could not find the IL sequence to replace for AMS range calculation.");
                }

                return codes.AsEnumerable();
            }

            public static float GetAMSRange(Weapon weapon)
            {
                AuraDef amsAura = weapon.weaponDef.GetAuras().FirstOrDefault(a => a.Name == "AMS");
                return amsAura?.Range > 0 ? amsAura.Range : weapon.MaxRange;
            }
        }
    }
}