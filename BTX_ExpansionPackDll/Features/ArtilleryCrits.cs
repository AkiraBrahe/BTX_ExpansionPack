using BattleTech;
using CustAmmoCategories;
using IRBTModUtils;
using Localize;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace BTX_ExpansionPack.Features
{
    /// <summary>
    /// Adds a chance for artillery strikes to concentrate their damage on a single location, simulating a critical hit effect.
    /// <br>The chance of a critical hit is inversely proportional to the distance from the strike's center to the target, 
    /// meaning closer targets have a higher chance of receiving a critical hit. It is capped at 50% to make them more special.</br>
    /// </summary>
    /// <remarks>
    /// <list type="bullet">Artillery crit chance = (MaxCritChance - MinCritChance) * (1 - (DistanceToTarget / MaxEffectiveDistance)) + MinCritChance</list>
    /// </remarks>
    internal class ArtilleryCrits
    {
        private static readonly System.Random Rng = new();

        private static readonly HashSet<string> ArtilleryCritTargets = [];

        [HarmonyPatch(typeof(AreaOfEffectHelper), "AoEProcessing")]
        public static class AreaOfEffectHelper_AoEProcessing
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var matcher = new CodeMatcher(instructions, il);

                var distance = matcher.MatchForward(false, new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Vector3), nameof(Vector3.Distance)))).Advance(1).Operand;
                var realdistance = matcher.MatchForward(false, new CodeMatch(OpCodes.Ldloc_S, distance), new CodeMatch(OpCodes.Stloc_S)).Advance(1).Operand;
                var reachableLocations = matcher.MatchForward(false, new CodeMatch(OpCodes.Newobj, AccessTools.Constructor(typeof(HashSet<int>))), new CodeMatch(OpCodes.Stloc_S)).Advance(1).Operand;

                var mech = matcher.MatchForward(false, new CodeMatch(OpCodes.Isinst, typeof(Mech)), new CodeMatch(OpCodes.Stloc_S)).Advance(1).Operand;
                var custMech = matcher.MatchForward(false, new CodeMatch(OpCodes.Isinst, typeof(ICustomMech)), new CodeMatch(OpCodes.Stloc_S)).Advance(1).Operand;

                return matcher.Start()
                    .MatchForward(true,
                        new CodeMatch(OpCodes.Ldloc_S, custMech),
                        new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ICustomMech), nameof(ICustomMech.GetAOESpreadArmorLocations))))
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldloc_S, reachableLocations),
                        new CodeInstruction(OpCodes.Ldloc_S, realdistance))
                    .SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ArtilleryCrits), nameof(GetDynamicAoESpread))))

                    .MatchForward(true,
                        new CodeMatch(OpCodes.Ldloc_S, mech),
                        new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(GetAOESpreadLocationsHelper), nameof(GetAOESpreadLocationsHelper.GetAOESpreadArmorLocations))))
                    .InsertAndAdvance(
                        new CodeInstruction(OpCodes.Ldloc_S, reachableLocations),
                        new CodeInstruction(OpCodes.Ldloc_S, realdistance))
                    .SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ArtilleryCrits), nameof(GetDynamicAoESpread))))
                    .InstructionEnumeration();
            }
        }

        public static Dictionary<int, float> GetDynamicAoESpread(Mech target, ICollection<int> reachableLocations, float distanceToTarget)
        {
            const float minCritChance = 0.10f;
            const float maxCritChance = 0.50f;
            const float maxEffectiveDistance = 30f;
            const float critLocationWeightMultiplier = 4f;

            float critChance = Mathf.Lerp(maxCritChance, minCritChance, distanceToTarget / maxEffectiveDistance);
            if (distanceToTarget > maxEffectiveDistance || Rng.NextDouble() > critChance)
            {
                return GetAOESpreadLocationsHelper.GetAOESpreadArmorLocations(target);
            }

            var validLocations = reachableLocations.Where(loc => (ArmorLocation)loc != ArmorLocation.Head).ToList();
            if (validLocations.Count == 0)
            {
                return GetAOESpreadLocationsHelper.GetAOESpreadArmorLocations(target);
            }

            var dynamicSpread = new Dictionary<int, float>(GetAOESpreadLocationsHelper.GetAOESpreadArmorLocations(target));
            int critLocation = validLocations[Rng.Next(validLocations.Count)];
            if (dynamicSpread.ContainsKey(critLocation))
            {
                dynamicSpread[critLocation] *= critLocationWeightMultiplier;
            }
            else
            {
                dynamicSpread.Add(critLocation, 100f * critLocationWeightMultiplier);
            }

            if (target.team.LocalPlayerControlsTeam)
            {
                AudioEventManager.PlayAudioEvent("audioeventdef_musictriggers_combat", "critical_hit_friendly");
            }
            else if (!target.team.IsFriendly(target.Combat.LocalPlayerTeam))
            {
                AudioEventManager.PlayAudioEvent("audioeventdef_musictriggers_combat", "critical_hit_enemy");
            }

            ArtilleryCritTargets.Add(target.GUID);
            Main.Log.LogDebug($"[ArtilleryCrits] Critical Hit! (rolled < {critChance:P0})\nConcentrating damage on {(ArmorLocation)critLocation} location of {target.DisplayName}.");
            return dynamicSpread;
        }

        [HarmonyPatch(typeof(Mech), "TakeWeaponDamage")]
        public static class Mech_TakeWeaponDamage
        {
            [HarmonyPrefix]
            public static void Prefix(Mech __instance)
            {
                if (ArtilleryCritTargets.Contains(__instance.GUID))
                {
                    __instance.Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.GUID, __instance.GUID, new Text("ARTILLERY CRIT"), FloatieMessage.MessageNature.CriticalHit));
                    ArtilleryCritTargets.Remove(__instance.GUID);
                }
            }
        }
    }
}
