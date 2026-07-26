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
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var matcher = new CodeMatcher(instructions)
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Endfinally),
                        new CodeMatch(OpCodes.Ldloc_0),
                        new CodeMatch(OpCodes.Callvirt),
                        new CodeMatch(OpCodes.Call))
                    .ThrowIfInvalid("Failed to find stray target loop end")
                    .Advance(1);

                return matcher
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WeaponStrayHelper_MainStray), "GetPotentialStrayTargets")),
                        new CodeInstruction(OpCodes.Stloc_1))
                    .InstructionEnumeration();
            }

            public static List<ICombatant> GetPotentialStrayTargets(AdvWeaponHitInfo advInfo)
            {
                var combat = advInfo.Combat;
                var weapon = advInfo.weapon;
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