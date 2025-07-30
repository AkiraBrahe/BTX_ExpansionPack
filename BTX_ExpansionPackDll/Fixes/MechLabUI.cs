using BattleTech;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using CustomUnits;
using MechAffinity;
using System.Text.RegularExpressions;

namespace BTX_ExpansionPack.Fixes
{
    internal class MechLabUI
    {
        [HarmonyPatch(typeof(TooltipPrefab_Mech), "SetData", [typeof(object)])]
        public class TooltipPrefab_Mech_SetData
        {
            [HarmonyPostfix]
            public static void Postfix(object data, LocalizableText ___RoleField)
            {
                if (data is MechDef mechDef)
                {
                    bool isVehicle = mechDef.IsVehicle();
                    ___RoleField.gameObject.SetActive(!isVehicle);
                }
            }
        }

        [HarmonyPatch(typeof(PilotAffinityManager), "getMechChassisAffinityDescription", [typeof(ChassisDef)])]
        public static class PilotAffinityManager_getMechChassisAffinityDescription
        {
            [HarmonyPostfix]
            public static void Postfix(ref string __result) =>
                __result = __result.Replace("\n<b> Unlockable Affinities: </b>", "");
        }

        [HarmonyPatch(typeof(Quirks.Tooltips.QuirkToolTips), "DetailMechQuirks", [typeof(ChassisDef)])]
        public static class QuirkToolTips_DetailMechQuirks
        {
            [HarmonyFinalizer]
            public static void Finalizer(ref string __result)
            {
                if (!Main.Settings.UI.MechTooltips.UseDefaultColors)
                {
                    __result = __result.Replace("<color=#00cc00>", "<color=#85dbf6>"); // Special Traits (Light Blue)
                    __result = __result.Replace("<color=#ffcc00>", "<color=#ffb347>"); // Good Quirks (Pastel Orange)
                    __result = __result.Replace("<color=#e40000>", "<color=#ff6961>"); // Bad Quirks (Pastel Red)
                }

                __result = __result.Replace("<b>", "").Replace("</b>", "");
                __result = __result.Replace("Mech Quirks", "");
                __result = __result.Replace("\n ", "\n");
                __result = Regex.Replace(__result, @"(\n|^)(?!<color>)([^:]+): ", "$1<b>$2</b>: ");
                // __result = __result.Replace("\n\n", "\n");
                __result = Regex.Replace(__result, @"<color=#[0-9A-Fa-f]{6,8}></color>", "");
            }
        }
    }
}