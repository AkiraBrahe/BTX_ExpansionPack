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
            public int CriticalSlots;
        }

        public static readonly Dictionary<StructureType, StructureInfo> StructureTypes = new()
        {
            { StructureType.Standard, new StructureInfo {
                Type = StructureType.Standard,
                Name = "Standard",
                Tag = string.Empty,
                CriticalSlots = 0,
            } },
            { StructureType.Primitive, new StructureInfo {
                Type = StructureType.Primitive,
                Name = "Primitive",
                Tag = "chassis_primitive",
                CriticalSlots = 0,
            } },
            { StructureType.Industrial, new StructureInfo {
                Type = StructureType.Industrial,
                Name = "Industrial",
                Tag = "chassis_industrial",
                CriticalSlots = 0,
            } },
            { StructureType.EndoSteel, new StructureInfo {
                Type = StructureType.EndoSteel,
                Name = "Endo Steel",
                Tag = "chassis_endo",
                CriticalSlots = 12,
            } },
            { StructureType.ClanEndoSteel, new StructureInfo {
                Type = StructureType.ClanEndoSteel,
                Name = "Clan Endo Steel",
                Tag = string.Empty,
                CriticalSlots = 6,
            } },
            { StructureType.Composite, new StructureInfo {
                Type = StructureType.Composite,
                Name = "Composite",
                Tag = "chassis_composite",
                CriticalSlots = 0,
            } },
            { StructureType.Reinforced, new StructureInfo {
                Type = StructureType.Reinforced,
                Name = "Reinforced",
                Tag = "chassis_reinforced",
                CriticalSlots = 0,
            } }
        };
    }
}