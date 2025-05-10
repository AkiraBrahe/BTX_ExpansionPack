using System;
using System.Collections.Generic;
using System.Reflection;
using BattleTech;
using BattleTech.UI;
using HarmonyLib;
using Localize;

namespace BTX_ExpansionPack.Fixes
{
    internal class NoBiome
    {
        [HarmonyPatch(typeof(LanceConfiguratorPanel), "OnConfirmClicked")]
        public static class LanceConfiguratorPanel_OnConfirmClicked
        {
            [HarmonyPrefix]
            [HarmonyBefore("io.mission.customunits")]
            public static bool Prefix(ref bool __runOriginal, LanceConfiguratorPanel __instance)
            {
                if (__instance?.sim != null && __instance.activeContract != null)
                {
                    List<string> blockedUnits = new List<string>();
                    string forbidTag = "NoBiome_" + __instance.activeContract.ContractBiome.ToString();
                    bool badBiome = false;

                    FieldInfo loadoutSlotsField = typeof(LanceConfiguratorPanel).GetField("loadoutSlots", BindingFlags.Instance | BindingFlags.NonPublic);
                    LanceLoadoutSlot[] loadoutSlots = (LanceLoadoutSlot[])loadoutSlotsField?.GetValue(__instance);

                    if (loadoutSlots != null)
                    {
                        foreach (LanceLoadoutSlot slot in loadoutSlots)
                        {
                            if (slot?.SelectedMech?.MechDef != null)
                            {
                                string unitName = slot.SelectedMech.MechDef.Name;
                                bool unitIsBadBiome = false;

                                if (slot.SelectedMech.MechDef.MechTags.Contains(forbidTag))
                                {
                                    if (!blockedUnits.Contains(unitName))
                                    {
                                        blockedUnits.Add(unitName);
                                    }

                                    Main.Log.LogDebug($"[NoBiomeFix] {unitName} has forbidden tag: {forbidTag}, preventing deployment.");
                                    unitIsBadBiome = true;
                                }
                                else
                                {
                                    foreach (MechComponentRef component in slot.SelectedMech.MechDef.Inventory)
                                    {
                                        if (component?.Def?.ComponentTags?.Contains(forbidTag) == true)
                                        {
                                            if (!blockedUnits.Contains(unitName))
                                            {
                                                blockedUnits.Add(unitName);
                                            }

                                            Main.Log.LogDebug($"[NoBiomeFix] {unitName} has {component.Def.Description.Name} with forbidden tag: {forbidTag}, preventing deployment.");
                                            unitIsBadBiome = true;
                                            break;
                                        }
                                    }
                                }
                                if (unitIsBadBiome)
                                {
                                    badBiome = true;
                                }
                            }
                        }

                        if (badBiome)
                        {
                            FieldInfo interruptQueueField = typeof(SimGameState).GetField("interruptQueue", BindingFlags.Instance | BindingFlags.NonPublic);
                            SimGameInterruptManager interruptQueue = (SimGameInterruptManager)interruptQueueField?.GetValue(__instance.sim);

                            Text text = new Text("__/DROP.BAD_BIOME.TITLE/__", string.Join(", ", blockedUnits));
                            Text message = new Text("__/DROP.BAD_BIOME.MESSAGE/__", Array.Empty<object>());

                            interruptQueue.QueuePauseNotification(text.ToString(true), message.ToString(true), __instance.sim.GetCrewPortrait(SimGameCrew.Crew_Yang), "notification_mechreadycomplete", delegate
                            {
                            }, "Continue", null, null);
                            __runOriginal = false;
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}