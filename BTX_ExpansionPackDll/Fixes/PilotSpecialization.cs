using BattleTech;
using BattleTech.UI;
using System.Collections.Generic;

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

        [HarmonyPatch(typeof(SGBarracksMWDetailPanel), "DisplayPilot")]
        public static class SGBarracksMWDetailPanel_DisplayPilot
        {
            [HarmonyPostfix]
            public static void Postfix(Pilot p)
            {
                var pilot = p;
                if (pilot == null || pilot.pilotDef == null || pilot.pilotDef.PilotTags == null)
                    return;

                bool hasMechSpecialization = pilot.pilotDef.PilotTags.Contains(MechPilotTag);
                bool hasVehicleSpecialization = pilot.pilotDef.PilotTags.Contains(VehiclePilotTag);

                if (hasMechSpecialization || hasVehicleSpecialization) return;
                Main.Log.LogDebug($"Pilot '{pilot.Callsign}' has no specialization. Assigning one.");
                pilot.pilotDef.PilotTags.Add(MechPilotTag);
            }
        }
    }
}
