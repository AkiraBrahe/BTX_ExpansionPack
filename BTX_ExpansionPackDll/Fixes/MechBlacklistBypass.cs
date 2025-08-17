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
            bool found = false;
            var code = new List<CodeInstruction>(instructions);
            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldloc_1 &&
                    (code[i + 1].opcode == OpCodes.Brfalse || code[i + 1].opcode == OpCodes.Brfalse_S))
                {
                    var newInstruction = new CodeInstruction(OpCodes.Ldc_I4_0);
                    newInstruction.labels.AddRange(code[i].labels);
                    code[i] = newInstruction;

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Main.Log.LogError("[MechBlacklistBypass] Could not find the IL sequence to replace for the flag check.");
            }

            return code;
        }
    }
}
