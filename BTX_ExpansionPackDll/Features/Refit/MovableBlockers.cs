using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;
using CustomComponents.Changes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static BTX_ExpansionPack.Core.Helpers.BlockerHelpers;

namespace BTX_ExpansionPack.Features.Refit
{
    internal class MovableBlockers
    {
        public static void Register()
        {
            AutoFixer.Shared.RegisterMechFixer(AutoFixBlockers);
            Validator.RegisterDropValidator(null, ReplaceValidateDrop, null);
        }

        #region Hook Methods

        public static string ReplaceValidateDrop(MechLabItemSlotElement drop_item, ChassisLocations location, Queue<IChange> changes) => ValidateArmorDrop(drop_item, location, changes);

        #endregion

        #region Cleanup Logic

        /// <summary>
        /// Migrates old blocker definitions to the new format and caches them for later use.
        /// </summary>
        [HarmonyPatch(typeof(ChassisDef), "FromJSON")]
        [HarmonyAfter("BEX.BattleTech.Extended_CE")]
        public static class ChassisDef_FromJSON_ArmorBlockers
        {
            [HarmonyPostfix]
            public static void Postfix(ChassisDef __instance)
            {
                if (__instance == null) return;

                // Step A: Identify and migrate blockers
                var fixedInv = __instance.FixedEquipment?.ToList() ?? [];
                for (int i = 0; i < fixedInv.Count; i++)
                {
                    if (fixedInv[i].ComponentDefID.StartsWith("Gear_EndoSteel") ||
                        fixedInv[i].ComponentDefID.StartsWith("Gear_FerroFibrous") ||
                        fixedInv[i].ComponentDefID.StartsWith("Gear_EndoFerroCombo"))
                    {
                        fixedInv[i].ComponentDefID = fixedInv[i].ComponentDefID.Replace("Gear_", "Gear_Armor_");
                    }
                }

                var stockBlockers = fixedInv
                    .Where(c => c.ComponentDefID.StartsWith("Gear_Armor_"))
                    .Select(c => new DefaultsInfoRecord { DefID = c.ComponentDefID, Location = c.MountedLocation, Type = c.ComponentDefType })
                    .ToArray();

                if (stockBlockers.Length > 0)
                {
                    fixedInv.RemoveAll(c => c.ComponentDefID.StartsWith("Gear_Armor_"));
                    __instance.fixedEquipment = [.. fixedInv];
                }

                // Step B: Store data in cache for the autofixer to use
                var cache = __instance.GetComponent<AdvancedChassisData>();
                if (cache == null)
                {
                    cache = new AdvancedChassisData();
                    __instance.AddComponent(cache);
                }

                cache.StockBlockers = stockBlockers;
            }
        }

        #endregion

        #region Autofix Logic

        /// <summary>
        /// Fixes the number of slots taken up by blockers to match the structure and armor type.
        /// </summary>
        private static void AutoFixBlockers(List<MechDef> mechs)
        {
            var sw = Stopwatch.StartNew();
            foreach (var mech in mechs)
            {
                try
                {
                    NormalizeBlockers(mech);
                }
                catch (Exception e)
                {
                    Main.Logger.LogError($"Error auto-fixing blockers for {mech.Description.Id}: {e}");
                }
            }
            sw.Stop();
            Main.Logger.LogDebug($"Auto-fixed blockers for {mechs.Count} mechs in {sw.Elapsed.TotalSeconds:F2} seconds.");
        }

