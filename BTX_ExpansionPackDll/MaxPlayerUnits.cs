using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BattleTech.Framework;
using HarmonyLib;

namespace BTX_ExpansionPack
{
    internal class MaxPlayerUnits
    {
        [HarmonyPatch]
        public static class ContractOverride_PatchContract
        {
            public static MethodInfo TargetMethod()
            {
                Type internalClassType = Type.GetType("BTX_CAC_CompatibilityDll.ContractOverride_FromJSONFull, BTX_CAC_CompatibilityDll");
                if (internalClassType != null)
                {
                    return AccessTools.Method(internalClassType, "PatchContract", new Type[] { typeof(ContractOverride) });
                }
                return null;
            }

            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                bool found = false;

                for (int i = 1; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Stfld &&
                        codes[i].operand is FieldInfo field &&
                        field.Name == "maxNumberOfPlayerUnits" &&
                        field.FieldType == typeof(int) &&
                        codes[i + 1].opcode == OpCodes.Ret)
                    {
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4, 16);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Main.Log.LogWarning("[MaxPlayerUnits] Could not find Ldc_I4 8 to transpile in CAC-C.");
                }

                return codes.AsEnumerable();
            }
        }
    }
}