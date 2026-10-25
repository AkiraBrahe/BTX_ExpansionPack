using BattleTech;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Core
{
    public class Globals
    {
        #region Custom Tags
        // Custom tags used by Advanced MechLab.
        public const string ArmorPrefix = "AML_Armor_";
        public const string CoolingPrefix = "AML_Cooling_";
        public const string PatchworkPrefix = "AML_Patchwork_";

        #endregion

        #region Constants

        /// <summary>
        /// Returns a list of all difficulty tags for a pilot.
        /// </summary>
        public static readonly string[] DifficultyTags = ["pilot_npc_d1", "pilot_npc_d2", "pilot_npc_d3", "pilot_npc_d4", "pilot_npc_d5", "pilot_npc_d6", "pilot_npc_d7", "pilot_npc_d8", "pilot_npc_d9", "pilot_npc_d10"];

        /// <summary>
        /// Returns a list of all weight class tags for a unit.
        /// </summary>
        public static readonly string[] WeightClassTags = ["unit_light", "unit_medium", "unit_heavy", "unit_assault"];

        /// <summary>
        /// Defines the priority order for repairing mech locations, from most critical to least critical.
        /// </summary>
        public static Dictionary<int, ChassisLocations> RepairPriorities = new()
        {
            { 0, ChassisLocations.CenterTorso },
            { 1, ChassisLocations.Head },
            { 2, ChassisLocations.LeftTorso },
            { 3, ChassisLocations.RightTorso },
            { 4, ChassisLocations.LeftLeg },
            { 5, ChassisLocations.RightLeg },
            { 6, ChassisLocations.LeftArm },
            { 7, ChassisLocations.RightArm }
        };

        /// <summary>
        /// Returns a list of all valid chassis locations for a mech.
        /// </summary>
        public static readonly ChassisLocations[] AllLocations = [
            ChassisLocations.Head,
            ChassisLocations.LeftArm,
            ChassisLocations.LeftTorso,
            ChassisLocations.CenterTorso,
            ChassisLocations.RightTorso,
            ChassisLocations.RightArm,
            ChassisLocations.LeftLeg,
            ChassisLocations.RightLeg
        ];

        /// <summary>
        /// Returns a list of all side locations (arms, torsos, legs) for a mech.
        /// </summary>
        public static readonly ChassisLocations[] SideLocations = [
            ChassisLocations.LeftArm, ChassisLocations.RightArm,
            ChassisLocations.LeftTorso, ChassisLocations.RightTorso,
            ChassisLocations.LeftLeg, ChassisLocations.RightLeg
        ];

        /// <summary>
        /// Returns a list of core locations (center torso, head) for a mech.
        /// </summary>
        public static readonly ChassisLocations[] CoreLocations = [
            ChassisLocations.CenterTorso,
            ChassisLocations.Head
        ];

        #endregion

        #region Factions
        
        /// <summary>
        /// Returns a list of all periphery factions.
        /// </summary>
        public static readonly HashSet<string> PeripheryFactions = [
            "AuriganDirectorate", "AuriganRestoration", "Calderon", "Circinus", "Elysia",
            "Illyrian", "Lothian", "MagistracyOfCanopus", "Marian", "NewColonyRegion",
            "Oberon", "Outworld", "Rim", "TaurianConcordat", "Tortuga", "Valkyrate"
        ];
        
        #endregion
    }
}