        internal static void NormalizeBlockers(MechDef mech)
        {
            if (!mech.DataManager.ChassisDefs.TryGet(mech.Chassis.Description.Id, out var chassis))
                chassis = mech.Chassis;

            var cache = chassis.GetComponent<AdvancedChassisData>();
            if (cache == null) return;

            // Step A: Determine if the mech has or needs any blockers
            var structure = mech.GetStructureInfo();
            var armor = mech.GetArmorInfo();

            int totalRequired = structure.CriticalSlots + armor.CriticalSlots;
            if (totalRequired == 0) return;

            var invBlockers = mech.Inventory.Where(c => c.IsCategory("Blocker"));
            if (invBlockers.Any()) return;

            // Step B: Get cached blockers from the chassis
            var cachedBlockers = (cache.StockBlockers == null || cache.StockBlockers.Length == 0)
                ? [] : cache.StockBlockers.Select(b => new MechComponentRef(b.DefID, "", b.Type, b.Location) { DataManager = mech.DataManager });
            var allBlockers = cachedBlockers.ToList();

            var blockerIDs = DetermineBlockerIds(structure, armor);
            if (blockerIDs.Count == 0) return;

            // Step C: Adjust blockers to match the required number of slots
            if (blockerIDs.Count == 1)
            {
                string blockerID = blockerIDs[0];
                string currentID = allBlockers.FirstOrDefault()?.ComponentDefID ?? "";
                if (currentID != blockerID) allBlockers.Clear();

                int currentSlots = GetTotalBlockerSlots(allBlockers);

                if (currentSlots != totalRequired)
                {
                    bool isClan = chassis.ChassisTags.Contains("chassis_clan");
                    if (!isClan)
                    {
                        // IS mech: Adjust existing blockers
                        if (currentSlots > totalRequired)
                        {
                            ReduceBlockers(allBlockers, currentSlots - totalRequired);
                        }
                        else
                        {
                            AddBlockers(mech, ref allBlockers, blockerID, totalRequired - currentSlots);
                        }
                    }
                    else
                    {
                        // Clan mech: Add blockers from scratch
                        allBlockers.Clear();
                        AddBlockers(mech, ref allBlockers, blockerID, totalRequired);
                    }
                }
            }

            var inventory = mech.Inventory;
            mech.SetInventory([.. inventory, .. allBlockers]);
            mech.RefreshInventory();
        }

        /// <summary>
        /// Reduces the number of inventory slots taken up by blockers.
        /// </summary>
        private static void ReduceBlockers(List<MechComponentRef> allBlockers, int slotsToRemove)
        {
            if (slotsToRemove <= 0) return;

            while (slotsToRemove > 0)
            {
                bool changed = false;
                foreach (var location in repairPriorities.Values)
                {
                    var blocker = allBlockers.FirstOrDefault(b => b.MountedLocation == location);
                    if (blocker != null)
                    {
                        int currentSize = blocker.Def.InventorySize;
                        if (currentSize <= 1)
                        {
                            allBlockers.Remove(blocker);
                        }
                        else
                        {
                            string currentSuffix = currentSize + "_Slot";
                            string newSuffix = currentSize - 1 + "_Slot";
                            blocker.ComponentDefID = blocker.ComponentDefID.Replace(currentSuffix, newSuffix);
                            blocker.RefreshComponentDef();
                        }
                        slotsToRemove--;
                        changed = true;
                        if (slotsToRemove <= 0) break;
                    }
                }
                if (!changed) break;
            }
        }

