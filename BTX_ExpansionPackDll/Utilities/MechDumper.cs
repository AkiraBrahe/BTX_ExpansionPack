using BattleTech;
using CustomUnits;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BTX_ExpansionPack.Utilities
{
    public static class MechDumper
    {

        [HarmonyPatch(typeof(SimGameState), "SetSimRoomState")]
        public static class SimGameState_SetSimRoomState
        {
            public static void Prefix(SimGameState __instance, DropshipLocation state)
            {
                if (state == DropshipLocation.MECH_BAY && Input.GetKey(KeyCode.LeftShift))
                {
                    ExportMechStats(__instance);
                }
            }
        }

        public static void ExportMechStats(SimGameState simGame)
        {
            try
            {
                string filePath = Path.Combine(Main.modDir, "misc", "scripts", "mech_stats.csv");
                Main.Logger.Log($"[MechDumper] Starting dump...");

                var processedChassis = new HashSet<string>();
                int count = 0;

                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("MechID,VariantName,MaxTonnage,CalculatedInitialTonnage,CurrentInitialTonnage,Difference,HeatRating,RangeRating");

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

                        var armor = mechDef.GetArmorInfo();
                        long armorPoints = GetArmorPointsTotal(mechDef);
                        long kgperpoint = (long)(800 * armor.PptMultiplier);

                        long armorWeightKG = armorPoints * 10L / kgperpoint;
                        long maxWeightKG = (long)Math.Round(chassis.Tonnage * 1000.0f);

                        long calculatedInitialTonnageKG = maxWeightKG - componentsKG - armorWeightKG;
                        double calculatedInitialTonnage = calculatedInitialTonnageKG / 1000.0;

                        float heatRating = 0f; float rangeRating = 0f; float maxRating = 10f;
                        MechStatisticsRules.CalculateHeatEfficiencyStat(mechDef, ref heatRating, ref maxRating);
                        MechStatisticsRules.CalculateRangeStat(mechDef, ref rangeRating, ref maxRating);

                        writer.WriteLine(
                            $"{chassis.Description.Id.Replace("chassisdef_", "mechdef_")}," +
                            $"{chassis.VariantName}," +
                            $"{chassis.Tonnage}," +
                            $"{calculatedInitialTonnage:F3}," +
                            $"{chassis.InitialTonnage:F3}," +
                            $"{calculatedInitialTonnage - chassis.InitialTonnage:F3}," +
                            $"{heatRating:F1}," +
                            $"{rangeRating:F1}"
                        );

                        processedChassis.Add(mechDef.ChassisID);
                        count++;
                    }
                }
                Main.Logger.Log($"[MechDumper] Dumped {count} chassis to mech_stats.csv!");
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

            static long GetArmorPointsInternal(float armorValue) => (long)Math.Round(armorValue * 1000.0f);
        }
    }
}
