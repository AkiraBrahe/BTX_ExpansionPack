using BattleTech;
using BattleTech.UI;
using CustomUnits;
using Localize;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Fixes
{
    internal static class LanceRestrictions
    {
        [HarmonyPatch(typeof(LanceConfiguratorPanel), "OnConfirmClicked")]
        public static class LanceConfiguratorPanel_OnConfirmClicked_NoBiome
        {
            [HarmonyPrefix]
            [HarmonyBefore("io.mission.customunits")]
            [HarmonyWrapSafe]
            public static bool Prefix(ref bool __runOriginal, LanceConfiguratorPanel __instance)
            {
                if (__instance?.sim != null && __instance.activeContract != null)
                {
                    var blockedUnits = new HashSet<string>();
                    string forbidTag = $"NoBiome_{__instance.activeContract.ContractBiome}";
                    bool badBiome = false;

                    var loadoutSlots = Traverse.Create(__instance).Field("loadoutSlots").GetValue<LanceLoadoutSlot[]>();
                    if (loadoutSlots != null)
                    {
                        foreach (var slot in loadoutSlots)
                        {
                            if (slot?.SelectedMech?.MechDef is MechDef mechDef)
                            {
                                if (HasForbiddenBiome(mechDef, forbidTag, out string logMessage))
                                {
                                    if (blockedUnits.Add(mechDef.Name))
                                    {
                                        Main.Log.LogDebug(logMessage);
                                    }
                                    badBiome = true;
                                }
                            }
                        }

                        if (badBiome)
                        {
                            Text title = new("__/DROP.BAD_BIOME.TITLE/__", string.Join(", ", blockedUnits));
                            Text message = new("__/DROP.BAD_BIOME.MESSAGE/__", []);

                            var interruptQueue = Traverse.Create(__instance.sim).Field("interruptQueue").GetValue<SimGameInterruptManager>();
                            interruptQueue.QueuePauseNotification(
                                title.ToString(true),
                                message.ToString(true),
                                __instance.sim.GetCrewPortrait(SimGameCrew.Crew_Yang),
                                "notification_mechreadycomplete",
                                delegate { },
                                "Continue",
                                null,
                                null
                            );
                            __runOriginal = false;
                            return false;
                        }
                    }
                }
                return true;
            }

            private static bool HasForbiddenBiome(MechDef mechDef, string forbidTag, out string logMessage)
            {
                if (mechDef.MechTags.Contains(forbidTag))
                {
                    logMessage = $"[NoBiomeFix] {mechDef.Name} has forbidden tag: {forbidTag}, preventing deployment.";
                    return true;
                }
                foreach (var component in mechDef.Inventory)
                {
                    if (component?.Def?.ComponentTags?.Contains(forbidTag) == true)
                    {
                        logMessage = $"[NoBiomeFix] {mechDef.Name} has {component.Def.Description.Name} with forbidden tag: {forbidTag}, preventing deployment.";
                        return true;
                    }
                }
                logMessage = null;
                return false;
            }
        }

        [HarmonyPatch(typeof(LanceConfiguratorPanel), "ValidateLance")]
        public static class LanceConfiguratorPanel_ValidateLance_NoVehicleDuel
        {
            [HarmonyPrefix]
            [HarmonyBefore("io.mission.customunits")]
            [HarmonyWrapSafe]
            public static void Prefix(ref bool __runOriginal, LanceConfiguratorPanel __instance, ref bool __result)
            {
                if (__instance.activeContract != null && __instance.activeContract.IsDuelContract())
                {
                    if (Main.Settings.Gameplay.AllowVehiclesInMechDuels) return;

                    bool foundVehicle = false;
                    var deployedUnits = new List<MechDef>();

                    var loadoutSlots = Traverse.Create(__instance).Field("loadoutSlots").GetValue<LanceLoadoutSlot[]>();
                    if (loadoutSlots != null)
                    {
                        foreach (var slot in loadoutSlots)
                        {
                            if (slot?.SelectedMech?.MechDef is MechDef mechDef)
                            {
                                deployedUnits.Add(mechDef);
                                if (mechDef.MechTags.Contains("unit_vehicle"))
                                {
                                    foundVehicle = true;
                                }
                            }
                        }
                    }

                    if (foundVehicle)
                    {
                        __instance.lanceValid = false;
                        Text lanceErrorText = new("Vehicles are not allowed in Duel contracts.");
                        var headerWidget = Traverse.Create(__instance).Field("headerWidget").GetValue<LanceHeaderWidget>();
                        headerWidget?.RefreshLanceInfo(__instance.lanceValid, lanceErrorText, deployedUnits, deployedUnits.Count, __instance.maxUnits);
                        __result = __instance.lanceValid;
                        __runOriginal = false;
                    }
                }
            }
        }

        private static bool IsDuelContract(this Contract contract) =>
            contract.Override.ID.StartsWith("SoloDuel") ||
            contract.Override.ID.StartsWith("DuoDuel");
    }
}