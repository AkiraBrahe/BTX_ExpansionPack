using BattleTech;
using BattleTech.Save;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using CustomSettings;
using CustomUnits;
using Extended_CE;
using Localize;
using MechAffinity;
using Quirks.Tooltips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BTX_ExpansionPack.Fixes.UI
{
    internal class MechLabUI
    {
        #region General UI Improvements

        /// <summary>
        /// Replaces the hardcoded "WEAPON ORDER" string from CAC.
        /// </summary>
        [HarmonyPatch(typeof(CustomAmmoCategoriesPatches.MechBayPanel_ViewBays), "Postfix")]
        public static class MechBayPanel_ViewBays_Postfix
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                return new CodeMatcher(instructions, il)
                    .MatchForward(false,
                        new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), "GetComponent", generics: [typeof(LocalizableText)])))
                    .Advance(1)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Dup))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(LocalizableText), "fontStyle")))
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Ldstr && i.operand is string s && s == "__/WEAPON ORDER/__"))
                    .SetOperandAndAdvance("__/CAC.WO.WEAPONS.HIGHLIGHT/__")
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Improves the description of non-standard ammunition boxes.
        /// </summary>
        [HarmonyPatch(typeof(AmmunitionBoxDef), "FromJSON")]
        [Obsolete("Temporary patch until the next CAC-C update.")]
        public static class AmmunitionBoxDef_FromJSON
        {
            [HarmonyPostfix]
            [HarmonyPriority(Priority.Last)]
            public static void Postfix(AmmunitionBoxDef __instance)
            {
                if (__instance?.Description?.UIName?.EndsWith(")") == true)
                {
                    string details = __instance.Description.Details;
                    if (details == null) return;

                    string halfText = "Ammo (Half) Bins each";
                    string doubleText = "Ammo (Double) Bins each";
                    string tripleText = "Ammo (Triple) Bins each";

                    if (details.Contains(halfText))
                    {
                        __instance.Description.Details = details.Replace(halfText, "half-capacity bins each");
                    }
                    else if (details.Contains(doubleText))
                    {
                        __instance.Description.Details = details.Replace(doubleText, "double-capacity bins each");
                    }
                    else if (details.Contains(tripleText))
                    {
                        __instance.Description.Details = details.Replace(tripleText, "triple-capacity bins each");
                    }
                }
            }
        }

        #endregion

        #region Mech Tooltip Improvements

        /// <summary>
        /// Removes the default spacing from all tooltips.
        /// </summary>
        [HarmonyPatch(typeof(TooltipPrefab), "Start")]
        public static class TooltipPrefab_Start_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(TooltipPrefab __instance)
            {
                var texts = __instance.GetComponentsInChildren<LocalizableText>(true);
                foreach (var text in texts)
                {
                    if (text.paragraphSpacing < 0f)
                    {
                        text.paragraphSpacing = 0f;
                    }
                }
            }
        }

        /// <summary>
        /// Replaces BEX's quirk tooltip patches to obtain the armor type directly from the mech definition.
        /// </summary>
        [HarmonyPatch(typeof(TooltipPrefab_Chassis), "SetData")]
        public static class TooltipPrefab_Chassis_SetData_Quirks
        {
            [HarmonyPostfix]
            public static void Postfix(TooltipPrefab_Chassis __instance, object data)
            {
                if (data is not ChassisDef chassisDef) return;

                string mechID = chassisDef.Description.Id.Replace("chassisdef_", "mechdef_");
                if (UnityGameInstance.BattleTechGame.Simulation.DataManager.MechDefs.TryGet(mechID, out var mechDef))
                {
                    __instance.descriptionText.SetText(__instance.descriptionText.text + DetailMechQuirks(mechDef));
                }
            }
        }

        [HarmonyPatch(typeof(TooltipPrefab_Mech), "SetData")]
        public static class TooltipPrefab_Mech_SetData_Quirks
        {
            [HarmonyPostfix]
            public static void Postfix(TooltipPrefab_Mech __instance, object data)
            {
                if (data is not MechDef mechDef) return;
                __instance.DetailsField.text += DetailMechQuirks(mechDef);
            }
        }

        [HarmonyPatch(typeof(MechLabMechInfoWidget), "SetData")]
        public static class MechLabMechInfoWidget_SetData_Quirks
        {
            [HarmonyPostfix]
            public static void Postfix(MechLabMechInfoWidget __instance)
            {
                if (__instance.stockRoleTooltip == null) return;

                var mechDef = __instance.mechLab?.activeMechDef;
                if (mechDef == null || mechDef.Chassis?.YangsThoughts == null) return;

                string title = $"YangsThoughts{mechDef.ChassisID}";
                string flavorText = mechDef.Chassis.YangsThoughts.TrimEnd([]);
                string roleText = "";

                if (Quirks.MechQuirks.modSettings.ShowBEXTRoleEffectsInMechLab)
                {
                    string roleInfo = QuirkToolTips.DetailRoleInfo(mechDef.Chassis);
                    if (!string.IsNullOrEmpty(roleInfo))
                    {
                        roleInfo = Regex.Replace(roleInfo, @"<color=[^>]+>|<b>|</b>|</color>", "").Trim();
                        roleText = "\n\n<size=12><color=#85dbf6>" + roleInfo + "</color></size>";
                    }
                }

                var baseDescriptionDef = new BaseDescriptionDef(title, "Yang's Thoughts", flavorText + roleText, string.Empty);
                __instance.stockRoleTooltip.SetDefaultStateData(TooltipUtilities.GetStateDataFromObject(baseDescriptionDef));
                __instance.GetComponent<LocalizableText>()?.paragraphSpacing = 0;
            }
        }

        /// <summary>
        /// Builds an improved version of the mech quirk section for the mech tooltip.
        /// </summary>
        internal static string DetailMechQuirks(MechDef mech)
        {
            var sections = new List<string>();
            var armor = mech.GetArmorInfo();

            // 1. Build Specialized Equipment traits (Armor, Engine, etc.)
            sections.Add(FormatSection(DetailMechQuirksBuild(mech.Chassis, armor), "#00cc00", "#85dbf6"));

            // 2. Build BEX Quirks
            if (mech.Chassis.ChassisTags.Any(t => t.StartsWith("mech_quirk")))
            {
                sections.Add(FormatSection(QuirkToolTips.DetailMechQuirksGood(mech.Chassis), "#ffcc00", "#ffb347"));
                sections.Add(FormatSection(QuirkToolTips.DetailMechQuirksBad(mech.Chassis), "#e40000", "#ff6961"));
            }

            return Sanitize(string.Join("\n\n", sections.Where(s => !string.IsNullOrEmpty(s))));
        }

        /// <summary>
        /// Formats the quirk section text with the appropriate color.
        /// </summary>
        private static string FormatSection(string text, string defaultColor, string overrideColor)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            // Strip existing color tags and bolding to normalize
            text = Regex.Replace(text, @"<color=[^>]+>|<b>|</b>|</color>", "").Trim();
            if (string.IsNullOrEmpty(text)) return string.Empty;

            string color = Main.Settings.UI.MechTooltips.UseDefaultColors ? defaultColor : overrideColor;
            return $"<color={color}>{text}</color>";
        }

        /// <summary>
        /// Sanitizes the quirk section text and wraps it in size=12 tags.
        /// </summary>
        private static string Sanitize(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            // Normalize whitespace and ensure labels are consistently bolded
            text = Regex.Replace(text, @"[ \t]*\n[ \t]*", "\n").Trim();
            text = Regex.Replace(text, @"(<color=[^>]+>)?([^:\n]+:)", "$1<b>$2</b>", RegexOptions.Multiline);

            return $"<size=12>{text}</size>";
        }

        /// <summary>
        /// Same logic as BEX but accomodate for new armor types and equipment from the Expansion Pack.
        /// </summary>
        internal static string DetailMechQuirksBuild(ChassisDef chassis, ArmorInfo armor)
        {
            var traits = new List<string>();

            // Unit Type
            if (chassis.ChassisTags.Contains("chassis_omni"))
                traits.Add("Omni Mech: Can be readied in to Omni variant of your choice. Readies in half the time.");

            if (chassis.ChassisTags.Contains("mech_quad"))
                traits.Add("Quad Mech: Arm locations are legs, increasing stability and improving movement on rough terrain.");
            else if (chassis.ChassisTags.Contains("mech_lam"))
                traits.Add("Land-Air Mech: Can switch between land and air modes. Jumping increases damage and reduces instability.");

            // Structure
            if (chassis.ChassisTags.Contains("chassis_composite"))
                traits.Add("Composite Chassis: Half the weight of a standard chassis, but more susceptible to damage.");
            else if (chassis.ChassisTags.Contains("chassis_endo"))
                traits.Add("Endo Steel Chassis: Half the weight of a standard chassis, but with increased internal bulk.");
            else if (chassis.ChassisTags.Contains("chassis_industrial"))
                traits.Add("Industrial Chassis: No benefits over a standard chassis.");
            else if (chassis.ChassisTags.Contains("chassis_reinforced"))
                traits.Add("Reinforced Chassis: Double the weight of a standard chassis, but with increased durability.");

            // Armor
            if (armor.Type != ArmorType.Standard)
                traits.Add($"{armor.Name} Armor: {armor.Description}");

            // CASE
            if (chassis.ChassisTags.Contains("chassis_clan"))
                traits.Add("Clan CASE: All locations except the Head are protected by CASE and vent ammo explosions out of the rear of the 'Mech.");
            else if (chassis.ChassisTags.Contains("mech_case_left") || chassis.ChassisTags.Contains("mech_case_right") || chassis.ChassisTags.Contains("mech_case_centre"))
                traits.Add("CASE: Locations highlighted in green in the Mech Lab are protected by CASE and vent ammo explosions out of the rear of the 'Mech.");

            // Gyro
            if (chassis.ChassisTags.Contains("mech_gyro_compact"))
                traits.Add("Compact Gyro: Weighs more but takes up half the space, making it less likely to be crit.");
            else if (chassis.ChassisTags.Contains("mech_gyro_heavyduty"))
                traits.Add("Heavy-Duty Gyro: Twice as heavy but can withstand more critical hits prior to failure.");
            else if (chassis.ChassisTags.Contains("mech_gyro_superheavy"))
                traits.Add("Superheavy Gyro: Malfunctioning. Cannot withstand a single critical hit.");
            else if (chassis.ChassisTags.Contains("mech_gyro_xl"))
                traits.Add("Extralight Gyro: Weighs less but takes up more space, making it more likely to be crit.");

            // Engine
            if (chassis.ChassisTags.Contains("chassis_DHS"))
                traits.Add("Double Heat Sink Engine: Provides extra cooling to the chassis.");

            if (chassis.ChassisTags.Contains("mech_quirk_largeengine"))
            {
                traits.Add("Large Engine: Provides +1 initiative.");
            }
            else if (chassis.ChassisTags.Contains("mech_quirk_extralargeengine"))
            {
                traits.Add("Extra Large Engine: Provides +1 hit defense and +1 initiative.");
            }
            else if (chassis.ChassisTags.Contains("mech_quirk_extremeengine"))
            {
                traits.Add("Extremely Large Engine: Provides +2 hit defense and +1 initiative.");
            }

            return string.Join("\n", traits);
        }

        /// <summary>
        /// Changes the Compact Gyro tag for clarity.
        /// </summary>
        [HarmonyPatch(typeof(BTComponents.Mech_InitStats), "Prefix")]
        [Obsolete("Add other gyro types soon.")]
        public static class Mech_InitStats_Prefix
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                return new CodeMatcher(instructions, il)
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Ldstr && i.operand is string s && s.StartsWith("chassis_compactgyro")))
                    .SetOperandAndAdvance("mech_gyro_compact")
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Removes redundant information from the mech tooltips.
        /// </summary>
        [HarmonyPatch(typeof(PilotAffinityManager), "getMechChassisAffinityDescription", [typeof(ChassisDef)])]
        public static class PilotAffinityManager_getMechChassisAffinityDescription
        {
            [HarmonyPostfix]
            [HarmonyPriority(Priority.Last)]
            public static void Postfix(ref string __result)
            {
                if (string.IsNullOrEmpty(__result)) return;

                __result = __result.Replace("<b> Unlockable Affinities: </b>\n\n", "");
                __result = __result.Replace("</b>:", ":</b>").Replace(":\n", "\n");
                __result = "<color=#a1a1a1>" + __result + "</color>";

                __result = string.Join("\n", RemoveWeightClassAffinity(__result));
                __result = $"<size=12>{__result}</size>";
            }

            private static List<string> RemoveWeightClassAffinity(string text)
            {
                var lines = text.Split(["\n"], StringSplitOptions.None).ToList();
                var result = new List<string>();

                foreach (string line in lines)
                {
                    if (line.Contains("(20)"))
                    {
                        continue;
                    }
                    result.Add(line);
                }

                return result;
            }
        }

        #endregion

        #region Vehicle UI Improvements

        /// <summary>
        /// Improves the description of the "Partial Refit" option.
        /// </summary>
        [HarmonyPatch(typeof(ReducedComponentRefInfoHelper), "description")]
        public static class ReducedComponentRefInfoHelper_description
        {
            [HarmonyPrefix]
            public static bool Prefix(Strings.Culture culture, ref string __result)
            {
                if (culture != Strings.Culture.CULTURE_RU_RU)
                {
                    __result = "Enabling this option allows for limited customization of vehicles.\n\n" +
                        "• Limited means you can replace existing weapons and ammo, but cannot add or remove any equipment. To replace or repair a component, simply drag a new one over it. The new component must be of the same type and have equal or lesser size, tonnage, and heat generation.\n\n" +
                        "• With this option active, your vehicles won't be automatically repaired after each battle. Additionally, you will be able to store vehicles, with all weapons and ammo being automatically moved to your storage. When you restore a stored vehicle, it will be equipped with destroyed versions of the original weapons and ammo.";

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Enables the "Partial Refit" option for vehicles by default.
        /// </summary>
        [HarmonyPatch(typeof(SaveManager), MethodType.Constructor, [typeof(MessageCenter)])]
        public static class SaveManager_Constructor
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.HasPlayableVehicles && Main.Settings.Gameplay.EnableVehiclePartialRefit;

            [HarmonyPostfix]
            public static void Postfix()
            {
                CustomUnits.Core.Settings.VehcilesPartialEditable = true;
                ModsLocalSettingsHelper.SaveSettings("CustomUnits");
            }
        }

        /// <summary>
        /// Prevents a known UI bug when trying to refit a vehicle before refitting a mech first.
        /// </summary>
        private static bool IsMechLabInitializedByMech = false;

        [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
        public static class MechLabPanel_LoadMech_InitializationTracker
        {
            [HarmonyPostfix]
            public static void Postfix(MechDef newMechDef)
            {
                if (newMechDef != null && !newMechDef.IsVehicle())
                    IsMechLabInitializedByMech = true;
            }
        }

        [HarmonyPatch(typeof(MechBayMechInfoWidget), "OnMechLabClicked")]
        public static class MechBayMechInfoWidget_OnMechLabClicked
        {
            [HarmonyPrefix]
            public static bool Prefix(ref bool __runOriginal, MechBayMechInfoWidget __instance)
            {
                if (!__runOriginal) return true;

                if (!IsMechLabInitializedByMech && __instance.selectedMech != null && __instance.selectedMech.IsVehicle())
                {
                    GenericPopupBuilder.Create(
                        "Can't refit vehicle",
                        "To prevent a known UI bug, you must refit a BattleMech at least once before refitting a vehicle.")
                        .AddFader(new UIColorRef?(HBS.LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true)
                        .Render();

                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Shows hardpoints and tonnage in the mech lab for vehicles.
        /// </summary>
        [HarmonyPatch(typeof(RedusedMechLabMechInfoWidget), "Init")]
        public class RedusedMechLabMechInfoWidget_Init
        {
            [HarmonyPostfix]
            public static void Postfix(RedusedMechLabMechInfoWidget __instance)
            {
                __instance.layout_hardpoints.SetActive(true);
                __instance.layout_tonnage.SetActive(true);
            }
        }

        /// <summary>
        /// Fixes the armor status bars in the mech lab for vehicles.
        /// </summary>
        [HarmonyPatch(typeof(LanceStat), "SetValue")]
        public static class LanceStat_SetValue
        {
            [HarmonyPrefix]
            public static bool Prefix(LanceStat __instance, float current, float max, float delta, UIColor dColor)
            {
                __instance.deltaColor.SetUIColor(dColor);
                if (max <= 0f)
                {
                    __instance.fillbar.fillAmount = 0f;
                    __instance.deltaBar.fillAmount = 0f;
                    return false;
                }

                float num = Mathf.Clamp01(current / max);
                float num2 = Mathf.Clamp01(delta / max);
                __instance.fillbar.fillAmount = num;
                __instance.deltaBar.fillAmount = num2;
                return false;
            }
        }

        /// <summary>
        /// Removes unused variant names for vehicles in the mech bay.
        /// </summary>
        [HarmonyPatch(typeof(MechBayMechInfoWidget), "SetDescriptions")]
        public static class MechBayMechInfoWidget_SetDescriptions
        {
            [HarmonyPostfix]
            public static void Postfix(MechBayMechInfoWidget __instance)
            {
                var mechDef = __instance.selectedMech;
                if (mechDef == null) return;

                if (mechDef.IsVehicle())
                {
                    __instance.mechConfiguration.SetText("{0} - {1}",
                        [
                            mechDef.Chassis.Description.Name,
                            mechDef.Chassis.weightClass.ToString()
                        ]);
                }
            }
        }

        /// <summary>
        /// Removes unused variant names for vehicles in the mech lab. Also changes the stock role.
        /// </summary>
        [HarmonyPatch(typeof(MechLabMechInfoWidget), "SetData")]
        public static class MechLabMechInfoWidget_SetData_VehicleInfo
        {
            [HarmonyPostfix]
            public static void Postfix(MechLabMechInfoWidget __instance)
            {
                var mechDef = __instance.mechLab?.activeMechDef;
                if (mechDef == null) return;

                if (mechDef.IsVehicle())
                {
                    __instance.mechDetails.SetText("{0} - {1}",
                        [
                            mechDef.Chassis.Description.Name,
                            mechDef.Chassis.weightClass.ToString()
                        ]);

                    var vehicleDef = mechDef.toVehicleDef(mechDef.DataManager);
                    string role = GetVehicleRole(vehicleDef?.VehicleTags);
                    __instance.mechStockRole.SetText(role);
                }
            }
        }

        /// <summary>
        /// Removes unused variant names for vehicles on the lance loadout screen.
        /// </summary>
        [HarmonyPatch(typeof(LanceLoadoutMechItem), "SetData")]
        public static class LanceLoadoutMechItem_SetData
        {
            [HarmonyPostfix]
            public static void Postfix(LanceLoadoutMechItem __instance, MechDef mechDef)
            {
                if (mechDef == null || !mechDef.IsVehicle()) return;
                __instance.MechDetailsText.SetText("{0} - {1}",
                        [
                            mechDef.Chassis.Description.Name,
                            mechDef.Chassis.weightClass.ToString()
                        ]);
            }
        }

        /// <summary>
        /// Shows vehicle roles instead of variant names in tooltips.
        /// </summary>
        [HarmonyPatch(typeof(TooltipPrefab_Mech), "SetData")]
        public class TooltipPrefab_Mech_SetData_VehicleRole
        {
            [HarmonyPostfix]
            public static void Postfix(TooltipPrefab_Mech __instance, object data)
            {
                if (data is MechDef mechDef)
                {
                    bool isVehicle = mechDef.IsVehicle();
                    __instance.VariantField.gameObject.SetActive(!isVehicle);

                    if (isVehicle)
                    {
                        var vehicleDef = mechDef.toVehicleDef(mechDef.DataManager);
                        string role = GetVehicleRole(vehicleDef?.VehicleTags);
                        __instance.RoleField.text = role;
                    }
                }
            }
        }

        public static string GetVehicleRole(IEnumerable<string> tags)
        {
            if (tags == null)
                return "VEHICLE";

            foreach (string tag in tags)
            {
                switch (tag)
                {
                    case "role_scout": return "Scout";
                    case "role_striker": return "Striker";
                    case "role_skirmisher": return "Skirmisher";
                    case "role_brawler": return "Brawler";
                    case "role_juggernaut": return "Juggernaut";
                    case "role_missileboat": return "Missile Boat";
                    case "role_sniper": return "Sniper";
                    case "role_ambusher": return "Ambusher";
                    case "role_none": return "None";
                }
            }

            return "VEHICLE";
        }

        /// <summary>
        /// Removes vehicle info from the description when assembling vehicles.
        /// </summary>
        [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
        public static class ChassisDef_FromJSON
        {
            [HarmonyPostfix]
            [HarmonyPriority(Priority.Last)]
            public static void Postfix(ChassisDef __instance)
            {
                if (__instance.IsVehicle())
                {
                    string thoughts = __instance.YangsThoughts;
                    string delimiter = "</b>\n\n";
                    int splitIndex = thoughts.IndexOf(delimiter);

                    if (splitIndex >= 0)
                    {
                        __instance.YangsThoughts = thoughts.Substring(splitIndex + delimiter.Length);
                    }
                }
            }
        }

        #endregion

        #region Location Names

        /// <summary>
        /// Shows real location names in the mech bay for vehicles, quads, and squads.
        /// </summary>
        [HarmonyPatch(typeof(LanceMechEquipmentList), "SetLoadout", [])]
        [Obsolete("Move to CAC-C when possible", false)]
        public static class LanceMechEquipmentList_SetLoadout
        {
            [HarmonyPostfix]
            public static void Postfix(LanceMechEquipmentList __instance)
            {
                var mechDef = __instance.activeMech;
                if (mechDef == null) return;

                var tags = mechDef.MechTags;
                if (mechDef.IsSquad())
                {
                    var info = mechDef.GetCustomInfo();
                    int troopersCount = info?.SquadInfo.Troopers ?? 0;
                    SetWidgetLabel(__instance.headLabel, "U0");
                    SetWidgetLabel(__instance.centerTorsoLabel, "U1", troopersCount >= 2);
                    SetWidgetLabel(__instance.leftTorsoLabel, "U2", troopersCount >= 3);
                    SetWidgetLabel(__instance.rightTorsoLabel, "U3", troopersCount >= 4);
                    SetWidgetLabel(__instance.leftArmLabel, "U4", troopersCount >= 5);
                    SetWidgetLabel(__instance.rightArmLabel, "U5", troopersCount >= 6);
                    SetWidgetLabel(__instance.leftLegLabel, "U6", troopersCount >= 7);
                    SetWidgetLabel(__instance.rightLegLabel, "U7", troopersCount >= 8);
                }
                else if (mechDef.IsVehicle())
                {
                    bool isVTOL = tags.Contains("unit_vtol");
                    SetWidgetLabel(__instance.headLabel, isVTOL ? "RO" : "TU");
                    SetWidgetLabel(__instance.leftArmLabel, "FR");
                    SetWidgetLabel(__instance.rightArmLabel, "RR");
                    SetWidgetLabel(__instance.leftLegLabel, "LS");
                    SetWidgetLabel(__instance.rightLegLabel, "RS");
                    SetWidgetLabel(__instance.centerTorsoLabel, "", false);
                    SetWidgetLabel(__instance.leftTorsoLabel, "", false);
                    SetWidgetLabel(__instance.rightTorsoLabel, "", false);
                }
                else
                {
                    bool isQuad = tags.Contains("unit_quad");
                    SetWidgetLabel(__instance.headLabel, "H");
                    SetWidgetLabel(__instance.centerTorsoLabel, "CT");
                    SetWidgetLabel(__instance.leftTorsoLabel, "LT");
                    SetWidgetLabel(__instance.rightTorsoLabel, "RT");
                    SetWidgetLabel(__instance.leftArmLabel, isQuad ? "FLL" : "LA");
                    SetWidgetLabel(__instance.rightArmLabel, isQuad ? "FRL" : "RA");
                    SetWidgetLabel(__instance.leftLegLabel, isQuad ? "RLL" : "LL");
                    SetWidgetLabel(__instance.rightLegLabel, isQuad ? "RRL" : "RL");
                }
            }
            private static void SetWidgetLabel(LocalizableText label, string text, bool active = true)
            {
                if (label != null)
                {
                    label.SetText(text);
                    label.gameObject.transform.parent.gameObject.SetActive(active);
                }
            }
        }

        /// <summary>
        /// Shows real location names in the mech lab for vehicles and quads.
        /// </summary>
        [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
        [Obsolete("Move to CAC-C when possible", false)]
        public static class MechLabPanel_LoadMech
        {
            [HarmonyPostfix]
            public static void Postfix(MechLabPanel __instance, MechDef newMechDef)
            {
                if (newMechDef == null) return;

                var tags = newMechDef.MechTags;
                bool isVehicle = newMechDef.IsVehicle();
                if (isVehicle)
                {
                    bool isVTOL = tags.Contains("unit_vtol");
                    bool hasTurret = newMechDef.toVehicleDef(newMechDef.DataManager).Chassis.HasTurret;
                    SetWidgetLabel(__instance.headWidget, isVTOL ? "ROTOR" : "TURRET", isVTOL || hasTurret, true);
                    SetWidgetLabel(__instance.leftArmWidget, "FRONT", true, true);
                    SetWidgetLabel(__instance.rightArmWidget, "REAR", true, true);
                    SetWidgetLabel(__instance.leftLegWidget, "LEFT SIDE", true, true);
                    SetWidgetLabel(__instance.rightLegWidget, "RIGHT SIDE", true, true);
                    SetWidgetLabel(__instance.centerTorsoWidget, "", false);
                    SetWidgetLabel(__instance.leftTorsoWidget, "", false);
                    SetWidgetLabel(__instance.rightTorsoWidget, "", false);
                }
                else
                {
                    bool isQuad = tags.Contains("unit_quad");
                    SetWidgetLabel(__instance.headWidget, "HEAD");
                    SetWidgetLabel(__instance.leftTorsoWidget, "LEFT TORSO");
                    SetWidgetLabel(__instance.centerTorsoWidget, "CENTER TORSO");
                    SetWidgetLabel(__instance.rightTorsoWidget, "RIGHT TORSO");
                    SetWidgetLabel(__instance.leftArmWidget, isQuad ? "FRONT LEFT LEG" : "LEFT ARM");
                    SetWidgetLabel(__instance.rightArmWidget, isQuad ? "FRONT RIGHT LEG" : "RIGHT ARM");
                    SetWidgetLabel(__instance.leftLegWidget, isQuad ? "REAR LEFT LEG" : "LEFT LEG");
                    SetWidgetLabel(__instance.rightLegWidget, isQuad ? "REAR RIGHT LEG" : "RIGHT LEG");
                }
            }

            private static void SetWidgetLabel(MechLabLocationWidget widget, string label, bool active = true, bool isVehicle = false)
            {
                if (widget != null)
                {
                    widget.locationName.SetText(label);
                    widget.gameObject.SetActive(active);
                    if (active)
                    {
                        widget.armorBar.transform.Find("bttn_plus")?.gameObject.SetActive(!isVehicle);
                        widget.armorBar.transform.Find("bttn_minus")?.gameObject.SetActive(!isVehicle);
                    }
                }
            }
        }

        #endregion
    }
}