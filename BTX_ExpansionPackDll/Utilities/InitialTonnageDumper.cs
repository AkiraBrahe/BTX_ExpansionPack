using BattleTech;
using CustomUnits;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BTX_ExpansionPack.Utilities
{
    public static class InitialTonnageDumper
    {

        [HarmonyPatch(typeof(SimGameState), "SetSimRoomState")]
        public static class SimGameState_SetSimRoomState
        {
            public static void Prefix(SimGameState __instance, DropshipLocation state)
            {
                if (state == DropshipLocation.MECH_BAY && Input.GetKey(KeyCode.LeftShift))
                {
                    ExportInitialTonnage(__instance);
                }
            }
        }

        public static void ExportInitialTonnage(SimGameState simGame)
        {
            try
            {
                string filePath = Path.Combine(Main.modDir, "InitialTonnageDump.csv");
                Main.Logger.Log($"[InitialTonnageDumper] Starting dump to {filePath}");

                var processedChassis = new HashSet<string>();
                int count = 0;

                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("ChassisID;VariantName;MaxTonnage;CalculatedInitialTonnage;CurrentInitialTonnage;Difference;HeatRating;RangeRating");

                    foreach (var kv in simGame.DataManager.MechDefs)
                    {
                        var mechDef = kv.Value;

                        if (mechDef.Chassis == null || mechDef.Description == null) continue;
                        if (mechDef.IsVehicle()) continue;
                        if (processedChassis.Contains(mechDef.ChassisID)) continue;

                        var chassis = mechDef.Chassis;

                        long componentsKG = 0;
                        foreach (var item in mechDef.Inventory)
                        {
                            if (item.Def == null) continue;
                            componentsKG += (long)Math.Round(item.Def.Tonnage * 1000.0f);
                        }

                        long armorPoints = GetArmorPointsTotal(mechDef);
                        long kgperpoint = GetKGPerPoint(chassis);
                        long armorWeightKG = armorPoints * 10L / kgperpoint;

                        long maxWeightKG = (long)Math.Round(chassis.Tonnage * 1000.0f);

                        long calculatedInitialTonnageKG = maxWeightKG - componentsKG - armorWeightKG;
                        double calculatedInitialTonnage = calculatedInitialTonnageKG / 1000.0;

                        float heatRating = 0f; float rangeRating = 0f; float maxRating = 10f;
                        MechStatisticsRules.CalculateHeatEfficiencyStat(mechDef, ref heatRating, ref maxRating);
                        MechStatisticsRules.CalculateRangeStat(mechDef, ref rangeRating, ref maxRating);

                        writer.WriteLine($"{chassis.Description.Id};{chassis.VariantName};{chassis.Tonnage};{calculatedInitialTonnage:F4};{chassis.InitialTonnage:F4};{calculatedInitialTonnage - chassis.InitialTonnage:F4};{heatRating:F4};{rangeRating:F4}");

                        processedChassis.Add(mechDef.ChassisID);
                        count++;
                    }
                }
                Main.Logger.Log($"[InitialTonnageDumper] Successfully dumped {count} chassis to {filePath}");
            }
            catch (Exception ex)
            {
                Main.Logger.LogException(ex);
            }
        }

        private static long GetArmorPointsTotal(MechDef m)
        {
            return GetArmorPointsInternal(m.Head.AssignedArmor) + GetArmorPointsInternal(m.CenterTorso.AssignedArmor) + GetArmorPointsInternal(m.CenterTorso.AssignedRearArmor)
                + GetArmorPointsInternal(m.LeftTorso.AssignedArmor) + GetArmorPointsInternal(m.LeftTorso.AssignedRearArmor)
                + GetArmorPointsInternal(m.RightTorso.AssignedArmor) + GetArmorPointsInternal(m.RightTorso.AssignedRearArmor)
                + GetArmorPointsInternal(m.LeftArm.AssignedArmor) + GetArmorPointsInternal(m.RightArm.AssignedArmor)
                + GetArmorPointsInternal(m.LeftLeg.AssignedArmor) + GetArmorPointsInternal(m.RightLeg.AssignedArmor);
        }

        private static long GetArmorPointsInternal(float armorValue) => (long)Math.Round(armorValue * 1000.0f);

        private static long GetKGPerPoint(ChassisDef c)
        {
            try
            {
                if (c == null)
                {
                    Main.Logger.LogError("[InitialTonnageDumper] GetKGPerPoint: ChassisDef is null");
                    return 800;
                }
                else if (c.ChassisTags == null || c.ChassisTags.Count == 0)
                {
                    Main.Logger.LogError($"[InitialTonnageDumper] GetKGPerPoint: ChassisDef {c.Description.Id} has no ChassisTags");
                    return 800;
                }

                if (c.ChassisTags.Contains("chassis_ferro"))
                    return c.ChassisTags.Contains("chassis_clan") ? 960 : 896;

                foreach (string tag in c.ChassisTags)
                {
                    var match = ArmorTypes.FirstOrDefault(at => at.Value.Tag == tag);
                    return (long)(800 * match.Value.PptMultiplier);
                }

                return 800;
            }
            catch (Exception ex)
            {
                Main.Logger.LogException(ex);
                return 800;
            }
        }
    }
}