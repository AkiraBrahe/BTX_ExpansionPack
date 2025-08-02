using BattleTech;
using System;
using System.Linq;

namespace BTX_ExpansionPack
{
    internal class YenLoWangUpgrade
    {
        [HarmonyPatch(typeof(SimGameState), "OnDayPassed")]
        class SimGameState_OnDayPassed
        {
            public static void Postfix(SimGameState __instance)
            {
                if (HasYenLoWang(__instance) && !__instance.CompanyTags.Contains("yenlowang_upgrade") &&
                    __instance.CurrentDate > new DateTime(3050, 9, 21) &&
                    __instance.GetFirstFreeMechBay() >= 0 && __instance.Funds > 10000000 &&
                    __instance.NetworkRandom.Int(0, 100) < 10)
                {
                    __instance.CompanyTags.Add("yenlowang_upgrade");
                    __instance.InterruptQueue.QueuePauseNotification(
                        "Yang's Offer",
                        $"I've been looking over our Yen-Lo-Wang, Boss. It's a fine machine, but it's a relic of the Succession Wars. For {SimGameState.GetCBillString(10000000)}, I can get it running with a modern 200-rated XL engine and a custom set of Triple Strength Myomer. We'll have it ready as the CN9-YLW2 in no time.",
                        __instance.GetCrewPortrait(SimGameCrew.Crew_Yang), "",
                        () => { RemoveYenLoWang(__instance); AddYenLoWang2(__instance); __instance.AddFunds(-10000000, null, true, true); },
                        "OK",
                        () => { __instance.CompanyTags.Remove("yenlowang_upgrade"); },
                        "Cancel");
                }
            }

            private static bool HasYenLoWang(SimGameState simState)
            {
                return simState.GetItemCount("chassisdef_centurion_CN9-YLW", typeof(MechDef), SimGameState.ItemCountType.UNDAMAGED_ONLY) > 0;
            }

            private static void RemoveYenLoWang(SimGameState simState)
            {
                simState.RemoveItemStat("chassisdef_centurion_CN9-YLW", typeof(MechDef), false);
            }

            private static void AddYenLoWang2(SimGameState simState)
            {
                var mechDef = simState.DataManager.MechDefs.Get("mechdef_centurion_CN9-YLW2");
                mechDef = new MechDef(mechDef, simState.GenerateSimGameUID(), true);


                if (simState.Constants.Salvage.EquipMechOnSalvage)
                {
                    mechDef.SetInventory([.. mechDef.Inventory.Where((x) => x.IsFixed ||
                    x.ComponentDefID.Equals("Weapon_Gauss_Gauss_NU_2-Grizzard") ||
                    x.ComponentDefID.Equals("Ammo_AmmunitionBox_Generic_GAUSS"))]);
                }

                int mechReadyTime = 625000; // about 50 days
                int baySlot = simState.GetFirstFreeMechBay();

                var order = new WorkOrderEntry_ReadyMech(
                    $"ReadyMech-{mechDef.GUID}",
                    $"Readying 'Mech - {mechDef.Chassis.Description.Name}",
                    mechReadyTime,
                    baySlot,
                    mechDef,
                    string.Format(simState.Constants.Story.MechReadiedWorkOrderCompletedText, mechDef.Chassis.Description.Name)
                );

                simState.MechLabQueue.Add(order);
                simState.ReadyingMechs[baySlot] = mechDef;
                simState.RoomManager.AddWorkQueueEntry(order);
                simState.UpdateMechLabWorkQueue(false);
                AudioEventManager.PlayAudioEvent("audioeventdef_simgame_vo_barks", "workqueue_readymech", WwiseManager.GlobalAudioObject, null);
            }
        }
    }
}