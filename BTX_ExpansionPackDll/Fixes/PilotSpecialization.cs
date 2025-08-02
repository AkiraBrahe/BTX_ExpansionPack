using BattleTech;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    internal class PilotSpecialization
    {
        private const string MechPilotTag = "pilot_mech_pilot";
        private const string VehiclePilotTag = "pilot_vehicle_crew";
        private const string NoMechPilotTag = "pilot_nomech_crew";

        [HarmonyPatch(typeof(PilotGenerator), "GeneratePilots")]
        public static class PilotGenerator_GeneratePilots
        {
            [HarmonyPostfix]
            public static void Postfix(List<PilotDef> __result)
            {
                foreach (PilotDef pilot in __result)
                {
                    if (pilot.PilotTags.Contains(NoMechPilotTag) && !pilot.PilotTags.Contains(VehiclePilotTag))
                    {
                        pilot.PilotTags.Add(VehiclePilotTag);
                        pilot.PilotTags.Remove(MechPilotTag);
                    }
                    else if (!pilot.PilotTags.Contains(MechPilotTag) && !pilot.PilotTags.Contains(VehiclePilotTag))
                    {
                        if (UnityEngine.Random.Range(0, 5) == 0)
                        {
                            pilot.PilotTags.Add(MechPilotTag);
                        }
                        else
                        {
                            pilot.PilotTags.Add(VehiclePilotTag);
                        }
                    }
                }
            }
        }

        private static bool pilotsChecked = false;

        [HarmonyPatch(typeof(SimGameState), "Rehydrate")]
        public static class SimGameState_Rehydrate
        {
            [HarmonyPostfix]
            public static void Postfix(SimGameState __instance)
            {
                if (__instance == null || __instance.PilotRoster == null)
                    return;

                if (pilotsChecked) return;
                pilotsChecked = true;

                foreach (var pilot in __instance.PilotRoster)
                {
                    bool hasMechSpecialization = pilot.pilotDef.PilotTags.Contains(MechPilotTag);
                    bool hasVehicleSpecialization = pilot.pilotDef.PilotTags.Contains(VehiclePilotTag);

                    if (pilot.pilotDef.PilotTags.Any(tag => tag.StartsWith("can_pilot_")))
                    {
                        if (pilot.pilotDef.PilotTags.Contains("can_pilot_generic_vehicle") && !hasVehicleSpecialization)
                            pilot.pilotDef.PilotTags.Add(VehiclePilotTag);

                        var tagsToRemove = pilot.pilotDef.PilotTags.Where(tag => tag.StartsWith("can_pilot_")).ToList();
                        foreach (var tag in tagsToRemove)
                        {
                            pilot.pilotDef.PilotTags.Remove(tag);
                        }

                        if (!pilot.pilotDef.PilotTags.Contains(MechPilotTag))
                            pilot.pilotDef.PilotTags.Add(MechPilotTag);
                    }

                    if (!hasMechSpecialization && !hasVehicleSpecialization)
                        pilot.pilotDef.PilotTags.Add(MechPilotTag);
                }
            }
        }

        [HarmonyPatch(typeof(SimGameState), "Dehydrate")]
        public static class SimGameState_Dehydrate
        {
            public static void Prefix()
            {
                pilotsChecked = false;
            }
        }
    }
}
