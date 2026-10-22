using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BTX_ExpansionPack.Fixes.Misc
{
    /// <summary>
    /// Allows to add blacklisted mechs via "Fell Off A Cargo Ship".
    /// </summary>
    [HarmonyPatch]
    public static class MechBlacklistBypass
    {
        [HarmonyPrepare]
        public static bool Prepare() => AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.GetName().Name.Equals("FellOffACargoShip"));

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            var type = Type.GetType("FellOffACargoShip.Cheater.Mech, FellOffACargoShip");
            return type != null ? AccessTools.Method(type, "<Add>g__AddMech|0_0") : null;
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Ldloc_1),
                    new CodeMatch(i => i.opcode.FlowControl == FlowControl.Cond_Branch))
                .ThrowIfInvalid("Failed to find blacklist check")
                .SetOpcodeAndAdvance(OpCodes.Ldc_I4_0)
                .InstructionEnumeration();
        }
    }

    /// <summary>
    /// Allows lostech weapons to be salvaged in battle by removing the blacklisted tag on them.
    /// </summary>
    [HarmonyPatch(typeof(WeaponDef), "FromJSON")]
    public static class WeaponDef_FromJSON
    {
        [HarmonyPostfix]
        public static void Postfix(WeaponDef __instance)
        {
            string id = __instance.Description?.Id;
            if (id != null && LostechWeapons.Contains(__instance.Description.Id))
                __instance.ComponentTags.Remove("BLACKLISTED");
        }

        private static readonly HashSet<string> LostechWeapons =
        [
            "Weapon_Autocannon_LB2X_0-STOCK",
            "Weapon_Autocannon_LB2X_1-Defiance",
            "Weapon_Autocannon_LB2X_2-Defiance",
            "Weapon_Autocannon_LB5X_0-STOCK",
            "Weapon_Autocannon_LB5X_1-GM",
            "Weapon_Autocannon_LB5X_2-GM",
            "Weapon_Autocannon_LB10X_0-STOCK",
            "Weapon_Autocannon_LB10X_1-Western",
            "Weapon_Autocannon_LB10X_2-Western",
            "Weapon_Autocannon_LB20X_0-STOCK",
            "Weapon_Autocannon_LB20X_1-Shengli_Arms",
            "Weapon_Autocannon_LB20X_2-Shengli_Arms",
            "Weapon_Autocannon_UAC2_0-STOCK",
            "Weapon_Autocannon_UAC2_1-Imperator",
            "Weapon_Autocannon_UAC2_2-Imperator",
            "Weapon_Autocannon_UAC5_0-STOCK",
            "Weapon_Autocannon_UAC5_1-Mydron",
            "Weapon_Autocannon_UAC5_2-Mydron",
            "Weapon_Autocannon_UAC10_0-STOCK",
            "Weapon_Autocannon_UAC10_1-Federated",
            "Weapon_Autocannon_UAC10_2-Federated",
            "Weapon_Autocannon_UAC20_0-STOCK",
            "Weapon_Autocannon_UAC20_1-Kali_Yama",
            "Weapon_Autocannon_UAC20_2-Kali_Yama",
            "Weapon_COIL_COIL-L_0-STOCK",
            "Weapon_COIL_COIL-M_0-STOCK",
            "Weapon_COIL_COIL-S_0-STOCK",
            "Weapon_Gauss_Gauss_0-STOCK",
            "Weapon_Gauss_Gauss_1-M7",
            "Weapon_Gauss_Gauss_2-M9",
            "Weapon_Laser_LargeLaserER_0-STOCK",
            "Weapon_Laser_LargeLaserER_1-Blankenburg25",
            "Weapon_Laser_LargeLaserER_2-BlazeFire",
            "Weapon_Laser_LargeLaserPulse_0-STOCK",
            "Weapon_Laser_LargeLaserPulse_1-Thunderbolt12",
            "Weapon_Laser_LargeLaserPulse_2-Exostar",
            "Weapon_Laser_MediumLaserER_0-STOCK",
            "Weapon_Laser_MediumLaserER_1-MagnaVI",
            "Weapon_Laser_MediumLaserER_2-BrightBloom",
            "Weapon_Laser_MediumLaserPulse_0-STOCK",
            "Weapon_Laser_MediumLaserPulse_1-RakerIV",
            "Weapon_Laser_MediumLaserPulse_2-Magna400P",
            "Weapon_Laser_SmallLaserER_0-STOCK",
            "Weapon_Laser_SmallLaserER_1-Diverse_Optics",
            "Weapon_Laser_SmallLaserER_2-BlazeFire",
            "Weapon_Laser_SmallLaserPulse_0-STOCK",
            "Weapon_Laser_SmallLaserPulse_1-Maxell",
            "Weapon_Laser_SmallLaserPulse_2-Magna200P",
            "Weapon_PPC_PPCER_0-STOCK",
            "Weapon_PPC_PPCER_1-MagnaFirestar",
            "Weapon_PPC_PPCER_2-TiegartMagnum",
            "Weapon_PPC_PPCSnub_0-STOCK",
            "Weapon_PPC_PPCSnub_1-Ceres_Arms",
            "Weapon_PPC_PPCSnub_1-Magna",
            "Weapon_PPC_PPCSnub_2-Ceres_Arms",
            "Weapon_PPC_PPCSnub_2-Magna"
        ];
    }
}