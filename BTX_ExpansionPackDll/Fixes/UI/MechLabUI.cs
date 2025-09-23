using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using CustomUnits;
using Localize;
using MechAffinity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quirks.Tooltips;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BTX_ExpansionPack.Fixes
{
    internal class MechLabUI
    {
        /// <summary>
        /// Improves the description of the "Reduced Component Refitting" option.
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
        /// Prevents a known UI bug when trying to refit a vehicle before refitting a mech first.
        /// </summary>
        private static bool IsMechLabInitializedByMech = false;

        [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
        public static class MechLabPanel_LoadMech_InitializationTracker
        {
            [HarmonyPostfix, HarmonyPriority(Priority.Last)]
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
            [HarmonyAfter("io.mission.customunits")]
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
        /// Shows hardpoints and tonnage in the mech lab.
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
        /// Shows real location names in the mech lab for vehicles and quads.
        /// </summary>
        [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
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
                if (widget == null) return;
                widget.locationName.SetText(label);
                widget.gameObject.SetActive(active);
                if (active)
                {
                    widget.armorBar.transform.Find("bttn_plus")?.gameObject.SetActive(!isVehicle);
                    widget.armorBar.transform.Find("bttn_minus")?.gameObject.SetActive(!isVehicle);
                }
            }
        }

        /// <summary>
        /// Shows real location names in the mech bay for vehicles, quads, and squads.
        /// </summary>
        [HarmonyPatch(typeof(LanceMechEquipmentList), "SetLoadout", [])]
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
                    UnitCustomInfo info = mechDef.GetCustomInfo();
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
                if (label == null) return;
                label.SetText(text);
                label.gameObject.transform.parent.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// Removes unused variant names for vehicles in the mech bay, mech lab, and lance loadout.
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

        [HarmonyPatch(typeof(MechLabMechInfoWidget), "SetData")]
        public static class MechLabMechInfoWidget_SetData
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
        [HarmonyPatch(typeof(TooltipPrefab_Mech), "SetData", [typeof(object)])]
        public class TooltipPrefab_Mech_SetData
        {
            [HarmonyPostfix]
            public static void Postfix(object data, LocalizableText ___RoleField, LocalizableText ___VariantField)
            {
                if (data is MechDef mechDef)
                {
                    bool isVehicle = mechDef.IsVehicle();
                    ___VariantField.gameObject.SetActive(!isVehicle);

                    if (isVehicle)
                    {
                        var vehicleDef = mechDef.toVehicleDef(mechDef.DataManager);
                        string role = GetVehicleRole(vehicleDef?.VehicleTags);
                        ___RoleField.text = role;
                    }
                }
            }
        }

        public static string GetVehicleRole(IEnumerable<string> tags)
        {
            if (tags == null)
                return "VEHICLE";

            foreach (var tag in tags)
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
        [HarmonyPatch(typeof(ChassisDef_FromJSON_fake), "ConstructMechFakeVehicle")]
        public static class CustomUnitsChassisDefPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref string __result)
            {
                try
                {
                    JObject chassisDef = JObject.Parse(__result);
                    var yangsThoughtsToken = chassisDef["YangsThoughts"];
                    if (yangsThoughtsToken != null && yangsThoughtsToken.Type == JTokenType.String)
                    {
                        string thoughts = yangsThoughtsToken.Value<string>();
                        string delimiter = "&lt;/b&gt;\n\n";
                        int splitIndex = thoughts.IndexOf(delimiter);

                        if (splitIndex >= 0)
                        {
                            chassisDef["YangsThoughts"] = thoughts.Substring(splitIndex + delimiter.Length);
                            __result = chassisDef.ToString(Formatting.Indented);
                        }
                    }
                }
                catch (JsonReaderException)
                {
                    // Not a valid JSON object. This is expected if the mech isn't a "fake" vehicle.
                }
                catch (Exception ex)
                {
                    Main.Log.LogException(ex);
                }
            }
        }

        /// <summary>
        /// Improves the mech tooltips by removing redundant information and improving readability.
        /// </summary>
        [HarmonyPatch(typeof(PilotAffinityManager), "getMechChassisAffinityDescription", [typeof(ChassisDef)])]
        public static class PilotAffinityManager_getMechChassisAffinityDescription
        {
            [HarmonyPostfix]
            public static void Postfix(ref string __result) =>
                __result = __result.Replace("\n<b> Unlockable Affinities: </b>", "");
        }

        [HarmonyPatch(typeof(QuirkToolTips), "DetailMechQuirks", [typeof(ChassisDef)])]
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
                __result = __result.Replace("\n\n", "\n");
                __result = Regex.Replace(__result, @"<color=#[0-9A-Fa-f]{6,8}></color>", "");
            }
        }
    }
}