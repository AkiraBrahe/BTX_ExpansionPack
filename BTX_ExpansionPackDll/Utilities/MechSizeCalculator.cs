using BattleTech.UI;
using CustomUnits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BTX_ExpansionPack.Utilities
{
    internal class MechSizeCalculator
    {
        private const float UnityUnitToMeter = 0.91f;
        private static readonly HashSet<string> LoggedChassis = [];

        [HarmonyPatch(typeof(CustomMechRepresentation), "Init")]
        public static class CustomMechRepresentation_Init
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.Debug.MechSizeLogging;

            [HarmonyPostfix]
            public static void Postfix(CustomMech mech)
            {
                if (mech == null || mech.MechDef == null || mech.MechDef.Chassis == null)
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

                        height = bounds.size.y; // * UnityUnitToMeter;
                        volume = bounds.size.x * bounds.size.y * bounds.size.z; // * (float)Math.Pow(UnityUnitToMeter, 3);
                    }
                }
                catch (Exception ex)
                {
                    Main.Log.LogDebug($"Error calculating bounds for {chassisName}: {ex}");
                }

                Main.Log.LogDebug($"Mech: {chassisName}, Tonnage: {tonnage} tons, Height: {height:F2}m, Volume: {volume:F2}m³");
            }
        }

        [HarmonyPatch(typeof(SGRoomController_MechBay), "EnterRoom")]
        public static class SGRoomController_MechBay_EnterRoom
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.Debug.MechSizeLogging;

            private static bool logged = false;

            [HarmonyPostfix]
            public static void Postfix(SGRoomController_MechBay __instance)
            {
                if (logged || __instance == null) return;
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

                            float height = bounds.size.y; // * UnityUnitToMeter;
                            float volume = bounds.size.x * bounds.size.y * bounds.size.z; // * (float)Math.Pow(UnityUnitToMeter, 3);
                            Main.Log.LogDebug($"Human: {name}, Height: {height:F2}m, Volume: {volume:F2}m³");

                            Vector3 size = Vector3.one;
                            Gizmos.color = Color.red;
                            Gizmos.DrawCube(renderers[0].transform.position, size);
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
