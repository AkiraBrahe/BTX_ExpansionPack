using BattleTech;
using CustomUnits;

namespace BTX_ExpansionPack.Fixes
{
    internal class VehicleMelee
    {
        [HarmonyPatch(typeof(AbstractActor), "SetBehaviorTree")]
        public static class AbstractActor_SetBehaviorTree
        {
            [HarmonyPostfix]
            public static void Postfix(AbstractActor __instance)
            {
                if (__instance is Vehicle or FakeVehicleMech)
                {
                    __instance.BehaviorTree.unitBehaviorVariables.SetVariable(BehaviorVariableName.Float_MeleeRevengeBonus, new BehaviorVariableValue(0.1f));
                    __instance.BehaviorTree.unitBehaviorVariables.SetVariable(BehaviorVariableName.Float_MeleeDamageMultiplier, new BehaviorVariableValue(0.1f));
                    __instance.BehaviorTree.unitBehaviorVariables.SetVariable(BehaviorVariableName.Float_MeleeVsUnsteadyTargetDamageMultiplier, new BehaviorVariableValue(0.1f));
                    __instance.BehaviorTree.unitBehaviorVariables.SetVariable(BehaviorVariableName.Float_MeleeBonusMultiplierWhenAttackingEvasiveTargets, new BehaviorVariableValue(0.1f));
                }
            }
        }
    }
}
