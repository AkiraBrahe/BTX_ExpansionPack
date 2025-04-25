using System;
using BattleTech;
using HarmonyLib;
using HBS.Logging;
using Newtonsoft.Json;

namespace BTX_ExpansionPack
{
    public static class Main
    {
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

            try
            {
                Settings = JsonConvert.DeserializeObject<ModSettings>(settingsJSON);
                if (Settings == null)
                {
                    Settings = new ModSettings();
                }

                Log = HBS.Logging.Logger.GetLogger("BTX_ExpansionPack");
                Log.Log($"Expansion Pack Mod Initialized!");

                var harmony = new Harmony("com.AkiraBrahe.BTX_ExpansionPack");
                harmony.PatchAll();
            }
            catch (Exception ex)
            {
                Log?.LogError($"Error initializing mod: {ex}");
            }
        }
    }
}