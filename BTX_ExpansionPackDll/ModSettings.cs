﻿namespace BTX_ExpansionPack
{
    public class ModSettings
    {
        public ModDebugSettings Debug { get; set; } = new ModDebugSettings();
        public GameplaySettings Gameplay { get; set; } = new GameplaySettings();
        public UISettings UI { get; set; } = new UISettings();
    }
    public class ModDebugSettings
    {
        public bool AllDropShipUpgrades { get; set; } = false;
        public bool SaveBetweenConsecutiveDrops { get; set; } = false;
        public bool PirateSystemLogging { get; set; } = false;
        public bool MechSizeLogging { get; set; } = false;
    }

    public class GameplaySettings
    {
        public bool Use4LimitOnStoryMissions { get; set; } = true;
        public bool AllowVehiclesInMechDuels { get; set; } = false;
        public bool OverrideDHSEngineCooling { get; set; } = true;
        public double DHSEngineCoolingMultiplier { get; set; } = 1.5;
        public bool DisableNonStandardAmmoBins { get; set; } = false;
    }

    public class UISettings
    {
        public IntelSettings ContractIntel { get; set; } = new IntelSettings();
        public MechTooltipSettings MechTooltips { get; set; } = new MechTooltipSettings();
    }

    public class IntelSettings
    {
        public bool IntelShowTarget { get; set; } = true;
        public bool IntelShowVariant { get; set; } = true;
    }

    public class MechTooltipSettings
    {
        public bool UseDefaultColors { get; set; } = false;
    }
}
