using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes
{
    [HarmonyPatch]
    public static class MechBlacklistBypass
    {
        [HarmonyPrepare]
        public static bool Prepare() => AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.GetName().Name.Equals("FellOffACargoShip"));

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("FellOffACargoShip.Cheater.Mech");
            var method = AccessTools.Method(type, "<Add>g__AddMech|0_0");
            return method;
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldloc_1),
                    new CodeMatch(inst => inst.opcode == OpCodes.Brfalse || inst.opcode == OpCodes.Brfalse_S)
                )
                .ThrowIfInvalid("Failed to bypass blacklist")
                .SetOpcodeAndAdvance(OpCodes.Ldc_I4_0)
                .InstructionEnumeration();
        }
    }
}
