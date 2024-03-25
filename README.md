# BTX Expansion Pack

A BattleTech Extended mod that adds tons of new content, including new 'Mechs, vehicles, and weapons, as well as expanding the timeline into the 3060s. Requires CAC-C.

## Installation

**TL;DR: BEX + Bigger Drops + CAB → CAC-C → (TBD) → Expansion Pack**

⚠ If you've already installed Playable Tanks, you'll need to do a clean install.

- Install the latest versions of [CAC-C](https://github.com/mcb5637/BTX_CAC_Compatibility/releases/latest) and, optionally, [The Big Deal Add-On](https://github.com/Hounfor/The-Big-Deal-Add-On) for BEX + CAC-C.
- Update the CAB if you haven't done so in a while.
- Remove the ".modtek" folder to force Modtek to rebuild the cache.
- Remove the "BTX_ExpansionPack" or "BTX_MechPack" mod folders if present.
- Unpack the mod folders from the "BTX_ExpansionPack" archive into the "Mods" folder, overwriting when prompted. The archive already includes a modified version of Playable Tanks/VTOLs.

### Optionals
- **For all DHS in the engine to dissipate 6 heat instead of 4**:
  - Enable the `BTX_DHS_Changes` mod by removing the `.bak` extension from its mod.json file.
- **For the AI to use different types of ammo randomly during combat**:
  - Enable the `Shell Shuffler` mod by removing the date part from one of its two mod.json files.
- **To disable playable vehicles**:
  - In the `BTSimpleMechAssembly` mod.json file, change `"SalvageAndAssembleVehicles"` to `"false"`.
  - Remove these mods from your mods folder: Abilifier, AbilityRealizer, BTX_CustomPilotDecorator, BTX_PlayableVehicles, and Lifepaths.

## Credits

This mod is bundled with Playable Tanks and DHS Changes by [LordRuthermore](https://github.com/lordruthermore), and Shell Shuffler by [T-Bone](https://github.com/ajkroeg).

Special thanks to [mcb](https://github.com/mcb5637) for enabling the display of the full vehicle names in battle, and **Hrothgar Heavenlight** for playtesting the mod extensively, suggesting exciting 'Mechs to add, and writing Yang's comments on them.

## Features

⚠ The mod is currently in beta. Some vehicles may spawn out of timeline, or lack an icon and description.

The mod features tons of new content, including new 'Mechs, vehicles, and weapons. The goal is to expand the BattleTech Extended timeline into the 3060s, introducing new technologies from the Fed Com Civil War. All vehicles are also playable, including VTOLs, and every faction in the game has a wider variety of 'Mechs and combat vehicles, so expect interesting matchups.

Listed below are a comprehensive list of changes and detailed information about the mod:

### General

#### Mechs
- 93 new BattleMechs and 325 new 'Mech variants, utilizing nearly every CAB model that fits the expanded 3025-3067 timeline.

<details>
  <summary>New Chassis</summary>

| Pack #1               | Pack #2            | Pack #3                       | Pack #4                     | Pack #5                                     |
| :-------------------- | :----------------- | :---------------------------- | :-------------------------- | :------------------------------------------ |
| Bellerophon           | Arctic Wolf        | Albatross                     | *Akuma*                     | *Arctic Fox*                                |
| Bombard               | Assassin II        | Anubis                        | Argus                       | *Black Watch*                               |
| Buccaneer             | *Battle Hawk*      | Barghest                      | Brigand                     | *Dervish IIC*                               |
| Dragoon               | Blitzkrieg         | Black Heart                   | Chimera                     | Gladiator-B (Executioner-B)                 |
| Fox                   | Champion LAM       | *Fire Falcon*                 | Crosscut                    | *Jinggau*                                   |
| Hellfire              | Cossack            | Galahad (Glass&nbsp;Spider)   | Dig King / Dig Lord         | *Kabuto*                                    |
| Jackrabbit            | *Hammer*           | *Grand Crusader*              | Fafnir                      | *Men Shen*                                  |
| *JagerMech III*       | Hybrid Rifleman    | Gulon                         | Gurkha                      | *Nexus*                                     |
| Juggernaut            | Lao Hu             | Iron Cheetah                  | Hellspawn                   | Night Chanter (Crab&nbsp;Omni)              |
| *Mercury II / Coyotl* | Mad Cat Mk II      | *Lineholder*                  | Kiso                        | *Raijin*                                    |
| Pulverizer            | Matar              | Lupus                         | Komodo                      | Slagmaiden                                  |
| Rampage               | Naga               | Mantis                        | *Marshal*                   | *Spartan*                                   |
| Schwerer Gustav       | Nightsky           | Osiris                        | Minsk                       | Spirit Walker (Black&nbsp;Knight&nbsp;Omni) |
| Screamer LAM          | *Phoenix Hawk IIC* | *Peregrine (Horned&nbsp;Owl)* | Razorback                   | *Tempest*                                   |
| Stag / Stag II        | Rattlesnake        | Phantom                       | Rising Star / Legacy        | *Thresher*                                  |
| Thanatos              | *Warthog*          | Prometheus                    | Roughneck                   | *Viper*                                     |
| Titan                 |                    | Sha Yu                        | Sidewinder                  | *War Dog*                                   |
|                       |                    | Templar                       | Star Adder (Blood&nbsp;Asp) |                                             |
|                       |                    | Zeus-X                        | Stilleto                    |                                             |
|                       |                    |                               | Storm Giant / Scylla        |                                             |
|                       |                    |                               | Uziel                       |                                             |
|                       |                    |                               | Vanquisher                  |                                             |
|                       |                    |                               | Verfolger                   |                                             |
|                       |                    |                               | Volkh                       |                                             |

**Note**: 'Mech names in italics do not have their own CAB model and instead use a proxy that looks similar.
</details>

- 10 Hero 'Mechs are available in the Heavy Metal crate. You can also add any of them to your game via Fell Off A Cargo Ship or a save editor, including the Exterminator 'Caine' 4DX, which is not available in-game.

#### Vehicles
- Over 150 new vehicles variants, including VTOLs and superheavy tanks.
- All vehicles have had their loadouts, armor values, and descriptions updated.
- Vehicles have received several bonuses to make them more comparable to 'Mechs:
  - Light vehicles gain a +2 To Hit modifier (Hit Defense), while medium vehicles get a +1.
  - Additionally, VTOLs have +2 Evasion and +3 Hit Defense, for a total of -10% to -12.5% chance of hitting them. Hover tanks receive a lesser bonus of +1 Evasion and +1 Hit Defense.
  - Vehicles no longer have a -25% armor penalty and instead have the same armor as in tabletop.
  - Support vehicles now have Comms Equipment that provides benefits based on the amount of equipment they carry, ranging from +1 Lance Initiative and +1 Resolve Gain with one ton to +2 Lance Initiative and +7 Resolve Gain with seven tons. Comms Equipment also has a 300m ECCM aura that increases in effectiveness, just like the other bonuses.

#### Weapons
- New weapons and upgrades become available in stores over time. Some weapons and upgrades are unique to certain 'Mech chassis, such as Stealth Armor.

<details>
  <summary>New Weapons/Upgrades (by Type/Intro)</summary>

| Name                              |   Type    | Intro | Factions                  |
| :-------------------------------- | :-------: | :---: | :------------------------ |
| Light/Medium/Heavy Rifle          | Ballistic |  PS   | *LosTech*                 |
| Thumper/Sniper/Long Tom Cannon    | Ballistic | 3012  | *Research*                |
| Magshot                           | Ballistic | 3059  | Steiner                   |
| Hyper-Velocity AC (HVAC)          | Ballistic | 3059  | Liao                      |
| Rotary AC (RAC)                   | Ballistic | 3060  | Davion                    |
| Light AC (LAC)                    | Ballistic | 3062  | Davion                    |
| Improved Heavy Gauss Rifle (iHGR) | Ballistic | 3065  | Steiner                   |
|  ----                             |           |       |                           |
| Rail Gun                          |  Energy   | 3051  | Marik                     |
| Plasma Rifle                      |  Energy   | 3061  | Liao                      |
| Heavy PPC                         |  Energy   | 3062  | Kurita                    |
| Light PPC                         |  Energy   | 3064  | Kurita                    |
| Bombast Laser                     |  Energy   | 3064  | Steiner                   |
|  ----                             |           |       |                           |
| Bomb Bay²                         |  Missile  | 2680  | *Mining*                  |
| Arrow IV                          |  Missile  | 3044  | Liao<br>All (3049+)       |
| Thunderbolt                       |  Missile  | 3052  | Davion<br>Steiner (3052+) |
| Extended LRM (ELRM)               |  Missile  | 3054  | Steiner<br>Davion (3054+) |
| Enhanced LRM (NLRM)               |  Missile  | 3058  | Davion                    |
|  ----                             |           |       |                           |
| Fluid Gun                         |  Support  |  PS   | *Chemical*                |
| Heavy Flamer                      |  Support  | 3063  | Steiner                   |
| Heavy Machine Gun                 |  Support  | 3063  | Calderon                  |
| Light Machine Gun                 |  Support  | 3064  | Liao                      |
|  ----                             |           |       |                           |
| Airburst Mortar                   |   Ammo    | 3043  | All                       |
| Shaped Charge Mortar              |   Ammo    | 3043  | All                       |
| Swarm Missile                     |   Ammo    | 3049  | Davion<br>All (3058+)     |
| Swarm-I Missile                   |   Ammo    | 3052  | Marik                     |
| Inferno-IV Missile                |   Ammo    | 3053  | Liao                      |
| Thunder-Inferno Missile           |   Ammo    | 3054  | Liao                      |
| Armor-Piercing Ammo               |   Ammo    | 3055  | Davion<br>Steiner (3055+) |
| Precision Ammo                    |   Ammo    | 3058  | Davion                    |
|  ----                             |           |       |                           |
| Laser Insulator                   |   Addon   | 2575  | *Electronics*             |
| Targeting Computer                |   Addon   | 3052  | *Research*                |
| Bloodhound Active Probe           |   Addon   | 3058  | *Black Market*            |
| Laser Anti-Missile System         |   Addon   | 3059  | *Research*                |
| Blue Shield Particle Field Damper |   Addon   | 3061  | *Research*                |
| Apollo MRM FCS                    |   Addon   | 3065  | *Research*                |
| Small/Medium/Large Shield         |   Addon   | 3065  | *Research*                |

² with High-Explosive, Laser-Guided, Cluster, and Inferno Bombs.
</details>

<details>
  <summary>Unique Weapons/Upgrades (by Type)</summary>

| Name                                  | Exclusive to                                                    |
| :------------------------------------ | :-------------------------------------------------------------- |
| Assault Katana                        | Hatamoto-Chi 'Shin'                                             |
| Claws                                 | Mantis                                                          |
| Industrial Weapons²                   | Crosscut, Dig King, Gulon, Kiso                                 |
| Lance / Katar / Mace                  | Volkh                                                           |
| Large Vibroblade<br>Large Shield      | Black Knight 'Red Reaper'                                       |
| Small Kukri                           | Gurkha                                                          |
| Small Vibroblade                      | Assassin 'Servitor'                                             |
| Spikes                                | Bombard                                                         |
|  ----                                 |                                                                 |
| Comms Equipment                       | Support Vehicles                                                |
| Direct Neural Interface               | Prometheus<br>Black Heart                                       |
| Light Active Probe                    | Vulture (Mad Dog) 'Fury'                                        |
| Supercharger                          | Exterminator 'Caine' 4DX<br>Slagmaiden, Super-Griffin and Wasp |
|  ----                                 |                                                                 |
| Composite Chassis<br>Reactive Armor   | Zeus-X                                                          |
| Light Ferro-Fibrous Armor             | Black Knight 'Red Reaper'                                       |
| Stealth Armor                         | Sha Yu<br>Anubis                                                | 

² Includes the Chainsaw, Mining Drill, Pile Driver, and other variants of these weapons.
</details>

- Gauss rifles and artillery have been somewhat nerfed, using the new tools available with CAC-C; both have a self-knockdown effect when firing equal to 25% of their actual damage, rounded down (from 10 instability for Light Gauss, to 50 for Rail Guns). Heavy Gauss Rifles also do more damage, but it decreases with range and is halved at maximum range.

- Mech mortars have been added. Artillery cannons are now available on research planets; they deal 25% less damage and have a much shorter range than their larger counterparts.

<details>
  <summary>Artillery Changes</summary>

| Name            | Damage | AoE Damage | Min. Range | Opt. Range | Max. Range |
| --------------- | -----: | ---------: | ---------: | ---------: | ---------: |
| Mortar/1        |     15 |          5 |        180 |        420 |        630 |
| Thumper Cannon  |     40 |         75 |         90 |        270 |        540 |
| Sniper Cannon   |     60 |        100 |         60 |        240 |        480 |
| Long Tom Cannon |     80 |        150 |        120 |        390 |        780 |
| Arrow IV        |     60 |        120 |        240 |        780 |       1560 |
| ----            |        |            |            |            |            |
| Standard LRM²   |      4 |          0 |        180 |        420 |        630 |
| Extended LRM    |      5 |          0 |        325 |        760 |       1140 |

² Ignores cover and acts like artillery with Swarm Ammo.
</details>

- All ballistic artillery requires an Artillery Loader addon. This addon allows the massive weapons to be mounted on a 'Mech. Only the Long Tom Artillery Piece is too large to fit on anything other than a vehicle.

### Faction Rosters
- MegaMek-generated rarity tables replace the Xtol RAT tables for vehicles. This allows for a wider variety of combat vehicles for all factions, as well as more accurate vehicle rosters for the expanded 3025-3067 timeline. Some factions, like the Great Houses and Periphery States, also have an "elite" version of their vehicle table that favors A-rated units.
- 'Mechs are also more varied, with ComStar/Word of Blake and the Clans having access to all 'Mechs from MegaMek RAT Generator. Minor factions that didn't have a RAT table before have one now, including:

<details>
  <summary>New Rarity Tables</summary>

| Faction/Unit                         |     Years Active         | Unit Rating                        |
| :----------------------------------- | :----------------------: | :--------------------------------- |
| Arc-Royal Defense Cordon             |        3058-3067         | C/Regulars                         |
| Chaos March                          |        3057-3073         | F/Locals                           |
| Clan Wolf-in-Exile                   |        3057-3151         | B/Front Lines<br>C/Second Lines    |
| Clan Snow Raven𓅪 /<br>Raven Alliance |    2807–3082<br>3083-    | F/Provisional Garrison             |
| Duchy of Andurien                    |    3030-3040<br>3079-    | C/Regulars                         |
| New Colony Region /<br>Fronc Reaches |    3060-3066<br>3067-    | C/Regulars                         |
|  ----                                |                          |                                    |
| Eridani Light Horse                  |          2702-           | B/Veterans                         |
| Gray Death Legion                    |        3024-3065         | A/Elites                           |
| Kell Hounds                          |          3010-           | B/Veterans<br>A/Elites (3040+)     |
| Northwind Highlanders                |          -3081           | B/Veterans<br>A/Elites (3059+)     |
| Other Mercenaries                    |           n/a            | C/Regulars<br>F/Locals (Periphery) |
| Pirates                              |           n/a            | F/Locals                           |
| Security Forces²                     |           n/a            | F-/Locals                          |
| Wolf's Dragoons                      |          3005-           | A/Elites                           |

𓅪 The Outworlds Alliance have access to Snow Raven garrison units after 3061.<br>
² Locals sometimes use security forces instead of their own units.
</details>

### Additional Lances
- New support lances can appear during missions with the Additional Lances setting from Mission Control enabled, such as:
    - Vehicle-only lances on lower-difficulty missions (2½ skulls or less).
    - Artillery lances with three artillery vehicles supported by an APC.
- Capellan Confederation elite units can deploy in augmented lance formations made up of four 'Mechs or vehicles supported by two other units.
- Free Worlds League and Taurian Concordat elite units both use augmented armor formations made up of six vehicles.

### Other Changes

<details>
  <summary>Shell Shuffler</summary>

This optional submod allows the AI to randomly use different types of ammunition when spawning. The mod has two presets, depending on the era you are playing in:

- **3025 preset:** Any faction can use Inferno SRM.
- **3050 preset:** Each faction has their own set of special ammo types, most of which were developed in the 3050s.

| Faction                 | Ammo Types                                          |
| :---------------------- | :-------------------------------------------------- |
| Davion                  | Armor-Piercing & Precision rounds, plus Swarm LRM   |
| Kurita                  | Dead-Fire SRM and LRM, plus Explosive Narc pods     |
| Liao                    | Inferno SRM, LRM and Arrow IV                       |
| Marik                   | Improved Swarm LRM (Swarm-I LRM)                    |
| Steiner                 | Armor-Piercing rounds and Swarm LRM                 |
| ----                    |                                                     |
| Clans                   | Extended-Range & High-Explosive ATM, plus Swarm LRM |
| ComStar                 | Swarm LRM                                           |
| Word of Blake           | Swarm-I LRM, Explosive and Haywire iNarc pods       |
| Elite Mercenaries       | Any type                                            |
| Pirates  Other Mercs    | Inferno SRM                                         | 
</details>

<details>
  <summary>Miscellaneous</summary>

- The UI in combat now only shows the ammunition type when a weapon has multiple types of ammo.
- Heavy Lasers now have distinct colors ranging from orange-red to golden.
- Changes have been made to address minor issues and to add the latest CAB models to the current version of BEX:

| Name                   | Changes                                                 |
| :--------------------- | :------------------------------------------------------ |
| Annihilator            | Reduced movement to 3/5 hexes (from 4/7)                |
| Atlas II AS7-D-HT      | Changed armor placement and moved one DHS to the engine |
| Behemoth (Stone Rhino) | Changed to the official designation "BHN"               |
| Champion CHP-1N2       | Added missing ferro-fibrous armor                       |
| Crab 27b "Royal Crab"  | Fixed melee damage (65 dmg. like other Crabs)           |
| Enfield                | Switched to a new CAB model                             |
| Exterminator           | Switched to a new CAB model                             |
| Firefly FFL-3A         | Fixed available tonnage                                 |
| Firestarter FS9-OF     | Added Large Engine quirk                                |
| Flashman               | Switched to a new CAB model                             |
| Flea FLE-14            | Fixed max armor values for an ultralight                |
| Garm                   | Reduced overall size to better match tonnage            |
| Goliath                | Reduced stability                                       |
| Grand Dragon DRG-1G    | Fixed energy hadpoints placement                        |
| Gunslinger             | Switched to a new CAB model                             |
| Hauptmann HA1-O        | Fixed weapon loadout                                    |
| Hermes II              | Reduced overall size to better match tonnage            |
| Hornet HNT-161         | Fixed armor placement                                   |
| Kodiak 5               | Added missing DHS in the engine                         |
| Linesman LMN-1PT       | Changed to the correct prefab base model                |
| Loader King LDK-5C     | Added more armor and missing Cargo Bay                  |
| Locust LCT-3S          | Fixed available tonnage                                 |
| Naginata               | Switched to a new CAB model                             |
| Ost Mechs              | Added cross assembly between Ostroc and Ostsol          |
| Phoenix Hawk LAM       | Changed to a different CAB model                        |
| Piranha 1              | Changed to single heat sinks                            |
| Rifleman RFL-5CS       | Added Large Engine quirk                                |
| Shadow Hawk            | Added Rugged quirk                                      |
| Shadow Hawk SHD-3H     | Changed loadout to the official one                     |
| Thunder THR-1L         | Added missing DHS in the engine                         |
| Trebuchet TBT-3C       | Added Large Engine quirk                                |
| UrbanMech              | Reduced movement to 3/5 hexes (from 4/7)                |
