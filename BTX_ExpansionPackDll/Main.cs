using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BTX_ExpansionPack.Features;
using BTX_ExpansionPack.Helpers;
using HBS.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public static bool HasPlayableVehicles => BTSimpleMechAssembly.Assembly.Settings.SalvageAndAssembleVehicles;

        public static void Init(string directory, string settingsJSON)
        {
            modDir = directory;
            Log = Logger.GetLogger(ModName, LogLevel.Debug);

            try
            {
                Settings = JsonConvert.DeserializeObject<ModSettings>(settingsJSON) ?? new ModSettings();
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
            mdd.BulkInsertDynamicLanceDifficulty(DatabaseHelpers.lanceDefs);
            Log.LogDebug("Successfully updated the Dynamic Lance Difficulty database.");
        }

        internal static void ApplyHarmonyPatches()
        {
            // --- Abilifier ---
            /* Custom Skill Tree */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Rehydrate"), HarmonyPatchType.Postfix, "ca.gnivler.BattleTech.Abilifier");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Dehydrate"), HarmonyPatchType.Prefix, "ca.gnivler.BattleTech.Abilifier");

            // --- BattleTech Extended ---
            /* Weather Conditions */
            harmony.Unpatch(AccessTools.PropertyGetter(typeof(Contract), "ShortDescription"), HarmonyPatchType.Postfix, "BEX.BattleTech.Extended_CE");
            /* Temp Jump Jets */
            harmony.Unpatch(AccessTools.Property(typeof(AbstractActor), "WorkingJumpjets").GetGetMethod(), HarmonyPatchType.Postfix, "BEX.BattleTech.Extended_CE");
            /* Firing Arc Quirks */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(Mech), "IsTargetPositionInFiringArc"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
            /* Stock Role Tooltip */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(MechLabMechInfoWidget), "SetData"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");

            // --- CAC-C ---
            /* Drop Slots Fix */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "InitCompanyStats"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Rehydrate"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");

            // --- Custom Units ---
            /* Piloting Expertise */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(PilotGenerator), "GeneratePilots"), HarmonyPatchType.Postfix, "io.mission.customunits");
            /* Location Labels */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(LanceMechEquipmentList), "SetLoadout", []), HarmonyPatchType.Postfix, "io.mission.customunits");

            // --- Mech Affinity ---
            /* Stock Config Tooltip */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(MechLabStockInfoPopup), "StockMechDefLoaded"), HarmonyPatchType.Postfix, "ca.jwolf.MechAffinity");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        internal static void ApplySettings()
        {
            // Override anti-air to hit bonus
            Quirks.MechQuirks.modSettings.AntiAircraftTargetingToHit = -4;

            // Override DHS engine cooling
            Extended_CE.Core.Settings.DHSEngineCooling = Settings.Gameplay.OverrideDHSEngineCooling
                ? (int)Math.Round(30 * Settings.Gameplay.DHSEngineCoolingMultiplier)
                : Extended_CE.Core.Settings.DHSEngineCooling;

            // Hide role description in mech lab
            Quirks.MechQuirks.modSettings.ShowBEXTRoleEffectsInMechLab = false;

            // Remove non-standard ammo bins from shops
            if (Settings.Gameplay.DisableNonStandardAmmoBins)
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
            if (BTX_CAC_CompatibilityDll.ItemCollectionDef_FromCSV.Replaces != null)
            {
                BTX_CAC_CompatibilityDll.ItemCollectionDef_FromCSV.Replaces["Gear_Mortar_MechMortar"].ID = "itemCollection_Weapons_MechMortars";
                BTX_CAC_CompatibilityDll.ItemCollectionDef_FromCSV.Replaces["Gear_Mortar_MechMortar"].Amount = 1;
            }

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
                    Log.LogError($"Initialization failed.");
                    GenericPopupBuilder.Create(GenericPopupType.Warning,
                        "There was a problem loading the Expansion Pack. Check your install, then restart the game.")
                        .AddButton("OK", null, true, null).Render();
                }
            }
        }
    }
}