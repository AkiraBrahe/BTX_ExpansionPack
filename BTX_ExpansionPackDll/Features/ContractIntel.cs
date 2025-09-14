using BattleTech;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using CustomUnits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BTX_ExpansionPack.Features
{
    internal class ContractIntel
    {
        /// <summary>
        /// Shows additional contract information, such as target faction and variant description.
        /// </summary>
        [HarmonyPatch(typeof(LanceContractIntelWidget), "Init")]
        public static class LanceContractIntelWidget_Init
        {
            private static readonly Dictionary<string, string> Variant = new()
            {
                { "ThreeWayBattle_SearchDenialCS", "Default" },
                { "ThreeWayBattle_SearchDenialCS_Easy", "Easy (Mixed Level II)" },
                { "ThreeWayBattle_SearchDenialCS_Hard", "Hard (Additional ComStar Forces)" },
                { "ThreeWayBattle_SearchDenialWoB", "Default" },
                { "ThreeWayBattle_SearchDenialWoB_Easy", "Easy (Mixed Level II)" },
                { "ThreeWayBattle_SearchDenialWoB_Hard", "Hard (Additional Blakist Forces)" },
                { "ThreeWayBattle_TagTeam_CS", "Default" },
                { "ThreeWayBattle_TagTeam_CS_Alt", "Alternate (Additional Forces)" },
                { "ThreeWayBattle_TagTeam_CS_Betray", "Betray (Additional ComStar Forces)" }
            };

            [HarmonyPostfix]
            public static void Postfix(LocalizableText ContractDescriptionField, Contract contract)
            {
                if (contract?.Override == null) return;

                GameObject parentObject = ContractDescriptionField.transform.parent.gameObject;

                LocalizableText targetText = parentObject.FindComponent<LocalizableText>("txt_target");
                LocalizableText variantText = parentObject.FindComponent<LocalizableText>("txt_variant");

                if (Main.Settings.UI.ContractIntel.IntelShowTarget)
                {
                    string factionId = contract.Override.targetTeam.faction;
                    if (!string.IsNullOrEmpty(factionId))
                    {
                        string targetFactionName = contract.Override.targetTeam.FactionDef?.Name ?? factionId;
                        if (targetFactionName.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
                        {
                            targetFactionName = targetFactionName.Substring(4);
                        }

                        targetText = SetupTextComponent(
                            targetText,
                            parentObject,
                            ContractDescriptionField,
                            "txt_target",
                            $"Target: <color=#F79B26>{targetFactionName}</color>",
                            parentObject.transform.GetSiblingIndex() + 1
                        );

                        SetupTooltip(targetText, contract.Override.targetTeam.faction);
                    }
                }

                if (Main.Settings.UI.ContractIntel.IntelShowVariant)
                {
                    if (Variant.TryGetValue(contract.Override.ID, out string variantDescription))
                    {
                        variantText = SetupTextComponent(
                            variantText,
                            parentObject,
                            ContractDescriptionField,
                            "txt_variant",
                            $"Variant: <color=#F79B26>{variantDescription}</color>",
                            parentObject.transform.GetSiblingIndex() + ((targetText != null) ? 2 : 1)
                        );
                        SetupTooltip(variantText, null);
                    }
                    else if (variantText != null)
                    {
                        UnityEngine.Object.Destroy(variantText.gameObject);
                    }
                }
            }

            internal static LocalizableText SetupTextComponent(LocalizableText existing, GameObject parent, LocalizableText template, string name, string text, int siblingIndex)
            {
                if (existing == null)
                {
                    var newText = UnityEngine.Object.Instantiate(template.gameObject).GetComponent<LocalizableText>();
                    if (newText != null)
                    {
                        newText.gameObject.transform.SetParent(parent.transform);
                        newText.gameObject.transform.SetSiblingIndex(siblingIndex);
                        newText.gameObject.transform.localScale = Vector3.one;
                        newText.gameObject.name = name;
                        newText.SetText(text);
                    }

                    return newText;
                }
                else
                {
                    existing.SetText(text);
                    return existing;
                }
            }

            internal static void SetupTooltip(LocalizableText text, string factionId)
            {
                if (text != null)
                {
                    var tooltip = text.gameObject.GetComponent<HBSTooltip>() ?? text.gameObject.AddComponent<HBSTooltip>();
                    tooltip.SetDefaultStateData(null);
                    if (!string.IsNullOrEmpty(factionId))
                    {
                        var factionDef = UnityGameInstance.BattleTechGame.Simulation?.GetFactionDef(factionId);
                        tooltip.SetDefaultStateData(TooltipUtilities.GetStateDataFromObject(factionDef));
                    }
                }
            }
        }
    }
}