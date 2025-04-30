using System;
using System.Collections.Generic;
using System.Reflection;
using BattleTech;
using BattleTech.UI;
using CustAmmoCategories;
using CustomActivatableEquipment;
using CustomUnits;
using HarmonyLib;
using Localize;

namespace BTX_ExpansionPack
{
    internal class AMSAuraFix
    {
        [HarmonyPatch]
        public static class AuraBubble_GetRadius
        {
            public static MethodInfo TargetMethod()
            {
                return typeof(AuraBubble).GetMethod("GetRadius", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            [HarmonyPrefix]
            public static bool Replace(AuraBubble __instance, ref float __result)
            {
                if (__instance?.Def == null) { __result = 0f; return false; }
                if (__instance.owner == null) { __result = __instance.Def.Range; return false; }

                if (__instance.source is Weapon weapon)
                {
                    if (__instance.Def.Id == "AMS" || __instance.Def.Id == "LAMS" || __instance.Def.Id == "RFAMS")
                    {
                        if (weapon.isAMS())
                        {
                            __result = __instance.Def.Range;
                        }
                        else
                        {
                            __result = 0.1f;
                        }
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(__instance.Def.RangeStatistic))
                {
                    __result = __instance.Def.Range;
                    return false;
                }
                __result = __instance.owner.StatCollection.GetStatistic(__instance.Def.RangeStatistic).Value<float>();
                return false;
            }
        }
    }

    internal class NoBiomeFix
    {
        [HarmonyPatch(typeof(CustomMech), "AddToTeam")]
        public static class CustomMech_AddToTeam_Logging
        {
            [HarmonyPrefix]
            public static void Prefix(CustomMech __instance)
            {
                if (__instance != null && __instance.MechDef != null)
                {
                    Main.Log.LogDebug($"[NoBiomeFix] Checking tags for {__instance.MechDef.ChassisID}");
                    foreach (string tag in __instance.MechDef.MechTags)
                    {
                        Main.Log.LogDebug($"  Tag: {tag}");
                    }
                    Main.Log.LogDebug($"[NoBiomeFix] Current Biome: {__instance.Combat?.ActiveContract?.ContractBiome}");
                    Main.Log.LogDebug($"[NoBiomeFix] NoBiome Tag to Check: NoBiome_{__instance.Combat?.ActiveContract?.ContractBiome}");
                }
                else
                {
                    Main.Log.LogDebug($"[NoBiomeFix] __instance or __instance.MechDef is null!");
                }
            }
        }

        [HarmonyPatch(typeof(LanceConfiguratorPanel), "OnConfirmClicked")]
        public static class LanceConfiguratorPanel_OnConfirmClicked
        {
            [HarmonyPrefix]
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
                                    Main.Log.LogWarning($"[NoBiomeFix] {unitName} has forbidden tag: {forbidTag}, preventing deployment.");
                                    unitIsBadBiome = true;
                                }
                                foreach (MechComponentRef component in slot.SelectedMech.MechDef.Inventory)
                                {
                                    if (component?.Def?.ComponentTags?.Contains(forbidTag) == true)
                                    {
                                        if (!blockedUnits.Contains(unitName))
                                        {
                                            blockedUnits.Add(unitName);
                                        }
                                        Main.Log.LogWarning($"[LanceConfiguratorPanel_OnConfirmClicked] {unitName} has {component.Def.Description.Name} with forbidden tag: {forbidTag}, preventing deployment.");
                                        unitIsBadBiome = true;
                                        break;
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

                            Text text = new Text("__/DROP.BAD_BIOME.TITLE/__", Array.Empty<object>());
                            Text message = new Text("__/DROP.BAD_BIOME.MESSAGE/__", new object[] { string.Join(", ", blockedUnits) });

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