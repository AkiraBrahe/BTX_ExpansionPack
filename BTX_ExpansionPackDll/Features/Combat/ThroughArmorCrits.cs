using BattleTech;
using CustAmmoCategories;

namespace BTX_ExpansionPack.Features.Combat
{
    internal class ThroughArmorCrits
    {
        /// <summary>
        /// Changes all Gauss Rifle variants to deal through armor criticals.
        /// </summary>
        /// <remarks>
        /// This replaces the structure damage that SLDF Gauss Rifles and Rail Guns dealt and essentially gives all Gauss Rifles this property to varying degrees.
        /// </remarks>
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
                    __instance.StructureDamage = 0f;

                    if (id != null && id.StartsWith("Weapon_Gauss_RailGun"))
                    {
                        if (id == "Weapon_Gauss_RailGun_2-BS")
                            __instance.Description.Details = "A Star League era relic using lost technology, the Rail Gun electromagnetically hurls massive metal projectiles. Those who assembled The General stripped the weapon of everything they could, which allowed it to be lightened to 25 tons, but at the expense of significantly worse heat dissipation. And there's no guarantee it's safe to fire...\n\nRail Guns explode if destroyed, taking the entire mounted location with them.";
                        else
                            __instance.Description.Details = "A Star League era relic using lost technology, the Rail Gun electromagnetically hurls massive metal projectiles. The staggering kinetic energy of its shells makes it an excellent armor-piercing weapon, easily penetrating the thickest plates to destroy internal systems. Like Gauss Rifles, Rail Guns cause instability when fired.\n\nRail Guns explode if destroyed, taking the entire mounted location with them.";
                    }
                    else if (id != null && id.StartsWith("Weapon_Gauss_Gauss") && !id.Contains("Magshot"))
                    {
                        __instance.Description.Details = "A Gauss Rifle uses electromagnetic charges to accelerate metallic rounds at extreme speeds, dealing massive damage by kinetic force alone. The high-velocity impact allows slugs to punch through armor to damage internal components. Unlike Autocannons, Gauss Rifles cause instability rather than recoil when fired.\n\nGauss Rifles explode if destroyed, taking the entire mounted location with them.";
                    }
                    else if (id != null && id.StartsWith("Weapon_Gauss_CGauss"))
                    {
                        __instance.Description.Details = "A Gauss Rifle uses electromagnetic charges to accelerate metallic rounds at extreme speeds, dealing massive damage by kinetic force alone. The high-velocity impact allows slugs to punch through armor to damage internal components. Unlike Autocannons, Gauss Rifles cause instability rather than recoil when fired.\n\nGauss Rifles explode if destroyed, taking the entire mounted location with them. Clan Gauss Rifles are lighter weight than their Inner Sphere counterparts.";
                    }

                    var extendedData = __instance.exDef();
                    if (extendedData != null)
                    {
                        bool isHAG = id != null && id.StartsWith("Weapon_Gauss_HAG");
                        bool isSBG = id != null && id.StartsWith("Weapon_Gauss_Silver_Bullet");
                        extendedData.APCriticalChanceMultiplier = (isHAG || isSBG) ? 0.25f : __instance.Damage * 0.02f;
                        extendedData.APMaxArmorThickness = __instance.Damage * 3.0f;
                        extendedData.APArmorShardsMod = 0f;
                    }
                    else
                    {
                        Main.Log.LogError($"[ThroughArmorCrits] Failed to retrieve extended data for weapon {id}. AP critical hit modifiers will not be applied.");
                    }
                }
            }
        }
    }
}