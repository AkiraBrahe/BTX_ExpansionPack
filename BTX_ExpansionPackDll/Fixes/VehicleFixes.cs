using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BTX_CAC_CompatibilityDll;
using CustomUnits;
using UnityEngine;

namespace BTX_ExpansionPack.Fixes
{
    internal class VehicleFixes
    {
        [HarmonyPatch(typeof(MechStatisticsRules), "CalculateTonnage")]
        public class MechStatisticsRules_CalculateTonnage
        {
            [HarmonyPrefix]
            public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
            {
                maxValue = mechDef.Chassis.Tonnage;
                currentValue = mechDef.IsVehicle() ? mechDef.Chassis.Tonnage : mechDef.CalculateWeightKG() / 1000f;
                return false;
            }
        }

        [HarmonyPatch(typeof(MechLabMechInfoWidget), "CalculateTonnage")]
        public class MechLabMechInfoWidget_CalculateTonnage
        {
            [HarmonyPrefix]
            public static bool Prefix(MechLabMechInfoWidget __instance, MechLabPanel ___mechLab, LocalizableText ___totalTonnage, UIColorRefTracker ___totalTonnageColor, LocalizableText ___remainingTonnage, UIColorRefTracker ___remainingTonnageColor)
            {
                if (___mechLab.activeMechDef == null)
                    return true;

                ChassisDef chassisDef = ___mechLab.activeMechDef.Chassis;
                int currentWeightKG = ___mechLab.CalculateWeightKG();
                int remainingWeightKG = (int)(chassisDef.Tonnage * 1000f) - currentWeightKG;

                __instance.currentTonnage = currentWeightKG / 1000f;

                if (___mechLab.activeMechDef.IsVehicle())
                {
                    ___totalTonnage.SetText("{0:0.##} ton{1}",
                    [
                        chassisDef.Tonnage,
                        (chassisDef.Tonnage == 1) ? "" : "s"
                    ]);
                    ___totalTonnageColor.SetUIColor(UIColor.White);
                    ___remainingTonnage.SetText("");
                    ___remainingTonnageColor.SetUIColor(UIColor.White);
                }
                else
                {
                    ___totalTonnage.SetText("{0:0.##} / {1}",
                    [
                        __instance.currentTonnage,
                        chassisDef.Tonnage
                    ]);
                    ___totalTonnageColor.SetUIColor((remainingWeightKG < 0) ? UIColor.Red : UIColor.WhiteHalf);

                    bool isOverweight = remainingWeightKG < 0;
                    if (isOverweight)
                    {
                        float overweightTons = -remainingWeightKG / 1000f;
                        ___remainingTonnage.SetText("{0:0.##} ton{1} overweight",
                        [
                            overweightTons,
                        (remainingWeightKG == -1000) ? "" : "s"
                        ]);
                    }
                    else
                    {
                        ___remainingTonnage.SetText("{0:0.##} ton{1} remaining",
                        [
                            remainingWeightKG / 1000f,
                        (remainingWeightKG == 1000) ? "" : "s"
                        ]);
                    }
                    ___remainingTonnageColor.SetUIColor((remainingWeightKG < 0) ? UIColor.Red : ((remainingWeightKG <= 500) ? UIColor.Gold : UIColor.White));
                }

                return false;
            }
        }
    }
}
