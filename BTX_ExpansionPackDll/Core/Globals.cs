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
        /// Defines the priority order for repairing mech locations, from most critical to least critical.
        /// </summary>
        public static Dictionary<int, ChassisLocations> repairPriorities = new()
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
        public static readonly ChassisLocations[] allLocations = [
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
        public static readonly ChassisLocations[] sideLocations = [
            ChassisLocations.LeftArm, ChassisLocations.RightArm,
            ChassisLocations.LeftTorso, ChassisLocations.RightTorso,
            ChassisLocations.LeftLeg, ChassisLocations.RightLeg
        ];

        /// <summary>
        /// Returns a list of core locations (center torso, head) for a mech.
        /// </summary>
        public static readonly ChassisLocations[] coreLocations = [
            ChassisLocations.CenterTorso,
            ChassisLocations.Head
        ];

        #endregion
    }
}
