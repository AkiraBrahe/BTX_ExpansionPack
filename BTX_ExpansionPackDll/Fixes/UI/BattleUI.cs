using BattleTech;
using BattleTech.UI;
using BTSimpleMechAssembly;
using CustAmmoCategories;
using CustAmmoCategoriesPatches;
using CustomUnits;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes.UI
{
    internal class BattleUI
    {
        #region Unit Nameplate & Info Panel

        /// <summary>
        /// Shortens vehicle names and makes them stand out on nameplates.
        /// Example: "Behemoth Heavy Tank" -> "Behemoth"
        /// </summary>
        [HarmonyPatch(typeof(CustomMech_GetActorInfoFromVisLevel), "Get")]
        public static class GetActorInfoFromVisLevel_Get
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.UI.Battle.UseShortenedVehicleNames;

            [HarmonyPostfix]
            public static void Postfix(AbstractActor a, ref string __result)
            {
                __result = a.UnitName ?? __result;

                if (__result.EndsWith(")"))
                {
                    __result = __result.Replace("(", "<size=75%>(").Replace(")", ")</size>");
                    return;
                }

                if (Main.Settings.UI.Battle.ShowStandardVehicleVariant)
                {
                    // Special case: Unique named vehicle
                    if (__result.EndsWith("”"))
                    {
                        return;
                    }

                    // Special case: Omni-vehicle
                    string[] parts = __result.Split(' ');
                    string lastPart = parts[parts.Length - 1];

                    if (lastPart == "PRIME" || (lastPart.Length == 1 && char.IsUpper(lastPart[0])))
                    {
                        return;
                    }

                    __result += " <size=75%>(Standard)</size>";
                }
            }
        }

        /// <summary>
        /// Shows the vehicle type and tonnage below its name on the advanced infotips.
        /// </summary>
        [HarmonyPatch(typeof(CombatHUDActorDetailsDisplay), "RefreshInfo")]
        public static class CombatHUDActorDetailsDisplay_RefreshInfo
        {
            [HarmonyPostfix]
            public static void Postfix(CombatHUDActorDetailsDisplay __instance)
            {
                if (__instance.DisplayedActor is not Mech mech || !mech.FakeVehicle()) return;

                // Adjust the position and size of the weight text to fit the vehicle info
                var textComponent = __instance.ActorWeightText;
                var rectTransform = textComponent.rectTransform;
                rectTransform.sizeDelta = new UnityEngine.Vector2(300f, rectTransform.sizeDelta.y);
                rectTransform.anchoredPosition = __instance.transform.parent.name == "CombatHUDTargetingComputer"
                    ? new UnityEngine.Vector2(75f, 20f)
                    : new UnityEngine.Vector2(75f, rectTransform.anchoredPosition.y);
                textComponent.enableAutoSizing = false;

                // Show the vehicle type and tonnage
                string stockRole = mech.MechDef?.Chassis?.StockRole;
                if (!string.IsNullOrEmpty(stockRole) && stockRole != "VEHICLE")
                {
                    __instance.ActorWeightText.SetText("{0} ({1}t)", stockRole, mech.tonnage);
                    return;
                }

                __instance.ActorWeightText.SetText("VEHICLE: {0} ({1}t)", mech.weightClass, mech.tonnage);
            }
        }

        /// <summary>
        /// Removes the target info from the side panel.
        /// </summary>
        [HarmonyPatch]
        public static class CombatHUDInfoSidePanel_Patches
        {
            [HarmonyTargetMethods]
            public static IEnumerable<System.Reflection.MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(CombatHUDInfoSidePanel_Update), "UpdateInfoText");
                yield return AccessTools.Method(typeof(MoveStatusPreview_DisplayPreviewStatus), "Prefix");
            }

            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var matcher = new CodeMatcher(instructions)
                    .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "\n__/TARGET/__:\n"))
                    .MatchBack(false, new CodeMatch(i => i.opcode.FlowControl == FlowControl.Cond_Branch));

                object jumpTarget = matcher.Operand;
                return matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Br, jumpTarget))
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Shortens the ammo box description when hovering over the ammo counter in the side panel.
        /// </summary>
        [HarmonyPatch(typeof(CustomAmmoCategoriesPatches.WeaponAmmoCounterHover), "ShowSidePanel")]
        public static class WeaponAmmoCounterHover_ShowSidePanel
        {
            public static string ShortenDescription(string description) =>
                description.Replace("Ammo Bins contain the rounds needed for projectile-based weaponry, with at least one bin required per weapon type.", "")
                           .Replace("Ammo Bins will explode and destroy their installed location when they receive a Critical Hit.", "").Trim();

            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(false, new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BaseDescriptionDef), "Details")))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WeaponAmmoCounterHover_ShowSidePanel), "ShortenDescription")))
                    .InstructionEnumeration();
            }
        }

        #endregion

        #region Floaty Messages & Popups

        /// <summary>
        /// Fixes the injury reason description for vehicle pilots.
        /// </summary>
        [HarmonyPatch(typeof(Pilot), "InjuryReasonDescription", MethodType.Getter)]
        public static class PilotInjury_InjuryReasonDescription
        {
            [HarmonyPostfix]
            public static void Postfix(Pilot __instance, ref string __result)
            {
                if (__instance.InjuryReason == InjuryReason.ActorDestroyed &&
                    __instance.ParentActor is FakeVehicleMech)
                {
                    __result = "VEHICLE DESTROYED";
                }
            }
        }

        ///// <summary>
        ///// Removes the popup when moving before move clamping is calculated.
        ///// </summary>
        //[HarmonyPatch(typeof(SelectionStateMove_ProcessLeftClickClamp), "Prefix")]
        //public static class SelectionStateMove_ProcessLeftClickClamp_Prefix
        //{
        //    [HarmonyTranspiler]
        //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        //    {
        //        return new CodeMatcher(instructions)
        //            .MatchForward(false,
        //                new CodeMatch(i => i.opcode == OpCodes.Call && i.operand is System.Reflection.MethodInfo mi && mi.Name == "Create" && mi.DeclaringType.Name == "GenericPopupBuilder"))
        //            .RemoveInstructions(2)
        //            .InstructionEnumeration();
        //    }
        //}

        #endregion

        #region To Hit Modifiers

        /// <summary>
        /// Shows the correct vehicle location abbreviations in battle.
        /// </summary>
        [HarmonyPatch(typeof(VehicleCustomInfoHelper), "GetAbbreviatedChassisLocationDelegate")]
        public static class VehicleCustomInfoHelper_GetAbbreviatedChassisLocationDelegate
        {
            [HarmonyPostfix]
            public static void Postfix(ChassisDef def, ChassisLocations location, ref string __result)
            {
                if (def.ChassisTags != null && def.ChassisTags.Contains("fake_vehicle_chassis"))
                {
                    bool isVTOL = def.ChassisTags.Contains("unit_vtol");
                    __result = LocationNamingHelpers.GetLocationName(isVTOL ? ["unit_vtol"] : ["unit_vehicle"], location, false);
                }
            }
        }

        /// <summary>
        /// Shows the correct vehicle location abbreviations in battle.
        /// </summary>
        [HarmonyPatch(typeof(ToHitModifiersHelper), "GetAbbreviatedChassisLocation", [typeof(VehicleChassisLocations)])]
        public static class ToHitModifiersHelper_GetAbbreviatedChassisLocation
        {
            [HarmonyPrefix]
            public static bool Prefix(VehicleChassisLocations location, ref string __result)
            {
                __result = LocationNamingHelpers.GetLocationName(["fake_vehicle_chassis"], location.toFakeChassis(), false);
                return false;
            }
        }

        #endregion
    }
}