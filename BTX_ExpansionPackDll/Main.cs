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
                Log.Log($"{ModName} Initialized!");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        internal static void ApplyHarmonyPatches()
        {
            var patchesToUnpatch = new List<(string harmonyId, string typeName, string methodName, HarmonyPatchType patchType)>
            {
                // --- BattleTech Extended ---
                /* Weather Conditions */
                ("BEX.BattleTech.Extended_CE", "Contract", "get_ShortDescription", HarmonyPatchType.Postfix),
                /* Temp Jump Jets */
                ("BEX.BattleTech.Extended_CE", "AbstractActor", "get_WorkingJumpjets", HarmonyPatchType.Postfix),
                /* Firing Arc Quirks */
                ("BEX.BattleTech.MechQuirks", "Mech", "IsTargetPositionInFiringArc", HarmonyPatchType.Postfix),

                // --- CAC-C ---
                /* Drop Slots Fix */
                ("com.github.mcb5637.BTX_CAC_Compatibility", "SimGameState", "InitCompanyStats", HarmonyPatchType.Postfix),
                ("com.github.mcb5637.BTX_CAC_Compatibility", "SimGameState", "Rehydrate", HarmonyPatchType.Postfix),
                /* Max Player Units Fix */
                ("com.github.mcb5637.BTX_CAC_Compatibility", "ContractOverride", "FromJSONFull", HarmonyPatchType.Postfix),
                ("com.github.mcb5637.BTX_CAC_Compatibility", "ContractOverride", "FullRehydrate", HarmonyPatchType.Postfix)
            };

            foreach (var (harmonyId, typeName, methodName, patchType) in patchesToUnpatch)
            {
                var type = AccessTools.TypeByName(typeName);
                if (type == null) continue;

                MethodBase method = AccessTools.DeclaredMethod(type, methodName);

                if (method == null)
                {
                    var propInfo = AccessTools.Property(type, methodName.Replace("get_", ""));
                    method = propInfo?.GetGetMethod(true);
                }
                if (method != null)
                {
                    harmony.Unpatch(method, patchType, harmonyId);
                }
            }

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        internal static void ApplySettings()
        {
            if (Settings.Gameplay.OverrideDHSEngineCooling)
            {
                Extended_CE.Core.Settings.DHSEngineCooling = (int)Math.Round(30 * Settings.Gameplay.DHSEngineCoolingMultiplier);
            }

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