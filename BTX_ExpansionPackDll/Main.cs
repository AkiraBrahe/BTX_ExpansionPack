using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using BTX_ExpansionPack.Features.Refit;
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
        internal static ILog Log { get; private set; }
        internal static ModSettings Settings { get; private set; }
        private static bool initSuccess = true;

        public static bool HasAdvancedMechLab { get; private set; }
        public static bool HasPlayableVehicles => BTSimpleMechAssembly.Assembly.Settings.SalvageAndAssembleVehicles;

        public static void Init(string directory, string settingsJSON)
        {
            modDir = directory;
            Log = Logger.GetLogger(ModName, LogLevel.Debug);

            try
            {
                Settings = JsonConvert.DeserializeObject<ModSettings>(settingsJSON) ?? new ModSettings();
                HasAdvancedMechLab = Directory.Exists(Path.Combine(Path.GetDirectoryName(directory), "BTX_AdvancedMechLab"));
                harmony = new Harmony(HarmonyInstanceId);
                InjectCustomLanceData(MetadataDatabase.Instance);
                ApplyHarmonyPatches();
                ApplySettings();
                ApplyCacOverrides();
                ComponentUpgrader.Register();
                Log.Log("Mod initialized!");
            }
            catch (Exception ex)
            {
                initSuccess = false;
                Log.LogException(ex);
            }
        }

        internal static void InjectCustomLanceData(MetadataDatabase mdd)
        {
            mdd.ClearDynamicLanceDifficulty();
            mdd.BulkInsertDynamicLanceDifficulty(dynamicLanceDefs);
            Log.LogDebug("Successfully updated the Dynamic Lance Difficulty database.");
        }

        internal static void ApplyHarmonyPatches()
        {
            // --- Abilifier ---
            /* Custom Skill Tree */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Rehydrate"), HarmonyPatchType.Postfix, "ca.gnivler.BattleTech.Abilifier");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Dehydrate"), HarmonyPatchType.Prefix, "ca.gnivler.BattleTech.Abilifier");

            // --- BattleTech Extended ---
            /* Firing Arc Quirks */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(Mech), "IsTargetPositionInFiringArc"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
            /* Mech Tooltips */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(TooltipPrefab_Chassis), "SetData"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(TooltipPrefab_Mech), "SetData"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(MechLabMechInfoWidget), "SetData"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
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

            // --- Mech Affinity ---
            /* Stock Config Tooltip */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(MechLabStockInfoPopup), "StockMechDefLoaded"), HarmonyPatchType.Postfix, "ca.jwolf.MechAffinity");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
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

            Log.LogDebug("Successfully applied CAC-C overrides.");
        }

        [HarmonyPatch(typeof(MainMenu), "Init")]
        public static class MainMenu_Init
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!initSuccess)
                {
                    Log.LogError("Initialization failed.");
                    GenericPopupBuilder.Create(GenericPopupType.Warning,
                        "There was a problem loading the Expansion Pack.\nCheck your install, then restart the game.")
                        .AddButton("OK", null, true, null).Render();
                }
            }
        }
    }
}