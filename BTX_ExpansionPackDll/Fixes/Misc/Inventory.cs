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
                            for (int j = 0; j < count; j++)
                            {
                                __instance.RemoveItemStat(obsoleteItem, itemType, false);
                                __instance.AddItemStat(newItem, itemType, false);
                            }
                        }
                    }
                }
            }
        }
    }
}