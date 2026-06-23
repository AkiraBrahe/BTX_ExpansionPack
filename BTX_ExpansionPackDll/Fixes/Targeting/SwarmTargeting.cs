using BattleTech;
using CustAmmoCategories;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes.Targeting
{
    internal class SwarmTargeting
    {
        /// <summary>
        /// Fixes stray targeting for improved swarm ammunition to properly exclude allied units.
        /// </summary>
        [HarmonyPatch(typeof(WeaponStrayHelper), "MainStray")]
        public static class WeaponStrayHelper_MainStray
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions)
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldloc_0),
                        new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(AdvWeaponHitInfo), "weapon")),
                        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(CustomAmmoCategories), "StrayRange")));

                var jumpTarget = il.DefineLabel();
                matcher.AddLabels([jumpTarget]);

                return matcher
                    .MatchStartBackwards(
                        new CodeMatch(OpCodes.Newobj, AccessTools.Constructor(typeof(List<ICombatant>))),
                        new CodeMatch(OpCodes.Stloc_1))
                    .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
                    .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WeaponStrayHelper_MainStray), "GetPotentialStrayTargets")))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_1), new CodeInstruction(OpCodes.Br, jumpTarget))
                    .InstructionEnumeration();
            }

            public static List<ICombatant> GetPotentialStrayTargets(AdvWeaponHitInfo advWeaponHitInfo)
            {
                var combat = advWeaponHitInfo.Combat;
                var weapon = advWeaponHitInfo.weapon;
                var attacker = weapon.parent;

                List<ICombatant> potentialTargets = [];
                string iffTransponderDef = weapon.IFFTransponderDef();
                if (string.IsNullOrEmpty(iffTransponderDef))
                {
                    // Standard swarm: all units except attacker
                    var allCombatants = combat.GetAllCombatants();
                    potentialTargets.AddRange(allCombatants.Where(c => c.GUID != attacker.GUID));
                }
                else
                {
                    // Improved swarm: only enemies
                    var allEnemies = combat.GetAllEnemiesOf(attacker);
                    potentialTargets.AddRange(allEnemies);
                }

                return potentialTargets;
            }
        }
    }
}