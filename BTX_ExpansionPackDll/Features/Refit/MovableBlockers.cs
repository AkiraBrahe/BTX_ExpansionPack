using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;
using CustomComponents.Changes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Features.Refit
{
    internal class MovableBlockers
    {
        public static void Register() => Validator.RegisterDropValidator(null, ReplaceValidateDrop, null);

        #region Hook Methods

        public static string ReplaceValidateDrop(MechLabItemSlotElement drop_item, ChassisLocations location, Queue<IChange> changes) => ValidateArmorDrop(drop_item, location, changes);

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

        #region Helper Methods

        /// <summary>
        /// Returns a list of all valid chassis locations for a mech.
        /// </summary>
        public static readonly ChassisLocations[] allLocations = [
            ChassisLocations.Head,
            ChassisLocations.LeftArm,
            ChassisLocations.LeftTorso,
            ChassisLocations.CenterTorso,
            ChassisLocations.RightTorso,
            ChassisLocations.RightArm,
            ChassisLocations.LeftLeg,
            ChassisLocations.RightLeg
        ];

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

        #endregion
    }
}