        /// <summary>
        /// Adds blockers by distributing them evenly across all locations.
        /// </summary>
        private static void AddBlockers(MechDef mech, ref List<MechComponentRef> allBlockers, string baseID, int slotsToAdd)
        {
            if (slotsToAdd <= 0) return;

            // Step A: Check available free slots in each location
            var distribution = allLocations.ToDictionary(loc => loc, loc => 0);
            var freeSlots = distribution.Keys.ToDictionary(loc => loc, loc => mech.GetFreeSlotsInLoc([.. mech.Inventory], loc));

            int totalFreeSlots = freeSlots.Values.Sum();

            // Step B: If necessary, internalize heat sinks to free up space
            if (totalFreeSlots < slotsToAdd)
            {
                int neededSlots = slotsToAdd - totalFreeSlots;

                // Find heat sinks in the mech's inventory
                var externalHeatSinks = new List<(MechComponentRef component, HeatSinkInfo info)>();
                foreach (var component in mech.Inventory)
                {
                    if (component.ComponentDefType == ComponentType.HeatSink && !component.IsCategory("Internal"))
                    {
                        var hsInfo = HeatSinkTypes.Values.FirstOrDefault(v => v.ExternalDefID == component.ComponentDefID);
                        if (!string.IsNullOrEmpty(hsInfo.ExternalDefID))
                        {
                            externalHeatSinks.Add((component, hsInfo));
                        }
                    }
                }

                // Internalize heat sinks to free up space
                if (externalHeatSinks.Count > 0)
                {
                    var convertableHS = externalHeatSinks.Take(neededSlots).ToList();
                    int potentialSlots = convertableHS.Sum(hs => hs.component.Def != null ? hs.component.Def.InventorySize : hs.info.Slots);

                    if (potentialSlots >= neededSlots)
                    {
                        foreach (var (component, info) in convertableHS)
                        {
                            int gainedSlots = component.Def != null ? component.Def.InventorySize : info.Slots;

                            component.ComponentDefID = info.InternalDefID;
                            component.RefreshComponentDef();

                            freeSlots[component.MountedLocation] += gainedSlots;
                            neededSlots -= gainedSlots;

                            if (neededSlots <= 0) break;
                        }
                    }
                }
            }

            // Step C: Distribute the required slots across locations
            void Fill(List<ChassisLocations> locations)
            {
                if (slotsToAdd <= 0) return;
                bool added;
                do
                {
                    added = false;
                    foreach (var loc in locations.OrderBy(l => distribution[l]))
                    {
                        if (slotsToAdd > 0 && freeSlots[loc] > 0)
                        {
                            distribution[loc]++;
                            freeSlots[loc]--;
                            slotsToAdd--;
                            added = true;
                            if (slotsToAdd <= 0) break;
                        }
                    }
                } while (added && slotsToAdd > 0);
            }

            Fill([.. sideLocations]);
            Fill([.. coreLocations]);

            if (slotsToAdd > 0)
            {
                Main.Logger.LogWarning($"{mech.Description.Id} doesn't have enough free slots for blockers.");
            }

            // Step D: Add blockers to the mech's inventory based on the distribution
            foreach (var kvp in distribution)
            {
                int needed = kvp.Value;
                while (needed > 0)
                {
                    int size = Math.Min(needed, 8);
                    string itemID = $"{baseID}_{size}_Slot";
                    allBlockers.Add(new MechComponentRef(itemID, "", ComponentType.Upgrade, kvp.Key) { DataManager = mech.DataManager });
                    needed -= size;
                }
            }
        }

        #endregion

        #region Validation Logic

        /// <summary>
        /// Optimizes the position of blockers when an item is dropped.
        /// As long as there are enough free slots, blockers can automatically move to another mech location.
        /// </summary>
        private static string ValidateArmorDrop(MechLabItemSlotElement drop_item, ChassisLocations drop_location, Queue<IChange> changes)
        {
            var mechDef = MechLabHelper.CurrentMechLab.ActiveMech;
            var chassis = mechDef.Chassis;

            // Get all blocker types present in the mech's inventory
            var blockerTypes = GetBlockerTypesInMech(mechDef);
            if (blockerTypes.Count == 0)
                return string.Empty;

            int slotsMax = chassis.GetLocationDef(drop_location).InventorySlots;
            int slotsNeeded = SlotsInLocation(drop_location, drop_item, changes, mechDef) - slotsMax;

            if (slotsNeeded <= 0)
                return string.Empty;

            // Process each blocker type independently
            foreach (string baseID in blockerTypes)
            {
                if (slotsNeeded <= 0)
                    break;

                var locBlockers = mechDef.Inventory
                    .Where(c => c.MountedLocation == drop_location &&
                                c.ComponentDefID.StartsWith(baseID));

                foreach (var blocker in locBlockers)
                {
                    if (slotsNeeded <= 0)
                        break;

                    slotsNeeded = MoveBlockerSlotsToOtherLocations(
                        blocker, baseID, slotsNeeded, drop_location, drop_item, changes, mechDef, chassis);
                }
            }

            // Optimize each blocker type independently
            foreach (string baseID in blockerTypes)
            {
                OptimizeBlockers(changes, mechDef, baseID);
            }

            return string.Empty;
        }

