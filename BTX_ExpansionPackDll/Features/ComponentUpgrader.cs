﻿using BattleTech;
using BTRandomMechComponentUpgrader;
using BTX_CAC_CompatibilityDll;
using CustomUnits;
using System.Collections.Generic;
using System.Linq;

namespace BTX_ExpansionPack.Features
{
    /// <summary>
    /// Updates the ammo adjustment logic from CAC-C, adding new special ammunition.
    /// </summary>
    internal class ComponentUpgrader
    {
        public static void Register()
        {
            Modifier_AmmoSwapper.SmartAmmoAdjust = SmartAmmoAdjust;
            Main.Log.LogDebug("Successfully updated SmartAmmoAdjust logic.");
        }

        private static void SmartAmmoAdjust(MechDef m, SimGameState s, UpgradeList l, float canFreeTonns, AmmoTracker ammo, MechDef fromData, FactionValue team)
        {
            bool pirate = team.IsPirate;
            bool kurita = team.Name.StartsWith("Kurita");
            bool davion = team.Name.StartsWith("Davion");
            bool marik = team.Name.StartsWith("Marik");
            var rand = s.NetworkRandom;
            var mood = s.SelectedContract?.mapMood;
            if (mood == null)
                Main.Log.Log("warning: contract mood null");
            Main.Log.Log($"handling {m.Description.Id} of {team.Name} in mood {mood.SafeToString()}");
            foreach (var kv in ammo.AmmoGroups)
            {
                if (kv.Key == "")
                    continue;
                Dictionary<string, int> ideal = kv.Value.IdealAmmoRatios;
                Main.Log.Log($"handling group {kv.Key}");
                if (kv.Key.StartsWith("AC"))
                {
                    var ammos = kv.Value.LongestSublist.Get(SubListType.Ammo);
                    var std = ammos.FirstOrDefault();
                    var ap = ammos.FirstOrDefault(x => x.ID.EndsWith("AP") && !kv.Value.AmmoLockout.Contains(x.ID));
                    var lbx = ammos.FirstOrDefault(x => x.ID.EndsWith("X") && !kv.Value.AmmoLockout.Contains(x.ID));
                    var prec = ammos.FirstOrDefault(x => x.ID.EndsWith("Precision") && !kv.Value.AmmoLockout.Contains(x.ID));
                    var tracer = ammos.FirstOrDefault(x => x.ID.EndsWith("Tracer") && !kv.Value.AmmoLockout.Contains(x.ID));

                    if (!ideal.TryGetValue(std.ID, out var stdcount))
                        stdcount = 0;
                    if (tracer == null || !ideal.TryGetValue(tracer.ID, out var tracercount))
                        tracercount = 0;
                    if (lbx == null || !ideal.TryGetValue(tracer.ID, out var lbxcount))
                        lbxcount = 0;
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

                    ideal.Clear();
                    ideal[std.ID] = stdcount * 100;

                    if (lbx != null)
                    {
                        Main.Log.Log("add lbx");
                        ideal[lbx.ID] = lbxcount * 100;
                    }
                    if (tracer != null)
                    {
                        Main.Log.Log("add tracer");
                        ideal[tracer.ID] = stdcount * 300;
                    }
                    if (davion && prec != null)
                    {
                        Main.Log.Log("add precision");
                        ideal[prec.ID] = stdcount * 25;
                    }
                    if (davion && ap != null)
                    {
                        Main.Log.Log("add ap");
                        ideal[ap.ID] = stdcount * 25;
                    }
                    kv.Value.RollForIdealBoxes(l, rand, s.CurrentDate);
                }
                else if (kv.Key == "SRM")
                {
                    var ammos = kv.Value.LongestSublist.Get(SubListType.Ammo);
                    var std = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_SRM");
                    var df = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_SRM_DF" && !kv.Value.AmmoLockout.Contains(x.ID));
                    var inf = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_SRM_Inferno" && !kv.Value.AmmoLockout.Contains(x.ID));

                    if (inf != null && ideal[inf.ID] < ideal[std.ID])
                        inf = null;
                    if (df != null && ideal[df.ID] < ideal[std.ID])
                        df = null;

                    ideal.Clear();
                    ideal[std.ID] = 1;

                    if (pirate && inf != null && rand.Float() < (AEPStatic.GetTimelineSettings().PirateInfernoChance / 100.0f))
                    {
                        ideal[inf.ID] = 10;
                        Main.Log.Log("add inferno");
                    }
                    if (kurita && df != null && df.MinDate <= s.CurrentDate)
                    {
                        ideal[df.ID] = 1;
                        Main.Log.Log("add DF");
                    }
                    kv.Value.RollForIdealBoxes(l, rand, s.CurrentDate);
                }
                else if (kv.Key == "LRM")
                {
                    var ammos = kv.Value.LongestSublist.Get(SubListType.Ammo);
                    var std = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_LRM");
                    var df = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_LRM_DF" && !kv.Value.AmmoLockout.Contains(x.ID));
                    var swarm = ammos.FirstOrDefault(x => x.ID == "Ammo_AmmunitionBox_Generic_LRM_Swarm" && !kv.Value.AmmoLockout.Contains(x.ID));

                    if (df != null && ideal[df.ID] < ideal[std.ID])
                        df = null;
                    if (swarm != null && ideal[swarm.ID] < ideal[std.ID])
                        swarm = null;

                    ideal.Clear();
                    ideal[std.ID] = 1;

                    if (kurita && df != null && df.MinDate <= s.CurrentDate)
                    {
                        ideal[df.ID] = 1;
                        Main.Log.Log("add DF");
                    }
                    if (marik && swarm != null && swarm.MinDate <= s.CurrentDate)
                    {
                        ideal[swarm.ID] = 1;
                        Main.Log.Log("add swarm");
                    }
                    kv.Value.RollForIdealBoxes(l, rand, s.CurrentDate);
                }
            }
        }
    }
}