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
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_1 &&
                    (codes[i + 1].opcode == OpCodes.Brfalse || codes[i + 1].opcode == OpCodes.Brfalse_S))
                {
                    var newInstruction = new CodeInstruction(OpCodes.Ldc_I4_0);
                    newInstruction.labels.AddRange(codes[i].labels);
                    codes[i] = newInstruction;

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Main.Log.LogWarning("Could not find the IL sequence to replace for mech blacklist bypass");
            }

            return codes;
        }
    }
}