        /// <summary>
        /// Attempts to move blocker slots from the current location to other locations.
        /// Prioritizes locations that already have blockers of the same type.
        /// </summary>
        /// <returns>
        /// Remaining slots needed after movement
        /// </returns>
        private static int MoveBlockerSlotsToOtherLocations(
            MechComponentRef blocker,
            string baseID,
            int slotsNeeded,
            ChassisLocations drop_location,
            MechLabItemSlotElement drop_item,
            Queue<IChange> changes,
            MechDef mechDef,
            ChassisDef chassis)
        {
            int slots = blocker.Def.InventorySize;

            var priorityLocations = allLocations
                .Where(loc => loc != drop_location)
                .OrderBy(loc => mechDef.Inventory.Any(c => c.MountedLocation == loc && c.ComponentDefID.StartsWith(baseID)))
                .ToList();
            slotsNeeded = AttemptMoveToLocations(priorityLocations, baseID, ref slots, slotsNeeded, drop_item, changes, mechDef, chassis);

            // Remove the original blocker and add remainder if any
            changes.Enqueue(new Change_Remove(blocker.ComponentDefID, blocker.MountedLocation));
            if (slots > 0)
            {
                changes.Enqueue(new Change_Add(
                    DefaultHelper.CreateSlot($"{baseID}_{slots}_Slot", ComponentType.Upgrade),
                    blocker.MountedLocation));
            }

            return slotsNeeded;
        }

        /// <summary>
        /// Attempts to move blocker slots to the specified locations.
        /// </summary>
        private static int AttemptMoveToLocations(
            List<ChassisLocations> priorityLocations,
            string baseID,
            ref int slotsAvailable,
            int slotsNeeded,
            MechLabItemSlotElement drop_item,
            Queue<IChange> changes,
            MechDef mechDef,
            ChassisDef chassis)
        {
            foreach (var location in priorityLocations)
            {
                if (slotsNeeded <= 0 || slotsAvailable <= 0)
                    break;

                int slotsInLocation = chassis.GetLocationDef(location).InventorySlots -
                                     SlotsInLocation(location, drop_item, changes, mechDef);

                if (slotsInLocation > 0)
                {
                    int toMove = Math.Min(slotsInLocation, Math.Min(slotsNeeded, slotsAvailable));

                    changes.Enqueue(new Change_Add(
                        DefaultHelper.CreateSlot($"{baseID}_{toMove}_Slot", ComponentType.Upgrade),
                        location));

                    slotsNeeded -= toMove;
                    slotsAvailable -= toMove;
                }
            }

            return slotsNeeded;
        }

        private static int SlotsInLocation(ChassisLocations location, MechLabItemSlotElement drop_item, Queue<IChange> changes, MechDef mechDef)
        {
            int slots = 0;
            foreach (var componentRef in mechDef.Inventory)
            {
                if (componentRef.MountedLocation == location)
                    slots += componentRef.Def.InventorySize;
            }
            if (drop_item != null && drop_item.MountedLocation == location)
                slots -= drop_item.ComponentRef.Def.InventorySize;

            foreach (var change in changes)
            {
                if (change is Change_Add a && a.Location == location)
                    slots += GetComponentSize(a.ItemID, a.Type);
                else if (change is Change_Remove r && r.Location == location)
                    slots -= GetComponentSize(r.ItemID, mechDef.DataManager);
            }
            return slots;
        }

