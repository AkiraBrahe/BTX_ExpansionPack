using BattleTech;
using BTX_ExpansionPack.Helpers;
using CustAmmoCategories;
using CustAmmoCategoriesPatches;
using CustomUnits;
using IRBTModUtils;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading;

namespace BTX_ExpansionPack.Fixes.UI
{
    internal class BattleUI
    {
        /// <summary>
        /// Removes the popup when moving before move clamping is calculated.
        /// </summary>
        [HarmonyPatch(typeof(SelectionStateMove_ProcessLeftClickClamp), "Prefix")]
        public static class SelectionStateMove_ProcessLeftClickClamp_Prefix
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                return new CodeMatcher(instructions, il)
                    .MatchForward(false,
                        new CodeMatch(i => i.opcode == OpCodes.Call && i.operand is System.Reflection.MethodInfo mi && mi.Name == "Create" && mi.DeclaringType.Name == "GenericPopupBuilder"))
                    .RemoveInstructions(2)
                    .InstructionEnumeration();
            }
        }

        /// <summary>
        /// Removes the target info from the side panel UI.
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
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions, il)
                    .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "\n__/TARGET/__:\n"))
                    .MatchBack(false, new CodeMatch(i => i.opcode == OpCodes.Brfalse || i.opcode == OpCodes.Brfalse_S));

                var jumpTarget = matcher.Operand;
                return matcher.SetInstructionAndAdvance(new CodeInstruction(OpCodes.Pop))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Br, jumpTarget))
                    .InstructionEnumeration();
            }
        }

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

        /// <summary>
        /// Shows full location names for mechs in the to-hit modifiers in battle.
        /// </summary>
        [HarmonyPatch(typeof(ToHitModifiersHelper), "GetToHitModifierName", [typeof(Mech), typeof(int)])]
        public static class ToHitModifiersHelper_GetToHitModifierName_Mech
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.UI.Battle.ShowFullLocationName;

            [HarmonyPrefix]
            public static bool Prefix(ref bool __runOriginal, Mech unit, int location, ref string __result)
            {
                if (unit == null)
                {
                    __result = string.Empty;
                    __runOriginal = false;
                    return false;
                }

                var cLoc = (ChassisLocations)location;
                if (string.IsNullOrEmpty(unit.GetStringForStructureDamageLevel(cLoc)))
                {
                    cLoc = ChassisLocations.CenterTorso;
                }

                Thread.CurrentThread.pushActor(unit);
                var locationDamageLevel = unit.GetLocationDamageLevel(cLoc);
                Thread.CurrentThread.clearActor();

                string text = locationDamageLevel switch
                {
                    LocationDamageLevel.Penalized => string.Format("{0} DAMAGED", GetAbbreviatedChassisLocation(unit, cLoc)),
                    LocationDamageLevel.NonFunctional => string.Format("{0} DESTROYED", GetAbbreviatedChassisLocation(unit, cLoc)),
                    _ => string.Empty,
                };

                __result = text;
                __runOriginal = false;
                return false;
            }

            public static string GetAbbreviatedChassisLocation(Mech unit, ChassisLocations cLoc)
            {
                var tags = unit.MechDef.MechTags;
                var location = cLoc;
                var locationName = LocationNamingHelpers.GetLocationName(tags, location, true);
                return !string.IsNullOrEmpty(locationName) ? locationName : string.Empty;
            }
        }

        /// <summary>
        /// Shows full location names for vehicles in the to-hit modifiers in battle.
        /// </summary>
        [HarmonyPatch(typeof(ToHitModifiersHelper), "GetToHitModifierName", [typeof(Vehicle), typeof(int)])]
        public static class ToHitModifiersHelper_GetToHitModifierName_Vehicle
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.UI.Battle.ShowFullLocationName;

            [HarmonyPrefix]
            public static bool Prefix(ref bool __runOriginal, Vehicle unit, int location, ref string __result)
            {
                if (unit == null)
                {
                    __result = string.Empty;
                    __runOriginal = false;
                    return false;
                }

                var vLoc = (VehicleChassisLocations)location;
                if (string.IsNullOrEmpty(unit.GetStringForStructureDamageLevel(vLoc)))
                {
                    vLoc = VehicleChassisLocations.Front;
                }

                var locationDamageLevel = unit.GetLocationDamageLevel(vLoc);

                string text = locationDamageLevel switch
                {
                    LocationDamageLevel.Penalized => string.Format("{0} DAMAGED", GetAbbreviatedChassisLocation(unit, vLoc)),
                    LocationDamageLevel.NonFunctional => string.Format("{0} DESTROYED", GetAbbreviatedChassisLocation(unit, vLoc)),
                    _ => string.Empty,
                };

                __result = text;
                __runOriginal = false;
                return false;
            }

            public static string GetAbbreviatedChassisLocation(Vehicle unit, VehicleChassisLocations vLoc)
            {
                var tags = unit.VehicleDef.VehicleTags;
                var location = vLoc.toFakeChassis();
                var locationName = LocationNamingHelpers.GetLocationName(tags, location, true);
                return !string.IsNullOrEmpty(locationName) ? locationName : string.Empty;
            }
        }
    }
}