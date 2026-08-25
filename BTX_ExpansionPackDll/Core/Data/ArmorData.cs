using System;
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
            public string FullDescription;
            public string ShortDescription;
            public string Tag;
            public string ScrapItemDefID;
            public int CriticalSlots;
            public DateTime IntroDate;
            public DateTime ProductionDate;
            public float PptMultiplier;
            public float TPCost;
            public float CBCost;
        }

        public static Dictionary<ArmorType, ArmorInfo> ArmorTypes = new()
        {
            { ArmorType.Standard, new ArmorInfo {
                Type = ArmorType.Standard,
                Name = "Standard",
                FullDescription = "Standard armor provides reliable protection with no special benefits or drawbacks.",
                ShortDescription = "Provides reliable protection with no special benefits or drawbacks.",
                Tag = string.Empty,
                ScrapItemDefID = string.Empty,
                CriticalSlots = 0,
                IntroDate = DateTime.MinValue,
                ProductionDate = DateTime.MinValue,
                PptMultiplier = 1f,
                TPCost = 1f,
                CBCost = 1f // 10,000 C-Bills per ton
            } },
            { ArmorType.Primitive, new ArmorInfo {
                Type = ArmorType.Primitive,
                Name = "Primitive",
                FullDescription = "Primitive armor provides two-thirds the protection per ton of standard armor at half the cost.",
                ShortDescription = "Provides 33% less protection per ton than standard armor.",
                Tag = "chassis_primitive",
                ScrapItemDefID = string.Empty,
                CriticalSlots = 0,
                IntroDate = DateTime.MinValue,
                ProductionDate = DateTime.MinValue,
                PptMultiplier = 0.67f,
                TPCost = 1f,
                CBCost = 0.5f // 5,000 C-Bills per ton
            } },
            { ArmorType.Industrial, new ArmorInfo {
                Type = ArmorType.Industrial,
                Name = "Industrial",
                FullDescription = "Industrial armor provides two-thirds the protection per ton of standard armor at half the cost.",
                ShortDescription = "Provides 33% less protection per ton than standard armor.",
                Tag = "chassis_industrial",
                ScrapItemDefID = string.Empty,
                CriticalSlots = 0,
                IntroDate = DateTime.MinValue,
                ProductionDate = DateTime.MinValue,
                PptMultiplier = 0.67f,
                TPCost = 1f,
                CBCost = 0.5f // 5,000 C-Bills per ton
            } },
            { ArmorType.HeavyIndustrial, new ArmorInfo {
                Type = ArmorType.HeavyIndustrial,
                Name = "Heavy Industrial",
                FullDescription = "Heavy Industrial armor provides the same protection as standard armor.",
                ShortDescription = "No benefits over standard armor.",
                Tag = "chassis_heavy_industrial",
                ScrapItemDefID = string.Empty,
                CriticalSlots = 0,
                IntroDate = DateTime.MinValue,
                ProductionDate = DateTime.MinValue,
                PptMultiplier = 1f,
                TPCost = 1f,
                CBCost = 1f // 10,000 C-Bills per ton
            } },
            { ArmorType.FerroFibrous, new ArmorInfo {
                Type = ArmorType.FerroFibrous,
                Name = "Ferro-Fibrous",
                FullDescription = "Ferro-Fibrous armor provides 12% more protection per ton than standard armor and takes up 12 critical slots.",
                ShortDescription = "Provides 12% more protection per ton than standard armor.",
                Tag = "chassis_ferro",
                ScrapItemDefID = "Lootable_Armor_FerroFibrous",
                CriticalSlots = 12,
                IntroDate = DateTime.MinValue, // Reintroduced in 3034
                ProductionDate = new DateTime(3040, 1, 1),
                PptMultiplier = 1.12f,
                TPCost = 1.5f,
                CBCost = 2f // 20,000 C-Bills per ton (60,000 C-Bills per ton before 3040)
            } },
            { ArmorType.ClanFerroFibrous, new ArmorInfo {
                Type = ArmorType.ClanFerroFibrous,
                Name = "Clan Ferro-Fibrous",
                FullDescription = "Clan Ferro-Fibrous armor provides 20% more protection per ton than standard armor and only takes up 6 critical slots.",
                ShortDescription = "Provides 20% more protection per ton than standard armor.",
                Tag = string.Empty,
                ScrapItemDefID = "Lootable_Armor_ClanFerro",
                CriticalSlots = 6,
                IntroDate = new DateTime(3049, 8, 1),
                ProductionDate = DateTime.MaxValue,
                PptMultiplier = 1.2f,
                TPCost = 1.5f, // 2.25x cost with default settings
                CBCost = 2f // 30,000 C-Bills per ton with default settings
            } },
            { ArmorType.LightFerro, new ArmorInfo {
                Type = ArmorType.LightFerro,
                Name = "Light Ferro-Fibrous",
                FullDescription = "Light Ferro-Fibrous armor provides 6% more protection per ton than standard armor and takes up 6 critical slots.",
                ShortDescription = "Provides 6% more protection per ton than standard armor and is more compact than standard ferro armor.",
                Tag = "chassis_light_ferro",
                ScrapItemDefID = "Lootable_Armor_LightFerro",
                CriticalSlots = 6,
                IntroDate = new DateTime(3055, 1, 1),
                ProductionDate = new DateTime(3067, 1, 1),
                PptMultiplier = 1.06f,
                TPCost = 1.25f,
                CBCost = 1.5f // 15,000 C-Bills per ton
            } },
            { ArmorType.HeavyFerro, new ArmorInfo {
                Type = ArmorType.HeavyFerro,
                Name = "Heavy Ferro-Fibrous",
                FullDescription = "Heavy Ferro-Fibrous armor provides 24% more protection per ton than standard armor but takes up 18 critical slots.",
                ShortDescription = "Provides 24% more protection per ton than standard armor but is bulkier than standard ferro armor.",
                Tag = "chassis_heavy_ferro",
                ScrapItemDefID = "Lootable_Armor_HeavyFerro",
                CriticalSlots = 18,
                IntroDate = new DateTime(3056, 1, 1),
                ProductionDate = new DateTime(3069, 1, 1),
                PptMultiplier = 1.24f,
                TPCost = 1.75f,
                CBCost = 2.5f // 25,000 C-Bills per ton
            } },
            { ArmorType.Hardened, new ArmorInfo {
                Type = ArmorType.Hardened,
                Name = "Hardened",
                FullDescription = "Hardened armor reduces incoming damage by 20% and prevents through-armor criticals. When applied to the legs, movement speed is decreased by 15%. It takes up 12 critical slots.",
                ShortDescription = "Reduces incoming damage by 20% and prevents through-armor critical hits at the expense of mobility.",
                Tag = "chassis_hardened",
                ScrapItemDefID = "Lootable_Armor_Hardened",
                CriticalSlots = 12, // Damage reduction at the cost of internal bulk
                IntroDate = new DateTime(3047, 1, 1),
                ProductionDate = new DateTime(3081, 1, 1),
                PptMultiplier = 1f, // Simplified logic
                TPCost = 2.5f,
                CBCost = 1.5f // 15,000 C-Bills per ton
            } },
            { ArmorType.Stealth, new ArmorInfo {
                Type = ArmorType.Stealth,
                Name = "Stealth",
                FullDescription = "Stealth armor makes the 'Mech harder to detect and target as long as its ECM Suite is active and that the armor covers the entire chassis. It takes up 12 critical slots.",
                ShortDescription = "Makes the 'Mech harder to detect and target as long as its ECM Suite is active.",
                Tag = "chassis_stealth",
                ScrapItemDefID = "Lootable_Armor_Stealth",
                CriticalSlots = 12,
                IntroDate = new DateTime(3051, 1, 1),
                ProductionDate = new DateTime(3063, 1, 1),
                PptMultiplier = 1f,
                TPCost = 2f,
                CBCost = 5f // 50,000 C-Bills per ton
            } },
            { ArmorType.Reactive, new ArmorInfo {
                Type = ArmorType.Reactive,
                Name = "Reactive",
                FullDescription = "Reactive armor reduces incoming missile and AoE damage by 50%. It takes up 12 critical slots.",
                ShortDescription = "Reduces incoming missile and artillery damage by 50%.",
                Tag = "chassis_reactive",
                ScrapItemDefID = "Lootable_Armor_Reactive",
                CriticalSlots = 12,
                IntroDate = new DateTime(3063, 1, 1),
                ProductionDate = new DateTime(3081, 1, 1),
                PptMultiplier = 1f,
                TPCost = 2f,
                CBCost = 3f // 30,000 C-Bills per ton
            } },
            { ArmorType.Reflective, new ArmorInfo {
                Type = ArmorType.Reflective,
                Name = "Reflective",
                FullDescription = "Reflective armor reflects 50% of incoming energy damage while being 50% more vulnerable to melee and artillery attacks. It takes up 8 critical slots.",
                ShortDescription = "Reduces incoming energy damage by 50%, but increases incoming melee and artillery damage by 50%.",
                Tag = "chassis_reflective",
                ScrapItemDefID = "Lootable_Armor_Reflective",
                CriticalSlots = 8,
                IntroDate = new DateTime(3058, 1, 1),
                ProductionDate = new DateTime(3080, 1, 1),
                PptMultiplier = 1f,
                TPCost = 2f,
                CBCost = 3f // 30,000 C-Bills per ton
            } },
        };
    }
}