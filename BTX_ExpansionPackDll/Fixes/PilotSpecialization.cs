using BattleTech;
using CustomUnits;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    internal class PilotSpecialization
    {
        private const string MechPilotTag = "pilot_mech_pilot";
        private const string VehiclePilotTag = "pilot_vehicle_crew";
        private const string NoMechPilotTag = "pilot_nomech_crew";

        [HarmonyPatch(typeof(SimGameState), "FirstTimeInitializeDataFromDefs")]
        public static class SimGameState_FirstTimeInitializeDataFromDefs
        {
            [HarmonyPostfix]
            [HarmonyAfter("io.mission.customunits")]
            public static void Postfix(SimGameState __instance)
            {
                bool isInTeamVenom = __instance.CompanyTags.Contains("start_team_venom");
                if (isInTeamVenom)
                {
                    PilotDef commander = __instance.Commander.pilotDef;
                    if (commander != null)
                    {
                        if (!commander.PilotTags.Contains(MechPilotTag))
                            commander.PilotTags.Add(MechPilotTag);
                        if (!commander.PilotTags.Contains(VehiclePilotTag))
                            commander.PilotTags.Add(VehiclePilotTag);
                    }

                    while (__instance.PilotRoster.Count > 0)
                    {
                        __instance.PilotRoster.RemoveAt(0);
                    }
                }
                else
                {
                    foreach (Pilot pilot in __instance.PilotRoster)
                    {
                        var pilotTags = pilot.pilotDef.PilotTags;
                        ValidatePilotSpecialization(pilotTags);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PilotGenerator), "GeneratePilots")]
        public static class PilotGenerator_GeneratePilots
        {
            [HarmonyPostfix]
            public static void Postfix(List<PilotDef> __result)
            {
                if (PilotingClassHelper.isInStartingPilotsGen())
                    return;

                foreach (PilotDef pilot in __result)
                {
                    var pilotTags = pilot.PilotTags;
                    ValidatePilotSpecialization(pilotTags, true);
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
                if (pilotsChecked) return;
                pilotsChecked = true;

                var allPilots = __instance.PilotRoster.Concat([__instance.Commander]);
                foreach (var pilot in allPilots)
                {
                    var pilotTags = pilot.pilotDef.PilotTags;
                    if (pilotTags.Any(tag => tag.StartsWith("can_pilot_")))
                    {
                        if (pilotTags.Contains("can_pilot_generic_vehicle") && !pilotTags.Contains(VehiclePilotTag))
                            pilotTags.Add(VehiclePilotTag);

                        var tagsToRemove = pilotTags.Where(tag => tag.StartsWith("can_pilot_")).ToList();
                        foreach (var tag in tagsToRemove)
                        {
                            pilotTags.Remove(tag);
                        }

                        if (!pilotTags.Contains(MechPilotTag))
                            pilotTags.Add(MechPilotTag);
                    }

                    ValidatePilotSpecialization(pilotTags);
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

        private static void ValidatePilotSpecialization(HBS.Collections.TagSet pilotTags, bool randomize = false)
        {
            bool hasMechSpecialization = pilotTags.Contains(MechPilotTag);
            bool hasVehicleSpecialization = pilotTags.Contains(VehiclePilotTag);

            if (hasMechSpecialization && pilotTags.Contains(NoMechPilotTag))
                pilotTags.Remove(NoMechPilotTag);

            if (!hasMechSpecialization && !hasVehicleSpecialization)
            {
                if (randomize)
                {
                    pilotTags.Add(UnityEngine.Random.Range(0, 5) == 0 ? MechPilotTag : VehiclePilotTag);
                }
                else
                {
                    pilotTags.Add(MechPilotTag);
                }
            }
        }
    }
}
