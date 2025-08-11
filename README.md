# BTX Expansion Pack

A BattleTech Extended mod that aims to add tons of new content, including new 'Mechs, vehicles, and weapons, as well as extend the timeline into the 3060s.

> [!IMPORTANT]
> The latest 1.0 release focuses on adding playable vehicles and related features to BattleTech Extended Tactics. Moving forward, you can expect more frequent updates bringing back the 100+ new 'Mechs and extending the timeline to the FedCom Civil War.

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

* **Enhanced Vehicle Durability:** Light vehicles receive +2 Hit Defense, medium vehicles +1. VTOLs gain +2 Evasion and +3 Hit Defense, while hover tanks get +1 Evasion and +1 Hit Defense.
* **Expanded Faction Rosters:** Each faction has its own vehicle roster, featuring numerous new variants.
* **Integrated Vehicle Economy:** Vehicles are fully integrated into the in-game economy, sold alongside 'Mechs in stores and factories, as well as 20 vehicle-exclusive factories.
* **Optional Vehicle Refit:** Customize vehicles by swapping weapons via the in-game setting.
* **Pilot Specialization:** Pilots now specialize in either 'Mech or vehicle piloting. The Lifepath system has been adjusted to make 'Mech-capable pilots rarer in certain systems and early game, with the possibility for conventional pilots to gain 'Mech piloting ability later.
* **Motive Repair Ability:** Vehicles can utilize a new ability to repair motive system debuffs (e.g., speed penalties from critical hits).
* **Vehicle CASE:** Implements CASE functionality for vehicles, converting critical ammunition explosions to rear damage instead of vehicle destruction.
* **Vehicle Pilot Injuries:** Pilots now suffer injuries from component and ammunition explosions (including vehicle destruction). CASE-equipped vehicles prevent explosion-related injuries.

### New Content

#### Weapons & Ammunition
* **Artillery Cannons:** Compact, faster-firing artillery options, with prototypes available on research planets.
* **Mech Mortars:** A less accurate but more damaging alternative to LRMs, widely available and replacing Thumpers in shops.
* **Rapid-Fire Autocannons:** New AC variants with reduced damage but improved accuracy against evasive targets. Also provide wide-area missile defense (like an AMS). Exclusive to anti-air 'Mechs (e.g., Rifleman) and always used by AI-controlled variants.
* **Infantry:** Infantry complements now serve as unkillable, integrated weapons for APCs, always utilizing their maximum capacity.
* **Specialized Ammunition:**
  * **Homing Arrow IV::** Allow for direct artillery strikes on TAG'd enemies.
  * **Swarm LRMs:** Ignore cover and can hit adjacent units on a miss.
  * **Thunder-Inferno LRMs:** Deal heat damage and start fires over a large area.

> [!NOTE]  
> The AI will also use these new ammunition types depending on the faction and in-game date.

#### Upgrades
* **Artillery Targeting System (TTS):** New upgrade that enhances artillery accuracy and automatically adjusts two-turn strikes towards the nearest enemy right before firing.
* **Advanced Comms Equipment:** Provides bonuses ranging from +1 Lance Initiative and +1 Resolve with one ton of equipment to +2 Lance Initiative and +7 Resolve Gain with seven tons. Primarily exclusive to command vehicles.

### Gameplay

...in construction!
