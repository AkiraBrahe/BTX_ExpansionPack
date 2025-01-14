# BTX Expansion Pack

A BattleTech Extended mod that adds tons of new content, including new 'Mechs, vehicles, and weapons, as well as expanding the timeline into the 3060s. Requires CAC-C.

## Installation

**⚠ This mod is currently not compatible with the latest BEX 2.0 (Tactics) update. To give everyone a chance to fully enjoy the new version, I'll wait a few months before releasing an update for the Expansion Pack.**

1. Install the latest versions of [CAC-C](https://github.com/mcb5637/BTX_CAC_Compatibility/releases/latest) and [The Big Deal Add-On](https://discourse.modsinexile.com/t/the-big-deal-add-on-for-battletech-extended-3025-3061/631) for BEX + CAC-C.
2. Update the CAB with the new [CAB installer](https://discourse.modsinexile.com/t/community-assets-bundle-cab/115), as the mod uses the latest models. The old installer, or "Legacy" mode, is no longer supported.
5. Remove the ".modtek" folder to force Modtek to rebuild the cache.
6. Remove the "AbilityRealizer", "BTX_ExpansionPack", "BTX_PlayableVehicles", and "Retrainer" mod folders if present.
7. Unpack the mod folders from the "BTX_ExpansionPack" archive into your mods folder, overwriting when prompted. The archive already includes a modified version of Playable Tanks/VTOLs.

### Optionals
- **For all DHS in the engine to dissipate 6 heat instead of 4**:
  - Enable the `BTX_DHS_Changes` mod by removing the `.bak` extension from its mod.json file.
- **For the AI to use different types of ammo randomly during combat**:
  - Enable the `Shell Shuffler` mod by removing the date part from one of its two mod.json files.
- **To disable playable vehicles**:
  - In the `BTSimpleMechAssembly` mod.json file, change `"SalvageAndAssembleVehicles"` to `"false"`.
  - Remove these mods from your mods folder: Abilifier, BTX_CustomPilotDecorator, BTX_PlayableVehicles, and Lifepaths.

## Credits

This mod is bundled with Playable Tanks and DHS Changes by [LordRuthermore](https://github.com/lordruthermore) and Shell Shuffler by [T-Bone](https://github.com/ajkroeg).

Special thanks to [mcb](https://github.com/mcb5637) for enabling the display of the full vehicle names in battle, and **Hrothgar Heavenlight** for playtesting the mod extensively, suggesting exciting 'Mechs to add, and writing Yang's comments on them.

## Features

The mod features tons of new content, including new 'Mechs, vehicles, and weapons. It also expands the BattleTech Extended timeline into the 3060s, introducing new technologies from the FedCom Civil War. All vehicles are playable, including VTOLs, and every faction in the game has a wider variety of 'Mechs and combat vehicles, so expect interesting matchups.

Listed below are a comprehensive list of changes and detailed information about the mod:

### New Content

#### Mechs
- Over 100 new BattleMechs and 468 new 'Mech variants, utilizing almost every CAB model that fits the expanded 3025-3067 timeline.

	<details>
	  <summary>New Chassis (by Name)</summary>
	
	| Name                              | Class            | Mass     | Intro        | Faction Availability                                                                |
	| :-------------------------------- | :--------------: | :------: | :----------: | :---------------------------------------------------------------------------------- |
	| Akuma                             | Assault          | 90       | 3058         | Draconis Combine                                                                    |
	| Albatross                         | Assault          | 95       | 3053         | Free Worlds League, Word of Blake                                                   |
	| Anubis                            | Light            | 30       | 3063         | Capellan Confederation, Magistracy of Canopus, Taurian Concordat                    |
	| Arctic Fox                        | Light            | 30       | 3059         | Arc-Royal DC, Clan Wolf-In-exile                                                    |
	| Arctic Wolf                       | Medium           | 40       | 3059         | Clan Wolf-In-Exile                                                                  |
	| Argus                             | Heavy            | 60       | 3062         | Federated Suns                                                                      |
	| Assassin II                       | Medium           | 45       | 3060         | Federated Suns                                                                      |
	| Barghest                          | Heavy            | 70       | 3058         | Lyran Commonwealth                                                                  |
	| Battle Cobra                      | Medium           | 40       | 2873<br>3063 | Clan Snow Raven, Clan Steel Viper / Clans (3067+)<br>ComStar                        |
	| Battle Hawk                       | Light            | 30       | 3053         | Federated Suns, Lyran Commonwealth                                                  |
	| Bellerophon                       | Heavy            | 60       | 2442         | Free Worlds League                                                                  |
	| Bishamon                          | Medium           | 45       | 3060         | Draconis Combine, Free Worlds League                                                |
	| Black Heart                       | Heavy            | 70       | 3069         | Word of Blake (3060+)                                                               |
	| Black Watch                       | Assault          | 85       | 3061         | Successor States                                                                    |
	| Blitzkrieg                        | Medium           | 50       | 3061         | Lyran Commonwealth, Free Worlds League                                              |
	| Bombard                           | Medium           | 50       | 3054         | Lyran Commonwealth                                                                  |
	| Bowman                            | Heavy            | 70       | 2923         | Clan Diamond Shark, Clan Hell's Horses                                              |
	| Brigand                           | Light            | 25       | 3065         | Pirates                                                                             |
	| Buccaneer                         | Medium           | 55       | 3055         | Free Worlds League, Word of Blake                                                   |
	| Canis                             | Assault          | 80       | 3058         | Clan Jade Falcon (Harvest Trials)                                                   |
	| Champion LAM                      | Heavy            | 60       | 2699         | Word of Blake                                                                       |
	| Chimera                           | Medium           | 40       | 3063         | FedCom, Draconis Combine, Word of Blake                                             |
	| Cossack                           | Light            | 20       | 3060         | St. Ives Compact, ComStar                                                           |
	| Crosscut                          | Light            | 30       | 2650         | Pirates                                                                             |
	| Dervish IIC                       | Medium           | 55       | 3058         | Clans                                                                               |
	| Dig King /<br>Dig Lord            | Light<br>Heavy   | 35<br>65 | 2573<br>3057 | Pirates<br>FedCom                                                                   |
	| Dragoon                           | Heavy            | 70       | 2771         | ComStar                                                                             |
	| Fafnir                            | Assault          | 100      | 3063         | Lyran Commonwealth                                                                  |
	| Fire Falcon                       | Light            | 25       | 3052         | Clan Jade Falcon<br>Clan Hell's Horses, Clan Nova Cat (3062+)                       |
	| Fox                               | Medium           | 50       | 2824         | Clan Ghost Bear                                                                     |
	| Galahad (Glass Spider)            | Heavy            | 60       | 2834         | Clans                                                                               |
	| Ghost Crab [Japanese Crab]        | Heavy            | 75       | 3067         | Capellan Confederation (Non-Canon)                                                  |
	| Gladiator-B (Executioner-B)       | Assault          | 95       | 2873         | Clans                                                                               |
	| Grand Crusader                    | Assault          | 80       | 3053         | Word of Blake                                                                       |
	| Great Turtle                      | Assault          | 100      | 3067         | Lyran Commonwealth                                                                  |
	| Grizzly                           | Heavy            | 70       | 2947         | Clans                                                                               |
	| Gulon                             | Light            | 25       | 3000         | Outworlds Alliance                                                                  |
	| Gurkha                            | Light            | 35       | 3063         | Word of Blake                                                                       |
	| Hammer                            | Light            | 30       | 3053         | Free Worlds League, Word of Blake, Capellan Confederation                           |
	| Hellfire                          | Heavy            | 60       | 3058         | Clan Steel Viper                                                                    |
	| Hellhound II (Hellcat)            | Medium           | 50       | 3065         | Clan Jade Falcon                                                                    |
	| Hellspawn                         | Medium           | 45       | 3062         | Federated Suns                                                                      |
	| Hybrid Rifleman                   | Heavy            | 60       | 3025         | Unique (Heavy Metal Crate)                                                          |
	| Iron Cheetah                      | Assault          | 100      | 3054         | Clan Smoke Jaguar                                                                   |
	| Jackrabbit                        | Light            | 25       | 2765         | ComStar, Word of Blake                                                              |
	| JagerMech III                     | Heavy            | 65       | 3058         | Federated Suns                                                                      |
	| Jinggau                           | Heavy            | 65       | 3060         | Trinity Alliance (Capellan Confederation, Magistracy of Canopus, Taurian Concordat) |
	| Juggernaut                        | Assault          | 90       | 3053         | Lyran Commonwealth                                                                  |
	| Kabuto                            | Light            | 20       | 3059         | Draconis Combine                                                                    |
	| Kiso                              | Assault          | 100      | 2703         | Draconis Combine                                                                    |
	| Komodo                            | Medium           | 45       | 3053         | Draconis Combine, Free Rasalhague Republic                                          |
	| Lao Hu                            | Heavy            | 75       | 3062         | Capellan Confederation                                                              |
	| Lightray                          | Medium           | 55       | 3064         | Word of Blake                                                                       |
	| Lineholder                        | Medium           | 55       | 3058         | Inner Sphere                                                                        |
	| Lupus                             | Heavy            | 60       | 2857         | Clan Steel Viper                                                                    |
	| Mad Cat Mk II                     | Assault          | 90       | 3062         | Clan Diamond Shark, Clan Nova Cat<br>Clans (3067+)                                  |
	| Mantis                            | Light            | 30       | 3052         | Lyran Commonwealth (3061+)                                                          |
	| Marshal                           | Medium           | 55       | 3059         | Trinity Alliance                                                                    |
	| Matar                             | Superheavy       | 110      | 2775         | ComStar (3036+)                                                                     |
	| Men Shen                          | Medium           | 55       | 3060         | Capellan Confederation, Magistracy of Canopus                                       |
	| Mercury II /<br>Coyotl            | Medium           | 40       | 2823<br>2854 | Clan Diamond Shark<br>Clan Wolf (Harvest Trials)                                    |
	| Minsk                             | Heavy            | 70       | 2830         | Clan Ghost Bear                                                                     |
	| Naga                              | Assault          | 80       | 2869         | Clans                                                                               |
	| Nexus                             | Light            | 25       | 3054         | ComStar / Word of Blake                                                             |
	| Night Chanter (Crab Omni)         | Medium           | 45       | 2865         | Clan Jade Falcon, Clan Wolf (Harvest Trials)                                        |
	| Nightsky                          | Medium           | 50       | 3053         | FedCom                                                                              |
	| Osiris                            | Light            | 30       | 3063         | Federated Suns                                                                      |
	| Peregrine (Horned Owl)            | Light            | 35       | 2835         | Clans                                                                               |
	| Phantom                           | Medium           | 40       | 3052         | Clans                                                                               |
	| Phoenix Hawk IIC                  | Assault          | 80       | 2851         | Clans                                                                               |
	| Pouncer                           | Medium           | 40       | 3050         | Clan Wolf<br>Clan Nova Cat (3062+)                                                  |
	| Prometheus                        | Heavy            | 75       | 3053         | Federated Suns                                                                      |
	| Pulverizer                        | Assault          | 90       | 2823         | Clan Snow Raven                                                                     |
	| Raijin                            | Medium           | 50       | 3052         | ComStar / Word of Blake                                                             |
	| Rampage                           | Assault          | 85       | 2735         | Periphery States, ComStar / Word of Blake                                           |
	| Rattlesnake                       | Light            | 35       | 3042         | Federated Suns                                                                      |
	| Razorback                         | Light            | 30       | 3063         | FedCom                                                                              |
	| Rising Star /<br>Legacy           | Assault          | 80       | 2692<br>3064 | ComStar<br>Word of Blake                                                            |
	| Roughneck                         | Heavy            | 65       | 3050         | FedCom                                                                              |
	| Schwerer Gustav                   | Assault          | 100      | 3073         | Arc-Royal DC (3064+)                                                                |
	| Screamer LAM                      | Medium           | 55       | 2774         | Snord's Irregulars                                                                  |
	| Sentry                            | Medium           | 40       | 3056         | Federated Suns, Word of Blake                                                       |
	| Sha Yu                            | Medium           | 40       | 3063         | Capellan Confederation, Magistracy of Canopus                                       |
	| Shadow Hawk IIC                   | Medium           | 45       | 2831         | Clans                                                                               |
	| Sidewinder                        | Heavy            | 75       | 3047         | Clan Jade Falcon                                                                    |
	| Slagmaiden                        | Medium           | 55       | 3076         | Arc-Royal DC (3067+)                                                                |
	| Sling                             | Light            | 25       | 2766         | ComStar, Clan Smoke Jaguar                                                          |
	| Spartan                           | Assault          | 80       | 2764         | ComStar / Word of Blake                                                             |
	| Spirit Walker (Black Knight Omni) | Heavy            | 75       | 2866         | Clan Jade Falcon, Clan Wolf (Harvest Trials)                                        |
	| Stag / Stag II                    | Medium           | 45       | 2823         | Clans / Clan Wolf                                                                   |
	| Star Adder (Blood Asp)            | Assault          | 90       | 3060         | Clan Hell's Horses                                                                  |
	| Stiletto                          | Light            | 35       | 3061         | Federated Suns, Lyran Commonwealth                                                  |
	| Stiletto [StarDrive]              | Ultralight       | 15       | 2473         | Draconis Combine                                                                    |
	| Storm Giant /<br>Scylla           | Assault          | 100      | 2862<br>3062 | Clan Steel Viper<br>Clan Jade Falcon, Clan Snow Raven, Clan Steel Viper             |
	| Tempest                           | Heavy            | 65       | 3055         | Free Worlds League, Word of Blake                                                   |
	| Templar                           | Assault          | 85       | 3062         | Federated Suns                                                                      |
	| Thanatos                          | Heavy            | 75       | 3061         | FedCom                                                                              |
	| Thresher                          | Heavy            | 60       | 2849         | Clans                                                                               |
	| Titan                             | Assault          | 100      | 2765         | Federated Suns                                                                      |
	| Uziel                             | Medium           | 50       | 3063         | FedCom                                                                              |
	| Valiant                           | Light            | 30       | 3066         | Draconis Combine, Federated Suns, Lyran Commonwealth                                |
	| Vanquisher                        | Assault          | 100      | 3063         | Word of Blake                                                                       |
	| Verfolger                         | Heavy            | 65       | 3063         | Arc-Royal DC, Lyran Commonwealth                                                    |
	| Viper                             | Heavy            | 70       | 2832         | Free Worlds League                                                                  |
	| Volkh                             | Medium           | 45       | 3063         | Lyran Commonwealth                                                                  |
	| War Dog /<br>Masauwu              | Heavy            | 75       | 3052<br>2832 | Inner Sphere<br>Clan Jade Falcon, Clan Wolf (Harvest Trials)                        |                                                                        |
	| Warthog                           | Assault          | 95       | 3059         | Clans                                                                               |
	| Wight                             | Light            | 35       | 3068         | Draconis Combine, Free Worlds League, Lyran Commonwealth                            |
	| Woodsman /<br>Naga II             | Heavy<br>Assault | 75<br>80 | 2866         | Clan Wolf<br>Clans                                                                  |
	| Zeus-X                            | Assault          | 80       | 3054         | Federated Suns                                                                      |
	
	² Wolf's Dragoons and mercenaries have access to many of these 'Mechs.
	</details>

	<details>
	  <summary>New Chassis (by Availability)</summary>
	
	| Name                              | Class            | Mass     | Avail.       | Faction Availability                                                                |
	| :-------------------------------- | :--------------: | :------: | :----------: | :---------------------------------------------------------------------------------- |
	| Bellerophon                       | Heavy            | 60       | 3025         | Free Worlds League                                                                  |
	| Champion LAM                      | Heavy            | 60       | 3025         | Word of Blake                                                                       |
	| Crosscut                          | Light            | 30       | 3025         | Pirates                                                                             |
	| Dig King /<br>Dig Lord            | Light<br>Heavy   | 35<br>65 | 3025<br>3057 | Pirates<br>FedCom                                                                   |
	| Dragoon                           | Heavy            | 70       | 3025         | ComStar                                                                             |
	| Gulon                             | Light            | 25       | 3025         | Outworlds Alliance                                                                  |
	| Hybrid Rifleman                   | Heavy            | 60       | 3025         | Unique (Heavy Metal Crate)                                                          |
	| Jackrabbit                        | Light            | 25       | 3025         | ComStar, Word of Blake                                                              |
	| Kiso                              | Assault          | 100      | 3025         | Draconis Combine                                                                    |
	| Rampage                           | Assault          | 85       | 3025         | Periphery States, ComStar / Word of Blake                                           |
	| Rising Star /<br>Legacy           | Assault          | 80       | 3025<br>3064 | ComStar<br>Word of Blake                                                            |
	| Screamer LAM                      | Medium           | 55       | 3025         | Snord's Irregulars                                                                  |
	| Sling                             | Light            | 25       | 3025         | ComStar, Clan Smoke Jaguar                                                          |
	| Spartan                           | Assault          | 80       | 3025         | ComStar / Word of Blake                                                             |
	| Stiletto [StarDrive]              | Ultralight       | 15       | 3025         | Draconis Combine                                                                    |
	| Titan                             | Assault          | 100      | 3025         | Federated Suns                                                                      |
	| Viper                             | Heavy            | 70       | 3025         | Free Worlds League                                                                  |
	| Matar                             | Superheavy       | 110      | 3036         | ComStar (3036+)                                                                     |
	| Rattlesnake                       | Light            | 35       | 3042         | Federated Suns                                                                      |
	| Battle Cobra                      | Medium           | 40       | 3049<br>3063 | Clan Snow Raven, Clan Steel Viper / Clans (3067+)<br>ComStar                        |
	| Bowman                            | Heavy            | 70       | 3049         | Clan Diamond Shark, Clan Hell's Horses                                              |
	| Fox                               | Medium           | 50       | 3049         | Clan Ghost Bear                                                                     |
	| Galahad (Glass Spider)            | Heavy            | 60       | 3049         | Clans                                                                               |
	| Gladiator-B (Executioner-B)       | Assault          | 95       | 3049         | Clans                                                                               |
	| Grizzly                           | Heavy            | 70       | 3049         | Clans                                                                               |
	| Lupus                             | Heavy            | 60       | 3049         | Clan Steel Viper                                                                    |
	| Mercury II /<br>Coyotl            | Medium           | 40       | 3049<br>3058 | Clan Diamond Shark<br>Clan Wolf (Harvest Trials)                                    |
	| Naga                              | Assault          | 80       | 3049         | Clans                                                                               |
	| Peregrine (Horned Owl)            | Light            | 35       | 3049         | Clans                                                                               |
	| Phoenix Hawk IIC                  | Assault          | 80       | 3049         | Clans                                                                               |
	| Pulverizer                        | Assault          | 90       | 3049         | Clan Snow Raven                                                                     |
	| Shadow Hawk IIC                   | Medium           | 45       | 3049         | Clans                                                                               |
	| Sidewinder                        | Heavy            | 75       | 3049         | Clan Jade Falcon                                                                    |
	| Thresher                          | Heavy            | 60       | 3049         | Clans                                                                               |
	| Woodsman /<br>Naga II             | Heavy<br>Assault | 75<br>80 | 3049         | Clan Wolf<br>Clans                                                                  |
	| Pouncer                           | Medium           | 40       | 3050         | Clan Wolf<br>Clan Nova Cat (3062+)                                                  |
	| Roughneck                         | Heavy            | 65       | 3050         | FedCom                                                                              |
	| Storm Giant /<br>Scylla           | Assault          | 100      | 3051<br>3062 | Clan Steel Viper<br>Clan Jade Falcon, Clan Snow Raven, Clan Steel Viper             |
	| Fire Falcon                       | Light            | 25       | 3052         | Clan Jade Falcon<br>Clan Hell's Horses, Clan Nova Cat (3062+)                       |
	| Minsk                             | Heavy            | 70       | 3052         | Clan Ghost Bear                                                                     |
	| Phantom                           | Medium           | 40       | 3052         | Clans                                                                               |
	| Raijin                            | Medium           | 50       | 3052         | ComStar / Word of Blake                                                             |
	| Stag / Stag II                    | Medium           | 45       | 3052         | Clans / Clan Wolf                                                                   |
	| War Dog /<br>Masauwu              | Heavy            | 75       | 3052<br>3058 | Inner Sphere<br>Clan Jade Falcon, Clan Wolf (Harvest Trials)                        |   
	| Albatross                         | Assault          | 95       | 3053         | Free Worlds League, Word of Blake                                                   |
	| Battle Hawk                       | Light            | 30       | 3053         | Federated Suns, Lyran Commonwealth                                                  |
	| Grand Crusader                    | Assault          | 80       | 3053         | Word of Blake                                                                       |
	| Hammer                            | Light            | 30       | 3053         | Free Worlds League, Word of Blake, Capellan Confederation                           |
	| Juggernaut                        | Assault          | 90       | 3053         | Lyran Commonwealth                                                                  |
	| Komodo                            | Medium           | 45       | 3053         | Draconis Combine, Free Rasalhague Republic                                          |
	| Nightsky                          | Medium           | 50       | 3053         | FedCom                                                                              |
	| Prometheus                        | Heavy            | 75       | 3053         | Federated Suns                                                                      |
	| Bombard                           | Medium           | 50       | 3054         | Lyran Commonwealth                                                                  |
	| Iron Cheetah                      | Assault          | 100      | 3054         | Clan Smoke Jaguar                                                                   |
	| Nexus                             | Light            | 25       | 3054         | ComStar / Word of Blake                                                             |
	| Zeus-X                            | Assault          | 80       | 3054         | Federated Suns                                                                      |
	| Buccaneer                         | Medium           | 55       | 3055         | Free Worlds League, Word of Blake                                                   |
	| Tempest                           | Heavy            | 65       | 3055         | Free Worlds League, Word of Blake                                                   |
	| Sentry                            | Medium           | 40       | 3056         | Federated Suns, Word of Blake                                                       |
	| Akuma                             | Assault          | 90       | 3058         | Draconis Combine                                                                    |
	| Barghest                          | Heavy            | 70       | 3058         | Lyran Commonwealth                                                                  |
	| Canis                             | Assault          | 80       | 3058         | Clan Jade Falcon (Harvest Trials)                                                   |
	| Dervish IIC                       | Medium           | 55       | 3058         | Clans                                                                               |
	| Hellfire                          | Heavy            | 60       | 3058         | Clan Steel Viper                                                                    |
	| JagerMech III                     | Heavy            | 65       | 3058         | Federated Suns                                                                      |
	| Lineholder                        | Medium           | 55       | 3058         | Inner Sphere                                                                        |
	| Arctic Fox                        | Light            | 30       | 3059         | Arc-Royal DC, Clan Wolf-In-exile                                                    |
	| Arctic Wolf                       | Medium           | 40       | 3059         | Clan Wolf-In-Exile                                                                  |
	| Kabuto                            | Light            | 20       | 3059         | Draconis Combine                                                                    |
	| Marshal                           | Medium           | 55       | 3059         | Trinity Alliance                                                                    |
	| Night Chanter (Crab Omni)         | Medium           | 45       | 3059         | Clan Jade Falcon, Clan Wolf (Harvest Trials)                                        |
	| Spirit Walker (Black Knight Omni) | Heavy            | 75       | 3059         | Clan Jade Falcon, Clan Wolf (Harvest Trials)                                        |
	| Warthog                           | Assault          | 95       | 3059         | Clans                                                                               |
	| Assassin II                       | Medium           | 45       | 3060         | Federated Suns                                                                      |
	| Bishamon                          | Medium           | 45       | 3060         | Draconis Combine, Free Worlds League                                                |
	| Black Heart                       | Heavy            | 70       | 3060         | Word of Blake (3060+)                                                               |
	| Cossack                           | Light            | 20       | 3060         | St. Ives Compact, ComStar                                                           |
	| Jinggau                           | Heavy            | 65       | 3060         | Trinity Alliance (Capellan Confederation, Magistracy of Canopus, Taurian Concordat) |
	| Men Shen                          | Medium           | 55       | 3060         | Capellan Confederation, Magistracy of Canopus                                       |
	| Star Adder (Blood Asp)            | Assault          | 90       | 3060         | Clan Hell's Horses                                                                  |
	| Black Watch                       | Assault          | 85       | 3061         | Successor States                                                                    |
	| Blitzkrieg                        | Medium           | 50       | 3061         | Lyran Commonwealth, Free Worlds League                                              |
	| Mantis                            | Light            | 30       | 3061         | Lyran Commonwealth (3061+)                                                          |
	| Stiletto                          | Light            | 35       | 3061         | Federated Suns, Lyran Commonwealth                                                  |
	| Thanatos                          | Heavy            | 75       | 3061         | FedCom                                                                              |
	| Argus                             | Heavy            | 60       | 3062         | Federated Suns                                                                      |
	| Hellspawn                         | Medium           | 45       | 3062         | Federated Suns                                                                      |
	| Lao Hu                            | Heavy            | 75       | 3062         | Capellan Confederation                                                              |
	| Mad Cat Mk II                     | Assault          | 90       | 3062         | Clan Diamond Shark, Clan Nova Cat<br>Clans (3067+)                                  |
	| Templar                           | Assault          | 85       | 3062         | Federated Suns                                                                      |
	| Anubis                            | Light            | 30       | 3063         | Capellan Confederation, Magistracy of Canopus, Taurian Concordat                    |
	| Chimera                           | Medium           | 40       | 3063         | FedCom, Draconis Combine, Word of Blake                                             |
	| Fafnir                            | Assault          | 100      | 3063         | Lyran Commonwealth                                                                  |
	| Gurkha                            | Light            | 35       | 3063         | Word of Blake                                                                       |
	| Osiris                            | Light            | 30       | 3063         | Federated Suns                                                                      |
	| Razorback                         | Light            | 30       | 3063         | FedCom                                                                              |
	| Sha Yu                            | Medium           | 40       | 3063         | Capellan Confederation, Magistracy of Canopus                                       |
	| Uziel                             | Medium           | 50       | 3063         | FedCom                                                                              |
	| Vanquisher                        | Assault          | 100      | 3063         | Word of Blake                                                                       |
	| Verfolger                         | Heavy            | 65       | 3063         | Arc-Royal DC, Lyran Commonwealth                                                    |
	| Volkh                             | Medium           | 45       | 3063         | Lyran Commonwealth                                                                  |
	| Lightray                          | Medium           | 55       | 3064         | Word of Blake                                                                       |
	| Schwerer Gustav                   | Assault          | 100      | 3064         | Arc-Royal DC (3064+)                                                                |
	| Brigand                           | Light            | 25       | 3065         | Pirates                                                                             |
	| Hellhound II (Hellcat)            | Medium           | 50       | 3065         | Clan Jade Falcon                                                                    |
	| Valiant                           | Light            | 30       | 3066         | Draconis Combine, Federated Suns, Lyran Commonwealth                                |
	| Ghost Crab [Japanese Crab]        | Heavy            | 75       | 3067         | Capellan Confederation (Non-Canon)                                                  |
	| Great Turtle                      | Assault          | 100      | 3067         | Lyran Commonwealth                                                                  |
	| Slagmaiden                        | Medium           | 55       | 3067         | Arc-Royal DC (3067+)                                                                |
	| Wight                             | Light            | 35       | 3068         | Draconis Combine, Free Worlds League, Lyran Commonwealth                            |
	
	² Wolf's Dragoons and mercenaries have access to many of these 'Mechs.
	</details>

- 10 Hero 'Mechs are available in the Heavy Metal crate. You can also add any of them to your game via Fell Off A Cargo Ship or a save editor, including the Exterminator 'Caine' 4DX And Marauder II 'Bounty Hunter', which can't be obtained from the HM crate.

	<details>
	  <summary>Hero Mechs</summary>
	
	| Name                            | Model Code | Intro | Pilot                                 |
	| :------------------------------ | :--------: | :---: | :------------------------------------ |
	| Assassin 'Servitor'             | ASN-SRV    | 3066  | None (custom variant of the ASN-99)   |
	| BattleMaster 'Red Corsair'      | BLR-RC     | 3055  | Nekane 'Red Corsair' Hazen            |
	| Black Knight 'Red Reaper'       | BL-X-KNT   | 3069  | Reginald VanJaster                    |
	| Centurion 'Yen-Lo-Wang'         | CN9-YLW    | 3027  | Justin Xiang Allard                   |
	| Centurion 'Yen-Lo-Wang 2'       | CN9-YLW2   | 3051  | Kai Allard-Liao                       |
	| Charger 'Number Seven'          | CGR-N7     | 3025  | Terry Ford                            |
	| Exterminator 'Caine'            | EXT-4DX    | 2754  | Caine Barclay                         |
	| Hatamoto-Chi 'Shin'             | HTM-S      | 3060  | Shin Yodama (?)                       |
	| Marauder II 'Bounty Hunter'     | MAD-BHIII  | 3064  | Vic Travers                           |
	| Hybrid Rifleman 'Sneede'        | RFL-SND    | 3025  | Samual 'Shorty' Sneede                |
	| Mackie 'Kill Roy's Little Buddy | MSK-9HKR   | 2750  | None (mascot of the Martial Olympiad) |
	| Schwerer Gustav 'Jäger'         | SJ-1X      | 3073  | None (unofficial custom variant)      |
	| Vulture (Mad Dog) 'Fury'        | VUL-FURY   | 3059  | Katherine Furey (non-canon variant)   |
	</details>

#### Vehicles
- Over 200 new vehicles variants, including VTOLs and superheavy tanks.
- All vehicles have had their loadouts, armor values, and descriptions updated.

#### Weapons
- New weapons and upgrades become available in stores over time. Some weapons and upgrades are unique to certain 'Mech chassis, such as Stealth Armor.

	<details>
	  <summary>New Weapons/Upgrades (by Type/Intro)</summary>
	
	| Name                              |   Type    | Intro | Factions                  |
	| :-------------------------------- | :-------: | :---: | :------------------------ |
	| Light/Medium/Heavy Rifle          | Ballistic |  PS   | *LosTech*                 |
	| Mech Mortars                      | Ballistic |  PS   | *LosTech*                 |
	| Thumper/Sniper/Long Tom Cannon    | Ballistic | 3012  | *Research*                |
	| Hyper-Velocity AC (HVAC)          | Ballistic | 3059  | Liao                      |
	| Rotary AC (RAC)                   | Ballistic | 3060  | Davion                    |
	| Light AC (LAC)                    | Ballistic | 3062  | Davion                    |
	| Improved Heavy Gauss Rifle (iHGR) | Ballistic | 3065  | Steiner                   |
	|  ----                             |           |       |                           |
	| Rail Gun                          |  Energy   | 3051  | Marik                     |
	| PPC-X                             |  Energy   | 3058  | *Solaris*<br>Liao (3067+) |
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
	| Magshot                           |  Support  | 3059  | Steiner                   |
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

### Timeline Expansion
 _**Note:** The timeline will be fully extended to 3067 with the next update (v1.0)._

#### Faction Stores
- All the faction stores (accessible when allied to a faction) have been updated with the new 'Mechs and vehicles. **NEW**
- 30+ factories have been added throughout the map, including specialized vehicle factories. **NEW**

#### Faction Rosters
- MegaMek-generated rarity tables replace Xtol's RAT tables for vehicles. This allows for a wider variety of combat vehicles for all factions, as well as more accurate vehicle rosters for the expanded 3025-3067 timeline. The Great Houses and Periphery States also have an "elite" version of their vehicle table that favors A-rated units.
- Additionally, some factions that didn't have a RAT table before have one now, including:

	<details>
	  <summary>New Rarity Tables</summary>
	
	| Faction/Unit                         |       Years Active        | Unit Rating                             |
	| :----------------------------------- | :-----------------------: | :-------------------------------------- |
	| Arc-Royal Defense Cordon             |        3058-3067          | C/Regulars                              |
	| Chaos March                          |        3057-3073          | F/Locals                                |
	| Clan Wolf-in-Exile                   |        3057-3151          | B/Front Lines                           |
	| Duchy of Andurien                    | 3030-3040<br>3079-Present | C/Regulars                              |
	| New Colony Region /<br>Fronc Reaches | 3060-3066<br>3067-Present | C/Regulars                              |
	|  ----                                |                           |                                         |
	| Eridani Light Horse                  |        2702-Present       | B/Veterans                              |
	| Gray Death Legion                    |        3024-3065          | A/Elites                                |
	| Kell Hounds                          |        3010-Present       | B/Veterans<br>A/Elites (3040+)          |
	| Northwind Highlanders                |        ????-3081          | B/Veterans<br>A/Elites (3059+)          |
	| Other Mercenaries                    |           n/a             | C/Regulars<br>F/Locals (Periphery)      |
	| Pirates                              |           n/a             | F/Locals                                |
	| Security Forces²                     |           n/a             | F-/Locals                               |
	| Wolf's Dragoons                      |        3005-Present       | A/Elites                                |
	
	𓅪 The Outworlds Alliance have access to Snow Raven garrison units after 3061.<br>
	² Locals sometimes use security forces instead of their own units.
	</details>

### Gameplay Changes

#### Self-Knockdown and Artillery
- Gauss rifles and artillery have been somewhat nerfed, using the new tools available with CAC-C; both have a self-knockdown effect when firing equal to 25% of their actual damage, rounded down (from 10 instability for Light Gauss, to 50 for Rail Guns). Heavy Gauss Rifles also do more damage, but it decreases with range and is halved at maximum range.

- Artillery pieces use a two-turn system, with one turn for aiming and another for firing. Their accuracy has been increased, as misses are more likely to land closer to their intended target. **NEW**

- Mech mortars and artillery cannons have been added. The first are available everywhere whereas the second are only available on research planets and serve as a more compact and quick-firing alternative to full-sized artillery pieces.

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

- All ballistic artillery requires an Artillery Loader addon. This addon allows the massive weapons to be mounted on a 'Mech. Only the Long Tom is too large to fit on anything other than the Mobile Long Tom Artillery vehicle.

_**Note**: Full-sized artillery deal 33-66% more damage than artillery cannons or CAC-C versions._

#### Playable Vehicles
- Vehicles can be salvaged or bought in stores, just like 'Mechs.
- The Lifepath system has been adjusted to better reflect BattleTech lore:
  - There are fewer Mech-capable pilots, especially in poorer systems and in early game.
  - Some pilots start out as conventional only, but get the ability to pilot 'Mechs later. 
- Vehicles have received several bonuses to make them more comparable to 'Mechs:
  - Vehicles no longer have a -25% armor penalty and instead have the same armor as in tabletop.
  - Light vehicles gain a +2 To Hit modifier (Hit Defense), while medium vehicles get a +1.
  - Additionally, VTOLs have +2 Evasion and +3 Hit Defense, for a total of -10% to -12.5% chance of hitting them. Hover tanks receive a lesser bonus of +1 Evasion and +1 Hit Defense.
  - Support vehicles now have Comms Equipment that provides benefits based on the amount of equipment they carry, ranging from +1 Lance Initiative and +1 Resolve Gain with one ton to +2 Lance Initiative and +7 Resolve Gain with seven tons. Comms Equipment also has a 300m ECCM aura that increases in effectiveness, just like the other bonuses.

_**Note**: Most of these features were already present in the Playable Tanks mod, which is now integrated into this mod._

#### Additional Lances
- New support lances can appear during missions with the Additional Lances setting from Mission Control enabled, such as:
  - Vehicle-only and VTOL lances, mainly on easier missions (2½ skulls or less).
  - Artillery lances with three artillery vehicles and an APC on harder missions (4 skulls or more).
- All Clans can deploy vehicle-only support lances, except for Clan Jade Falcon. In contrast, Clan Hell's Horses only sends vehicles for support.
- Capellan Confederation elite units can use augmented lance formations of four 'Mechs or vehicles supported by two other units.
- Free Worlds League and Taurian Concordat elite units can use augmented armor formations of six vehicles.

### Other Changes
- Shell Shuffler randomizes the type of ammunition other units have when spawning. The mod has two presets, depending on the era you are playing in:
  - **3025 preset:** Any faction can use Inferno SRM.
  - **3050 preset:** Each faction has its own set of special ammo types, most of which were developed in the 3050s.

	<details>
	  <summary>Shell Shuffler (3050 Preset)</summary>
	
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

- Search Denial missions may spawn mixed ComStar lances with a chance to include royal tanks.
- Allied convoys in Escort Convoy missions are directly controlled by the player. **NEW**
- Avatars fielded by elite Kuritan units carry a Kuritan banner (sashimono) in battle. **NEW**
- The UI in combat only shows the ammunition type when a weapon has multiple types of ammo.
- ATMs may fire indirectly and have a +2 accuracy bonus when firing in direct fire. **NEW**
- Heavy Lasers have distinct colors ranging from orange-red to golden.
- Some minor issues with some 'Mechs in the current version of BEX have been adressed:

	<details>
	  <summary>Misc. Fixes</summary>
	
	| Name                      | Changes                                                                                |
	| :------------------------ | :------------------------------------------------------------------------------------- |
	| Annihilator               | Reduced movement to 3/5 hexes (from 4/7)                                               |
	| Atlas II AS7-D-HT         | Changed armor placement and moved one DHS to the engine                                |
	| Catapult CPLT-K5          | Added missing DHS in the engine                                                        |
	| Champion CHP-1N2          | Added missing ferro-fibrous armor                                                      |
	| Crab 27b "Royal Crab"     | Fixed melee damage (65 dmg. like other Crabs)                                          |
	| Cyclops 11-series         | Added extra hardpoints (different from the 10-series)                                  |
	| Firestarter FS9-OF        | Added Large Engine quirk                                                               |
	| Flea FLE-14               | Fixed max armor values for an ultralight                                               |
	| Garm                      | Reduced overall size to better match tonnage                                           |
	| Grand Dragon DRG-1G       | Fixed energy hadpoints placement                                                       |
	| Hatamoto 27T & 27V        | Added missing CASE on side torsos                                                      |
	| Hermes II                 | Reduced overall size to better match tonnage                                           |
	| Hornet HNT-161            | Fixed armor placement                                                                  |
	| Kodiak 5                  | Added missing DHS in the engine                                                        |
	| Loader King LDK-5C        | Added more armor and missing Cargo Bay                                                 |
	| Locust LCT-3S             | Fixed available tonnage                                                                |
	| Ost Mechs                 | Added cross assembly between Ostroc and Ostsol                                         |
	| Piranha 1                 | Changed to single heat sinks                                                           |
	| Shadow Hawk               | Added Rugged quirk                                                                     |
	| Shadow Hawk SHD-3H        | Changed loadout to the official one                                                    |
	| Thunder THR-1L            | Added missing DHS in the engine                                                        |
	| Trebuchet TBT-3C          | Added Large Engine quirk                                                               |
	| UrbanMech                 | Reduced movement to 3/5 hexes (from 4/7)<br>Changed model to support more weapon types |
	</details>
