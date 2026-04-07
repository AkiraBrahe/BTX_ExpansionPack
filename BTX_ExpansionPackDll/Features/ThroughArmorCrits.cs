using BattleTech;
using CustAmmoCategories;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Features
{
    internal class ThroughArmorCrits
    {
        private static readonly HashSet<string> GaussWeaponIds =
        [
            "Weapon_Gauss_Gauss_0-STOCK",
            "Weapon_Gauss_Gauss_1-M7",
            "Weapon_Gauss_Gauss_2-M9",
            "Weapon_Gauss_Gauss_NU_0-STOCK",
            "Weapon_Gauss_Gauss_NU_1-Grizzard",
            "Weapon_Gauss_Gauss_NU_1-Imperator",
            "Weapon_Gauss_Gauss_NU_1-Kali_Yama",
            "Weapon_Gauss_Gauss_NU_2-Grizzard",
            "Weapon_Gauss_Gauss_NU_2-Imperator",
            "Weapon_Gauss_Gauss_NU_2-Kali_Yama",
            "Weapon_Gauss_Gauss_Sa_0-STOCK"
        ];

        private static readonly HashSet<string> RailGunWeaponIds =
        [
            "Weapon_Gauss_RailGun_0-STOCK",
            "Weapon_Gauss_RailGun_1-SCI",
            "Weapon_Gauss_RailGun_2-BL",
            "Weapon_Gauss_RailGun_2-SCI"
        ];

        [HarmonyPatch(typeof(WeaponDef), "FromJSON")]
        public static class WeaponDef_FromJSON
        {
            [HarmonyPrepare]
            public static bool Prepare() => Main.Settings.Gameplay.GaussDealThroughArmorCrits;

            [HarmonyPostfix]
            public static void Postfix(WeaponDef __instance)
            {
                if (__instance == null)
                    return;

                string id = __instance.Description?.Id;
                if (__instance.WeaponSubType == WeaponSubType.Gauss)
                {
                    if (id != null && GaussWeaponIds.Contains(id))
                    {
                        __instance.Description.Details = "A Gauss Rifle uses electromagnetic charges to accelerate metallic rounds at extreme speeds, dealing massive damage by kinetic force alone. The high-velocity impact allows slugs to punch through armor to damage internal components. Unlike Autocannons, Gauss Rifles cause instability rather than recoil when fired.\n\nGauss Rifles explode if destroyed, taking the entire mounted location with them.";
                    }
                    else if (id != null && RailGunWeaponIds.Contains(id))
                    {
                        if (id == "Weapon_Gauss_RailGun_2-BS")
                            __instance.Description.Details = "A Star League era relic using lost technology, the Rail Gun electromagnetically hurls massive metal projectiles. Those who assembled The General stripped the weapon of everything they could, which allowed it to be lightened to 25 tons, but at the expense of significantly worse heat dissipation. And there's no guarantee it's safe to fire...\n\nRail Guns explode if destroyed, taking the entire mounted location with them.";
                        else
                            __instance.Description.Details = "A Star League era relic using lost technology, the Rail Gun electromagnetically hurls massive metal projectiles. The staggering kinetic energy of its shells makes it an excellent armor-piercing weapon, easily penetrating the thickest plates to destroy internal systems. Like Gauss Rifles, Rail Guns cause instability when fired.\n\nRail Guns explode if destroyed, taking the entire mounted location with them.";
                    }

                    var extendedData = __instance.exDef();
                    if (extendedData != null)
                    {
                        bool isHAG = id != null && id.StartsWith("Weapon_Gauss_HAG");
                        extendedData.APCriticalChanceMultiplier = isHAG ? __instance.Damage * 0.005f : __instance.Damage * 0.02f;
                        extendedData.APMaxArmorThickness = __instance.Damage * 3.0f;
                        extendedData.APArmorShardsMod = 0f;
                    }
                }
            }
        }
    }
}