using BattleTech;
using BattleTech.Framework;
using HBS.Logging;
using Newtonsoft.Json;
using System;
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
                OverrideDHSEngineCooling();
                Log.Log($"{ModName} Initialized!");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        internal static void ApplyHarmonyPatches()
        {
            // Drop Slots Fix
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "InitCompanyStats"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(SimGameState), "Rehydrate"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");

            // Max Player Units Fix
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(ContractOverride), "FromJSONFull"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(ContractOverride), "FullRehydrate"), HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");

            // Firing Arc Quirks
            harmony.Unpatch(AccessTools.DeclaredMethod(typeof(Mech), "IsTargetPositionInFiringArc"), HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");

            // Temp Jump Jets
            harmony.Unpatch(AccessTools.Property(typeof(AbstractActor), "WorkingJumpjets").GetGetMethod(), HarmonyPatchType.Postfix, "BEX.BattleTech.Extended_CE");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        internal static void OverrideDHSEngineCooling()
        {
            if (Settings.Gameplay.OverrideDHSEngineCooling)
            {
                Extended_CE.Core.Settings.DHSEngineCooling = (int)Math.Round(30 * Settings.Gameplay.DHSEngineCoolingMultiplier);
            }
        }
    }
}