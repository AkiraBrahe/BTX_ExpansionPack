using BattleTech;
using Quirks;
using System.Collections.Generic;

namespace BTX_ExpansionPack.Core.Data
{
    public static class QuirkData
    {
        #region Quirk Tags

        public static class QuirkTags
        {
            // Mech Quirks
            public const string ANTI_AIRCRAFT = "mech_quirk_antiaircraft";
            public const string DIRECTIONAL_TORSO_MOUNT = "mech_quirk_directionaltorsomount";
            public const string EASY_TO_PILOT = "mech_quirk_easytopilot";
            public const string EXTENDED_TORSO_TWIST = "mech_quirk_extendedtorsotwist";
            public const string NO_TORSO_TWIST = "mech_quirk_notorsotwist";
            public const string POOR_PERFORMANCE = "mech_quirk_poorperformance";
            public const string POOR_WORKMANSHIP = "mech_quirk_poorworkmanship";

            // Weapon Quirks
            public const string ACCURATE_LEGS = "weapon_quirk_accurate_Legs";
            public const string ACCURATE_LT = "weapon_quirk_accurate_LT";
            public const string ACCURATE_RT_PPC = "weapon_quirk_accurate_RT_PPC";
            public const string AMMOFEED_PROBLEM = "weapon_quirk_ammofeedproblem";
            public const string EM_INTERFERENCE = "weapon_quirk_eminterference";
            public const string EXPOSED_LINKAGE_RT = "weapon_quirk_exposedlinkage_RT";
            public const string FAST_RELOAD = "weapon_quirk_fast_reload";
            public const string IMPROVED_COOLING = "weapon_quirk_improved_cooling";
            public const string JETTISON_CAPABLE = "weapon_quirk_jettison_capable";
            public const string MODULAR = "weapon_quirk_modular";
            public const string POOR_COOLING = "weapon_quirk_poor_cooling";
        }

        #endregion

        #region Custom Quirk Data

        public static readonly Dictionary<string, CustomQuirkList> CustomQuirkStore = [];
        public class CustomQuirkList
        {
            // OK
            public bool AccurateLegs = false;
            public bool AccurateLT = false;
            public bool AccurateRTPPC = false;
            public bool ExposedWeaponLinkageRT = false;

            // WIP
            public bool FastReload = false;
            public bool ImprovedCooling = false;
            public bool JettisonCapable = false;
            public bool Modular = false;
            public bool PoorCooling = false;
            public bool PoorPerformance = false;

            // public bool DirectionalMount = false; // Already exists as Directional Torso Mount quirk
        }

        #endregion

        #region Custom Quirk Effects

        internal static class CustomQuirkEffects
        {
            internal static EffectData AmmoFeedProblemEffect => new()
            {
                effectType = EffectType.StatisticEffect,
                targetingData = QuirkStatusEffects.ShowInStatus,
                Description = new DescriptionDef(
                    nameof(AmmoFeedProblemEffect),
                    "AMMO FEED PROBLEM",
                    "Ammunition feed issues give this 'Mech a 15% chance to jam ammo-fed weapon systems.",
                    "uixSvgIcon_status_sensorsImpaired",
                    0, 0f, false, null, null, null),
                durationData = QuirkStatusEffects.Duration,
                statisticData = new StatisticEffectData()
                {
                    statName = "CACFlatJammingChance",
                    operation = StatCollection.StatOperation.Float_Add,
                    modValue = "0.15",
                    modType = "System.Single"
                }
            };

            internal static EffectData EMInterferenceEffect => new()
            {
                effectType = EffectType.StatisticEffect,
                targetingData = QuirkStatusEffects.ShowInStatus,
                Description = new DescriptionDef(
                    nameof(EMInterferenceEffect),
                    "EM INTERFERENCE",
                    "This Mech's poor electromagnetic shielding makes it less protected by a friendly ECM.",
                    "uixSvgIcon_status_sensorsImpaired",
                    0, 0f, false, null, null, null),
                durationData = QuirkStatusEffects.Duration,
                statisticData = new StatisticEffectData()
                {
                    statName = "DefendedByECM",
                    operation = StatCollection.StatOperation.Float_Add,
                    modValue = "-1.0",
                    modType = "System.Single"
                }
            };

            internal static EffectData EasyToPilotEffect => new()
            {
                effectType = EffectType.StatisticEffect,
                targetingData = QuirkStatusEffects.OnActivation,
                Description = new DescriptionDef(
                    "TraitDefEvasiveChargeAddOne",
                    "INCREASED EVASION",
                    "Gains +[AMT] EVASIVE charge when moving",
                    "uixSvgIcon_ability_mastertactician",
                    0, 0f, false, null, null, null),
                durationData = QuirkStatusEffects.Duration,
                statisticData = new StatisticEffectData
                {
                    statName = "EvasivePipsGainedAdditional",
                    operation = StatCollection.StatOperation.Int_Add,
                    modValue = "1",
                    modType = "System.Int32"
                },
                nature = EffectNature.Buff
            };

            internal static EffectData PoorWorkmanshipEffect => new()
            {
                effectType = EffectType.StatisticEffect,
                targetingData = QuirkStatusEffects.ShowInStatus,
                Description = new DescriptionDef(
                    nameof(PoorWorkmanshipEffect),
                    "POOR WORKMANSHIP",
                    "This 'Mech's subpar manufacturing makes it 25% more susceptible to critical hits.",
                    "uixSvgIcon_special_Equipment",
                    0, 0f, false, null, null, null),
                durationData = QuirkStatusEffects.Duration,
                statisticData = new StatisticEffectData()
                {
                    statName = "CAC_FlatCritChance",
                    operation = StatCollection.StatOperation.Float_Multiply,
                    modValue = "1.25",
                    modType = "System.Single"
                }
            };
        }

        #endregion
    }
}