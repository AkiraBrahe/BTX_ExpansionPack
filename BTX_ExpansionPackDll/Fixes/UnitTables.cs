using FullXotlTables;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes
{
    internal class UnitTables
    {
        /// <summary>
        /// Migrates the unit tables to the "XotlTablesV2" folder.
        /// </summary>
        [HarmonyPatch(typeof(GenerateTables), "GenerateFromFiles")]
        public static class GenerateTables_GenerateFromFiles
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Ldstr, "XotlTables"))
                    .SetOperandAndAdvance("XotlTablesV2")
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Skips the logic that doubles the rarity of certain vehicles.
        /// </summary>
        [HarmonyPatch(typeof(XotlTable), "RequestUnit")]
        internal class XotlTables
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions, il)
                    .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "vehicledef_APC_Maxim_3052AP"))
                    .MatchBack(false, new CodeMatch(i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S));

                var jumpTarget = matcher.Operand;
                return matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Br, jumpTarget))
                    .InstructionEnumeration();
            }
        }
    }
}
