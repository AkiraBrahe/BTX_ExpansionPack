using BattleTech;
using BattleTech.Framework;
using BattleTech.UI;
using CustAmmoCategories;
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

        public static void Init(string directory, string settingsJSON)
        {
            modDir = directory;
            Log = Logger.GetLogger(ModName);
            Logger.SetLoggerLevel(ModName, LogLevel.Debug);

            try
            {
                Settings = JsonConvert.DeserializeObject<ModSettings>(settingsJSON) ?? new ModSettings();
                harmony = new Harmony(HarmonyInstanceId);
                ApplyHarmonyPatches();
                ApplySettings();
                Log.Log("Mod initialized!");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        internal static void ApplyHarmonyPatches()
        {
            // --- BattleTech Extended ---
            /* Weather Conditions */
            harmony.Unpatch(AccessTools.PropertyGetter(typeof(Contract), "ShortDescription"), HarmonyPatchType.Postfix, "BEX.BattleTech.Extended_CE");
            /* Temp Jump Jets */
            harmony.Unpatch(AccessTools.Property(typeof(AbstractActor), "WorkingJumpjets").GetGetMethod(), HarmonyPatchType.Postfix, "BEX.BattleTech.Extended_CE");
            /* Firing Arc Quirks */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(Mech), "IsTargetPositionInFiringArc"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");
            // --- CAC-C ---
            /* Drop Slots Fix */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "InitCompanyStats"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Rehydrate"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");
            /* Max Player Units Fix */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(ContractOverride), "FromJSONFull"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(ContractOverride), "FullRehydrate"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");
            // --- Custom Units ---
            /* Piloting Expertise */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(PilotGenerator), "GeneratePilots"), HarmonyPatchType.Postfix, "io.mission.customunits");
            /* Location Labels */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(LanceMechEquipmentList), "SetLoadout", []), HarmonyPatchType.Postfix, "io.mission.customunits");
            // --- Mech Affinity ---
            /* Stock Config Description */
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(MechLabStockInfoPopup), "StockMechDefLoaded"), HarmonyPatchType.Postfix, "ca.jwolf.MechAffinity");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        internal static void ApplySettings()
        {
            // Re-enable saves between consecutive drops
            PreForceTakeContractSave.SkipSave = !Settings.Debug.SaveBetweenConsecutiveDrops;

            // Override DHS engine cooling
            Extended_CE.Core.Settings.DHSEngineCooling = Settings.Gameplay.OverrideDHSEngineCooling
                ? (int)Math.Round(30 * Settings.Gameplay.DHSEngineCoolingMultiplier)
                : Extended_CE.Core.Settings.DHSEngineCooling;

            // Hide role description in MechLab
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
    }
}