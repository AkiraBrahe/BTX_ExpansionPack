using CustomComponents;

namespace BTX_ExpansionPack.Core
{
    /// <summary>
    /// Stores the stock configuration and heat sink counts for a chassis.
    /// This component is persistent on ChassisDef and used as a master reference.
    /// </summary>
    [CustomComponent("AdvancedChassisData")]
    public class AdvancedChassisData : SimpleCustomChassis
    {
        public DefaultsInfoRecord[] StockBlockers { get; set; } = [];
        public int BaseHSCount { get; set; }
        public int ExtraHSCount { get; set; }
    }
}