using BattleTech;
using static BattleTech.SimGameState;

namespace BTX_ExpansionPack.Fixes.Misc
{
    internal class Inventory
    {
        /// <summary>
        /// Replaces obsolete items in inventory when loading a save.
        /// </summary>
        [HarmonyPatch(typeof(SimGameState), "Rehydrate")]
        public static class SimGameState_Rehydrate
        {
            [HarmonyPostfix]
            [HarmonyWrapSafe]
            public static void Postfix(SimGameState __instance)
            {
                foreach (var kvp in BTX_CAC_CompatibilityDll.Main.Splits)
                {
                    string obsoleteItem = kvp.Key;
                    string newItem = kvp.Value.WeaponId;
                    var itemType = kvp.Value.WeaponType switch
                    {
                        ComponentType.Weapon => typeof(WeaponDef),
                        ComponentType.AmmunitionBox => typeof(AmmunitionBoxDef),
                        ComponentType.Upgrade => typeof(UpgradeDef),
                        _ => null,
                    };

                    if (itemType != null)
                    {
                        int count = __instance.GetItemCount(obsoleteItem, itemType, ItemCountType.ALL);
                        if (count > 0)
                        {
                            string oldStatId = __instance.GetItemStatID(obsoleteItem, itemType);
                            if (__instance.companyStats.ContainsStatistic(oldStatId))
                            {
                                __instance.companyStats.RemoveStatistic(oldStatId);
                            }

                            bool isArmorItem = obsoleteItem.StartsWith("Gear_Endo") || obsoleteItem.StartsWith("Gear_Ferro");
                            if (!isArmorItem)
                            {
                                Main.Logger.LogDebug($"Replacing {count} instances of obsolete item '{obsoleteItem}' with new item '{newItem}' in inventory.");

                                string newStatId = __instance.GetItemStatID(newItem, itemType);
                                if (__instance.companyStats.ContainsStatistic(newStatId))
                                {
                                    __instance.companyStats.ModifyStat("SimGameState", 0, newStatId, StatCollection.StatOperation.Int_Add, count);
                                }
                                else
                                {
                                    __instance.companyStats.AddStatistic(newStatId, count);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}