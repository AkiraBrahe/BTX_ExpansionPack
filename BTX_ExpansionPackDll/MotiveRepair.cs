using System.Linq;
using BattleTech;
using CustAmmoCategories;
using HarmonyLib;

namespace BTX_ExpansionPack
{
    internal class MotiveRepair
    {
        [HarmonyPatch(typeof(AbstractActor), "OnActivationEnd")]
        public static class AbstractActor_OnActivationEnd
        {
            [HarmonyPrefix]
            public static void Prefix(AbstractActor __instance)
            {
                if (__instance.FakeVehicle() && __instance.Combat.EffectManager.GetAllEffectsTargetingWithBaseID(__instance, "motiveSystemGain").Any())
                {
                    int removedCruiseDebuffs = 0;
                    int removedFlankDebuffs = 0;

                    var motiveLossEffects = __instance.Combat.EffectManager.GetAllEffectsTargeting(__instance)
                        .Where(effect => effect.EffectData?.Description?.Id == "motiveSystemLoss" || effect.EffectData?.Description?.Id == "motiveSystemLossSprint")
                        .ToList();

                    foreach (Effect debuffEffect in motiveLossEffects)
                    {
                        if (removedCruiseDebuffs < 3 && debuffEffect?.EffectData?.statisticData?.statName == "CruiseSpeed")
                        {
                            __instance.Combat.EffectManager.CancelEffect(debuffEffect, true);
                            removedCruiseDebuffs++;
                        }
                        else if (removedFlankDebuffs < 3 && debuffEffect?.EffectData?.statisticData?.statName == "FlankSpeed")
                        {
                            __instance.Combat.EffectManager.CancelEffect(debuffEffect, true);
                            removedFlankDebuffs++;
                        }
                        if (removedCruiseDebuffs >= 3 && removedFlankDebuffs >= 3)
                        {
                            Main.Log.LogDebug($"[MotiveRepair] Removed motive system debuffs from '{__instance.DisplayName}'.");
                            break;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Contract), "CompleteContract")]
        public static class Contract_CompleteContract_TempFix
        {
            [HarmonyPrefix]
            public static void Prefix(Contract __instance)
            {
                if (__instance.State == Contract.ContractState.InProgress)
                {
                    var allActors = __instance.BattleTechGame.Combat.AllActors.ToList();
                    foreach (var actor in allActors)
                    {
                        if (actor is Mech mech && mech.MechDef.MechTags.Contains("fake_vehicle"))
                        {
                            Main.Log.LogDebug($"[TempFix] Removed BEX_Motive_System components from {mech.MechDef.Description.UIName} at contract start.");
                            mech.allComponents.RemoveAll((MechComponent mechComponent) => mechComponent.Description.Id == "Gear_BEX_MotiveSystem");
                            mech.miscComponents.RemoveAll((MechComponent mechComponent) => mechComponent.Description.Id == "Gear_BEX_MotiveSystem");
                        }
                    }
                }
            }
        }
    }
}