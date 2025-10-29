using BattleTech;
using CustAmmoCategories;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes.Targeting
{
    /// <summary>
    /// Fixes stray targeting for improved swarm ammunition to properly exclude allied units.
    /// </summary>
    internal class SwarmTargeting
    {
        /// <summary>
        /// Fixes stray targeting for improved swarm ammunition to properly exclude allied units.
        /// </summary>
        [HarmonyPatch(typeof(WeaponStrayHelper), "MainStray")]
        public static class WeaponStrayHelper_MainStray
        {
            [HarmonyTranspiler]
            [HarmonyEmitIL]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions)
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldloc_0),
                        new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(AdvWeaponHitInfo), "weapon")),
                        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(CustomAmmoCategories), "StrayRange")));

                object jumpTarget = matcher.Operand;
                return matcher
                    .MatchStartBackwards(
                        new CodeMatch(OpCodes.Newobj, AccessTools.Constructor(typeof(List<ICombatant>))),
                        new CodeMatch(OpCodes.Stloc_1))
                    .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
                    .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SwarmTargeting), "GetPotentialStrayTargets")))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_1), new CodeInstruction(OpCodes.Br, jumpTarget))
                    .InstructionEnumeration();
            }

            public static List<ICombatant> GetPotentialStrayTargets(AdvWeaponHitInfo advWeaponHitInfo) => NotImplementedException();

            private static List<ICombatant> NotImplementedException() => throw new NotImplementedException();
        }
    }
}
