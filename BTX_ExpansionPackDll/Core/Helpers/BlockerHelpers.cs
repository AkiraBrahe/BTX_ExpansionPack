using BattleTech;
using HBS.Collections;
using System;
using System.Linq;

namespace BTX_ExpansionPack.Core.Helpers
{
    public static class BlockerHelpers
    {
        /// <summary>
        /// Retrieves the structure info of a mech.
        /// </summary>
        public static StructureInfo GetStructureInfo(this MechDef mech)
        {
            var type = StructureType.Standard;

            bool isClan = mech.Chassis.ChassisTags.Contains("chassis_clan");
            foreach (string tag in mech.Chassis.ChassisTags)
            {
                var match = StructureTypes.FirstOrDefault(st => !string.IsNullOrEmpty(st.Value.Tag) && st.Value.Tag == tag);
                if (match.Value.Tag != null)
                {
                    type = match.Key;
                    break;
                }
            }

            if (isClan && type == StructureType.EndoSteel)
                type = StructureType.ClanEndoSteel;

            return StructureTypes[type];
        }

        /// <summary>
        /// Retrieves the armor info of a mech.
        /// </summary>
        public static ArmorInfo GetArmorInfo(this MechDef mech)
        {
            if (mech.MechTags != null)
            {
                var armorType = mech.MechTags.GetArmorType();
                if (armorType != null) return ArmorTypes[(ArmorType)armorType];
            }

            var type = ArmorType.Standard;
            bool isClan = mech.Chassis.ChassisTags.Contains("chassis_clan");
            foreach (string tag in mech.Chassis.ChassisTags)
            {
                var match = ArmorTypes.FirstOrDefault(at => !string.IsNullOrEmpty(at.Value.Tag) && at.Value.Tag == tag);
                if (match.Value.Tag != null)
                {
                    type = match.Key;
                    break;
                }
            }

            if (isClan && type == ArmorType.FerroFibrous)
                type = ArmorType.ClanFerroFibrous;

            return ArmorTypes[type];
        }

        private const string ArmorPrefix = "AML_Armor_";

        /// <summary>
        /// Retrieves the armor type from a tag set.
        /// </summary>
        public static ArmorType? GetArmorType(this TagSet tags)
        {
            string tag = tags.FirstOrDefault(t => t.StartsWith(ArmorPrefix));
            return tag != null && Enum.TryParse<ArmorType>(tag.Substring(ArmorPrefix.Length), out var type) ? type : null;
        }
    }
}