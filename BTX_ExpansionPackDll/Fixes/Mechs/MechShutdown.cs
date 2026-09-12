using BattleTech;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Fixes.Mechanics
{
    /// <summary>
    /// Allows certain mech components to ignore shutdown effects, such as armor and components with the "ignore_shutdown" tag.
    /// </summary>
    [HarmonyPatch(typeof(Mech), nameof(Mech.RestartCreatedEffects))]
    public static class Mech_RestartCreatedEffects_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(MechComponent), nameof(MechComponent.RestartPassiveEffects)),
                    AccessTools.Method(typeof(Mech_RestartCreatedEffects_Patch), nameof(RestartPassiveEffects))
                );
        }

        public static void RestartPassiveEffects(this MechComponent @this, bool performAuraRefresh)
        {
            if (!IgnoreShutdown(@this))
            {
                @this.RestartPassiveEffects(performAuraRefresh);
            }
        }
    }

    public static bool IgnoreShutdown(this MechComponent mechComponent)
        {
            if (mechComponent.defId.StartsWith("Gear_Armor_"))
            {
                return true;
            }

            var componentTags = mechComponent.componentDef.ComponentTags;
            return componentTags.Contains("ignore_shutdown");
        }
    }
}