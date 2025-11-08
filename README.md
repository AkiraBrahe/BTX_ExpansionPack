# BTX Expansion Pack

A BattleTech Extended mod that aims to add tons of new content, including new 'Mechs, vehicles, and weapons, as well as extend the timeline into the 3060s.

> [!IMPORTANT]
> Version 1.0 focuses on adding playable vehicles and related features to BattleTech Extended Tactics. Moving forward, you can expect more frequent updates bringing back the 100+ new 'Mechs and extending the timeline to the FedCom Civil War.

## Installation

1. Install the latest versions of [BattleTech Extended Tactics](https://discourse.modsinexile.com/t/battletech-extended-tactics/1859) and [CAC-C](https://github.com/mcb5637/BTX_CAC_Compatibility/releases/latest).
2. Remove the ".modtek" folder if present to force Modtek to rebuild the cache.
3. When updating the mod, remove the "BTX_ExpansionPack" and "BTX_PlayableVehicles" mod folders.
4. Unpack the mod folders from the "BTX_ExpansionPack" archive into the mods folder, overwriting when prompted.

### Optionals:
- **To disable playable vehicles**:
  - In the `BTSimpleMechAssembly` mod.json file, change `"SalvageAndAssembleVehicles"` to `"false"`.
  - Remove these mods from your mods folder: BTX_CustomPilotDecorator, BTX_PlayableVehicles, and Lifepaths.
- **To disable infantry complements**:
  - In the `BTX_PlayableVehicles` mod.json file, remove or comment out the last three lines of the manifest.

## Credits

This mod is bundled with a modified version of Playable Tanks by [LordRuthermore](https://github.com/lordruthermore).

Special thanks to [mcb](https://github.com/mcb5637) for enabling the display of the full vehicle names in battle, and **Hrothgar Heavenlight** for playtesting the mod extensively, suggesting exciting 'Mechs to add, and writing Yang's comments on them. 

## Features

### Playable Vehicles & Mechanics
The Expansion Pack includes a complete vehicle overhaul, allowing you to command over 580 different vehicle variants across 180 chassis, including VTOLs and superheavy tanks. This means over 300 new vehicle variants, effectively doubling the number found in BattleTech Extended! All vehicles have had their loadouts, armor values, and descriptions updated for a fresh combat experience.

* **Enhanced Vehicle Durability:** Light vehicles receive +2 Hit Defense, and medium vehicles +1. VTOLs also gain +2 Evasion, while hovercrafts get +1.
* **Turret Flexibility**: Turreted vehicles have a 360° firing arc, enabling them to engage targets from any direction without needing to reposition.
* **Deployment Restrictions:** Vehicles unsuitable for specific biomes (e.g., low/no atmosphere) are prevented from dropping there.
* **Expanded Faction Rosters:** Each faction has its own vehicle roster, featuring numerous new variants.
* **Integrated Vehicle Economy:** Vehicles are fully integrated into the in-game economy. Find them for sale in stores and factories alongside 'Mechs, with 20 new vehicle-exclusive factories added.
* **Partial Vehicle Refit:** Customize your vehicles by swapping weapons and ammunition to fine-tune their combat role.
> [!NOTE]  
> Vehicle refit is limited. You can replace existing weapons and ammo, but cannot add or remove any equipment.
* **Pilot Specialization:** Pilots specialize in either 'Mech or vehicle piloting. The Lifepath system has been adjusted to make 'Mech-capable pilots rarer in certain systems and the early game, with conventional pilots having the possibility to gain 'Mech piloting ability later.
* **Motive Repair Ability:** Pilots have the ability to repair motive system debuffs.
* **Vehicle CASE:** Implements CASE functionality for vehicles, converting critical ammunition explosions to rear damage instead of vehicle destruction.
* **Vehicle Pilot Injuries:** Pilots suffer injuries from component and ammunition explosions (including vehicle destruction). CASE-equipped vehicles prevent explosion-related injuries.

### New Content
The Expansion Pack pushes the timeline into the 3060s and will eventually include most 'Mechs and equipment from after the Clan Invasion up to 3067. It also adds new weapons that were made possible by CAC-C, like new artillery systems.

#### 'Mechs
> [!IMPORTANT]
> All v0.9 'Mechs are being added back as a priority now that the new features are complete.
* **Clan 'Mechs:** All 31 Clan 'Mechs have been re-added to the rosters in the latest beta. Note that 10 of these currently use a proxy model (**NEW**).
* **Unique 'Mechs:**
  * The Yen-Lo-Wang is now an unique reward for completing the "Bourbon & BattleMechs" flashpoint. It can be upgraded after September 3050 to its CN9-YLW2 configuration for 10 million C-bills (**NEW**).
  * The Big Steel Claw from the "Steel Beast" flashpoint got a similar upgrade after 3051 that includes an XL engine and pulse lasers (**NEW**).

#### Weapons & Ammunition
* **Artillery Cannons:** Compact, faster-firing artillery options, with prototypes available on research planets.
* **Mech Mortars:** A more common and direct-fire alternative to artillery. While less accurate than LRMs, they deal significantly more damage.
> [!NOTE]
> - New artillery fires directly with headshot capability. Standard artillery and near misses deal only AoE damage.
> - AoE damage is split across all hit locations. However, strikes have a chance to inflict a critical hit, focusing most damage on a single location. This chance increases as the impact center lands closer to the target (up to 50%).
> - The Mech Bay UI displays the combined total damage for artillery (Direct + AoE).
* **Rapid-Fire Autocannons:** New AC variants with reduced damage but improved accuracy against evasive targets. Also provide wide-area missile defense (like an AMS). Exclusive to anti-air 'Mechs (e.g., Rifleman) and always used by AI-controlled variants.
* **Infantry:** Infantry complements serve as unkillable, integrated weapons for APCs, always utilizing their maximum capacity.
* **Specialized Ammunition:**
  * **Homing Arrow IV:** Allow for direct artillery strikes on TAG'd enemies.
  * **Swarm LRMs:** Ignore cover and can hit adjacent units on a miss.
  * **Thunder-Inferno LRMs:** Deal heat damage and start fires over a large area.
> [!NOTE]  
> The AI will also use these new ammunition types depending on the faction and in-game date.

#### Upgrades
* **Artillery Targeting System (TTS):** New upgrade that enhances artillery accuracy and automatically adjusts two-turn strikes towards the nearest enemy right before firing.
* **Advanced Comms Equipment:** Provides bonuses ranging from +1 Lance Initiative and +1 Resolve with one ton of equipment to +2 Lance Initiative and +7 Resolve Gain with seven tons. Primarily exclusive to command vehicles.

### Gameplay

#### General
* **New Starting Options:** Choose between `Ultralights Only` (featuring 7 unique StarDrive magazine 'Mechs), `Solo` (a nod to Team Venom in _MechWarrior 2: Mercenaries_), and `Vehicles Only` career starts.
* **Self-Knockdown:** Firing Gauss weapons and artillery generates instability (150% of weapon weight), halved if the 'Mech braced the previous turn (e.g., two-turn artillery strikes).
* **Gear Rebalance:**
  * Targeting Computers (TAC) improves the accuracy of all direct-fire weapons, not just ballistic and energy types.
  * 'Mechs with two C3 Master Computers establish a C3 network with all allied units, not just lancemates.
* **AI Improvements:** The AI knows how to aim artillery more effectively and how to use the AMS mode on rapid-fire autocannons as well as the new movement-related abilities (Careful Maneuvers and Motive Repair) (**NEW**).
  
#### Quality of Life & UI
* **Missions:**
  * Mission briefings display primary target information.
  * ComStar missions are color-coded with variants displayed below the descriptions.
  * Search Denial missions may feature mixed ComStar forces with rare royal tanks.
  * Vehicle names are fully displayed in combat for easier identification (e.g., Manticore Heavy Tank).
* **MechBay:**
  * Standard parts can be used when assembling 'Mechs or vehicles with ferro-fibrous armor.
  * Skirmish MechBay separates 'Mechs and vehicles for faster loading, sorting them by name.
* **Argo Upgrades:** All upgrades have better effect descriptions.
* **Visuals:** Heavy lasers have distinct beam colors (from orange-red to golden). The same goes for Snub-Nose PPCs (blue-green).
* **UI Tooltips:** Improved 'Mech tooltips with cleaner text, better formatting, and subtle color changes.
