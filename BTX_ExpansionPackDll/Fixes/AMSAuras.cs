using BattleTech;
using CustAmmoCategories;
using CustomActivatableEquipment;
using Localize;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes
{
    internal class AMSAuras
    {
        /// <summary>
        /// Shows the correct AMS range when using AMS.
        /// </summary>
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
                    .GetStatistic(__instance.Def.RangeStatistic).Value<float>();
                return false;
            }
        }

        [HarmonyPatch(typeof(CustomAmmoCategories), "CalcAMSAIDamageCoeff")]
        public static class CalcAMSAIDamageCoeff
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(true,
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Weapon), "get_MaxRange")))
                    .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CalcAMSAIDamageCoeff), nameof(GetAMSRange))))
                    .InstructionEnumeration();
            }

            public static float GetAMSRange(Weapon weapon)
            {
                AuraDef amsAura = weapon.weaponDef.GetAuras().FirstOrDefault(a => a.Name == "AMS");
                return amsAura?.Range > 0 ? amsAura.Range : weapon.MaxRange;
            }
        }

        /// <summary>
        /// Limits AMS protection floaties to one per ally per turn.
        /// </summary>
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
    }
}