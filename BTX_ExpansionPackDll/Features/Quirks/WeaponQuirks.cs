using BattleTech;
using global::Quirks.Tooltips;
using System.Linq;

namespace BTX_ExpansionPack.Features.Quirks
{
    public static class WeaponQuirks
    {
        #region Quirk Descriptions

        /// <summary>
        /// Adds custom good quirk descriptions to the mech tooltips.
        /// </summary>
        [HarmonyPatch(typeof(QuirkToolTips), "DetailMechQuirksGood")]
        public static class QuirkToolTips_DetailMechQuirksGood
        {
            [HarmonyPostfix]
            public static void Postfix(ChassisDef chassisDef, ref string __result)
            {
                if (chassisDef.ChassisTags.Contains(QuirkTags.ACCURATE_RT_PPC))
                    __result += "\nAccurate Weapon: Any right torso mounted PPC gains +1 accuracy";
                if (chassisDef.ChassisTags.Contains(QuirkTags.ACCURATE_LT))
                    __result += "\nAccurate Weapon: Any left torso mounted weapon gains +1 accuracy";
                if (chassisDef.ChassisTags.Contains(QuirkTags.ACCURATE_LEGS))
                    __result += "\nAccurate Weapon: Any leg mounted weapon gains +1 accuracy";
            }
        }

        /// <summary>
        /// Adds custom bad quirk descriptions to the mech tooltips.
        /// </summary>
        [HarmonyPatch(typeof(QuirkToolTips), "DetailMechQuirksBad")]
        public static class QuirkToolTips_DetailMechQuirksBad
        {
            [HarmonyPostfix]
            public static void Postfix(ChassisDef chassisDef, ref string __result)
            {
                if (chassisDef.ChassisTags.Contains(QuirkTags.AMMOFEED_PROBLEM))
                    __result += "\nAmmo Feed Problem: Any ammo-fed weapon has a chance to jam";

                if (chassisDef.ChassisTags.Contains(QuirkTags.EM_INTERFERENCE))
                    __result += "\nEM Interference: 'Mech doesn't benefit from friendly ECM fields";

                if (chassisDef.ChassisTags.Contains(QuirkTags.EXPOSED_LINKAGE_RT))
                    __result += "\nExposed Weapon Linkage: Weapons in the right torso can be crit through armor";

            }
        }

        #endregion

        #region Quirk Delayed Effects

        /// <summary>
        /// Initializes weapon quirks in the custom quirk store.
        /// </summary>
        [HarmonyPatch(typeof(Mech), "InitStats")]
        public static class Mech_InitStats
        {
            [HarmonyPrefix]
            [HarmonyWrapSafe]
            public static void Prefix(Mech __instance)
            {
                var customQuirks = __instance.MechDef.GetOrSetCustomQuirks();

                // Delayed Effects
                foreach (string tag in __instance.MechDef.Chassis.ChassisTags.Where(tag => tag.StartsWith("weapon_quirk_")))
                {
                    // Accurate Weapon
                    if (tag == QuirkTags.ACCURATE_RT_PPC)
                        customQuirks.AccurateRTPPC = true;
                    if (tag == QuirkTags.ACCURATE_LT)
                        customQuirks.AccurateLT = true;
                    if (tag == QuirkTags.ACCURATE_LEGS)
                        customQuirks.AccurateLegs = true;

                    // Exposed Weapon Linkage
                    if (tag == QuirkTags.EXPOSED_LINKAGE_RT)
                        customQuirks.ExposedWeaponLinkageRT = true;

                    if (tag == QuirkTags.FAST_RELOAD)
                        customQuirks.FastReload = true;
                    if (tag == QuirkTags.IMPROVED_COOLING)
                        customQuirks.ImprovedCooling = true;
                    if (tag == QuirkTags.JETTISON_CAPABLE)
                        customQuirks.JettisonCapable = true;
                    if (tag == QuirkTags.MODULAR)
                        customQuirks.Modular = true;
                    if (tag == QuirkTags.POOR_COOLING)
                        customQuirks.PoorCooling = true;
                }

                // Instant Effects
                if (__instance.MechDef.Chassis.ChassisTags.Contains(QuirkTags.AMMOFEED_PROBLEM))
                {
                    var effectManager = UnityGameInstance.BattleTechGame.Combat.EffectManager;
                    effectManager.CreateEffect(CustomQuirkEffects.AmmoFeedProblemEffect, "AmmoFeedProblemJamming", UnityEngine.Random.Range(1, int.MaxValue), __instance, __instance, default, 0, false);
                }
                else if (__instance.MechDef.Chassis.ChassisTags.Contains(QuirkTags.EM_INTERFERENCE))
                {
                    var effectManager = UnityGameInstance.BattleTechGame.Combat.EffectManager;
                    effectManager.CreateEffect(CustomQuirkEffects.EMInterferenceEffect, "EMInterferenceECM", UnityEngine.Random.Range(1, int.MaxValue), __instance, __instance, default, 0, false);
                }
            }
        }

        /// <summary>
        /// Applies Accurate Weapon bonuses to to-hit modifiers.
        /// </summary>
        [HarmonyPatch(typeof(ToHit), "GetSelfArmMountedModifier")]
        public static class ToHit_GetSelfArmMountedModifier
        {
            [HarmonyPostfix]
            public static void Postfix(Weapon weapon, ref float __result)
            {
                if (weapon.parent is not Mech mech) return;

                var mountedLocation = weapon.mechComponentRef.MountedLocation;
                switch (mountedLocation)
                {
                    case ChassisLocations.LeftTorso:
                        {
                            bool accurateLT = CustomQuirkStore[mech.GUID].AccurateLT;
                            if (accurateLT)
                                __result++;
                            break;
                        }
                    case ChassisLocations.RightTorso:
                        {
                            bool accurateRTPPC = CustomQuirkStore[mech.GUID].AccurateRTPPC;
                            if (accurateRTPPC && weapon.Type == WeaponType.PPC)
                                __result++;
                            break;
                        }
                    case ChassisLocations.LeftLeg or ChassisLocations.RightLeg:
                        {
                            bool accurateLegs = CustomQuirkStore[mech.GUID].AccurateLegs;
                            if (accurateLegs)
                                __result++;
                            break;
                        }
                }
            }
        }

        // TODO: Patch Mech_ResolveWeaponDamage, add ExposedWeaponLinkageRT (crit through armor)
        // The patch logic is quite complex; will try to add it later. 

        #endregion
    }
}