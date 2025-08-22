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
                    var renderer = mech.GameRep?.gameObject?.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Bounds bounds = renderer.bounds;
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
                        var renderer = go.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            Bounds bounds = renderer.bounds;
                            var height = bounds.size.y; // * UnityUnitToMeter;
                            var volume = bounds.size.x * bounds.size.y * bounds.size.z; // * (float)Math.Pow(UnityUnitToMeter, 3);
                            Main.Log.LogDebug($"Human: {name}, Height: {height:F2}m, Volume: {volume:F2}m³");
                        }
                    }
                }
            }
        }
    }
}
