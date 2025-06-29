using BattleTech;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Fixes
{
    internal class PilotSpecialization
    {
        [HarmonyPatch(typeof(PilotGenerator), "GeneratePilots")]
        public static class PilotGenerator_GeneratePilots
        {
            [HarmonyPostfix]
            public static void Postfix(List<PilotDef> __result)
            {
                foreach (PilotDef pilot in __result)
                {
                    if (pilot.PilotTags.Contains("pilot_nomech_crew") && !pilot.PilotTags.Contains("pilot_vehicle_crew"))
                    {
                        pilot.PilotTags.Add("pilot_vehicle_crew");
                        pilot.PilotTags.Remove("pilot_mech_pilot");
                    }
                    else if (!pilot.PilotTags.Contains("pilot_mech_pilot") && !pilot.PilotTags.Contains("pilot_vehicle_crew"))
                    {
                        if (UnityEngine.Random.Range(0, 5) == 0)
                        {
                            pilot.PilotTags.Add("pilot_mech_pilot");
                        }
                        else
                        {
                            pilot.PilotTags.Add("pilot_vehicle_crew");
                        }
                    }
                }
            }
        }
    }
}
