using System;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Core.Data
{
    public static class HeatSinkData
    {
        public enum HeatSinkType
        {
            Single,
            Double,
            ClanDouble
        }

        public struct HeatSinkInfo
        {
            public HeatSinkType Type;
            public string Name;
            public string Abbreviation;
            public string InternalDefID;
            public string ExternalDefID;
            public DateTime IntroDate;
            public int Dissipation;
            public int Slots;
        }

        public static Dictionary<HeatSinkType, HeatSinkInfo> HeatSinkTypes = new()
        {
            { HeatSinkType.Single, new HeatSinkInfo {
                Type = HeatSinkType.Single,
                Name = "Single",
                Abbreviation = "SHS",
                InternalDefID = "Gear_HeatSink_Internal_Standard",
                ExternalDefID = "Gear_HeatSink_Generic_Standard",
                IntroDate = DateTime.MinValue,
                Dissipation = 3,
                Slots = 1
            } },
            { HeatSinkType.Double, new HeatSinkInfo {
                Type = HeatSinkType.Double,
                Name = "Double",
                Abbreviation = "DHS",
                InternalDefID = "Gear_HeatSink_Internal_Double",
                ExternalDefID = "Gear_HeatSink_Generic_Double",
                IntroDate = DateTime.MinValue,
                Dissipation = 6,
                Slots = 3
            } },
            { HeatSinkType.ClanDouble, new HeatSinkInfo {
                Type = HeatSinkType.ClanDouble,
                Name = "Clan Double",
                Abbreviation = "cDHS",
                InternalDefID = "Gear_HeatSink_Internal_Double_Clan",
                ExternalDefID = "Gear_HeatSink_Clan_Double",
                IntroDate = new DateTime(3049, 8, 1),
                Dissipation = 6,
                Slots = 2
            } }
        };
    }
}