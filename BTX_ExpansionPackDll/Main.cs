using System;
using System.Reflection;
using BattleTech;
using HarmonyLib;
using HBS.Logging;
using Newtonsoft.Json;

namespace BTX_ExpansionPack
{
    public class Main
    {
        internal static Harmony harmony;
        internal static string modDir;
        internal static ILog Log { get; private set; }
        internal static ModSettings Settings { get; private set; }

        public class ModSettings
        {
            //public bool Debug { get; set; }
        }

        public static void Init(string directory, string settingsJSON)
        {
            modDir = directory;
            Log = Logger.GetLogger("BTX_ExpansionPack");
            Logger.SetLoggerLevel("BTX_ExpansionPack", new LogLevel?(LogLevel.Debug));

            try
            {
                Settings = JsonConvert.DeserializeObject<ModSettings>(settingsJSON) ?? new ModSettings();
                harmony = new Harmony("com.github.AkiraBrahe.BTX_ExpansionPack");
                ApplyHarmonyPatches();
                Log.Log($"Expansion Pack Mod Initialized!");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        static void ApplyHarmonyPatches()
        {
            // Firing Arc Quirks
            harmony.Unpatch(
                AccessTools.DeclaredMethod(typeof(Mech), "IsTargetPositionInFiringArc"),
                HarmonyPatchType.Postfix, "BEX.BattleTech.MechQuirks");

            // Bigger Drops Fix
            harmony.Unpatch(
                Type.GetType("BTX_CAC_CompatibilityDll.SimGameState_InitStats, BTX_CAC_CompatibilityDll").GetMethod("Postfix",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic),
                HarmonyPatchType.Postfix, "com.github.mcb5637.BTX_CAC_Compatibility");

            harmony.PatchAll();
        }
    }
}