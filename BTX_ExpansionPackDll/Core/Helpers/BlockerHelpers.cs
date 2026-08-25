using BattleTech;
using CustomComponents;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Core.Helpers
{
    internal class BlockerHelpers
    {
        /// <summary>
        /// Determines all blocker types currently present in the mech's inventory.
        /// </summary>
        public static List<string> GetBlockerTypesInMech(MechDef mechDef)
        {
            var blockerTypes = new HashSet<string>();

            foreach (var component in mechDef.Inventory)
            {
                if (component.IsCategory("Blocker"))
                {
                    string baseID = GetBlockerBaseID(component.ComponentDefID);
                    blockerTypes.Add(baseID);
                }
            }

            return [.. blockerTypes];
        }

        /// <summary>
        /// Extracts the base blocker ID prefix from a full component ID.
        /// <br/>Example: "Gear_Armor_EndoFerroCombo_5_Slot" -> "Gear_Armor_EndoFerroCombo"
        /// </summary>
        public static string GetBlockerBaseID(string componentDefID)
        {
            string[] parts = componentDefID.Split('_');
            return parts.Length >= 3 && int.TryParse(parts[parts.Length - 2], out _)
                ? string.Join("_", parts.Take(parts.Length - 2))
                : componentDefID;
        }

        /// <summary>
        /// Determines the appropriate blockers for a mech based on its structure and armor types.
        /// </summary>
        public static List<string> DetermineBlockerIds(StructureInfo structure, ArmorInfo armor)
        {
            var result = new List<string>();

            if (structure.CriticalSlots == 0 && armor.CriticalSlots == 0)
                return result;

            // Try to find a valid combo blocker first
            if (structure.CriticalSlots > 0 && armor.CriticalSlots > 0)
            {
                if (structure.Type == StructureType.EndoSteel)
                {
                    switch (armor.Type)
                    {
                        case ArmorType.FerroFibrous:
                            result.Add("Gear_Armor_EndoFerroCombo");
                            return result;
                        case ArmorType.LightFerro:
                            result.Add("Gear_Armor_EndoLightFerroCombo");
                            return result;
                        case ArmorType.HeavyFerro:
                            result.Add("Gear_Armor_EndoHeavyFerroCombo");
                            return result;
                    }
                }
                else if (structure.Type == StructureType.ClanEndoSteel
                    && armor.Type == ArmorType.ClanFerroFibrous)
                {
                    result.Add("Gear_Armor_ClanEndoFerroCombo");
                    return result;
                }

                // No combo found, add both types separately
                result.Add($"Gear_Armor_{structure.Type}");
                result.Add($"Gear_Armor_{armor.Type}");
                return result;
            }

            // Only one type needs slots
            if (structure.CriticalSlots > 0)
                result.Add($"Gear_Armor_{structure.Type}");
            if (armor.CriticalSlots > 0)
                result.Add($"Gear_Armor_{armor.Type}");

            return result;
        }

        /// <summary>
        /// Gets the total number of inventory slots taken up by a list of blockers.
        /// </summary>
        public static int GetTotalBlockerSlots(List<MechComponentRef> allBlockers)
        {
            return allBlockers == null || allBlockers.Count == 0
                ? 0 : allBlockers.SelectMany(b => b.Def != null ? [b.Def.InventorySize] : new int[0]).Sum();
        }
    }
}
