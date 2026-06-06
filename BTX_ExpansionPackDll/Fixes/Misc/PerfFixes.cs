using BattleTech.Save;
using CustAmmoCategories;
using CustomSettings;

namespace BTX_ExpansionPack.Fixes.Misc
{
    /// <summary>
    /// Improves performance during combat by disabling various debug logs in CAC.
    /// </summary>
    [HarmonyPatch(typeof(SaveManager), MethodType.Constructor, [typeof(MessageCenter)])]
    public static class SaveManager_Constructor
    {
        [HarmonyPrepare]
        public static bool Prepare() => !Main.Settings.Debug.DebugLogging;

        [HarmonyPostfix]
        public static void Postfix()
        {
            CustomUnits.Core.Settings.debugLog = false;
            CustomAmmoCategories.Settings.debugLog = false;
            CustomAmmoCategories.Settings.AttackLogWrite = false;
            ModsLocalSettingsHelper.SaveSettings("CustomAmmoCategoriesSettings");
        }
    }
}