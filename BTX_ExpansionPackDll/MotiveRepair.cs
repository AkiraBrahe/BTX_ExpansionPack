using System.Linq;
using BattleTech;
using CustAmmoCategories;
using HarmonyLib;

namespace BTX_ExpansionPack
{
    internal class MotiveRepair
    {
        [HarmonyPatch(typeof(Mech), "OnActivationEnd")]
        public static class Mech_OnActivationEnd
        {
            [HarmonyPrefix]
            public static void Prefix(Mech __instance)
            {
                Main.Log.LogDebug($"[MotiveRepair] Prefix triggered for: {__instance.Description.UIName}");

                if (__instance.FakeVehicle() && __instance.Combat.EffectManager.GetAllEffectsWithID("motiveSystemGain")
                        .Any(effect => effect.Target == __instance))
                {
                    Main.Log.LogDebug($"[MotiveRepair] Custom Unit Vehicle '{__instance.Description.UIName}' has 'motiveSystemGain' effect.");
                    int removedCruiseDebuffs = 0;
                    int removedFlankDebuffs = 0;

                    var motiveLossEffects = __instance.Combat.EffectManager.GetAllEffectsWithID("motiveSystemLoss")
                        .Where(effect => effect.Target == __instance)
                        .Concat(__instance.Combat.EffectManager.GetAllEffectsWithID("motiveSystemLossSprint")
                            .Where(effect => effect.Target == __instance))
                        .ToList();

                    foreach (Effect debuffEffect in motiveLossEffects)
                    {
                        if (removedCruiseDebuffs < 2 && debuffEffect?.EffectData?.statisticData?.statName == "CruiseSpeed")
                        {
                            Main.Log.LogDebug($"[MotiveRepair] Removing CruiseSpeed debuff from '{__instance.Description.UIName}'.");
                            __instance.Combat.EffectManager.CancelEffect(debuffEffect, true);
                            removedCruiseDebuffs++;
                        }
                        else if (removedFlankDebuffs < 2 && debuffEffect?.EffectData?.statisticData?.statName == "FlankSpeed")
                        {
                            Main.Log.LogDebug($"[MotiveRepair] Removing FlankSpeed debuff from '{__instance.Description.UIName}'.");
                            __instance.Combat.EffectManager.CancelEffect(debuffEffect, true);
                            removedFlankDebuffs++;
                        }
                        if (removedCruiseDebuffs >= 2 && removedFlankDebuffs >= 2)
                        {
                            Main.Log.LogDebug($"[MotiveRepair] Removed max debuffs for '{__instance.Description.UIName}'.");
                            break;
                        }
                    }
                }
                else if (__instance.FakeVehicle())
                {
                    Main.Log.LogDebug($"[MotiveRepair] Fake Vehicle '{__instance.Description.UIName}' does not have 'motiveSystemGain' effect.");
                }
                else
                {
                    Main.Log.LogDebug($"[MotiveRepair] '{__instance.Description.UIName}' is not a fake vehicle.");
                }
            }
        }
    }
}