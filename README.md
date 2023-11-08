# BTX Mech Pack

A BattleTech Extended mod that adds over 190 new 'Mech variants and over 20 new weapons that were prototyped in the 3050s and early 3060s. All 'Mechs are ported from scratch using MegaMekLab. Requires CAC-C.

## Installation / Usage

**TL;DR: BEX + Bigger Drops + CAB → CAC-C → (TBD) → Mech Pack**

- Install the latest versions of [CAC-C](https://github.com/mcb5637/BTX_CAC_Compatibility) and, optionally, [The Big Deal Add-On](https://github.com/Hounfor/The-Big-Deal-Add-On) for BEX + CAC-C
- Update the CAB, as the mod uses the most recent models
- Extract BTX_MechPack archive to the mods folder, overwriting when prompted
- (Optional) Enable DHS Changes, which gives all DHS in the engine 6 heatsinking, by setting "Enabled" to true in the mod.json file of the mod.
- (Optional) Enable Shell Shuffler, which makes the AI use different ammo types at random, by removing the date part from one the two mod.json files of the mod.

New 'Mechs are fielded by the factions that should have them according to the lore. Hero 'Mechs are available in the Heavy Metal crate. Alternatively, you can add the new 'Mechs to your game using Fell Off A Cargo Ship or a save editor.

New rare weapons and upgrades are available in stores, based on the year they were prototyped. Some weapons and upgrades are unique to certain 'Mech chassis, such as Stealth Armor.

## Mod Progress

The mod currently includes 62 new BattleMechs and 198 new 'Mech variants. When completed, the mod will feature over 70 new BattleMechs, corresponding to nearly all CAB models that fit the BattleTech Extended timeline and beyond, up to the year 3065.

The next major update is set to be released sometime this fall, and will feature new end-game BattleMechs and add some of the new 'Mechs to the factory shops. 

| Pack #1             | Pack #2          | Pack #3                | Pack #4       |
| :------------------ | :--------------- | :--------------------- | :------------ |
| Bellerophon         | Arctic Wolf      | Albatross              | Akuma         |
| Bombard             | Assassin II      | Anubis                 | ?????         |
| Buccaneer           | Battle Hawk      | Barghest               | ????? ???     |
| Dragoon             | Blitzkrieg       | Black Heart            | Buster        |
| Fox                 | Champion LAM     | Fire Falcon            | Chimera       |
| Hellfire            | Cossack          | Galahad (Glass Spider) | Crosscut      |
| Jackrabbit          | Hammer           | Grand Crusader         | Dig King/Lord |
| JagerMech III       | Hybrid Rifleman  | Gulon                  | ??????        |
| Juggernaut          | Lao Hu           | Iron Cheetah           | ?????????     |
| Mercury II / Coyotl | Mad Cat Mk II    | Lineholder             | Kiso          |
| Pulverizer          | Matar            | Lupus                  | Komodo        |
| Rampage             | Naga             | Mantis                 | ??????        |
| Schwerer Gustav     | Nightsky         | Osiris                 | ?????         |
| Screamer LAM        | Phoenix Hawk IIC | Peregrine              | ?????????     |
| Stag / Stag II      | Rattlesnake      | Phantom                | Roughneck     |
| Thanatos            | Warthog          | Prometheus             | ??????        |
| Titan               |                  | Sha Yu                 | ??????????    |
|                     |                  | Templar                | ????????      |
|                     |                  | Zeus-X                 | ??????????    |
|                     |                  |                        | ?????????     |

## Contents

### Mechs

<details>
  <summary>New Chassis (by Name)</summary>

| Name                               |   Class    | Mass |  Tech Base   | Intro | Factions                                               |
| :--------------------------------- | :--------: | :--: | :----------: | :---: | :----------------------------------------------------- |
| Akuma AKU-1X                       |  Assault   |  90  | Inner Sphere | 3058  | Kurita                                                 |
| Akuma AKU-1XJ                      |  Assault   |  90  | Inner Sphere | 3064  | Kurita                                                 |
| Albatross ALB-3U                   |  Assault   |  95  | Inner Sphere | 3053  | Marik, Word of Blake                                   |
| Albatross ALB-4U                   |  Assault   |  95  | Inner Sphere | 3063  | Marik, Word of Blake                                   |
| Anubis ABS-3L                      |   Light    |  30  | Inner Sphere | 3063  | Liao, Centrella, Calderon                              |
| Anubis ABS-3R                      |   Light    |  30  | Inner Sphere | 3064  | Liao, Centrella, Calderon                              |
| Arctic Wolf 1                      |   Medium   |  40  |     Clan     | 3059  | Clan Wolf                                              |
| Arctic Wolf 2                      |   Medium   |  40  |     Clan     | 3060  | Clan Wolf                                              |
| Assassin II ASN-56                 |   Medium   |  45  | Inner Sphere | 3060  | Davion                                                 |
| Barghest BGS-1T                    |   Heavy    |  70  | Inner Sphere | 3058  | Steiner                                                |
| Barghest BGS-2T                    |   Heavy    |  70  | Inner Sphere | 3060  | Steiner                                                |
| Barghest BGS-3T                    |   Heavy    |  70  | Inner Sphere | 3062  | Steiner                                                |
| Battle Hawk BH-K305                |   Light    |  30  | Inner Sphere | 3053  | Steiner-Davion, Mercenaries                            |
| Bellerophon BEL-1X                 |   Heavy    |  60  | Inner Sphere | 2442  | Marik                                                  |
| Bellerophon BEL-2X                 |   Heavy    |  60  | Inner Sphere | 2712  | ComStar, Snord's Irregulars                            |
| Black Heart BH-1                   |   Heavy    |  70  | Inner Sphere | 3060  | Word of Blake                                          |
| Blitzkrieg BTZ-3F                  |   Medium   |  50  | Inner Sphere | 3061  | Marik, Steiner-Davion                                  |
| Bombard BMB-010                    |   Medium   |  50  | Inner Sphere | 3054  | Steiner                                                |
| Bombard BMB-013                    |   Medium   |  50  | Inner Sphere | 3063  | Steiner                                                |
| Buccaneer BCN-3R                   |   Medium   |  55  | Inner Sphere | 3055  | Marik, Word of Blake                                   |
| Buster BC X-M ConstructionMech MOD |  Medium    |  50  | Inner Sphere | 2720  | **Industrial Start**                                   |
| Champion LAM CPN-1X1               |   Heavy    |  60  | Inner Sphere | 2699  | Word of Blake (3053+)                                  |
| Chimera CMA-1S                     |   Medium   |  40  | Inner Sphere | 3063  | Kurita, Steiner-Davion                                 |
| Chimera CMA-C                      |   Medium   |  40  | Inner Sphere | 3063  | Kurita, Marik, Steiner-Davion                          |
| Cossack C-SK1                      |   Light    |  20  | Inner Sphere | 3060  | St. Ives Compact                                       |
| Coyotl Prime                       |   Medium   |  40  |     Clan     | 2854  | Clan Wolf (<3058)                                      |
| Coyotl A                           |   Medium   |  40  |     Clan     | 2854  | Clan Wolf (<3058)                                      |
| Coyotl B                           |   Medium   |  40  |     Clan     | 2854  | Clan Wolf (<3058)                                      |
| Dig King RCL-1M MiningMech         |   Light    |  35  | Inner Sphere | 2802  | Pirates                                                |
| Dig Lord RCL-4 MiningMech          |   Heavy    |  65  | Inner Sphere | 3057  | Steiner-Davion                                         |
| Dragoon AEM-01                     |   Heavy    |  70  | Inner Sphere | 2771  | ComStar                                                |
| Dragoon AEM-02                     |   Heavy    |  70  | Inner Sphere | 2771  | ComStar                                                |
| Dragoon AEM-03                     |   Heavy    |  70  | Inner Sphere | 2771  | ComStar                                                |
| Dragoon AEM-04                     |   Heavy    |  70  | Inner Sphere | 2771  | ComStar                                                |
| Fire Falcon Prime                  |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Fire Falcon A                      |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Fire Falcon B                      |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Fire Falcon C                      |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Fire Falcon D                      |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Fox CS-1                           |   Medium   |  50  |  Mixed-tech  | 2824  | Clan Ghost Bear                                        |
| Fox                                |   Medium   |  50  |     Clan     | 2835  | Clan Ghost Bear                                        |
| Grand Crusader GRN-D-01            |  Assault   |  80  | Inner Sphere | 3053  | Word of Blake                                          |
| Grand Crusader GRN-D-02            |  Assault   |  80  | Inner Sphere | 3056  | Word of Blake                                          |
| Gulon MiningMech GLN-1A            |   Light    |  25  | Inner Sphere | 3000  | Outworlds Alliance                                     |
| Gulon SecurityMech GLN-1B          |   Light    |  25  | Inner Sphere | 3000  | Outworlds Alliance                                     |
| Hammer HMR-3C 'Claw-Hammer'        |   Light    |  30  | Inner Sphere | 3056  | Marik, Word of Blake                                   |
| Hammer HMR-3M                      |   Light    |  30  | Inner Sphere | 3053  | Liao, Marik, Word of Blake                             |
| Hammer HMR-3P 'Pein-Hammer'        |   Light    |  30  | Inner Sphere | 3060  | Marik, Word of Blake                                   |
| Hammer HMR-3S 'Slammer'            |   Light    |  30  | Inner Sphere | 3054  | Marik, Word of Blake                                   |
| Hellfire 1                         |   Heavy    |  60  |     Clan     | 3058  | Clan Steel Viper                                       |
| Hybrid Rifleman RFL-SND 'Sneede'   |   Heavy    |  60  | Inner Sphere | 3025  | **Heavy Metal Crate**                                  |
| Iron Cheetah Prime                 |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Iron Cheetah A                     |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Iron Cheetah B                     |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Iron Cheetah C                     |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Iron Cheetah D                     |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Jackrabbit JKR-8T                  |   Light    |  25  | Inner Sphere | 2765  | ComStar                                                |
| JagerMech III JM6-D3               |   Heavy    |  65  | Inner Sphere | 3058  | Davion                                                 |
| Juggernaut JG-R9T1                 |  Assault   |  90  | Inner Sphere | 3053  | Steiner                                                |
| Juggernaut JG-R9T2                 |  Assault   |  90  | Inner Sphere | 3057  | Steiner                                                |
| Juggernaut JG-R9T3                 |  Assault   |  90  | Inner Sphere | 3065  | Steiner                                                |
| Kiso ConstructionMech K-3N-KR4     |  Assault   | 100  | Inner Sphere | 2703  | Kurita                                                 |
| Kiso CommandMech K-3N-KRHQ         |  Assault   | 100  | Inner Sphere | 2823  | Kurita                                                 |
| Komodo KIM-2                       |   Medium   |  45  | Inner Sphere | 3053  | Kurita, Rasalhague                                     |
| Komodo KIM-2A                      |   Medium   |  45  | Inner Sphere | 3053  | Kurita                                                 |
| Komodo KIM-3C                      |   Medium   |  45  | Inner Sphere | 3053  | Kurita                                                 |
| Lao Hu LHU-2B                      |   Heavy    |  75  | Inner Sphere | 3062  | Liao                                                   |
| Lao Hu LHU-3B                      |   Heavy    |  75  | Inner Sphere | 3063  | Liao                                                   |
| Lineholder KW1-LH2                 |   Medium   |  55  | Inner Sphere | 3058  | Inner Sphere                                           |
| Lineholder KW1-LH3                 |   Medium   |  55  | Inner Sphere | 3059  | Inner Sphere                                           |
| Lupus Prime                        |   Heavy    |  60  |     Clan     | 2857  | Clan Steel Viper                                       |
| Lupus A                            |   Heavy    |  60  |     Clan     | 2857  | Clan Steel Viper                                       |
| Lupus B                            |   Heavy    |  60  |     Clan     | 2857  | Clan Steel Viper                                       |
| Mad Cat Mk II                      |  Assault   |  90  |     Clan     | 3062  | Clan Diamond Shark                                     |
| Mantis MTS-S                       |   Light    |  30  | Inner Sphere | 3061  | Steiner                                                |
| Matar SAM-RS2                      | Superheavy | 110  | Inner Sphere | 2775  | ComStar (3036+)                                        |
| Mercury II MCY-100                 |   Medium   |  40  |  Mixed-tech  | 2823  | Clan Diamond Shark, Bandit Caste                       |
| Naga Prime                         |  Assault   |  80  |     Clan     | 2945  | Clans                                                  |
| Naga A                             |  Assault   |  80  |     Clan     | 2869  | Clans                                                  |
| Naga B                             |  Assault   |  80  |     Clan     | 2869  | Clans                                                  |
| Naga C                             |  Assault   |  80  |     Clan     | 2869  | Clans                                                  |
| Naga D                             |  Assault   |  80  |     Clan     | 2869  | Clans                                                  |
| Nightsky NGS-4S                    |   Medium   |  50  | Inner Sphere | 3053  | Steiner-Davion                                         |
| Nightsky NGS-4T                    |   Medium   |  50  | Inner Sphere | 3056  | Steiner-Davion                                         |
| Nightsky NGS-5S                    |   Medium   |  50  | Inner Sphere | 3056  | Steiner-Davion                                         |
| Nightsky NGS-5T                    |   Medium   |  50  | Inner Sphere | 3057  | Steiner-Davion                                         |
| Osiris OSR-3D                      |   Light    |  30  | Inner Sphere | 3063  | Davion                                                 |
| Peregrine (Horned Owl) 1           |   Light    |  35  |     Clan     | 2835  | Clans                                                  |
| Peregrine (Horned Owl) 2           |   Light    |  35  |     Clan     | 2856  | Clans                                                  |
| Peregrine (Horned Owl) 3           |   Light    |  35  |     Clan     | 3061  | Clans                                                  |
| Phantom Prime                      |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phantom A                          |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phantom B                          |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phantom C                          |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phantom D                          |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phoenix Hawk IIC                   |  Assault   |  80  |     Clan     | 2851  | Clans                                                  |
| Phoenix Hawk IIC 2                 |  Assault   |  80  |     Clan     | 2852  | Clans                                                  |
| Phoenix Hawk IIC 3                 |  Assault   |  80  |     Clan     | 3062  | Clans                                                  |
| Phoenix Hawk IIC 9                 |  Assault   |  80  |     Clan     | 2853  | Clans                                                  |
| Prometheus                         |   Heavy    |  75  |  Mixed-tech  | 3053  | Davion                                                 |
| Pulverizer PUL-2V                  |  Assault   |  90  |  Mixed-tech  | 2823  | Clan Ghost Bear                                        |
| Pulverizer PUL-3R                  |  Assault   |  90  |  Mixed-tech  | 2823  | Clan Ghost Bear                                        |
| Pulverizer                         |  Assault   |  90  |     Clan     | 2845  | Clan Ghost Bear                                        |
| Rampage RMP-2G                     |  Assault   |  85  | Inner Sphere | 2735  | Periphery States                                       |
| Rampage RMP-4G                     |  Assault   |  85  | Inner Sphere | 2750  | ComStar/Word of Blake                                  |
| Rampage RMP-5G                     |  Assault   |  85  | Inner Sphere | 2767  | ComStar/Word of Blake                                  |
| Rattlesnake JR7-31                 |   Light    |  35  | Inner Sphere | 3042  | Davion                                                 |
| Rattlesnake JR7-31P                |   Light    |  35  | Inner Sphere | 3043  | Davion                                                 |
| Roughneck RGH-1A                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Roughneck RGH-1B                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Roughneck RGH-1C                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Roughneck RGH-2A                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Roughneck RGH-3A                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Schwerer Gustav SG-1X              |  Assault   | 100  |  Mixed-tech  | 3064  | Marik                                                  |
| Schwerer Gustav SJ-1X 'Jäger'      |  Assault   | 100  |  Mixed-tech  | 3064  | **Heavy Metal Crate**                                  |
| Screamer LAM SCR-1X-LAM            |   Medium   |  55  | Inner Sphere | 2774  | Snord's Irregulars                                     |
| Sha Yu SYU-2B                      |   Medium   |  40  | Inner Sphere | 3063  | Liao, Centrella                                        |
| Stag ST-14G                        |   Medium   |  45  |  Mixed-tech  | 2823  | Clans (3052+)                                          |
| Stag II ST-24G                     |   Medium   |  45  |  Mixed-tech  | 2823  | Clan Wolf (3052+)                                      |
| Templar TLR1-O                     |  Assault   |  85  | Inner Sphere | 3062  | Davion                                                 |
| Templar TLR1-OA                    |  Assault   |  85  | Inner Sphere | 3062  | Davion                                                 |
| Templar TLR1-OB                    |  Assault   |  85  | Inner Sphere | 3062  | Davion                                                 |
| Templar TLR1-OC                    |  Assault   |  85  | Inner Sphere | 3062  | Davion                                                 |
| Thanatos TNS-4S                    |   Heavy    |  75  | Inner Sphere | 3061  | Steiner-Davion                                         |
| Thanatos TNS-4T                    |   Heavy    |  75  | Inner Sphere | 3062  | Steiner-Davion                                         |
| Titan TI-1A                        |  Assault   | 100  | Inner Sphere | 2765  | Davion                                                 |
| Warthog Prime                      |  Assault   |  95  |     Clan     | 3059  | Clans                                                  |
| Zeus-X ZEU-X                       |  Assault   |  80  | Inner Sphere | 3054  | Davion                                                 |
</details>

<details>
  <summary>Existing Chassis (by Name)</summary>

| Name                               |   Class    | Mass |  Tech Base   | Intro | Factions                                               |
| :--------------------------------- | :--------: | :--: | :----------: | :---: | :----------------------------------------------------- |
| Annihilator C                      |  Assault   | 100  |     Clan     | 2848  | Clans (3051+)                                          |
| Annihilator C 2                    |  Assault   | 100  |     Clan     | 2850  | Clans (3051+)                                          |
| Anvil ANV-8M                       |   Heavy    |  60  | Inner Sphere | 3060  | Marik, Word of Blake                                   |
| Archer C                           |   Heavy    |  70  |  Mixed-tech  | 2824  | Clans (3051+)<br />Kurita, Steiner-Davion (3055+)      |
| Archer C 2                         |   Heavy    |  70  |     Clan     | 3063  | Wolf's Dragoons²                                       |
| Assassin ASN-SRV 'Servitor'        |   Medium   |  40  | Inner Sphere | 3066  | **Heavy Metal Crate**                                  |
| Atlas AS7-K-DC                     |  Assault   | 100  | Inner Sphere | 3050  | ComStar/Word of Blake                                  |
| Avatar AV1-OR                      |   Heavy    |  70  |  Mixed-tech  | 3059  | Kurita                                                 |
| BattleMaster BLR-3M-DC             |  Assault   |  85  | Inner Sphere | 3053  | ComStar/Word of Blake                                  |
| BattleMaster BLR-RC 'Red Corsair'  |  Assault   |  85  |     Clan     | 3055  | **Heavy Metal Crate**                                  |
| Black Hawk-KU BHKU-OR              |   Heavy    |  60  |  Mixed-tech  | 3059  | Kurita, Liao, Steiner-Davion<br />Rasalhague, St. Ives |
| Black Knight BL-X-KNT 'Red Reaper' |   Heavy    |  75  | Inner Sphere | 3069  | **Heavy Metal Crate**                                  |
| Blackjack BJ2-OR                   |   Medium   |  50  |  Mixed-tech  | 3059  | Kurita                                                 |
| Cataphract CTF-3X                  |   Heavy    |  70  | Inner Sphere | 3062  | Davion                                                 |
| Catapult CPLT-C3                   |   Heavy    |  65  | Inner Sphere | 3049  | Liao, ComStar/Word of Blake                            |
| Catapult CPLT-C5                   |   Heavy    |  65  | Inner Sphere | 3061  | Liao                                                   |
| Catapult CPLT-H2                   |   Heavy    |  65  | Inner Sphere | 3064  | Pirates                                                |
| Centurion CN9-D5                   |   Medium   |  50  | Inner Sphere | 3062  | Steiner-Davion                                         |
| Centurion CN9-YLW 'Yen-Lo-Wang'    |   Medium   |  50  | Inner Sphere | 3027  | **Heavy Metal Crate**                                  |
| Centurion CN9-YLW2 'Yen-Lo-Wang'   |   Medium   |  50  | Inner Sphere | 3051  | **Heavy Metal Crate**                                  |
| Charger CGR-2A2                    |  Assault   |  80  | Inner Sphere | 3064  | Centrella, Outworlds Alliance, Pirates                 |
| Charger CGR-SA5                    |  Assault   |  80  | Inner Sphere | 3063  | Kurita                                                 |
| Commando COM-4H                    |   Light    |  25  | Inner Sphere | 3064  | Pirates                                                |
| Cronus CNS-5M                      |  Medium    |  55  | Inner Sphere | 3060  | Mercenaries, Pirates                                   |
| Crosscut ED-X2M LoggerMech         |   Light    |  30  | Inner Sphere | 2801  | Pirates                                                |
| Crosscut ED-X4D DemolitionMech     |   Light    |  30  | Inner Sphere | 2910  | Pirates                                                |
| Crosscut ED-X4K LoggerMech         |   Light    |  30  | Inner Sphere | 2786  | Pirates                                                |
| Cyclops CP-11-A-DC                 |  Assault   |  90  | Inner Sphere | 3045  | ComStar                                                |
| Cyclops CP-11-H                    |  Assault   |  90  | Inner Sphere | 3064  | Calderon, Pirates                                      |
| Dasher (Fire Moth) E               |   Light    |  20  |     Clan     | 3055  | Clan Ghost Bear                                        |
| Dervish DV-8D                      |  Medium    |  55  | Inner Sphere | 3062  | Davion                                                 |
| Firestarter FS9-OR                 |   Medium   |  45  |  Mixed-tech  | 3059  | Kurita, Liao, Marik, Steiner-Davion                    |
| Firestarter FS9-OX                 |   Medium   |  45  | Inner Sphere | 3059  | Kurita                                                 |
| Flashman FLS-9C                    |   Heavy    |  75  | Inner Sphere | 3061  | ComStar                                                |
| Galahad (Glass Spider) 1           |   Heavy    |  60  |     Clan     | 2834  | Clans                                                  |
| Galahad (Glass Spider) 2           |   Heavy    |  60  |     Clan     | 2952  | Clan Wolf                                              |
| Garm GRM-01C                       |   Light    |  35  | Inner Sphere | 3062  | Davion                                                 |
| Grand Dragon DRG-7K                |   Heavy    |  60  | Inner Sphere | 3063  | Kurita                                                 |
| Gunslinger GUN-2ERD                |  Assault   |  85  | Inner Sphere | 3062  | Kurita, Steiner                                        |
| Hankyu (Arctic Cheetah) H          |   Light    |  30  |     Clan     | 3062  | Clans                                                  |
| Hatamoto-Chi HTM-S 'Shin'          |  Assault   |  80  | Inner Sphere | 3060  | **Heavy Metal Crate**                                  |
| Hatchetman HCT-6D                  |   Medium   |  45  | Inner Sphere | 3062  | Davion                                                 |
| Hellhound (Conjurer) 2             |   Medium   |  50  |     Clan     | 3062  | Clan Nova Cat                                          |
| Hermes II HER-5C                   |   Medium   |  40  | Inner Sphere | 3062  | Word of Blake                                          |
| Hermes II HER-6D                   |   Medium   |  40  | Inner Sphere | 3062  | Davion                                                 |
| Highlander HGN-694                 |  Assault   |  90  | Inner Sphere | 3062  | Steiner                                                |
| Hollander II BZK-F7                |  Medium    |  45  | Inner Sphere | 3061  | Steiner-Davion                                         |
| Hunchback HBK-5H                   |  Medium    |  50  | Inner Sphere | 3064  | Periphery States, Pirates                              |
| Huron Warrior HUR-WO-R4N           |  Medium    |  50  | Inner Sphere | 3063  | Liao                                                   |
| Imp C                              |  Assault   | 100  |     Clan     | 2863  | Wolf's Dragoons, Pirates                               |
| JagerMech JM6-H                    |   Heavy    |  65  | Inner Sphere | 3064  | Pirates                                                |
| JagerMech JM7-F                    |   Heavy    |  70  | Inner Sphere | 3062  | Davion                                                 |
| King Crab KGC-010                  |  Assault   | 100  | Inner Sphere | 2743  | ComStar/Word of Blake                                  |
| Longbow LGB-0H                     |  Assault   |  85  | Inner Sphere | 3065  | Pirates                                                |
| Marauder C                         |   Heavy    |  75  |  Mixed-tech  | 2827  | Clans (3051+)<br />Kurita, Steiner-Davion (3055+)      |
| Orion ON1-M-DC                     |   Heavy    |  75  | Inner Sphere | 3053  | ComStar/Word of Blake                                  |
| Orion ON1-MD                       |   Heavy    |  75  | Inner Sphere | 3062  | Davion, Marik, ComStar/Word of Blake                   |
| Raptor RTX1-OR                     |   Light    |  25  |  Mixed-tech  | 3059  | Kurita, Davion, ComStar                                |
| Strider SR1-OR                     |   Medium   |  40  |  Mixed-tech  | 3059  | Kurita, Marik, Steiner-Davion                          |
| Sunder SD1-OB                      |  Assault   |  90  | Inner Sphere | 3056  | Kurita, Davion, St. Ives                               |
| Sunder SD1-OR                      |  Assault   |  90  |  Mixed-tech  | 3059  | Kurita, Steiner-Davion                                 |
| Supernova 2                        |  Assault   |  90  |     Clan     | 3062  | Clan Nova Cat                                          |
| Supernova 3                        |  Assault   |  90  |     Clan     | 3064  | Clan Nova Cat                                          |
| Thunder Hawk TDK-7KMA              |  Assault   | 100  | Inner Sphere | 3059  | Steiner                                                |
| Thunderbolt TDR-8M                 |   Heavy    |  65  | Inner Sphere | 3058  | Centrella, Word of Blake                               |
| Viking VKG-2G                      |  Assault   |  90  | Inner Sphere | 3060  | Rasalhague, ComStar/Word of Blake                      |
| Vulture (Mad Dog) 'Fury'           |   Heavy    |  60  |     Clan     | 3059  | **Heavy Metal Crate**                                  |
| Warhammer C                        |   Heavy    |  70  |  Mixed-tech  | 2825  | Clans (3051+)<br />Kurita, Steiner-Davion (3055+)      |
| Warhammer C 2                      |   Heavy    |  70  |  Mixed-tech  | 3052  | Clan Wolf<br />Kurita, Steiner-Davion (3055+)          |
| Warhammer C 3                      |   Heavy    |  70  |     Clan     | 2862  | Wolf's Dragoons (3050+)²                               |

² Wolf's Dragoons have access to many of these mechs; the list only shows variants that are exclusive to them.
</details>

<details>
  <summary>All Chassis (by Availability)</summary>

| Name                               |   Class    | Mass |  Tech Base   | Avail.| Factions                                               |
| :--------------------------------- | :--------: | :--: | :----------: | :---: | :----------------------------------------------------- |
| Bellerophon BEL-1X                 |   Heavy    |  60  | Inner Sphere | 3025  | Marik                                                  |
| Kiso ConstructionMech K-3N-KR4     |  Assault   | 100  | Inner Sphere | 3025  | Kurita                                                 |
| Bellerophon BEL-2X                 |   Heavy    |  60  | Inner Sphere | 3025  | ComStar, Snord's Irregulars                            |
| Rampage RMP-2G                     |  Assault   |  85  | Inner Sphere | 3025  | Periphery States                                       |
| King Crab KGC-010                  |  Assault   | 100  | Inner Sphere | 3025  | ComStar/Word of Blake                                  |
| Rampage RMP-4G                     |  Assault   |  85  | Inner Sphere | 3025  | ComStar/Word of Blake                                  |
| Jackrabbit JKR-8T                  |   Light    |  25  | Inner Sphere | 3025  | ComStar                                                |
| Titan TI-1A                        |  Assault   | 100  | Inner Sphere | 3025  | Davion                                                 |
| Rampage RMP-5G                     |  Assault   |  85  | Inner Sphere | 3025  | ComStar/Word of Blake                                  |
| Dragoon AEM-01                     |   Heavy    |  70  | Inner Sphere | 3025  | ComStar                                                |
| Dragoon AEM-02                     |   Heavy    |  70  | Inner Sphere | 3025  | ComStar                                                |
| Dragoon AEM-03                     |   Heavy    |  70  | Inner Sphere | 3025  | ComStar                                                |
| Dragoon AEM-04                     |   Heavy    |  70  | Inner Sphere | 3025  | ComStar                                                |
| Screamer LAM SCR-1X-LAM            |   Medium   |  55  | Inner Sphere | 3025  | Snord's Irregulars                                     |
| Crosscut ED-X4K LoggerMech         |   Light    |  30  | Inner Sphere | 3025  | Pirates                                                |
| Crosscut ED-X2M LoggerMech         |   Light    |  30  | Inner Sphere | 3025  | Pirates                                                |
| Dig King RCL-1M MiningMech         |   Light    |  35  | Inner Sphere | 3025  | Pirates                                                |
| Kiso CommandMech K-3N-KRHQ         |  Assault   | 100  | Inner Sphere | 3025  | Kurita                                                 |
| Crosscut ED-X4D DemolitionMech     |   Light    |  30  | Inner Sphere | 3025  | Pirates                                                |
| Gulon MiningMech GLN-1A            |   Light    |  25  | Inner Sphere | 3025  | Outworlds Alliance                                     |
| Gulon SecurityMech GLN-1B          |   Light    |  25  | Inner Sphere | 3025  | Outworlds Alliance                                     |
| Matar SAM-RS2                      | Superheavy | 110  | Inner Sphere | 3036  | ComStar (3036+)                                        |
| Rattlesnake JR7-31                 |   Light    |  35  | Inner Sphere | 3042  | Davion                                                 |
| Rattlesnake JR7-31P                |   Light    |  35  | Inner Sphere | 3043  | Davion                                                 |
| Cyclops CP-11-A-DC                 |  Assault   |  90  | Inner Sphere | 3045  | ComStar                                                |
| Catapult CPLT-C3                   |   Heavy    |  65  | Inner Sphere | 3049  | Liao, ComStar/Word of Blake                            |
| Coyotl Prime                       |   Medium   |  40  |     Clan     | 3049  | Clan Wolf (<3058)                                      |
| Coyotl A                           |   Medium   |  40  |     Clan     | 3049  | Clan Wolf (<3058)                                      |
| Coyotl B                           |   Medium   |  40  |     Clan     | 3049  | Clan Wolf (<3058)                                      |
| Fox CS-1                           |   Medium   |  50  |  Mixed-tech  | 3049  | Clan Ghost Bear                                        |
| Fox                                |   Medium   |  50  |     Clan     | 3049  | Clan Ghost Bear                                        |
| Galahad (Glass Spider) 1           |   Heavy    |  60  |     Clan     | 3049  | Clans                                                  |
| Galahad (Glass Spider) 2           |   Heavy    |  60  |     Clan     | 3049  | Clan Wolf                                              |
| Imp C                              |  Assault   | 100  |     Clan     | 3049  | Wolf's Dragoons, Pirates                               |
| Lupus Prime                        |   Heavy    |  60  |     Clan     | 3049  | Clan Steel Viper                                       |
| Lupus A                            |   Heavy    |  60  |     Clan     | 3049  | Clan Steel Viper                                       |
| Lupus B                            |   Heavy    |  60  |     Clan     | 3049  | Clan Steel Viper                                       |
| Mercury II MCY-100                 |   Medium   |  40  |  Mixed-tech  | 3049  | Clan Diamond Shark, Bandit Caste                       |
| Naga Prime                         |  Assault   |  80  |     Clan     | 3049  | Clans                                                  |
| Naga A                             |  Assault   |  80  |     Clan     | 3049  | Clans                                                  |
| Naga B                             |  Assault   |  80  |     Clan     | 3049  | Clans                                                  |
| Naga C                             |  Assault   |  80  |     Clan     | 3049  | Clans                                                  |
| Naga D                             |  Assault   |  80  |     Clan     | 3049  | Clans                                                  |
| Peregrine (Horned Owl) 1           |   Light    |  35  |     Clan     | 3049  | Clans                                                  |
| Peregrine (Horned Owl) 2           |   Light    |  35  |     Clan     | 3049  | Clans                                                  |
| Phoenix Hawk IIC                   |  Assault   |  80  |     Clan     | 3049  | Clans                                                  |
| Phoenix Hawk IIC 2                 |  Assault   |  80  |     Clan     | 3049  | Clans                                                  |
| Phoenix Hawk IIC 9                 |  Assault   |  80  |     Clan     | 3049  | Clans                                                  |
| Pulverizer PUL-2V                  |  Assault   |  90  |  Mixed-tech  | 3049  | Clan Ghost Bear                                        |
| Pulverizer PUL-3R                  |  Assault   |  90  |  Mixed-tech  | 3049  | Clan Ghost Bear                                        |
| Pulverizer                         |  Assault   |  90  |     Clan     | 3049  | Clan Ghost Bear                                        |
| Atlas AS7-K-DC                     |  Assault   | 100  | Inner Sphere | 3050  | ComStar/Word of Blake                                  |
| Roughneck RGH-1A                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Roughneck RGH-1B                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Roughneck RGH-1C                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Roughneck RGH-2A                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Roughneck RGH-3A                   |   Heavy    |  65  | Inner Sphere | 3050  | Steiner-Davion, Mercenaries                            |
| Warhammer C 3                      |   Heavy    |  70  |     Clan     | 3050  | Wolf's Dragoons (3050+)                                |
| Annihilator C                      |  Assault   | 100  |     Clan     | 3051  | Clans (3051+)                                          |
| Annihilator C 2                    |  Assault   | 100  |     Clan     | 3051  | Clans (3051+)                                          |
| Archer C                           |   Heavy    |  70  |  Mixed-tech  | 3051  | Clans (3051+)<br />Kurita, Steiner-Davion (3055+)      |
| Marauder C                         |   Heavy    |  75  |  Mixed-tech  | 3051  | Clans (3051+)<br />Kurita, Steiner-Davion (3055+)      |
| Warhammer C                        |   Heavy    |  70  |  Mixed-tech  | 3051  | Clans (3051+)<br />Kurita, Steiner-Davion (3055+)      |
| Fire Falcon Prime                  |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Fire Falcon A                      |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Fire Falcon B                      |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Fire Falcon C                      |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Fire Falcon D                      |   Light    |  25  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phantom Prime                      |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phantom A                          |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phantom B                          |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phantom C                          |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Phantom D                          |   Medium   |  40  |     Clan     | 3052  | Clan Jade Falcon                                       |
| Stag ST-14G                        |   Medium   |  45  |  Mixed-tech  | 3052  | Clans (3052+)                                          |
| Stag II ST-24G                     |   Medium   |  45  |  Mixed-tech  | 3052  | Clan Wolf (3052+)                                      |
| Warhammer C 2                      |   Heavy    |  70  |  Mixed-tech  | 3052  | Clan Wolf<br />Kurita, Steiner-Davion (3055+)          |
| Albatross ALB-3U                   |  Assault   |  95  | Inner Sphere | 3053  | Marik, Word of Blake                                   |
| Battle Hawk BH-K305                |   Light    |  30  | Inner Sphere | 3053  | Steiner-Davion, Mercenaries                            |
| BattleMaster BLR-3M-DC             |  Assault   |  85  | Inner Sphere | 3053  | ComStar/Word of Blake                                  |
| Champion LAM CPN-1X1               |   Heavy    |  60  | Inner Sphere | 3053  | Word of Blake                                          |
| Grand Crusader GRN-D-01            |  Assault   |  80  | Inner Sphere | 3053  | Word of Blake                                          |
| Hammer HMR-3M                      |   Light    |  30  | Inner Sphere | 3053  | Liao, Marik, Word of Blake                             |
| Juggernaut JG-R9T1                 |  Assault   |  90  | Inner Sphere | 3053  | Steiner                                                |
| Komodo KIM-2                       |   Medium   |  45  | Inner Sphere | 3053  | Kurita, Rasalhague                                     |
| Komodo KIM-2A                      |   Medium   |  45  | Inner Sphere | 3053  | Kurita                                                 |
| Komodo KIM-3C                      |   Medium   |  45  | Inner Sphere | 3053  | Kurita                                                 |
| Nightsky NGS-4S                    |   Medium   |  50  | Inner Sphere | 3053  | Steiner-Davion                                         |
| Orion ON1-M-DC                     |   Heavy    |  75  | Inner Sphere | 3053  | ComStar/Word of Blake                                  |
| Prometheus                         |   Heavy    |  75  |  Mixed-tech  | 3053  | Davion                                                 |
| Bombard BMB-010                    |   Medium   |  50  | Inner Sphere | 3054  | Steiner                                                |
| Hammer HMR-3S 'Slammer'            |   Light    |  30  | Inner Sphere | 3054  | Marik, Word of Blake                                   |
| Iron Cheetah Prime                 |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Iron Cheetah A                     |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Iron Cheetah B                     |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Iron Cheetah C                     |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Iron Cheetah D                     |  Assault   | 100  |     Clan     | 3054  | Clan Smoke Jaguar                                      |
| Zeus-X ZEU-X                       |  Assault   |  80  | Inner Sphere | 3054  | Davion                                                 |
| Buccaneer BCN-3R                   |   Medium   |  55  | Inner Sphere | 3055  | Marik, Word of Blake                                   |
| Dasher (Fire Moth) E               |   Light    |  20  |     Clan     | 3055  | Clan Ghost Bear                                        |
| Grand Crusader GRN-D-02            |  Assault   |  80  | Inner Sphere | 3056  | Word of Blake                                          |
| Hammer HMR-3C 'Claw-Hammer'        |   Light    |  30  | Inner Sphere | 3056  | Marik, Word of Blake                                   |
| Nightsky NGS-4T                    |   Medium   |  50  | Inner Sphere | 3056  | Steiner-Davion                                         |
| Nightsky NGS-5S                    |   Medium   |  50  | Inner Sphere | 3056  | Steiner-Davion                                         |
| Sunder SD1-OB                      |  Assault   |  90  | Inner Sphere | 3056  | Kurita, Davion, St. Ives                               |
| Dig Lord RCL-4 MiningMech          |   Heavy    |  65  | Inner Sphere | 3057  | Steiner-Davion                                         |
| Juggernaut JG-R9T2                 |  Assault   |  90  | Inner Sphere | 3057  | Steiner                                                |
| Nightsky NGS-5T                    |   Medium   |  50  | Inner Sphere | 3057  | Steiner-Davion                                         |
| Akuma AKU-1X                       |  Assault   |  90  | Inner Sphere | 3058  | Kurita                                                 |
| Barghest BGS-1T                    |   Heavy    |  70  | Inner Sphere | 3058  | Steiner                                                |
| Hellfire 1                         |   Heavy    |  60  |     Clan     | 3058  | Clan Steel Viper                                       |
| JagerMech III JM6-D3               |   Heavy    |  65  | Inner Sphere | 3058  | Davion                                                 |
| Lineholder KW1-LH2                 |   Medium   |  55  | Inner Sphere | 3058  | Inner Sphere                                           |
| Thunderbolt TDR-8M                 |   Heavy    |  65  | Inner Sphere | 3058  | Centrella, Word of Blake                               |
| Arctic Wolf 1                      |   Medium   |  40  |     Clan     | 3059  | Clan Wolf                                              |
| Avatar AV1-OR                      |   Heavy    |  70  |  Mixed-tech  | 3059  | Kurita                                                 |
| Black Hawk-KU BHKU-OR              |   Heavy    |  60  |  Mixed-tech  | 3059  | Kurita, Liao, Steiner-Davion<br />Rasalhague, St. Ives |
| Blackjack BJ2-OR                   |   Medium   |  50  |  Mixed-tech  | 3059  | Kurita                                                 |
| Firestarter FS9-OR                 |   Medium   |  45  |  Mixed-tech  | 3059  | Kurita, Liao, Marik, Steiner-Davion                    |
| Firestarter FS9-OX                 |   Medium   |  45  | Inner Sphere | 3059  | Kurita                                                 |
| Lineholder KW1-LH3                 |   Medium   |  55  | Inner Sphere | 3059  | Inner Sphere                                           |
| Raptor RTX1-OR                     |   Light    |  25  |  Mixed-tech  | 3059  | Kurita, Davion, ComStar                                |
| Strider SR1-OR                     |   Medium   |  40  |  Mixed-tech  | 3059  | Kurita, Marik, Steiner-Davion                          |
| Sunder SD1-OR                      |  Assault   |  90  |  Mixed-tech  | 3059  | Kurita, Steiner-Davion                                 |
| Thunder Hawk TDK-7KMA              |  Assault   | 100  | Inner Sphere | 3059  | Steiner                                                |
| Warthog Prime                      |  Assault   |  95  |     Clan     | 3059  | Clans                                                  |
| Anvil ANV-8M                       |   Heavy    |  60  | Inner Sphere | 3060  | Marik, Word of Blake                                   |
| Arctic Wolf 2                      |   Medium   |  40  |     Clan     | 3060  | Clan Wolf                                              |
| Assassin II ASN-56                 |   Medium   |  45  | Inner Sphere | 3060  | Davion                                                 |
| Barghest BGS-2T                    |   Heavy    |  70  | Inner Sphere | 3060  | Steiner                                                |
| Black Heart BH-1                   |   Heavy    |  70  | Inner Sphere | 3060  | Word of Blake                                          |
| Cossack C-SK1                      |   Light    |  20  | Inner Sphere | 3060  | St. Ives Compact                                       |
| Cronus CNS-5M                      |   Medium   |  55  | Inner Sphere | 3060  | Mercenaries, Pirates                                   |
| Hammer HMR-3P 'Pein-Hammer'        |   Light    |  30  | Inner Sphere | 3060  | Marik, Word of Blake                                   |
| Viking VKG-2G                      |  Assault   |  90  | Inner Sphere | 3060  | Rasalhague, ComStar/Word of Blake                      |
| Blitzkrieg BTZ-3F                  |   Medium   |  50  | Inner Sphere | 3061  | Marik, Steiner-Davion                                  |
| Catapult CPLT-C5                   |   Heavy    |  65  | Inner Sphere | 3061  | Liao                                                   |
| Flashman FLS-9C                    |   Heavy    |  75  | Inner Sphere | 3061  | ComStar                                                |
| Hollander II BZK-F7                |   Medium   |  45  | Inner Sphere | 3061  | Steiner-Davion                                         |
| Mantis MTS-S                       |   Light    |  30  | Inner Sphere | 3061  | Steiner                                                |
| Peregrine (Horned Owl) 3           |   Light    |  35  |     Clan     | 3061  | Clans                                                  |
| Thanatos TNS-4S                    |   Heavy    |  75  | Inner Sphere | 3061  | Steiner-Davion                                         |
| Barghest BGS-3T                    |   Heavy    |  70  | Inner Sphere | 3062  | Steiner                                                |
| Cataphract CTF-3X                  |   Heavy    |  70  | Inner Sphere | 3062  | Davion                                                 |
| Centurion CN9-D5                   |   Medium   |  50  | Inner Sphere | 3062  | Steiner-Davion                                         |
| Dervish DV-8D                      |   Medium   |  55  | Inner Sphere | 3062  | Davion                                                 |
| Garm GRM-01C                       |   Light    |  35  | Inner Sphere | 3062  | Davion                                                 |
| Gunslinger GUN-2ERD                |  Assault   |  85  | Inner Sphere | 3062  | Kurita, Steiner                                        |
| Hankyu (Arctic Cheetah) H          |   Light    |  30  |     Clan     | 3062  | Clans                                                  |
| Hatchetman HCT-6D                  |   Medium   |  45  | Inner Sphere | 3062  | Davion                                                 |
| Hellhound (Conjurer) 2             |   Medium   |  50  |     Clan     | 3062  | Clan Nova Cat                                          |
| Hermes II HER-5C                   |   Medium   |  40  | Inner Sphere | 3062  | Word of Blake                                          |
| Hermes II HER-6D                   |   Medium   |  40  | Inner Sphere | 3062  | Davion                                                 |
| Highlander HGN-694                 |  Assault   |  90  | Inner Sphere | 3062  | Steiner                                                |
| JagerMech JM7-F                    |   Heavy    |  70  | Inner Sphere | 3062  | Davion                                                 |
| Lao Hu LHU-2B                      |   Heavy    |  75  | Inner Sphere | 3062  | Liao                                                   |
| Mad Cat Mk II                      |  Assault   |  90  |     Clan     | 3062  | Clan Diamond Shark                                     |
| Orion ON1-MD                       |   Heavy    |  75  | Inner Sphere | 3062  | Davion, Marik, ComStar/Word of Blake                   |
| Phoenix Hawk IIC 3                 |  Assault   |  80  |     Clan     | 3062  | Clans                                                  |
| Supernova 2                        |  Assault   |  90  |     Clan     | 3062  | Clan Nova Cat                                          |
| Templar TLR1-O                     |  Assault   |  85  | Inner Sphere | 3062  | Davion                                                 |
| Templar TLR1-OA                    |  Assault   |  85  | Inner Sphere | 3062  | Davion                                                 |
| Templar TLR1-OB                    |  Assault   |  85  | Inner Sphere | 3062  | Davion                                                 |
| Templar TLR1-OC                    |  Assault   |  85  | Inner Sphere | 3062  | Davion                                                 |
| Thanatos TNS-4T                    |   Heavy    |  75  | Inner Sphere | 3062  | Steiner-Davion                                         |
| Albatross ALB-4U                   |  Assault   |  95  | Inner Sphere | 3063  | Marik, Word of Blake                                   |
| Anubis ABS-3L                      |   Light    |  30  | Inner Sphere | 3063  | Liao, Centrella, Calderon                              |
| Archer C 2                         |   Heavy    |  70  |     Clan     | 3063  | Wolf's Dragoons²                                       |
| Bombard BMB-013                    |   Medium   |  50  | Inner Sphere | 3063  | Steiner                                                |
| Charger CGR-SA5                    |  Assault   |  80  | Inner Sphere | 3063  | Kurita                                                 |
| Chimera CMA-1S                     |   Medium   |  40  | Inner Sphere | 3063  | Kurita, Steiner-Davion                                 |
| Chimera CMA-C                      |   Medium   |  40  | Inner Sphere | 3063  | Kurita, Marik, Steiner-Davion                          |
| Grand Dragon DRG-7K                |   Heavy    |  60  | Inner Sphere | 3063  | Kurita                                                 |
| Huron Warrior HUR-WO-R4N           |   Medium   |  50  | Inner Sphere | 3063  | Liao                                                   |
| Lao Hu LHU-3B                      |   Heavy    |  75  | Inner Sphere | 3063  | Liao                                                   |
| Osiris OSR-3D                      |   Light    |  30  | Inner Sphere | 3063  | Davion                                                 |
| Schwerer Gustav SG-1X              |  Assault   | 100  |  Mixed-tech  | 3063  | Marik                                                  |
| Sha Yu SYU-2B                      |   Medium   |  40  | Inner Sphere | 3063  | Liao, Centrella                                        |
| Akuma AKU-1XJ                      |  Assault   |  90  | Inner Sphere | 3064  | Kurita                                                 |
| Anubis ABS-3R                      |   Light    |  30  | Inner Sphere | 3064  | Liao, Centrella, Calderon                              |
| Catapult CPLT-H2                   |   Heavy    |  65  | Inner Sphere | 3064  | Pirates                                                |
| Charger CGR-2A2                    |  Assault   |  80  | Inner Sphere | 3064  | Centrella, Outworlds Alliance, Pirates                 |
| Commando COM-4H                    |   Light    |  25  | Inner Sphere | 3064  | Pirates                                                |
| Cyclops CP-11-H                    |  Assault   |  90  | Inner Sphere | 3064  | Calderon, Pirates                                      |
| Hunchback HBK-5H                   |   Medium   |  50  | Inner Sphere | 3064  | Periphery States, Pirates                              |
| JagerMech JM6-H                    |   Heavy    |  65  | Inner Sphere | 3064  | Pirates                                                |
| Supernova 3                        |  Assault   |  90  |     Clan     | 3064  | Clan Nova Cat                                          |
| Juggernaut JG-R9T3                 |  Assault   |  90  | Inner Sphere | 3065  | Steiner                                                |
| Longbow LGB-0H                     |  Assault   |  85  | Inner Sphere | 3065  | Pirates                                                |

</details>


### Weapons / Upgrades

<details>
  <summary>Buyable (by Type/Intro)</summary>

| Name                              |   Type    | Intro | Factions                    |
| :-------------------------------- | :-------: | :---: | :-------------------------- |
| Light/Medium/Heavy Rifle          | Ballistic |  PS   | *LosTech*                   |
| Thumper/Sniper/Long Tom Cannon    | Ballistic | 3012  | *Research*                  |
| Magshot                           | Ballistic | 3059  | Steiner                     |
| Hyper-Velocity AC (HVAC)          | Ballistic | 3059  | Liao                        |
| Rotary AC (RAC)                   | Ballistic | 3060  | Davion                      |
| Light AC (LAC)                    | Ballistic | 3062  | Davion                      |
|  ----                             |           |       |                             |
| Rail Gun                          |  Energy   | 3051  | Marik                       |
| Plasma Rifle                      |  Energy   | 3061  | Liao                        |
| Heavy PPC                         |  Energy   | 3062  | Kurita                      |
| Light PPC                         |  Energy   | 3064  | Kurita                      |
| Bombast Laser                     |  Energy   | 3064  | Steiner                     |
|  ----                             |           |       |                             |
| Bomb Bay²                         |  Missile  | 2680  | *Mining*                    |
| Arrow IV                          |  Missile  | 3044  | Liao<br />All (3049+)       |
| Thunderbolt                       |  Missile  | 3052  | Davion<br />Steiner (3052+) |
| Extended LRM (ELRM)               |  Missile  | 3054  | Steiner<br />Davion (3054+) |
| Enhanced LRM (NLRM)               |  Missile  | 3058  | Davion                      |
|  ----                             |           |       |                             |
| Fluid Gun                         |  Support  |  PS   | *Chemicals*                 |
| Heavy Flamer                      |  Support  | 3063  | Steiner                     |
| Heavy Machine Gun                 |  Support  | 3063  | Calderon                    |
| Light Machine Gun                 |  Support  | 3064  | Liao                        |
|  ----                             |           |       |                             |
| Airburst Mortar                   |   Ammo    | 3043  | All                         |
| Shaped Charge Mortar              |   Ammo    | 3043  | All                         |
| Swarm Missile                     |   Ammo    | 3049  | Davion<br />All (3058+)     |
| Swarm-I Missile                   |   Ammo    | 3052  | Marik                       |
| Inferno-IV Missile                |   Ammo    | 3053  | Liao                        |
| Thunder-Inferno Missile           |   Ammo    | 3054  | Liao                        |
| Armor-Piercing Ammo               |   Ammo    | 3055  | Davion<br />Steiner (3055+) |
| Precision Ammo                    |   Ammo    | 3058  | Davion                      |
|  ----                             |           |       |                             |
| Targeting Computer                |  Upgrade  | 3052  | *Research*                  |
| Bloodhound Active Probe           |  Upgrade  | 3058  | *Black Market*              |
| Laser Anti-Missile System         |  Upgrade  | 3059  | *Research*                  |
| Blue Shield Particle Field Damper |  Upgrade  | 3061  | *Research*                  |
| Small/Medium/Large Shield         |  Upgrade  | 3065  | *Research*                  |

² with High-Explosive, Laser-Guided, Cluster, and Inferno Bombs.
</details>

<details>
  <summary>Unique (by Type)</summary>

| Name                                  | Exclusive to                                |
| :------------------------------------ | :------------------------------------------ |
| Claws                                 | Mantis                                      |
| Industrial Weapons²                   | Crosscut, Dig King, Gulon, Kiso
| Katana                                | Hatamoto-Chi 'Shin'                         |
| Spikes                                | Bombard                                     |
| Small Vibroblade                      | Assassin 'Servitor'                         |
| Large Vibroblade<br />Large Shield    | Black Knight 'Red Reaper'                   |
|  ----                                 |                                             |
| Direct Neural Interface               | Prometheus<br />Black Heart                 |
| Light Active Probe                    | Vulture (Mad Dog) 'Fury'                    |
|  ----                                 |                                             |
| Composite Chassis<br />Reactive Armor | Zeus-X                                      |
| Light Ferro-Fibrous Armor             | Black Knight 'Red Reaper'                   |
| Stealth Armor                         | Sha Yu<br />Anubis                          |

² Includes the Chainsaw, Mining Drill, Pile Driver, and other variants of these weapons.
</details>

### Other Changes

<details>
  <summary>Shell Shuffler</summary>
  
This optional submod allows the AI to randomly use different types of ammunition when spawning. The mod has two presets, depending on the era you are playing in:

- **3025 preset:** Any faction can use Inferno SRM.
- **3050 preset:** Each faction has their own set of special ammo types, most of which were developed in the 3050s.

| Faction                 | Ammo Types                                          |
| :---------------------- | :-------------------------------------------------- |
| Davion                  | Armor-Piercing & Precision rounds, plus Swarm LRM   |
| Kurita                  | Dead-Fire SRM and LRM                               |
| Liao                    | Inferno SRM, LRM and Arrow IV                       |
| Marik                   | Improved Swarm LRM (Swarm-I LRM)                    |
| Steiner                 | Armor-Piercing rounds and Swarm LRM                 |
| ----                    |                                                     |
| Clans                   | Extended-Range & High-Explosive ATM, plus Swarm LRM |
| ComStar / Word of Blake | Swarm LRM / Swarm-I LRM                             |
| Mercernaries & Pirates  | Inferno SRM                                         |
</details>

<details>
  <summary>Artillery</summary>
  
- Mech mortars are now available in 3025. The Thumper, Sniper, and Long Tom cannons have been moved to research planets, as they are only prototypes in the current timeline.

- Artillery cannons have been reworked to be more in line with the tabletop rules. They now deal 30% less damage and have a much shorter range than their larger counterparts. Additionally, all artillery deal less stability damage and are less accurate due to the indirect fire penalty.

| Name            | Damage | AoE Damage | Min. Range | Opt. Range | Max. Range |
| --------------- | -----: | ---------: | ---------: | ---------: | ---------: |
| Mortar/1        |     15 |          5 |        180 |        420 |        630 |
| Thumper Cannon  |     40 |         50 |         90 |        270 |        540 |
| Sniper Cannon   |     60 |         75 |         60 |        240 |        480 |
| Long Tom Cannon |     80 |        100 |        120 |        390 |        780 |
| Arrow IV        |     60 |        120 |        240 |        780 |       1560 |
| ----            |        |            |            |            |            |
| Standard LRM²   |      4 |          0 |        180 |        420 |        630 |
| Extended LRM    |      5 |          0 |        325 |        760 |       1140 |

² Ignores cover and acts like artillery with Swarm Ammo.

- The Sniper and Long Tom cannons both have a Loading Mechanism addon that works in the same way as the Artemis IV FCS. This addon allows the two massive weapons to be mounted on more 'Mechs.
</details>

<details>
  <summary>Miscellaneous Fixes</summary>

Some changes have been made to address minor issues and to add the latest CAB models to the current version of BEX.

| Name                | Changes                                             |
| :------------------ | :-------------------------------------------------- |
| Annihilator         | Reduced movement to 3/5 hexes (from 4/7)            |
| Atlas II AS7-D-HT   | Changed armor placement and moved DHS to the engine |
| Enfield             | Switched to a new CAB model                         |
| Exterminator        | Switched to a new CAB model                         |
| Firefly FFL-3A      | Fixed available tonnage                             |
| Firestarter FS9-OF  | Added Large Engine quirk                            |
| Flashman            | Switched to a new CAB model                         |
| Garm                | Reduced overall size to better match tonnage        |
| Goliath             | Reduced stability                                   |
| Grand Dragon DRG-1G | Fixed energy hadpoints placement                    |
| Gunslinger          | Switched to a new CAB model                         |
| Hermes II           | Reduced overall size to better match tonnage        |
| Hornet HNT-161      | Fixed armor placement                               |
| Linesman LMN-1PT    | Changed to the correct prefab base model            |
| Loader King LDK-5C  | Added more armor and missing Cargo Bay              |
| Locust LCT-3S       | Fixed available tonnage                             |
| Naginata            | Switched to a new CAB model                         |
| Piranha 1           | Switched to single heat sinks                       |
| Phoenix Hawk LAM    | Changed to a different CAB model                    |
| Rifleman RFL-5CS    | Added Large Engine quirk                            |
| Shadow Hawk         | Added Rugged quirk                                  |
| Trebuchet TBT-3C    | Added Large Engine quirk                            |
| Urbanmech           | Reduced movement to 3/5 hexes (from 4/7)            |
