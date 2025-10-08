using Extended_CE;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace BTX_ExpansionPack.Fixes
{
    /// <summary>
    /// Loads custom actuator settings from EP's MechSettings.json.
    /// </summary>
    [HarmonyPatch(typeof(BTComponents), "Actuators", MethodType.Getter)]
    public static class MechActuators
    {
        private static bool patched = false;

        [HarmonyPostfix]
        public static void Postfix(ref ActuatorInfo __result)
        {
            if (patched || __result == null)
                return;

            try
            {
                string mechSettingsPath = Path.Combine(Main.modDir, "MechSettings.json");
                if (!File.Exists(mechSettingsPath))
                {
                    Main.Log.LogDebug("MechSettings.json not found, skipping merge.");
                    patched = true;
                    return;
                }

                var actuatorInfo = JsonConvert.DeserializeObject<ActuatorInfo>(File.ReadAllText(mechSettingsPath));
                __result.MechsWithoutLeftArmLower.AddRange(actuatorInfo.MechsWithoutLeftArmLower.Except(__result.MechsWithoutLeftArmLower));
                __result.MechsWithoutRightArmLower.AddRange(actuatorInfo.MechsWithoutRightArmLower.Except(__result.MechsWithoutRightArmLower));
                __result.MechsWithoutLeftHand.AddRange(actuatorInfo.MechsWithoutLeftHand.Except(__result.MechsWithoutLeftHand));
                __result.MechsWithoutRightHand.AddRange(actuatorInfo.MechsWithoutRightHand.Except(__result.MechsWithoutRightHand));

                Main.Log.LogDebug("Loaded EP's mech actuator settings.");
            }
            catch (System.Exception ex)
            {
                Main.Log.LogException(ex);
            }

            patched = true;
        }
    }
}