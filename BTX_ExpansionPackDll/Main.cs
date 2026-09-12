using BattleTech;
using BattleTech.Data;
using BattleTech.Framework;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using BTX_ExpansionPack.Features.Refit;
using BTX_ExpansionPack.Features.Simulation;
using FullXotlTables;
using HBS.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BTX_ExpansionPack
{
    public class Main
    {
        private const string ModName = "BTX_ExpansionPack";
        private const string HarmonyInstanceId = "com.github.AkiraBrahe.BTX_ExpansionPack";

        internal static Harmony harmony;
        internal static string modDir;
        internal static ILog Logger { get; private set; }
        internal static ModSettings Settings { get; private set; }
        private static bool initSuccess = true;

        public static bool HasAdvancedMechLab { get; private set; }
        public static bool HasPlayableVehicles => BTSimpleMechAssembly.Assembly.Settings.SalvageAndAssembleVehicles;

        public static void Init(string directory, string settingsJSON)
        {
            modDir = directory;
            Logger = HBS.Logging.Logger.GetLogger(ModName, LogLevel.Debug);

            try
            {
                Settings = JsonConvert.DeserializeObject<ModSettings>(settingsJSON) ?? new ModSettings();
                HasAdvancedMechLab = Directory.Exists(Path.Combine(Path.GetDirectoryName(directory), "BTX_AdvancedMechLab"));
                harmony = new Harmony(HarmonyInstanceId);

                UpdateDynamicDifficultyLances(MetadataDatabase.Instance);
                ApplyHarmonyPatches();
                RegisterModComponents();

                Logger.Log("Mod initialized!");
            }
            catch (Exception ex)
            {
                initSuccess = false;
                Logger.LogError($"Failed during initialization: {ex.Message}");
            }
        }

        public static void FinishedLoading()
        {
            if (!initSuccess)
            {
                Logger.LogWarning("Skipping post-config setup. Initialization failed.");
                return;
            }

            try
            {
                ApplySettings();
                ApplyCacOverrides();
                SetupFactionStores();
                // LoadCustomFactionTables();
            }
            catch (Exception ex)
            {
                initSuccess = false;
                Logger.LogError("FinishedLoading encountered an error: " + ex.Message);
            }
        }

        internal static void UpdateDynamicDifficultyLances(MetadataDatabase mdd)
        {
            mdd.ClearDynamicLanceDifficulty();
            mdd.BulkInsertDynamicLanceDifficulty(dynamicLanceDefs);
            Logger.LogDebug("[1/3] Successfully updated dynamic lance definitions.");
        }

        internal static void ApplyHarmonyPatches()
        {
            try
            {
                UnpatchMethods();
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Logger.LogDebug("[2/3] Successfully applied Harmony patches.");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error unpatching conflicting mods: {ex.Message}");
            }
        }

        private static void UnpatchMethods()
        {
            // --- Abilifier ---
            /* Custom Ability Tree */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(Pilot), "InitAbilities"), HarmonyPatchType.Prefix, "ca.gnivler.BattleTech.Abilifier");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Rehydrate"), HarmonyPatchType.Postfix, "ca.gnivler.BattleTech.Abilifier");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Dehydrate"), HarmonyPatchType.Prefix, "ca.gnivler.BattleTech.Abilifier");
            /* Ability Ordering */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SGBarracksMWDetailPanel), "SetPilot"), HarmonyPatchType.Postfix, "ca.gnivler.BattleTech.Abilifier");

            // --- BattleTech Extended ---
            /* Firing Arc Quirks */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(Mech), "IsTargetPositionInFiringArc"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
            /* Mech Tooltips */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(TooltipPrefab_Chassis), "SetData"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(TooltipPrefab_Mech), "SetData"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(MechLabMechInfoWidget), "SetData"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
            /* Pathfinding */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(PathNodeGrid), "GetTerrainModifiedCost", [typeof(PathNode), typeof(PathNode), typeof(float)]), HarmonyPatchType.Postfix, "BEX.BattleTech.Extended_CE");
            /* Temp Jump Jets */
            harmony.Unpatch(AccessTools.Property(typeof(AbstractActor), "WorkingJumpjets").GetGetMethod(), HarmonyPatchType.Postfix, "BEX.BattleTech.Extended_CE");
            /* Weather Conditions */
            harmony.Unpatch(AccessTools.PropertyGetter(typeof(Contract), "ShortDescription"), HarmonyPatchType.Postfix, "BEX.BattleTech.Extended_CE");

            // --- CAC-C ---
            /* Actuators */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(Mech), "InitStats"), AccessTools.DeclaredMethod(typeof(BTX_CAC_CompatibilityDll.AbstractActor_InitStats), "Prefix"));
            /* Drop Slots Fix */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "InitCompanyStats"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Rehydrate"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");
            /* Inventory Blockers */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(ChassisDef), "FromJSON"), AccessTools.DeclaredMethod(typeof(BTX_CAC_CompatibilityDll.MovableBlockers), "ChassisDef_FromJSON"));

            // --- Custom Units ---
            /* Location Labels */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(LanceMechEquipmentList), "SetLoadout", []), HarmonyPatchType.Postfix, "io.mission.customunits");
            /* Piloting Expertise */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(PilotGenerator), "GeneratePilots"), HarmonyPatchType.Postfix, "io.mission.customunits");

            // --- Full Xotl Tables ---
            /* Unit Selection */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(UnitSpawnPointOverride), "RequestUnit"), HarmonyPatchType.Prefix, "BattleTech.Haree.FullXotlTables");

            // --- Mech Affinity ---
            /* Stock Config Tooltip */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(MechLabStockInfoPopup), "StockMechDefLoaded"), HarmonyPatchType.Postfix, "ca.jwolf.MechAffinity");
        }

        internal static void RegisterModComponents()
        {
            ComponentUpgrader.Register();
            MovableBlockers.Register();

            Logger.LogDebug("[3/3] Successfully registered mod components.");
        }

        internal static void ApplySettings()
        {
            // Override anti-air to hit bonus
            Quirks.MechQuirks.modSettings.AntiAircraftTargetingToHit = -4;

            // Override engine quirk bonuses
            Quirks.MechQuirks.modSettings.LargeToHit = 0;
            Quirks.MechQuirks.modSettings.LargeInitiative = -1;
            Quirks.MechQuirks.modSettings.ExtraLargeToHit = 1;
            Quirks.MechQuirks.modSettings.ExtraLargeInitiative = -1;
            Quirks.MechQuirks.modSettings.ExtremeToHit = 2;
            Quirks.MechQuirks.modSettings.ExtremeInitiative = -1;

            // Override DHS engine cooling
            Extended_CE.Core.Settings.DHSEngineCooling = HasAdvancedMechLab ? 60 : Settings.Gameplay.OverrideDHSEngineCooling
                ? (int)Math.Round(30 * Settings.Gameplay.DHSEngineCoolingMultiplier)
                : Extended_CE.Core.Settings.DHSEngineCooling;

            // Remove non-standard ammo bins from shops
            if (Settings.Gameplay.RemoveNonStandardAmmoBins)
            {
                if (BTX_CAC_CompatibilityDll.ItemCollectionDef_FromCSV.Replaces != null)
                {
                    List<string> keysToRemove = [.. BTX_CAC_CompatibilityDll.ItemCollectionDef_FromCSV.Replaces
                        .Keys
                        .Where(key => key.StartsWith("Ammo_"))];

                    foreach (string key in keysToRemove)
                    {
                        BTX_CAC_CompatibilityDll.ItemCollectionDef_FromCSV.Replaces.Remove(key);
                    }
                }
            }
        }

        internal static void ApplyCacOverrides()
        {
            // Remove drop validator for inventory blockers 
            CustomComponents.Validator.rep_drop_validators.RemoveAll(d =>
                d.Method.DeclaringType.Name == "MovableBlockers" &&
                d.Method.Name == "ReplaceValidateDropDelegate"
            );

            // Replace HM mortar with actual mech mortars
            if (BTX_CAC_CompatibilityDll.ItemCollectionDef_FromCSV.Replaces != null)
            {
                BTX_CAC_CompatibilityDll.ItemCollectionDef_FromCSV.Replaces["Gear_Mortar_MechMortar"].ID = "itemCollection_Weapons_MechMortars";
                BTX_CAC_CompatibilityDll.ItemCollectionDef_FromCSV.Replaces["Gear_Mortar_MechMortar"].Amount = 1;
            }

            // Update splits for new ammo bins and artillery weapons, remove non-salvageable SLDF weapons, and rename inventory blockers.
            if (BTX_CAC_CompatibilityDll.Main.Splits != null)
            {
                BTX_CAC_CompatibilityDll.Main.Splits.Remove("Ammo_AmmunitionBox_Generic_SRM_Inferno_Half");

                var customSplits = new Dictionary<string, BTX_CAC_CompatibilityDll.WeaponAddonSplit>
                {
                    ["Ammo_AmmunitionBox_Generic_SRMInferno"] = new() { WeaponId = "Ammo_AmmunitionBox_Generic_SRM_Inferno", Link = false, WeaponType = ComponentType.AmmunitionBox },
                    ["Ammo_AmmunitionBox_Generic_SRMInferno_Half"] = new() { WeaponId = "Ammo_AmmunitionBox_Generic_SRM_Inferno_Half", Link = false, WeaponType = ComponentType.AmmunitionBox },
                    ["Ammo_AmmunitionBox_Generic_SRMInferno_Double"] = new() { WeaponId = "Ammo_AmmunitionBox_Generic_SRM_Inferno_Double", Link = false, WeaponType = ComponentType.AmmunitionBox },
                    ["Ammo_AmmunitionBox_Generic_Arrow4"] = new() { WeaponId = "Ammo_AmmunitionBox_Generic_ArrowIV", Link = false, WeaponType = ComponentType.AmmunitionBox },
                    ["Ammo_AmmunitionBox_Generic_Arrow4_Homing"] = new() { WeaponId = "Ammo_AmmunitionBox_Generic_ArrowIV_Homing", Link = false, WeaponType = ComponentType.AmmunitionBox },
                    ["Ammo_AmmunitionBox_Generic_Arrow4_Inferno"] = new() { WeaponId = "Ammo_AmmunitionBox_Generic_ArrowIV_Inferno", Link = false, WeaponType = ComponentType.AmmunitionBox },
                    ["Weapon_MortarCAC_Arrow4"] = new() { WeaponId = "Weapon_Artillery_ArrowIV_0-STOCK", Link = false },
                    ["Weapon_MortarCAC_LongTom"] = new() { WeaponId = "Weapon_Artillery_LongTomCannon_0-STOCK", Link = false },
                    ["Weapon_MortarCAC_Sniper"] = new() { WeaponId = "Weapon_Artillery_SniperCannon_0-STOCK", Link = false },
                    ["Weapon_MortarCAC_ThumperFree"] = new() { WeaponId = "Weapon_Artillery_ThumperCannon_0-STOCK", Link = false },
                    ["Weapon_RL_RL10_Sa_0-STOCK"] = new() { WeaponId = "Weapon_RL_PRL10_0-STOCK", Link = false },
                    ["Weapon_RL_RL15_Sa_0-STOCK"] = new() { WeaponId = "Weapon_RL_PRL10_0-STOCK", Link = false },
                    ["Weapon_RL_RL20_Sa_0-STOCK"] = new() { WeaponId = "Weapon_RL_PRL10_0-STOCK", Link = false },
                    ["Weapon_Autocannon_LB10X_Sa_0-STOCK"] = new() { WeaponId = "Weapon_Autocannon_LB10X_0-STOCK", Link = false },
                    ["Weapon_Autocannon_UAC5_Sa_0-STOCK"] = new() { WeaponId = "Weapon_Autocannon_UAC5_0-STOCK", Link = false },
                    ["Weapon_Gauss_Gauss_Sa_0-STOCK"] = new() { WeaponId = "Weapon_Gauss_Gauss_0-STOCK", Link = false },
                    ["Weapon_Laser_LargeLaserER_Sa_0-STOCK"] = new() { WeaponId = "Weapon_Laser_LargeLaserER_0-STOCK", Link = false },
                    ["Weapon_Laser_LargeLaserPulse_Sa_0-STOCK"] = new() { WeaponId = "Weapon_Laser_LargeLaserPulse_0-STOCK", Link = false },
                    ["Weapon_Laser_MediumLaserPulse_Sa_0-STOCK"] = new() { WeaponId = "Weapon_Laser_MediumLaserPulse_0-STOCK", Link = false },
                    ["Weapon_Laser_SmallLaserPulse_Sa_0-STOCK"] = new() { WeaponId = "Weapon_Laser_SmallLaserPulse_0-STOCK", Link = false },
                    ["Weapon_PPC_PPCER_Sa_0-STOCK"] = new() { WeaponId = "Weapon_PPC_PPCER_0-STOCK", Link = false }
                };

                foreach (var kvp in customSplits)
                {
                    BTX_CAC_CompatibilityDll.Main.Splits[kvp.Key] = kvp.Value;
                }

                string[] armorTypes = ["EndoSteel", "FerroFibrous", "EndoFerroCombo"];
                for (int i = 1; i <= 8; i++)
                {
                    foreach (string type in armorTypes)
                    {
                        string oldId = $"Gear_{type}_{i}_Slot";
                        BTX_CAC_CompatibilityDll.Main.Splits[oldId] = new()
                        {
                            WeaponId = $"Gear_Armor_{type}_{i}_Slot",
                            Link = false,
                            WeaponType = ComponentType.Upgrade
                        };
                    }
                }
            }
        }

        internal static void SetupFactionStores()
        {
            var factionShops = BEXTimeline.Core.Settings.FactionShopCreation;

            // Add starting faction stores
            var startDate = new DateTime(3025, 1, 1);
            if (!factionShops.ContainsKey(startDate))
                factionShops[startDate] = [];

            int count = 0;

            foreach (var entry in FactionStores.StartingFactionStores)
            {
                // Filter based on vehicle availability
                if (HasPlayableVehicles || !entry.Value.VehicleOnly)
                {
                    factionShops[startDate][entry.Key] = entry.Value.Faction;
                    count++;
                }
            }

            if (!HasPlayableVehicles)
            {
                // Add mech-only faction stores (when they become available without vehicles)
                foreach (var entry in FactionStores.MechOnlyStoresByDate)
                {
                    if (DateTime.TryParse(entry.Key, out var date))
                    {
                        if (!factionShops.ContainsKey(date))
                            factionShops[date] = [];

                        foreach (string systemId in entry.Value)
                        {
                            factionShops[date][systemId] = FactionStores.StartingFactionStores[systemId].Faction;
                            count++;
                        }
                    }
                }

                // Remove vehicle-only faction stores
                foreach (var entry in FactionStores.VehicleOnlyStoresByDate)
                {
                    if (DateTime.TryParse(entry.Key, out var date) && factionShops.ContainsKey(date))
                    {
                        foreach (string systemId in entry.Value)
                        {
                            factionShops[date].Remove(systemId);
                        }
                    }
                }
            }

            Logger.LogDebug($"[FactionStores] Added {count} new faction stores.");
        }

        internal static void LoadCustomFactionTables()
        {
            Assembly xotlAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "FullXotlTables");

            if (xotlAssembly == null) return;

            var xotlDir = Path.GetDirectoryName(xotlAssembly.Location);
            var customTables = FactionTables.GenerateFromCustomFolder(Path.Combine(xotlDir, "XotlTablesV2"));

            FullXotlTables.Logger.Log("Adding custom faction tables from XotlTablesV2 folder...");

            foreach (var faction in customTables.Factions)
            {
                GetXotlTables().Factions[faction.Key] = faction.Value;

                FullXotlTables.Logger.Log("Faction: " + faction.Key);
                FullXotlTables.Logger.Log(" Number of Lights: " + faction.Value.Mechs.Lights.Count.ToString());
                FullXotlTables.Logger.Log(" Number of Mediums: " + faction.Value.Mechs.Mediums.Count.ToString());
                FullXotlTables.Logger.Log(" Number of Heavies: " + faction.Value.Mechs.Heavies.Count.ToString());
                FullXotlTables.Logger.Log(" Number of Assaults: " + faction.Value.Mechs.Assaults.Count.ToString());
            }

            Logger.LogDebug($"[FactionTables] Loaded {customTables.Factions.Count} custom faction tables.");

            static XotlTable GetXotlTables() => FullXotlTables.Core.xotlTables;
        }

        [HarmonyPatch(typeof(MainMenu), "Init")]
        public static class MainMenu_Init
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!initSuccess)
                {
                    Logger.LogError("Initialization failed.");
                    GenericPopupBuilder.Create(GenericPopupType.Warning,
                        "There was a problem loading the Expansion Pack.\nCheck your install, then restart the game.")
                        .AddButton("OK", null, true, null).Render();
                }
            }
        }
    }
}