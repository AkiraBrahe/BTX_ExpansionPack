using System;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Core.Data
{
    public static class StructureData
    {
        public enum StructureType
        {
            Standard,
            Primitive,
            Industrial,
            EndoSteel,
            ClanEndoSteel,
            Composite,
            Reinforced
        }

        public struct StructureInfo
        {
            public StructureType Type;
            public string Name;
            public string Tag;
            public string ScrapItemDefID;
            public int CriticalSlots;
            public DateTime IntroDate;
            public DateTime ProductionDate;
            public float WeightMultiplier;
            public float TPCost;
            public float CBCost;
        }

        public static Dictionary<StructureType, StructureInfo> StructureTypes = new()
        {
            { StructureType.Standard, new StructureInfo {
                Type = StructureType.Standard,
                Name = "Standard",
                Tag = string.Empty,
                ScrapItemDefID = string.Empty,
                CriticalSlots = 0,
                IntroDate = DateTime.MinValue,
                ProductionDate = DateTime.MinValue,
                WeightMultiplier = 1f,
                TPCost = 1f,
                CBCost = 1f // 4,000 C-Bills per ton
            } },
            { StructureType.Primitive, new StructureInfo {
                Type = StructureType.Primitive,
                Name = "Primitive",
                Tag = "chassis_primitive",
                ScrapItemDefID = string.Empty,
                CriticalSlots = 0,
                IntroDate = DateTime.MinValue,
                ProductionDate = DateTime.MinValue,
                WeightMultiplier = 1f,
                TPCost = 1f,
                CBCost = 0.75f // 3,000 C-Bills per ton
            } },
            { StructureType.Industrial, new StructureInfo {
                Type = StructureType.Industrial,
                Name = "Industrial",
                Tag = "chassis_industrial",
                ScrapItemDefID = string.Empty,
                CriticalSlots = 0,
                IntroDate = DateTime.MinValue,
                ProductionDate = DateTime.MinValue,
                WeightMultiplier = 2f,
                TPCost = 1f,
                CBCost = 0.75f // 3,000 C-Bills per ton
            } },
            { StructureType.EndoSteel, new StructureInfo {
                Type = StructureType.EndoSteel,
                Name = "Endo Steel",
                Tag = "chassis_endo",
                ScrapItemDefID = string.Empty,
                CriticalSlots = 12,
                IntroDate = new DateTime(3035, 1, 1),
                ProductionDate = new DateTime(3040, 1, 1),
                WeightMultiplier = 0.5f,
                TPCost = 2f,
                CBCost = 8f // 32,000 C-Bills per ton (96,000 C-Bills per ton before 3040)
            } },
            { StructureType.ClanEndoSteel, new StructureInfo {
                Type = StructureType.ClanEndoSteel,
                Name = "Clan Endo Steel",
                Tag = string.Empty,
                ScrapItemDefID = string.Empty,
                CriticalSlots = 6,
                IntroDate = new DateTime(3049, 8, 1),
                ProductionDate = DateTime.MaxValue,
                WeightMultiplier = 0.5f,
                TPCost = 2f, // 3x cost with default settings
                CBCost = 8f // 48,000 C-Bills per ton with default settings
            } },
            { StructureType.Composite, new StructureInfo {
                Type = StructureType.Composite,
                Name = "Composite",
                Tag = "chassis_composite",
                ScrapItemDefID = string.Empty,
                CriticalSlots = 0,
                IntroDate = new DateTime(3054, 1, 1),
                ProductionDate = new DateTime(3061, 1, 1),
                WeightMultiplier = 0.5f,
                TPCost = 2f,
                CBCost = 8f // 32,000 C-Bills per ton
            } },
            { StructureType.Reinforced, new StructureInfo {
                Type = StructureType.Reinforced,
                Name = "Reinforced",
                Tag = "chassis_reinforced",
                ScrapItemDefID = string.Empty,
                CriticalSlots = 0,
                IntroDate = new DateTime(3055, 1, 1),
                ProductionDate = new DateTime(3057, 1, 1),
                WeightMultiplier = 2f,
                TPCost = 2f,
                CBCost = 8f // 32,000 C-Bills per ton
            } }
        };
        // Note: Costs are scaled for a 100-ton mech.
    }
}