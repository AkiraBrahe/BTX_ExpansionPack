using BattleTech;
using System;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Features
{
    /// <summary>
    /// Checks each day if the player has an upgradeable mech and offers an upgrade if conditions are met.
    /// </summary>
    internal class YangUpgrades
    {
        private class UpgradeableMech
        {
            public string OriginalChassisDef;
            public string UpgradedMechDef;
            public string UpgradeTag;
            public DateTime MinDate;
            public int Cost;
            public string OfferText;
        }

        private static readonly List<UpgradeableMech> Upgrades =
        [
            new UpgradeableMech
            {
                OriginalChassisDef = "chassisdef_centurion_CN9-YLW",
                UpgradedMechDef = "mechdef_centurion_CN9-YLW2",
                UpgradeTag = "yenlowang_upgrade",
                MinDate = new DateTime(3050, 9, 21),
                Cost = 10000000,
                OfferText = "I've been looking over our Yen-Lo-Wang, Boss. It's a fine machine, but it's a relic of the Succession Wars. For {0}, I can get it running with a modern 200-rated XL engine and a custom set of Triple Strength Myomer. We'll have it ready as the CN9-YLW2 in no time."
            },
            new UpgradeableMech
            {
                OriginalChassisDef = "chassisdef_crab__fp_gladiator_BSC-27",
                UpgradedMechDef = "mechdef_crab_BSC-30",
                UpgradeTag = "bigsteelclaw_upgrade",
                MinDate = new DateTime(3052, 1, 1),
                Cost = 10000000,
                OfferText = "Hey, Boss, remember that FrankenMech, the Big Steel Claw? I've been doing some serious thinking on it. That thing has real potential, but it's choked with old-school tech. For {0}, I can get it into the refit bay and give it the works. We're talking a full engine swap, an endo steel skeleton, DHS, and some wicked new weaponry. Oh, and I'll give it a proper steel claw on that left arm to finish the job."
            }
        ];

        [HarmonyPatch(typeof(SimGameState), "OnDayPassed")]
        public static class SimGameState_OnDayPassed
        {
            [HarmonyPostfix]
            public static void Postfix(SimGameState __instance)
            {
                if (__instance.GetFirstFreeMechBay() < 0)
                {
                    return;
                }

                foreach (var upgrade in Upgrades)
                {
                    if (CanOfferUpgrade(__instance, upgrade))
                    {
                        OfferUpgrade(__instance, upgrade);
                        break;
                    }
                }
            }

            private static bool CanOfferUpgrade(SimGameState simGame, UpgradeableMech upgrade)
            {
                return !simGame.CompanyTags.Contains(upgrade.UpgradeTag) &&
                       simGame.CurrentDate > upgrade.MinDate &&
                       simGame.Funds > upgrade.Cost &&
                       simGame.GetItemCount(upgrade.OriginalChassisDef, typeof(MechDef), SimGameState.ItemCountType.UNDAMAGED_ONLY) > 0 &&
                       simGame.NetworkRandom.Int(0, 100) < 10;
            }

            private static void OfferUpgrade(SimGameState simGame, UpgradeableMech upgrade)
            {
                simGame.CompanyTags.Add(upgrade.UpgradeTag);
                string formattedOfferText = string.Format(upgrade.OfferText, SimGameState.GetCBillString(upgrade.Cost));

                simGame.InterruptQueue.QueuePauseNotification(
                    "Yang's Offer",
                    formattedOfferText,
                    simGame.GetCrewPortrait(SimGameCrew.Crew_Yang), "",
                    () =>
                    {
                        simGame.RemoveItemStat(upgrade.OriginalChassisDef, typeof(MechDef), false);
                        AddMechToQueue(simGame, upgrade.UpgradedMechDef);
                        simGame.AddFunds(-upgrade.Cost, null, true, true);
                    },
                    "OK",
                    () => simGame.CompanyTags.Remove(upgrade.UpgradeTag),
                    "Cancel");
            }

            private static void AddMechToQueue(SimGameState simGame, string mechDefId)
            {
                var mechDef = simGame.DataManager.MechDefs.Get(mechDefId);
                mechDef = new MechDef(mechDef, simGame.GenerateSimGameUID(), true);

                int mechReadyTime = 625000; // about 50 days
                int baySlot = simGame.GetFirstFreeMechBay();

                var order = new WorkOrderEntry_ReadyMech(
                    $"ReadyMech-{mechDef.GUID}",
                    $"Readying 'Mech - {mechDef.Chassis.Description.Name}",
                    mechReadyTime,
                    baySlot,
                    mechDef,
                    string.Format(simGame.Constants.Story.MechReadiedWorkOrderCompletedText, mechDef.Chassis.Description.Name)
                );

                simGame.MechLabQueue.Add(order);
                simGame.ReadyingMechs[baySlot] = mechDef;
                simGame.RoomManager.AddWorkQueueEntry(order);
                simGame.UpdateMechLabWorkQueue(false);
                AudioEventManager.PlayAudioEvent("audioeventdef_simgame_vo_barks", "workqueue_readymech", WwiseManager.GlobalAudioObject, null);
            }
        }
    }
}