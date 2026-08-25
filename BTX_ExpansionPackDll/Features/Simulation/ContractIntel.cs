using BattleTech;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using CustomUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BTX_ExpansionPack.Features.Simulation
{
    internal class ContractIntel
    {
        /// <summary>
        /// Shows additional contract information, such as target faction and variant description.
        /// </summary>
        [HarmonyPatch(typeof(LanceContractIntelWidget), "Init")]
        public static class LanceContractIntelWidget_Init
        {
            #region Mission Variants and Map Names

            private class IntelData
            {
                public string Name { get; set; }
                public string Description { get; set; }
            }

            private static readonly Dictionary<string, IntelData> VariantDescriptions = new()
            {
                // Salvage Race
                { "Rescue_SalvageRaceCS", new() { Name = "Normal", Description = "Normal ComStar Forces" } },
                { "Rescue_SalvageRaceCS_Hard", new() { Name = "Hard", Description = "Additional ComStar Forces" } },
                { "Rescue_SalvageRaceWoB", new() { Name = "Normal", Description = "Normal Blakist Forces" } },
                { "Rescue_SalvageRaceWoB_Hard", new() { Name = "Hard", Description = "Additional Blakist Forces" } },
            
                // Search Denial
                { "ThreeWayBattle_SearchDenialCS", new() { Name = "Normal", Description = "Mixed Level IIs" } },
                { "ThreeWayBattle_SearchDenialCS_Easy", new() { Name = "Easy", Description = "Vehicle-heavy Level IIs" } },
                { "ThreeWayBattle_SearchDenialCS_Elite", new() { Name = "Very Hard", Description = "Elite ComStar Forces" } },
                { "ThreeWayBattle_SearchDenialCS_Hard", new() { Name = "Hard", Description = "Mech-heavy Level IIs" } },
                { "ThreeWayBattle_SearchDenialWoB", new() { Name = "Normal", Description = "Mixed Level IIs" } },
                { "ThreeWayBattle_SearchDenialWoB_Easy", new() { Name = "Easy", Description = "Vehicle-heavy Level IIs" } },
                { "ThreeWayBattle_SearchDenialWoB_Elite", new() { Name = "Very Hard", Description = "Elite Blakist Forces" } },
                { "ThreeWayBattle_SearchDenialWoB_Hard", new() { Name = "Hard", Description = "Mech-heavy Level IIs" } },
            
                // Tag Team
                { "ThreeWayBattle_TagTeam_CS", new() { Name = "Default", Description = "Normal ComStar Forces" } },
                { "ThreeWayBattle_TagTeam_CS_Alt", new() { Name = "Alternate", Description = "Additional ComStar Forces" } },
                { "ThreeWayBattle_TagTeam_CS_Betray", new() { Name = "Betray", Description = "Two-Front Infighting" } }
            };

            private static readonly Dictionary<string, string> MapIdToFriendlyName = new()
            {
                { "mapGeneral_alpineCathedral_iTnd", "Alpine Cathedral" },
                { "mapGeneral_alpinePass_iGlc", "Alpine Pass" },
                { "mapArena_AlpineRiver_iGlc", "ARENA - Alpine River" },
                { "mapArena_bigLoch_vHigh", "ARENA - Big Loch" },
                { "mapArena_brokenGrotto_vLow", "ARENA - Broken Grotto" },
                { "mapArena_canyon_aDes", "ARENA - Canyon" },
                { "mapArena_centralMountain_aDes", "ARENA - Central Mountain" },
                { "mapMultPurp_deadPark_uDeso", "ARENA - Dead Park" },
                { "mapArena_deathValley_aDes", "ARENA - Death Valley" },
                { "mapMultiPurp_metroGreen_uTech", "ARENA - Metro Green" },
                { "mapArena_plateauValleys_vLow", "ARENA - Plateau Valley" },
                { "mapArena_riverCrossing_vHigh", "ARENA - River Crossing" },
                { "mapArena_RockyCraters_bMoon", "ARENA - Rocky Crater" },
                { "mapArena_splitRiver_iTnd", "ARENA - Split River" },
                { "mapArena_stillValley_iGlc", "ARENA - Still Valley" },
                { "mapArena_redCity_bMars", "ARENA - The Red City" },
                { "mapArena_theStacks_vHigh", "ARENA - The Stacks" },
                { "mapArena_tideBay_vJung", "ARENA - Tide Bay" },
                { "mapGeneral_barterTown_aDes", "Barter Town" },
                { "mapGeneral_bigCrater_bMoon", "Big Crater" },
                { "mapGeneral_bleakRidge_aBad", "Bleak Ridge" },
                { "mapGeneral_bluffs_vHigh", "Bluffs" },
                { "mapGeneral_boggyRocks_vLow", "Boggy Rocks" },
                { "mapGeneral_borealForest_iGlc", "Boreal Forest" },
                { "mapGeneral_boulderField_vLow", "Boulder Field" },
                { "mapGeneral_capitolHill_uTech", "Capitol Hill" },
                { "mapGeneral_CentralMound_iTnd", "Central Mound" },
                { "mapGeneral_centralPond_uTech", "Central Pond" },
                { "mapGeneral_cityCenter_uTech", "City Center" },
                { "mapGeneral_hollowCore_uDeso", "City Center" },
                { "mapGeneral_cragMire_vLow", "Crag Mire" },
                { "mapGeneral_craterField_bMars", "Crater Field" },
                { "mapGeneral_crimsonValley_bMars", "Crimson Valley" },
                { "mapGeneral_desertDam_aDes", "Desert Dam" },
                { "mapGeneral_escarpmentValley_vLow", "Escarpment Valley" },
                { "mapGeneral_fallenHills_uDeso", "Fallen Hills" },
                { "mapGeneral_frigidSteppes_iTnd", "Frigid Steppes" },
                { "mapGeneral_frostySlopes_iTnd", "Frosty Slopes" },
                { "mapGeneral_grandeRiver_aWst", "Grande River" },
                { "mapGeneral_gridLock_uTech", "Grid Lock" },
                { "mapGeneral_hiddenLagoon_vHigh", "Hidden Lagoon" },
                { "mapGeneral_highPeak_iGlc", "High Peak" },
                { "mapGeneral_highPlateaus_aDes", "High Plateaus" },
                { "mapGeneral_icyOutpost_iGlc", "Icy Outpost" },
                { "mapGeneral_Interchange_aDes", "Interchange" },
                { "mapRestoration_Itrom_bMars", "Itrom" },
                { "mapGeneral_jumbledKarst_aDes", "Jumbled Karst" },
                { "mapGeneral_lostCanyon_aBad", "Lost Canyon" },
                { "mapGeneral_lostWorld_vJung", "Lost World" },
                { "mapGeneral_lushIsthmus_vJung", "Lush Isthmus" },
                { "mapGeneral_MonsFoothills_bMars", "Mons Foothills" },
                { "mapGeneral_mountainHold_bMoon", "Mountain Hold" },
                { "mapRestoration_Panzyr_iGlc", "Panzyr" },
                { "mapGeneral_paupersCanyon_aWst", "Paupers Canyon" },
                { "mapGeneral_pocketLakes_vJung", "Pocket Lakes" },
                { "mapGeneral_powerStructure_uTech", "Power Structure" },
                { "mapGeneral_rawCliffsides_vHigh", "Raw Cliffsides" },
                { "mapGeneral_riftValley_vJung", "Rift Valley" },
                { "mapGeneral_riverBend_vLow", "River Bend" },
                { "mapGeneral_riverDelta_vLow", "River Delta" },
                { "mapGeneral_rockyCliffs_vHigh", "Rocky Cliffs" },
                { "mapGeneral_rockyMesas_aDes", "Rocky Mesas" },
                { "mapGeneral_rollingHillsLakeA_vLow", "Rolling Hills Lake" },
                { "mapGeneral_ruggedAtoll_vJung", "Rugged Atoll" },
                { "mapGeneral_sandyMesa_aDes", "Sandy Mesa" },
                { "mapRestoration_Smithon_aDes", "Smithon" },
                { "mapGeneral_splitRange_iGlc", "Split Range" },
                { "mapStory_StoryEncounter1a_vHigh", "Story Encounter 1a: Tutorial" },
                { "mapStory_StoryEncounter1b_vHigh", "Story Encounter 1b: Coronation Day" },
                { "mapStory_StoryEncounter2_aDes", "Story Encounter 2: Three Years Later" },
                { "mapStory_StoryEncounter3_mMoon", "Story Encounter 3: Capture the Argo" },
                { "mapStory_StoryEncounter4_iGlc", "Story Encounter 4: Liberation of Weldry" },
                { "mapStory_StoryEncounter5_iGlc", "Story Encounter 5: Served Cold" },
                { "mapStory_StoryEncounter6_iGlc", "Story Encounter 6a: Raising the Dead" },
                { "mapStory_StoryEncounter6b_iGlc", "Story Encounter 6b: Escape" },
                { "mapStory_StoryEncounter7_vLow", "Story Encounter 7: Extraction" },
                { "mapStory_StoryEncounter8_vHigh", "Story Encounter 8: Locura" },
                { "mapStory_StoryEncounter9_vHigh", " Story Encounter 9: Showdown" },
                { "mapGeneral_sunkenHills_iTnd", "Sunken Hills" },
                { "mapGeneral_taigaRiver_iTnd", "Taiga River" },
                { "mapGeneral_terraceLakes_vLow", "Terrace Lakes" },
                { "mapGeneral_terracePlaza_uTech", "Terrace Plaza" },
                { "mapGeneral_highway_vLow", "The Lowway" },
                { "mapGeneral_theMine_aDes", "The Mine" },
                { "mapGeneral_theMound_vHigh", "The Mound" },
                { "mapGeneral_toxicMire_aBad", "Toxic Mire" },
                { "mapGeneral_tropicalCove_vJung", "Tropical Cove" },
                { "mapRestoration_Tyrlon_vHigh", "Tyrlon" }
            };

            #endregion

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
                    bool isThreeWayBattle = contract.Override.contractTypeID == "ThreeWayBattle";

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

                    if (isThreeWayBattle)
                    {
                        string secondaryFactionId = GetThreeWayBattleSecondaryFaction(contract);

                        if (!string.IsNullOrEmpty(secondaryFactionId))
                        {
                            var factionDef = contract.Override.hostileToAllTeam.faction == secondaryFactionId
                                ? contract.Override.hostileToAllTeam.FactionDef
                                : contract.Override.targetsAllyTeam.FactionDef;

                            string secondaryFactionName = factionDef?.Name ?? secondaryFactionId;
                            if (secondaryFactionName.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
                            {
                                secondaryFactionName = secondaryFactionName.Substring(4);
                            }

                            hostileText = SetupTextComponent(
                                hostileText,
                                parentObject,
                                ContractDescriptionField,
                                "txt_hostile",
                                $"Secondary Target: <color=#F79B26>{secondaryFactionName}</color>",
                                parentObject.transform.GetSiblingIndex() + siblingOffset
                            );
                            SetupFactionTooltip(hostileText, secondaryFactionId);
                            siblingOffset++;
                        }
                        else if (hostileText != null)
                        {
                            UnityEngine.Object.Destroy(hostileText.gameObject);
                        }
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(hostileText.gameObject);
                    }

                    var minimap = parentObject.FindComponent<Image>("img_minimap_back");
                    if (minimap != null)
                    {
                        var tooltip = minimap.gameObject.GetComponent<HBSTooltip>() ?? minimap.gameObject.AddComponent<HBSTooltip>();
                        string mapName = MapIdToFriendlyName.FirstOrDefault(kvp => kvp.Key.Equals(contract.mapName, StringComparison.OrdinalIgnoreCase)).Value ?? contract.mapName;

                        tooltip.SetDefaultStateData(TooltipUtilities.GetStateDataFromObject(mapName));
                    }
                }

                if (Main.Settings.UI.ContractIntel.IntelShowVariant)
                {
                    if (!string.IsNullOrEmpty(contract.Override.ID) && VariantDescriptions.TryGetValue(contract.Override.ID, out var variantData))
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

            private static string GetThreeWayBattleSecondaryFaction(Contract contract)
            {
                if (contract.Override.chunkList == null)
                    return null;

                bool hasHostileToAll = contract.Override.chunkList.Exists(chunk => chunk.name.Contains("Chunk_HostileAll") == true && chunk.enableChunkFromContract);
                if (hasHostileToAll)
                {
                    return contract.Override.hostileToAllTeam.faction;
                }

                bool hasEnemyAlly = contract.Override.chunkList.Exists(chunk => chunk.name.Contains("Chunk_EnemyAlly") == true && chunk.enableChunkFromContract);
                return hasEnemyAlly ? contract.Override.targetsAllyTeam.faction : null;
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
}