        private static int GetComponentSize(string id, ComponentType type)
        {
            var def = DefaultHelper.GetComponentDef(id, type);
            return def != null ? def.InventorySize : 0;
        }

        private static int GetComponentSize(string id, DataManager dm)
        {
            return dm.UpgradeDefs.TryGet(id, out var upgrade) ? upgrade.InventorySize
                : dm.WeaponDefs.TryGet(id, out var weapon) ? weapon.InventorySize
                : dm.JumpJetDefs.TryGet(id, out var jumpJet) ? jumpJet.InventorySize
                : dm.AmmoBoxDefs.TryGet(id, out var ammoBox) ? ammoBox.InventorySize
                : dm.HeatSinkDefs.TryGet(id, out var heatSink) ? heatSink.InventorySize : 0;
        }

        /// <summary>
        /// Consolidates fragmented blockers of a specific type into the largest possible single blocker per location.
        /// </summary>
        private static void OptimizeBlockers(Queue<IChange> changes, MechDef mechDef, string baseID)
        {
            foreach (var location in allLocations)
            {
                int total = 0;
                var items = new List<(int Size, MechComponentRef Ref, Change_Add Change, string ID)>();

                // Existing blockers of this type
                foreach (var componentRef in mechDef.Inventory.Where(x => x.MountedLocation == location))
                {
                    if (componentRef.IsCategory("Blocker") && componentRef.ComponentDefID.StartsWith(baseID))
                    {
                        int slots = componentRef.Def.InventorySize;
                        total += slots;
                        items.Add((slots, componentRef, null, componentRef.ComponentDefID));
                    }
                }

                // Changed blockers of this type
                foreach (var change in changes)
                {
                    switch (change)
                    {
                        case Change_Add a when a.Location == location:
                            {
                                var comp = DefaultHelper.GetComponentDef(a.ItemID, a.Type);
                                if (comp != null && comp.IsCategory("Blocker") && a.ItemID.StartsWith(baseID))
                                {
                                    int slots = comp.InventorySize;
                                    total += slots;
                                    items.Add((slots, null, a, a.ItemID));
                                }
                                break;
                            }

                        case Change_Remove r when r.Location == location:
                            {
                                if (r.ItemID.StartsWith(baseID))
                                {
                                    for (int i = 0; i < items.Count; i++)
                                    {
                                        if (items[i].ID == r.ItemID)
                                        {
                                            total -= items[i].Size;
                                            items.RemoveAt(i);
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }

                // Optimization logic: consolidate multiple blockers into minimum required slots
                if (items.Count > 1 && total <= 8)
                {
                    RemoveAll(items, changes);
                    changes.Enqueue(new Change_Add(
                        DefaultHelper.CreateSlot($"{baseID}_{total}_Slot", ComponentType.Upgrade),
                        location));
                }
                else if (items.Count > 2)
                {
                    RemoveAll(items, changes);
                    changes.Enqueue(new Change_Add(
                        DefaultHelper.CreateSlot($"{baseID}_8_Slot", ComponentType.Upgrade),
                        location));
                    if (total > 8)
                    {
                        changes.Enqueue(new Change_Add(
                            DefaultHelper.CreateSlot($"{baseID}_{total - 8}_Slot", ComponentType.Upgrade),
                            location));
                    }
                }
            }
        }

        private static void RemoveAll(List<(int Size, MechComponentRef Ref, Change_Add Change, string ID)> items, Queue<IChange> changes)
        {
            foreach (var (_, Ref, Change, _) in items)
            {
                if (Ref != null)
                    changes.Enqueue(new Change_Remove(Ref.ComponentDefID, Ref.MountedLocation));
                else if (Change != null)
                    changes.Enqueue(new Change_Remove(Change.ItemID, Change.Location));
            }
        }

        #endregion
    }
}