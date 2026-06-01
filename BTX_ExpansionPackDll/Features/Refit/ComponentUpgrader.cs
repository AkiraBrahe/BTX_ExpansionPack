using BattleTech;
using BTRandomMechComponentUpgrader;
using BTX_CAC_CompatibilityDll;
using CustomUnits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Features.Refit
{
    /// <summary>
    /// Improves the ammo adjustment logic from CAC-C and adds more special ammo types.
    /// </summary>
    internal class ComponentUpgrader
    {
        public static void Register()
        {
            Modifier_AmmoSwapper.SmartAmmoAdjust = SmartAmmoAdjust;
            Main.Log.LogDebug("Successfully replaced the Smart Ammo Adjust logic.");
        }

        private static void SmartAmmoAdjust(MechDef m, SimGameState s, UpgradeList l, float canFreeTonns, AmmoTracker ammo, MechDef fromData, FactionValue team)
        {
            if (team?.Name == null) return;

            bool pirate = team.IsPirate;
            bool kurita = team.Name.StartsWith("Kurita");
            bool davion = team.Name.StartsWith("Davion");
            bool marik = team.Name.StartsWith("Marik");
            bool liao = team.Name.StartsWith("Liao");
            bool isClan = team.IsClan;
            string factionShortName = team.FactionDef?.ShortName ?? "";

            var rand = s.NetworkRandom;
            string mood = s.SelectedContract?.mapMood;
            if (mood == null) Main.Log.Log("warning: contract mood null");

            Main.Log.Log($"handling {m.Description.Id} of {team.Name} in mood {mood.SafeToString()}");

            foreach (var kv in ammo.AmmoGroups)
            {
                if (string.IsNullOrEmpty(kv.Key))
                    continue;

                var ideal = kv.Value.IdealAmmoRatios;
                Main.Log.Log($"handling group {kv.Key}");

                if (kv.Key.StartsWith("AC"))
                {
                    var ammos = kv.Value.LongestSublist.Get(SubListType.Ammo);
                    var std = ammos.FirstOrDefault();
                    var ap = ammos.FirstOrDefault(x => x.ID.EndsWith("AP") && !kv.Value.AmmoLockout.Contains(x.ID));
                    var lbx = ammos.FirstOrDefault(x => x.ID.EndsWith("X") && !kv.Value.AmmoLockout.Contains(x.ID));
                    var prec = ammos.FirstOrDefault(x => x.ID.EndsWith("Precision") && !kv.Value.AmmoLockout.Contains(x.ID));
                    var tracer = ammos.FirstOrDefault(x => x.ID.EndsWith("Tracer") && !kv.Value.AmmoLockout.Contains(x.ID));

                    if (std == null)
                        continue;

                    if (!ideal.TryGetValue(std.ID, out int stdcount))
                        stdcount = 0;
                    if (tracer == null || !ideal.TryGetValue(tracer.ID, out int tracercount))
                        tracercount = 0;

                    if (stdcount > tracercount)
                    {
                        Main.Log.Log("has uacs, no special ammo");
                        tracer = null;
                        prec = null;
                        ap = null;
                    }

                    if (mood == null || !(mood.Contains("Night") || mood.Contains("Sunset") || mood.Contains("Twilight")))
                        tracer = null;
                    if (prec != null && prec.MinDate > s.CurrentDate)
                        prec = null;
                    if (ap != null && ap.MinDate > s.CurrentDate)
                        ap = null;

                    kv.Value.IdealBoxes.Clear();
                    int totalBoxes = kv.Value.CurrentAmmoRatios.Values.Sum();

                    var originalBoxIDs = new List<string>();
                    foreach (var pair in kv.Value.CurrentAmmoRatios)
                    {
                        for (int c = 0; c < pair.Value; c++)
                        {
                            originalBoxIDs.Add(pair.Key);
                        }
                    }

                    for (int i = 0; i < totalBoxes; ++i)
                    {
                        string originalID = i < originalBoxIDs.Count ? originalBoxIDs[i] : std.ID;
                        string replacementID = originalID;

                        // Roll for AC special ammo replacements based on converted ratios
                        if (lbx != null && rand.Float() < 0.50f)
                        {
                            replacementID = lbx.ID;
                        }
                        else if (tracer != null && rand.Float() < 0.75f)
                        {
                            replacementID = tracer.ID;
                        }
                        else if (davion && prec != null && rand.Float() < 0.20f)
                        {
                            replacementID = prec.ID;
                        }
                        else if (davion && ap != null && rand.Float() < 0.20f)
                        {
                            replacementID = ap.ID;
                        }

                        AddToDict(kv.Value.IdealBoxes, replacementID, 1);
                    }
                }
                else if (kv.Key == "SRM")
                {
                    var ammos = kv.Value.LongestSublist.Get(SubListType.Ammo);
                    var std = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_SRM");
                    var df = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_SRM_DF" && !kv.Value.AmmoLockout.Contains(x.ID));
                    var inf = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_SRM_Inferno" && !kv.Value.AmmoLockout.Contains(x.ID));

                    if (std == null)
                        continue;

                    kv.Value.IdealBoxes.Clear();
                    int totalBoxes = kv.Value.CurrentAmmoRatios.Values.Sum();

                    var originalBoxIDs = new List<string>();
                    foreach (var pair in kv.Value.CurrentAmmoRatios)
                    {
                        for (int c = 0; c < pair.Value; c++)
                        {
                            originalBoxIDs.Add(pair.Key);
                        }
                    }

                    for (int i = 0; i < totalBoxes; ++i)
                    {
                        string originalID = i < originalBoxIDs.Count ? originalBoxIDs[i] : std.ID;
                        string replacementID = originalID;

                        if (kurita && df != null)
                        {
                            float chance = s.CurrentDate < new DateTime(3058, 1, 1)
                                ? InterpolateChance(s.CurrentDate, new DateTime(3052, 1, 1), new DateTime(3058, 1, 1), 0.15f, 0.05f)
                                : 0.05f;
                            if (rand.Float() < chance)
                            {
                                replacementID = df.ID;
                            }
                        }
                        else if (liao && inf != null)
                        {
                            if (rand.Float() < 0.15f)
                            {
                                replacementID = inf.ID;
                            }
                        }
                        else if (pirate && inf != null)
                        {
                            if (rand.Float() < (AEPStatic.GetTimelineSettings().PirateInfernoChance / 100.0f))
                            {
                                replacementID = inf.ID;
                            }
                        }

                        AddToDict(kv.Value.IdealBoxes, replacementID, 1);
                    }
                }
                else if (kv.Key == "LRM")
                {
                    var ammos = kv.Value.LongestSublist.Get(SubListType.Ammo);
                    var std = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_LRM");
                    var df = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_LRM_DF" && !kv.Value.AmmoLockout.Contains(x.ID));
                    var swarm = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_LRM_Swarm" && !kv.Value.AmmoLockout.Contains(x.ID));
                    var swarmI = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_LRM_Swarm-I" && !kv.Value.AmmoLockout.Contains(x.ID));
                    var inf = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_LRM_Inferno" && !kv.Value.AmmoLockout.Contains(x.ID));

                    if (std == null)
                        continue;

                    kv.Value.IdealBoxes.Clear();
                    int totalBoxes = kv.Value.CurrentAmmoRatios.Values.Sum();

                    var originalBoxIDs = new List<string>();
                    foreach (var pair in kv.Value.CurrentAmmoRatios)
                    {
                        for (int c = 0; c < pair.Value; c++)
                        {
                            originalBoxIDs.Add(pair.Key);
                        }
                    }

                    for (int i = 0; i < totalBoxes; ++i)
                    {
                        string originalID = i < originalBoxIDs.Count ? originalBoxIDs[i] : std.ID;
                        string replacementID = originalID;

                        if (liao && inf != null)
                        {
                            float chance = InterpolateChance(s.CurrentDate, new DateTime(3056, 1, 1), new DateTime(3062, 1, 1), 0.10f);
                            if (rand.Float() < chance)
                            {
                                replacementID = inf.ID;
                            }
                        }
                        else if (marik && s.CurrentDate < new DateTime(3057, 1, 1) && swarm != null)
                        {
                            float chance = InterpolateChance(s.CurrentDate, new DateTime(3053, 1, 1), new DateTime(3058, 1, 1), 0.10f);
                            if (rand.Float() < chance)
                            {
                                replacementID = swarm.ID;
                            }
                        }
                        else if (marik && s.CurrentDate >= new DateTime(3057, 1, 1) && swarmI != null)
                        {
                            float chance = InterpolateChance(s.CurrentDate, new DateTime(3057, 1, 1), new DateTime(3066, 1, 1), 0.10f);
                            if (rand.Float() < chance)
                            {
                                replacementID = swarmI.ID;
                            }
                        }
                        else if (factionShortName == "ComStar" && swarm != null)
                        {
                            float chance = InterpolateChance(s.CurrentDate, new DateTime(3053, 1, 1), new DateTime(3058, 1, 1), 0.15f);
                            if (rand.Float() < chance)
                            {
                                replacementID = swarm.ID;
                            }
                        }
                        else if (factionShortName == "Word of Blake" && swarmI != null)
                        {
                            float chance = InterpolateChance(s.CurrentDate, new DateTime(3057, 1, 1), new DateTime(3066, 1, 1), 0.15f);
                            if (rand.Float() < chance)
                            {
                                replacementID = swarmI.ID;
                            }
                        }
                        else if ((isClan || factionShortName == "Black Widow Company" || factionShortName == "Wolf's Dragoons") && swarm != null)
                        {
                            if (rand.Float() < 0.10f)
                            {
                                replacementID = swarm.ID;
                            }
                        }
                        else if (kurita && df != null && df.MinDate <= s.CurrentDate)
                        {
                            if (rand.Float() < 0.50f)
                            {
                                replacementID = df.ID;
                            }
                        }

                        AddToDict(kv.Value.IdealBoxes, replacementID, 1);
                    }
                }
                else if (kv.Key == "ArrowIV")
                {
                    var ammos = kv.Value.LongestSublist.Get(SubListType.Ammo);
                    var std = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_ArrowIV");
                    var homing = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_ArrowIV_Homing" && !kv.Value.AmmoLockout.Contains(x.ID));
                    var inf = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_ArrowIV_Inferno" && !kv.Value.AmmoLockout.Contains(x.ID));

                    if (std == null)
                        continue;

                    kv.Value.IdealBoxes.Clear();
                    int totalBoxes = kv.Value.CurrentAmmoRatios.Values.Sum();

                    var originalBoxIDs = new List<string>();
                    foreach (var pair in kv.Value.CurrentAmmoRatios)
                    {
                        for (int c = 0; c < pair.Value; c++)
                        {
                            originalBoxIDs.Add(pair.Key);
                        }
                    }

                    for (int i = 0; i < totalBoxes; ++i)
                    {
                        string originalID = i < originalBoxIDs.Count ? originalBoxIDs[i] : std.ID;
                        string replacementID = originalID;

                        if (liao && inf != null)
                        {
                            float chance = InterpolateChance(s.CurrentDate, new DateTime(3053, 1, 1), new DateTime(3083, 1, 1), 0.10f, 0.01f);
                            if (rand.Float() < chance)
                            {
                                replacementID = inf.ID;
                            }
                        }
                        else if (!isClan && homing != null && team.AnyUnitHasTAG())
                        {
                            if (originalID == "Ammo_AmmunitionBox_Generic_ArrowIV")
                            {
                                if (rand.Float() < 0.25f)
                                {
                                    replacementID = homing.ID;
                                }
                            }
                        }

                        AddToDict(kv.Value.IdealBoxes, replacementID, 1);
                    }
                }
            }
        }

        private static float InterpolateChance(DateTime currentDate, DateTime productionDate, DateTime commonDate, float maxChance, float minChance = 0.05f)
        {
            if (currentDate < productionDate) return 0.00f;
            if (currentDate >= commonDate) return maxChance;

            double currentDays = (currentDate - productionDate).TotalDays;
            double totalDays = (commonDate - productionDate).TotalDays;

            float interpolationFactor = (float)(currentDays / totalDays);
            return minChance + ((maxChance - minChance) * interpolationFactor);
        }

        private static void AddToDict(Dictionary<string, int> dict, string key, int value)
        {
            if (dict.TryGetValue(key, out int current))
                dict[key] = current + value;
            else
                dict[key] = value;
        }
    }
}