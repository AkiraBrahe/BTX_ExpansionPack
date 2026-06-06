using System.Collections.Generic;

namespace BTX_ExpansionPack.Core.Data
{
    public static class ArmorData
    {
        public enum ArmorType
        {
            Standard,
            Primitive,
            Industrial,
            HeavyIndustrial,
            FerroFibrous,
            ClanFerroFibrous,
            Hardened,
            Stealth,
            LightFerro,
            HeavyFerro,
            Reflective,
            Reactive
        }

        public struct ArmorInfo
        {
            public ArmorType Type;
            public string Name;
            public string Description { get; set; }
            public string Tag { get; set; }
            public int CriticalSlots;
            public float PptMultiplier { get; set; }
        }

        public static readonly Dictionary<ArmorType, ArmorInfo> ArmorTypes = new()
        {
            { ArmorType.Standard, new ArmorInfo {
                Type = ArmorType.Standard,
                Name = "Standard",
                Description = "Provides reliable protection with no special benefits or drawbacks.",
                Tag = string.Empty,
                CriticalSlots = 0,
                PptMultiplier = 1f
            } },
            { ArmorType.Primitive, new ArmorInfo {
                Type = ArmorType.Primitive,
                Name = "Primitive",
                Description = "Provides 33% less protection per ton than standard armor.",
                Tag = "chassis_primitive",
                CriticalSlots = 0,
                PptMultiplier = 0.67f
            } },
            { ArmorType.Industrial, new ArmorInfo {
                Type = ArmorType.Industrial,
                Name = "Industrial",
                Description = "Provides 33% less protection per ton than standard armor.",
                Tag = "chassis_industrial",
                CriticalSlots = 0,
                PptMultiplier = 0.67f
            } },
            { ArmorType.HeavyIndustrial, new ArmorInfo {
                Type = ArmorType.HeavyIndustrial,
                Name = "Heavy Industrial",
                Description = "No benefits over standard armor.",
                Tag = "chassis_heavy_industrial",
                CriticalSlots = 0,
                PptMultiplier = 1f
            } },
            { ArmorType.FerroFibrous, new ArmorInfo {
                Type = ArmorType.FerroFibrous,
                Name = "Ferro-Fibrous",
                Description = "Provides 12% more protection per ton than standard armor.",
                Tag = "chassis_ferro",
                CriticalSlots = 12,
                PptMultiplier = 1.12f
            } },
            { ArmorType.ClanFerroFibrous, new ArmorInfo {
                Type = ArmorType.ClanFerroFibrous,
                Name = "Clan Ferro-Fibrous",
                Description = "Provides 20% more protection per ton than standard armor.",
                Tag = "chassis_clan_ferro",
                CriticalSlots = 6,
                PptMultiplier = 1.2f
            } },
            { ArmorType.LightFerro, new ArmorInfo {
                Type = ArmorType.LightFerro,
                Name = "Light Ferro-Fibrous",
                Description = "Provides 6% more protection per ton than standard armor and is more compact than standard ferro armor.",
                Tag = "chassis_light_ferro",
                CriticalSlots = 6,
                PptMultiplier = 1.06f
            } },
            { ArmorType.HeavyFerro, new ArmorInfo {
                Type = ArmorType.HeavyFerro,
                Name = "Heavy Ferro-Fibrous",
                Description = "Provides 24% more protection per ton than standard armor but is bulkier than standard ferro armor.",
                Tag = "chassis_heavy_ferro",
                CriticalSlots = 18,
                PptMultiplier = 1.24f
            } },
            { ArmorType.Hardened, new ArmorInfo {
                Type = ArmorType.Hardened,
                Name = "Hardened",
                Description = "Reduces incoming damage by 20% and prevents through-armor critical hits at the expense of mobility.",
                Tag = "chassis_hardened",
                CriticalSlots = 12,
                PptMultiplier = 1f
            } },
            { ArmorType.Stealth, new ArmorInfo {
                Type = ArmorType.Stealth,
                Name = "Stealth",
                Description = "Makes the 'Mech harder to detect and target as long as its ECM Suite is active.",
                Tag = "chassis_stealth",
                CriticalSlots = 12,
                PptMultiplier = 1f
            } },
            { ArmorType.Reactive, new ArmorInfo {
                Type = ArmorType.Reactive,
                Name = "Reactive",
                Description = "Reduces incoming missile and artillery damage by 50%.",
                Tag = "chassis_reactive",
                CriticalSlots = 12,
                PptMultiplier = 1f
            } },
            { ArmorType.Reflective, new ArmorInfo {
                Type = ArmorType.Reflective,
                Name = "Reflective",
                Description = "Reduces incoming energy damage by 50%, but increases incoming melee and artillery damage by 50%.",
                Tag = "chassis_reflective",
                CriticalSlots = 8,
                PptMultiplier = 1f
            } },
        };
    }
}