# BTX Expansion Pack

An expansion for **BattleTech Extended (BEX)** featuring hundreds of new 'Mechs and vehicles, with the goal of expanding the timeline into the late 3060s and including the FedCom Civil War.

## Key Features:

* **Expanded Arsenal:** Adds over 100 new BattleMechs and 300+ new vehicle variants.
* **Playable Vehicles:** Command tanks, hovercraft, and VTOLs directly in your lances.
* **Strategic Depth:** Enhances gameplay with new tactical options and lance variety.

> [!IMPORTANT]
> The newest version (1.4) reintegrates all 100+ BattleMechs into the faction stores. The faction rosters are being updated and will be released as minor updates until the 1.5 release.

## Installation

1. Install the latest versions of [BattleTech Extended Tactics](https://discourse.modsinexile.com/t/battletech-extended-tactics/1859) and [CAC-C](https://github.com/mcb5637/BTX_CAC_Compatibility/releases/latest).
2. Download the most recent version of the [Expansion Pack](https://github.com/AkiraBrahe/BTX_ExpansionPack/releases/latest).
3. Remove the `.modtek` folder if present to force Modtek to rebuild the cache.
4. Unpack the mod folders into your Mods directory, overwriting all files when prompted.

> [!NOTE]
> When updating, first remove the existing `BTX_ExpansionPack` folder from your Mods directory.

### 🛠️ Optionals

* **Disable Playable Vehicles**:
  - In `BTSimpleMechAssembly\mod.json`, set `SalvageAndAssembleVehicles` to false.
  - Remove `BTX_PlayableVehicles`, `CustomPilotDecorator`, and `Lifepaths` from your Mods folder.
* **Disable Infantry**:
  - In `BTX_PlayableVehicles\mod.json`, remove or comment out the last three lines of the manifest.

## Credits

This mod is bundled with a modified version of **Playable Tanks** by [LordRuthermore](https://github.com/lordruthermore).

Special thanks to [mcb](https://github.com/mcb5637) for the combat UI names, and **Hrothgar Heavenlight** for playtesting the mod extensively, suggesting new 'Mechs to add, and writing Yang's comments.

## Features

### 🤖 New 'Mechs

The Expansion Pack adds over **100 new BattleMechs** and **680 variants**. The new chassis primarily bolster the 3025–3061 BEX timeline, but nearly half of all variants expand your options into the 3060s.

* **Faction stores have been updated** to include the new 'Mechs based on lore and date.
* **New hero 'Mechs are available** as unique rewards for flashpoints, like the upgraded *Yen-Lo-Wang* and *Big Steel Claw*, as well as end-of-career rewards.

> [!NOTE]
> 35 chassis use curated proxy models selected for their visual consistency and aesthetic fit.

  <details>
    <summary>New Chassis (by Availability)</summary>
  
  | Name                              | Class            | Mass     | Avail.       | Faction Availability                                                    |
  | :-------------------------------- | :--------------: | :------: | :----------: | :---------------------------------------------------------------------- |
  | Ambassador                        | Ultralight       | 15       | 3025         | Unique (Ultralight Start)                                               |
  | Apollo                            | Ultralight       | 15       | 3025         | Free Worlds League                                                      |
  | Bellerophon                       | Heavy            | 60       | 3025         | Free Worlds League                                                      |
  | Celerity                          | Ultralight       | 15       | 3025         | ComStar                                                                 |
  | Crosscut                          | Light            | 30       | 3025         | Pirates                                                                 |
  | Dig King /<br>Dig Lord            | Light<br>Heavy   | 35<br>65 | 3025<br>3057 | Pirates<br>FedCom                                                       |
  | Dragoon                           | Heavy            | 70       | 3025         | ComStar                                                                 |
  | Foxfire                           | Ultralight       | 15       | 3025         | Lyran Commonwealth                                                      |
  | Gulon                             | Light            | 25       | 3025         | Outworlds Alliance                                                      |
  | Hybrid Rifleman                   | Heavy            | 60       | 3025         | Unique (Heavy Metal Crate)                                              |
  | Jackrabbit                        | Light            | 25       | 3025         | ComStar, Word of Blake                                                  |
  | Junior                            | Ultralight       | 10       | 3025         | Federated Suns                                                          |
  | Kiso                              | Assault          | 100      | 3025         | Draconis Combine                                                        |
  | Mite                              | Ultralight       | 10       | 3025         | Federated Suns                                                          |
  | Rampage                           | Assault          | 85       | 3025         | Periphery States, ComStar / Word of Blake                               |
  | Rising Star /<br>Legacy           | Assault          | 80       | 3025<br>3064 | ComStar<br>Word of Blake                                                |
  | Sling                             | Light            | 25       | 3025         | ComStar, Clan Smoke Jaguar                                              |
  | Slowpoke                          | Ultralight       | 10       | 3025         | Capellan Confederation                                                  |
  | Spartan                           | Assault          | 80       | 3025         | ComStar / Word of Blake                                                 |
  | Stiletto (StarDrive)              | Ultralight       | 15       | 3025         | Draconis Combine                                                        |
  | Titan                             | Assault          | 100      | 3025         | Federated Suns                                                          |
  | Matar                             | Superheavy       | 110      | 3036         | ComStar (3036+)                                                         |
  | Rattlesnake                       | Light            | 35       | 3042         | Federated Suns                                                          |
  | Battle Cobra                      | Medium           | 40       | 3049<br>3063 | Clan Snow Raven, Clan Steel Viper / Clans (3067+)<br>ComStar            |
  | Bowman                            | Heavy            | 70       | 3049         | Clan Diamond Shark, Clan Hell's Horses                                  |
  | Corvis                            | Medium           | 40       | 3049         | Clans                                                                   |
  | Crossbow (Omni)                   | Heavy            | 65       | 3049         | Clans                                                                   |
  | Fox                               | Medium           | 50       | 3049         | Clan Ghost Bear                                                         |
  | Galahad (Glass Spider)            | Heavy            | 60       | 3049         | Clans                                                                   |
  | Gladiator-B (Executioner-B)       | Assault          | 95       | 3049         | Clans                                                                   |
  | Grizzly                           | Heavy            | 70       | 3049         | Clans                                                                   |
  | Lupus                             | Heavy            | 60       | 3049         | Clan Steel Viper                                                        |
  | Matador                           | Heavy            | 60       | 3049         | Clans                                                                   |
  | Mercury II /<br>Coyotl            | Medium           | 40       | 3049<br>3058 | Clan Diamond Shark<br>Clan Wolf (Harvest Trials)                        |
  | Peregrine (Horned Owl)            | Light            | 35       | 3049         | Clans                                                                   |
  | Phoenix Hawk IIC                  | Assault          | 80       | 3049         | Clans                                                                   |
  | Pulverizer                        | Assault          | 90       | 3049         | Clan Snow Raven                                                         |
  | Shadow Hawk IIC                   | Medium           | 45       | 3049         | Clans                                                                   |
  | Sidewinder                        | Heavy            | 75       | 3049         | Clan Jade Falcon                                                        |
  | Thresher                          | Heavy            | 60       | 3049         | Clans                                                                   |
  | Woodsman /<br>Naga II             | Heavy<br>Assault | 75<br>80 | 3049         | Clan Wolf<br>Clans                                                      |
  | Pouncer                           | Medium           | 40       | 3050         | Clan Wolf<br>Clan Nova Cat (3062+)                                      |
  | Roughneck                         | Heavy            | 65       | 3050         | FedCom                                                                  |
  | Storm Giant /<br>Scylla           | Assault          | 100      | 3051<br>3062 | Clan Steel Viper<br>Clan Jade Falcon, Clan Snow Raven, Clan Steel Viper |
  | Fire Falcon                       | Light            | 25       | 3052         | Clan Jade Falcon<br>Clan Hell's Horses, Clan Nova Cat (3062+)           |
  | Minsk                             | Heavy            | 70       | 3052         | Clan Ghost Bear                                                         |
  | Phantom                           | Medium           | 40       | 3052         | Clans                                                                   |
  | Raijin                            | Medium           | 50       | 3052         | ComStar / Word of Blake                                                 |
  | Stag / Stag II                    | Medium           | 45       | 3052         | Clans / Clan Wolf                                                       |
  | War Dog                           | Heavy            | 75       | 3052         | Inner Sphere                                                            |
  | War Dog /<br>Masauwu              | Heavy            | 75       | 3052<br>3058 | Inner Sphere<br>Clan Jade Falcon, Clan Wolf (Harvest Trials)            |
  | Battle Hawk                       | Light            | 30       | 3053         | Federated Suns, Lyran Commonwealth                                      |
  | Grand Crusader                    | Assault          | 80       | 3053         | Word of Blake                                                           |
  | Hammer                            | Light            | 30       | 3053         | Free Worlds League, Word of Blake, Capellan Confederation               |
  | Juggernaut                        | Assault          | 90       | 3053         | Lyran Commonwealth                                                      |
  | Nightsky                          | Medium           | 50       | 3053         | FedCom                                                                  |
  | Prometheus                        | Heavy            | 75       | 3053         | Federated Suns                                                          |
  | Bombard                           | Medium           | 50       | 3054         | Lyran Commonwealth                                                      |
  | Iron Cheetah                      | Assault          | 100      | 3054         | Clan Smoke Jaguar                                                       |
  | Nexus                             | Light            | 25       | 3054         | ComStar / Word of Blake                                                 |
  | Zeus-X                            | Assault          | 80       | 3054         | Federated Suns                                                          |
  | Buccaneer                         | Medium           | 55       | 3055         | Free Worlds League, Word of Blake                                       |
  | Daikyu                            | Heavy            | 70       | 3055         | Draconis Combine                                                        |
  | Tempest                           | Heavy            | 65       | 3055         | Free Worlds League, Word of Blake                                       |
  | Sentry                            | Medium           | 40       | 3056         | Federated Suns, Word of Blake                                           |
  | Akuma                             | Assault          | 90       | 3058         | Draconis Combine                                                        |
  | Canis                             | Assault          | 80       | 3058         | Clan Jade Falcon (Harvest Trials)                                       |
  | Dervish IIC                       | Medium           | 55       | 3058         | Clans                                                                   |
  | Hellfire                          | Heavy            | 60       | 3058         | Clan Steel Viper                                                        |
  | JagerMech III                     | Heavy            | 65       | 3058         | Federated Suns                                                          |
  | Lineholder                        | Medium           | 55       | 3058         | Inner Sphere                                                            |
  | Arctic Fox                        | Light            | 30       | 3059         | Arc-Royal DC, Clan Wolf-In-exile                                        |
  | Arctic Wolf                       | Medium           | 40       | 3059         | Clan Wolf-In-Exile                                                      |
  | Hellion                           | Light            | 30       | 3059         | Clans                                                                   |
  | Kabuto                            | Light            | 20       | 3059         | Draconis Combine                                                        |
  | Marshal                           | Medium           | 55       | 3059         | Trinity Alliance                                                        |
  | Night Chanter (Crab Omni)         | Medium           | 45       | 3059         | Clan Jade Falcon, Clan Wolf (Harvest Trials)                            |
  | Spirit Walker (Black Knight Omni) | Heavy            | 75       | 3059         | Clan Jade Falcon, Clan Wolf (Harvest Trials)                            |
  | Warthog                           | Assault          | 95       | 3059         | Clans                                                                   |
  | Assassin II                       | Medium           | 45       | 3060         | Federated Suns                                                          |
  | Bishamon                          | Medium           | 45       | 3060         | Draconis Combine, Free Worlds League                                    |
  | Black Heart                       | Heavy            | 70       | 3060         | Word of Blake (3060+)                                                   |
  | Star Adder (Blood Asp)            | Assault          | 90       | 3060         | Clan Hell's Horses                                                      |
  | Black Watch                       | Assault          | 85       | 3061         | Successor States                                                        |
  | Blitzkrieg                        | Medium           | 50       | 3061         | Lyran Commonwealth, Free Worlds League                                  |
  | Mantis                            | Light            | 30       | 3061         | Lyran Commonwealth                                                      |
  | Stiletto                          | Light            | 35       | 3061         | Federated Suns, Lyran Commonwealth                                      |
  | Thanatos                          | Heavy            | 75       | 3061         | FedCom                                                                  |
  | Argus                             | Heavy            | 60       | 3062         | Federated Suns                                                          |
  | Hellspawn                         | Medium           | 45       | 3062         | Federated Suns                                                          |
  | Lao Hu                            | Heavy            | 75       | 3062         | Capellan Confederation                                                  |
  | Mad Cat Mk II                     | Assault          | 90       | 3062         | Clan Diamond Shark, Clan Nova Cat<br>Clans (3067+)                      |
  | Templar                           | Assault          | 85       | 3062         | Federated Suns                                                          |
  | Anubis                            | Light            | 30       | 3063         | Capellan Confederation, Magistracy of Canopus, Taurian Concordat        |
  | Chimera                           | Medium           | 40       | 3063         | FedCom, Draconis Combine, Word of Blake                                 |
  | Fafnir                            | Assault          | 100      | 3063         | Lyran Commonwealth                                                      |
  | Gurkha                            | Light            | 35       | 3063         | Word of Blake                                                           |
  | Osiris                            | Light            | 30       | 3063         | Federated Suns                                                          |
  | Razorback                         | Light            | 30       | 3063         | FedCom                                                                  |
  | Sha Yu                            | Medium           | 40       | 3063         | Capellan Confederation, Magistracy of Canopus                           |
  | Tomahawk                          | Assault          | 100      | 3063         | Clan Wolf                                                               |
  | Uziel                             | Medium           | 50       | 3063         | FedCom                                                                  |
  | Vanquisher                        | Assault          | 100      | 3063         | Word of Blake                                                           |
  | Verfolger                         | Heavy            | 65       | 3063         | Arc-Royal DC, Lyran Commonwealth                                        |
  | Volkh                             | Medium           | 45       | 3063         | Lyran Commonwealth                                                      |
  | Lightray                          | Medium           | 55       | 3064         | Word of Blake                                                           |
  | Solitaire                         | Light            | 25       | 3064         | Clans                                                                   |
  | Brigand                           | Light            | 25       | 3065         | Pirates                                                                 |
  | Hellhound II (Hellcat)            | Medium           | 50       | 3065         | Clan Jade Falcon                                                        |
  | Valiant                           | Light            | 30       | 3066         | Draconis Combine, Federated Suns, Lyran Commonwealth                    |
  | Great Turtle                      | Assault          | 100      | 3067         | Lyran Commonwealth                                                      |
  | Wight                             | Light            | 35       | 3068         | Draconis Combine, Free Worlds League, Lyran Commonwealth                |
  | Schwerer Gustav                   | Assault          | 100      | 3073         | Arc-Royal DC (3067+)                                                    |
  | Slagmaiden                        | Medium           | 55       | 3076         | Arc-Royal DC (3067+)                                                    |
  
  ² Wolf's Dragoons and mercenaries have access to many of these 'Mechs.
  </details>

  <details>
    <summary>Hero Mechs</summary>
  
  | Name                             | Model Code | Intro | Pilot                                 |
  | :------------------------------- | :--------: | :---: | :------------------------------------ |
  | Mackie 'Kill Roy's Little Buddy' | MSK-9HKR   | 2750  | None (mascot of the Martial Olympiad) |
  | Exterminator 'Caine'             | EXT-4DX    | 2754  | Caine Barclay                         |
  | Hybrid Rifleman 'Sneede'         | RFL-SND    | 3025  | Samual 'Shorty' Sneede                |
  | Charger 'Number Seven'           | CGR-N7     | 3025  | Terry Ford                            |
  | Centurion 'Yen-Lo-Wang'          | CN9-YLW    | 3027  | Justin Xiang Allard                   |
  | Centurion 'Yen-Lo-Wang 2'        | CN9-YLW2   | 3051  | Kai Allard-Liao                       |
  | BattleMaster 'Red Corsair'       | BLR-RC     | 3055  | Nekane 'Red Corsair' Hazen            |
  | Vulture (Mad Dog) 'Fury'         | VUL-FURY   | 3059  | Katherine Furey (non-canon variant)   |
  | Hatamoto-Chi 'Shin'              | HTM-S      | 3060  | Shin Yodama (?)                       |
  | Marauder II 'Bounty Hunter'      | MAD-BHIII  | 3064  | Vic Travers                           |
  | Assassin 'Servitor'              | ASN-SRV    | 3066  | None (custom variant)                 |
  | Black Knight 'Red Reaper'        | BL-X-KNT   | 3069  | Reginald VanJaster                    |
  | Schwerer Gustav 'Jäger'          | SJ-1X      | 3073  | None (non-canon variant)              |

  </details>

### 🚛 Playable Vehicles & Mechanics

Command over **580 different vehicle variants** across **180 chassis**, including VTOLs and superheavy tanks, effectively doubling the vehicle pool found in BEX.

* **Rebalanced stats and 360° turrets** give conventional forces greater flexibility and survivability. Light and medium vehicles gain hit defense, while VTOLs and hovercraft gain evasion and initiative bonuses.
* **Biome-specific deployment restrictions** prevent units like VTOLs from dropping in unsuitable environments (e.g., low atmosphere).
* **Limited vehicle refits** allow you to customize your conventional forces by swapping out existing weapons and ammunition.
* **Dedicated vehicle pilots** can be trained using a refined lifepath system, and now benefit from integrated CASE and injury mechanics.

### ⚔️ Advanced Weaponry & Tech

The Expansion Pack adds new weapons and refines existing mechanics. The leap into the 3060s also brings new technology to the ever-changing battlefield.

* **New artillery systems** like direct-firing Mech Mortars and Artillery Cannons expand your tactical options.
* **Rapid-fire autocannons** provide a good alternative to AMS for anti-missile and anti-air defense, trading raw damage of the autocannon for high accuracy against evasive targets.
* **Advanced ammunition types** include Homing Arrow IV (requiring TAG), Swarm LRMs for area saturation, and Thunder-Inferno LRMs for area denial.
* **Comms equipment and artillery TTS** grant lance-wide bonuses or enhance the accuracy of any artillery system.
* **Gauss Rifles have been rebalanced** across the board to crit through armor, replacing the flat structure damage previously exclusive to SLDF models. In addition, firing Gauss weapons and artillery now generates instability.
* **Integrated infantry complements** on APCs act as unkillable, integrated weapon systems to support your forces in the field.

> [!TIP]
> **Artillery Crits:** AoE damage typically spreads across all locations, but impacts near the target center now have a chance to inflict a critical hit, concentrating the full blast on a single location.

### 🗺️ World & Economy

* **Overhauled lances and missions** bring greater variety to encounters, especially when facing ComStar and the Clans, with increasingly dangerous support lances.
* **Expanded vehicle economy** adds 20+ new vehicle-exclusive factories and updates faction stores.
* **New career starts** let you begin your mercenary journey with "Ultralights Only", "Solo", or "Vehicles Only" options.
* **Improved enemy AI** now understands how to use artillery and specialized munitions effectively in combat.

  <details>
    <summary>Lance & Mission Variety</summary>

  The mod significantly expands mission and lance generation logic:

  * **Additional Lances:** Every mission has a low chance of having additional ally and enemy lances with varying compositions (from VTOL to artillery and command lances).
  * **Salvage Race:** A new mission where you must retrieve an SLDF cache while ComStar defends it. Fighting is optional, making it an easier alternative to Search Denial and Tag Team missions.
  * **Dynamic Search Denial:** ComStar now fields varied forces (vehicles, 'Mechs, or mixed) and elite Com Guard variants. You might also encounter Snord's Irregulars fighting against ComStar!
  * **Lore-Accurate Lances:** ComStar Level IIs and Clan Stars now spawn with balanced, diverse unit compositions, replacing the default behavior of simply cloning a unit, which often resulted in all-assault lances.
  * **Specialized Formations:** The Capellan Confederation utilizes augmented lances post-Clan Invasion, while dedicated artillery lances with custom spawn logic ensure unique tactical challenges.

  </details>

> [!TIP]
> **High-Value Loot:** Com Guards have a doubled chance of fielding royal 'Mechs and tanks, whereas command lances favor high BV units.

### 🎨 Quality of Life & Visuals

* **Cleaner UI and tooltips** provide better information formatting in and out of combat. This includes mech tooltips and various info panels in the mech bay and in battle.
* **ComStar missions are color-coded** for better visibility in the mission select screen.
* **Target information** in the mission contract screen now clearly shows the mission's target faction and equipment rating.
* **Enhanced weapon visuals** give Heavy Lasers and Snub-Nose PPCs distinct, recognizable beam colors.
* **Improved combat UI** ensures full vehicle names are always displayed for easier target identification in battle.
* **Improved urban performance** through the destruction of many buildings before battle depending on the mission type.

## Roadmap

### History

* **v0.1 to v0.9** focused on adding new 'Mechs and equipment, then later playable vehicles.
* **v1.0 to v1.4** expanded the mod scope to include various features beyond just adding new 'Mechs and vehicles.

### Future Plans

* **v1.5** will focus on updating the faction rosters and allowing more customization in the MechLab.
* **v2.0** will focus on adding new factions and the FedCom Civil War. This is currently the last planned major update.
