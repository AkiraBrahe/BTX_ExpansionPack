using BattleTech;
using BattleTech.Framework;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using BTX_CAC_CompatibilityDll;
using ColourfulFlashPoints;
using CustomUnits;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BTX_ExpansionPack.Features.Simulation
{
    internal class ContractIntel
    {
        public class IntelData
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        /// <summary>
        /// Shows additional contract information, such as target faction and variant description.
        /// </summary>
        [HarmonyPatch(typeof(LanceContractIntelWidget), "Init")]
        public static class LanceContractIntelWidget_Init
        {
            private static readonly Dictionary<string, IntelData> VariantDescriptions = new()
            {
                { "ThreeWayBattle_SearchDenialCS", new() { Name = "Normal", Description = "Mixed Level IIs" } },
                { "ThreeWayBattle_SearchDenialCS_Easy", new() { Name = "Easy", Description = "Vehicle-heavy Level IIs" } },
                { "ThreeWayBattle_SearchDenialCS_Hard", new() { Name = "Hard", Description = "Mech-heavy Level IIs" } },
                { "ThreeWayBattle_SearchDenialCS_Elite", new() { Name = "Very Hard", Description = "Elite ComStar Forces" } },
                { "ThreeWayBattle_SearchDenialWoB", new() { Name = "Normal", Description = "Mixed Level IIs" } },
                { "ThreeWayBattle_SearchDenialWoB_Easy", new() { Name = "Easy", Description = "Vehicle-heavy Level IIs" } },
                { "ThreeWayBattle_SearchDenialWoB_Hard", new() { Name = "Hard", Description = "Mech-heavy Level IIs" } },
                { "ThreeWayBattle_SearchDenialWoB_Elite", new() { Name = "Very Hard", Description = "Elite Blakist Forces" } },
                { "ThreeWayBattle_TagTeam_CS", new() { Name = "Default", Description = "Normal ComStar Forces" } },
                { "ThreeWayBattle_TagTeam_CS_Alt", new() { Name = "Alternate", Description = "Additional Dropped Forces" } },
                { "ThreeWayBattle_TagTeam_CS_Betray", new() { Name = "Betray", Description = "Additional ComStar Forces" } }
            };

            [HarmonyPostfix]
            public static void Postfix(LocalizableText ContractDescriptionField, Contract contract)
            {
                if (contract?.Override == null) return;

                var parentObject = ContractDescriptionField.transform.parent.gameObject;
                var targetText = parentObject.FindComponent<LocalizableText>("txt_target");
                var hostileText = parentObject.FindComponent<LocalizableText>("txt_hostile");
                var variantText = parentObject.FindComponent<LocalizableText>("txt_variant");

                int siblingOffset = 1;

                if (Main.Settings.UI.ContractIntel.IntelShowTarget)
                {
                    bool isThreeWayBattle = contract.Override.contractType == ContractType.ThreeWayBattle;

                    string targetId = contract.Override.targetTeam.faction;
                    if (!string.IsNullOrEmpty(targetId))
                    {
                        string targetFactionName = contract.Override.targetTeam.FactionDef?.Name ?? targetId;
                        if (targetFactionName.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
                        {
                            targetFactionName = targetFactionName.Substring(4);
                        }

                        string targetLabel = isThreeWayBattle ? "Primary Target" : "Target";
                        targetText = SetupTextComponent(
                            targetText,
                            parentObject,
                            ContractDescriptionField,
                            "txt_target",
                            $"{targetLabel}: <color=#F79B26>{targetFactionName}</color>",
                            parentObject.transform.GetSiblingIndex() + siblingOffset
                        );
                        SetupFactionTooltip(targetText, contract.Override.targetTeam.faction);
                        siblingOffset++;
                    }

                    string hostileId = contract.Override.hostileToAllTeam.faction;
                    if (!string.IsNullOrEmpty(hostileId))
                    {
                        string hostileFactionName = contract.Override.hostileToAllTeam.FactionDef?.Name ?? hostileId;
                        if (hostileFactionName.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
                        {
                            hostileFactionName = hostileFactionName.Substring(4);
                        }

                        hostileText = SetupTextComponent(
                            hostileText,
                            parentObject,
                            ContractDescriptionField,
                            "txt_hostile",
                            $"Secondary Target: <color=#F79B26>{hostileFactionName}</color>",
                            parentObject.transform.GetSiblingIndex() + siblingOffset
                        );
                        SetupFactionTooltip(hostileText, contract.Override.hostileToAllTeam.faction);
                        siblingOffset++;
                    }
                    else if (hostileText != null)
                    {
                        UnityEngine.Object.Destroy(hostileText.gameObject);
                    }
                }

                if (Main.Settings.UI.ContractIntel.IntelShowVariant)
                {
                    if (!string.IsNullOrEmpty(contract.Override.ID) && VariantDescriptions.TryGetValue(contract.Override.ID, out IntelData variantData))
                    {
                        variantText = SetupTextComponent(
                            variantText,
                            parentObject,
                            ContractDescriptionField,
                            "txt_variant",
                            $"Variant: <color=#F79B26>{variantData.Name}</color>",
                            parentObject.transform.GetSiblingIndex() + siblingOffset
                        );
                        SetupTextTooltip(variantText, variantData.Description);
                        // siblingOffset++;
                    }
                    else if (variantText != null)
                    {
                        UnityEngine.Object.Destroy(variantText.gameObject);
                    }
                }
            }

            private static LocalizableText SetupTextComponent(LocalizableText existing, GameObject parent, LocalizableText template, string name, string text, int siblingIndex)
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

            private static void SetupFactionTooltip(LocalizableText text, string factionId)
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

            private static void SetupTextTooltip(LocalizableText text, string tooltipText)
            {
                if (text != null)
                {
                    var tooltip = text.gameObject.GetComponent<HBSTooltip>() ?? text.gameObject.AddComponent<HBSTooltip>();
                    tooltip.SetDefaultStateData(null);
                    if (!string.IsNullOrEmpty(tooltipText))
                    {
                        tooltip.SetDefaultStateData(TooltipUtilities.GetStateDataFromObject(tooltipText));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Changes the color of the contract card when Wolfs Dragoons are the employer.
    /// </summary>
    [HarmonyPatch(typeof(SGContractsWidget), "ListContracts")]
    public static class SGContractsWidget_ListContracts
    {
        [HarmonyPostfix]
        public static void Postfix(SGContractsWidget __instance)
        {
            foreach (SGContractsListItem contractListItem in __instance.listedContracts)
            {
                GameObject bgFill = contractListItem.gameObject.transform.Find("ENABLED-bg-fill").gameObject;
                if (bgFill != null)
                {
                    var contractOverride = contractListItem.Contract.Override;
                    if (!contractOverride.IsAnyStoryContract() && contractOverride.employerTeam.faction == "WolfsDragoons")
                    {
                        Image component = bgFill.GetComponent<Image>();
                        var contractCardFixup = bgFill.GetComponent<ContractCardFixup>() ?? bgFill.AddComponent<ContractCardFixup>();
                        if (ColorUtility.TryParseHtmlString("#C00008", out Color color))
                        {
                            color.a = 0.5f;
                            component.color = color;
                            contractCardFixup.setColour(color);
                            contractCardFixup.setUp(component);
                        }
                    }
                }
            }
        }
    }
}