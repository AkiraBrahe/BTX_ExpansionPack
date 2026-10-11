using BattleTech;
using CustomUnits;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BTX_ExpansionPack.Utilities
{
    public static class MechDumper
    {

        /// <summary>
        /// Exports mech stats to a CSV file and checks for internal space issues (Shift + Click Mech Bay in dropship).
        /// </summary>
        [HarmonyPatch(typeof(SimGameState), "SetSimRoomState")]
        public static class SimGameState_SetSimRoomState
        {
            public static void Prefix(SimGameState __instance, DropshipLocation state)
            {
                if (state == DropshipLocation.MECH_BAY && Input.GetKey(KeyCode.LeftShift))
                {
                    // ExportMechStats(__instance); // Up-to-date mech stats are already exported
                    CheckInternalSpace(__instance);
                }
            }
        }

        /// <summary>
        /// Calculates mech stats (max tonnage, current tonnage, etc.) and exports them to a CSV file.
        /// </summary>
        /// <remarks>
        /// The output CSV is used to sync the initial tonnage and assign proper tags based on heat and range ratings.
        /// </remarks>
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
                    writer.WriteLine("Mech ID,Variant Name,Max Tonnage,Calculated Initial Tonnage,Current Initial Tonnage,Difference,Heat Rating,Range Rating");

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

            static long GetArmorPointsTotal(MechDef m)
            {
                return GetArmorPointsInternal(m.Head.AssignedArmor) + GetArmorPointsInternal(m.CenterTorso.AssignedArmor) + GetArmorPointsInternal(m.CenterTorso.AssignedRearArmor)
                    + GetArmorPointsInternal(m.LeftTorso.AssignedArmor) + GetArmorPointsInternal(m.LeftTorso.AssignedRearArmor)
                    + GetArmorPointsInternal(m.RightTorso.AssignedArmor) + GetArmorPointsInternal(m.RightTorso.AssignedRearArmor)
                    + GetArmorPointsInternal(m.LeftArm.AssignedArmor) + GetArmorPointsInternal(m.RightArm.AssignedArmor)
                    + GetArmorPointsInternal(m.LeftLeg.AssignedArmor) + GetArmorPointsInternal(m.RightLeg.AssignedArmor);

                static long GetArmorPointsInternal(float armorValue) => (long)Math.Round(armorValue * 1000.0f);
            }
        }

        /// <summary>
        /// Checks if mechs have negative internal space, or enough space for more heat sinks in their inventory.
        /// </summary>
        public static void CheckInternalSpace(SimGameState simGame)
        {
            try
            {
                foreach (var kv in simGame.DataManager.MechDefs)
                {
                    if (kv.Value.IsVehicle())
                        continue;

                    var mechDef = kv.Value;
                    var chassis = mechDef.Chassis;

                    bool hasDoubleHeatSinks = chassis.ChassisTags.Contains("chassis_DHS");
                    int heatSinkingPerHS = hasDoubleHeatSinks ? 6 : 3;
                    int heatSinkSize = hasDoubleHeatSinks ? chassis.ChassisTags.Contains("chassis_clan") ? 2 : 3 : 1;

                    // Adjust for BEX heat dissipation
                    int baseDissipation = 30 + chassis.Heatsinks;
                    int extraDissipation = hasDoubleHeatSinks ? baseDissipation - Extended_CE.Core.Settings.DHSEngineCooling : chassis.Heatsinks;
                    int extraHSCount = extraDissipation / heatSinkingPerHS;

                    if (extraHSCount <= 0)
                        continue;

                    var freeSlots = allLocations.ToDictionary(l => l, l => mechDef.GetFreeSlotsInLoc([.. mechDef.inventory], l, heatSinkSize));
                    int totalFreeSlots = freeSlots.Values.Sum();
                    int possibleHSCount = 0;

                    foreach (var loc in freeSlots)
                    {
                        if (loc.Value < 0)
                        {
                            Main.Logger.LogWarning($"[MechDumper] {mechDef.ChassisID} has negative free slots in {loc.Key}: {loc.Value}");
                        }
                        else if (extraHSCount > 0 && loc.Value >= heatSinkSize)
                        {
                            possibleHSCount++;
                        }
                    }

                    if (possibleHSCount > 0)
                    {
                        Main.Logger.Log($"[MechDumper] {mechDef.ChassisID} ({mechDef.Name}) has {extraHSCount} more {(hasDoubleHeatSinks ? "double" : "single")} heat sinks inside the engine.");
                        Main.Logger.Log($"[MechDumper] {mechDef.ChassisID} ({mechDef.Name}) has space for {possibleHSCount} {(hasDoubleHeatSinks ? "double" : "single")} heat sinks (total free slots: {totalFreeSlots}).");
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Logger.LogException(ex);
            }
        }
    }
}
