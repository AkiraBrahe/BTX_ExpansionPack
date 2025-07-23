using CustomUnits;
using System;
using System.Collections.Generic;
using UnityEngine;
using BattleTech.UI;

namespace BTX_ExpansionPack.Utilities
{
    internal class MechSizeCalculator
    {
        private static readonly HashSet<string> LoggedChassis = [];

        [HarmonyPatch(typeof(CustomMechRepresentation), "Init")]
        public static class CustomMechRepresentation_Init 
        {
            [HarmonyPostfix]
            public static void Postfix(CustomMech mech)
            {
                if (!Main.Settings.Debug.MechSizeLogging || mech == null || mech.MechDef == null || mech.MechDef.Chassis == null)
                    return;

                string chassisName = mech.MechDef.Chassis.Description.Name;
                if (!LoggedChassis.Add(chassisName))
                    return;

                float tonnage = mech.MechDef.Chassis.Tonnage;
                float height = 0f;
                float volume = 0f;

                try
                {
                    var renderers = mech.GameRep?.gameObject?.GetComponentsInChildren<Renderer>();
                    if (renderers != null && renderers.Length > 0)
                    {
                        Bounds bounds = renderers[0].bounds;
                        for (int i = 1; i < renderers.Length; i++)
                            bounds.Encapsulate(renderers[i].bounds);

                        height = bounds.size.y;
                        volume = bounds.size.x * bounds.size.y * bounds.size.z;
                    }
                }
                catch (Exception ex)
                {
                    Main.Log.LogDebug($"Error calculating bounds for {chassisName}: {ex}");
                }

                Main.Log.LogDebug($"Mech: {chassisName}, Tonnage: {tonnage}, Height: {height:F2}, Volume: {volume:F2}");
            }
        }

        [HarmonyPatch(typeof(SGRoomController_MechBay), "EnterRoom")]
        public static class SGRoomController_MechBay_EnterRoom 
        {
            private static bool logged = false;

            [HarmonyPostfix]
            public static void Postfix(SGRoomController_MechBay __instance)
            {
                if (!Main.Settings.Debug.MechSizeLogging || __instance == null)
                    return;

                if (logged) return;
                logged = true;

                string[] humanModels = [
                    "chrPrfCrew_backgroundActor_mechF",
                    "chrPrfCrew_backgroundActor_mechM",
                    "chrPrfCrew_backgroundActor_welder",
                    "chrPrfCrew_backgroundActor_welder (1)",
                    "chrPrfCrew_backgroundActor_welder (2)"
                ];
                foreach (var name in humanModels)
                {
                    var go = GameObject.Find(name);
                    if (go != null)
                    {
                        var renderers = go.GetComponentsInChildren<Renderer>();
                        if (renderers.Length > 0)
                        {
                            Bounds bounds = renderers[0].bounds;
                            for (int i = 1; i < renderers.Length; i++)
                                bounds.Encapsulate(renderers[i].bounds);

                            float height = bounds.size.y;
                            float volume = bounds.size.x * bounds.size.y * bounds.size.z;
                            Main.Log.LogDebug($"Human: {name}, Height: {height:F2}, Volume: {volume:F2}");
                        }
                        else
                        {
                            Main.Log.LogDebug($"Human: {name} found, but no renderers.");
                        }
                    }
                    else
                    {
                        Main.Log.LogDebug($"Human: {name} not found in scene.");
                    }
                }
            }
        }
    }
}
