# Visual Asset Usage

All camera positions are Unity world coordinates. Generated fallback assets use project-owned URP materials under `Assets/Art/Materials` and `Assets/Materials`.

| Screenshot | Scene | Camera position | Visible models/materials |
|---|---|---|---|
| `MainMenu.png` | MainMenu | `(0,4,-14)` | Composed beacon, volcanic silhouettes, menu UI |
| `Level01_Start.png` | Level01 | `(0,1.65,0.3)` | Continuous floor, facility walls/supports, emergency lights, protection zone |
| `Level01_Backpack.png` | Level01 | `(0,1.8,6)` | Multi-part backpack: body, pocket, side pockets, straps, handle, label |
| `Level01_Terminal.png` | Level01 | `(0,1.8,10)` | Terminal housing, emissive screen, emergency label |
| `Level01_Exit.png` | Level01 | `(0,1.8,14)` | Evacuation signage, route marker, exit trigger visualization |
| `Level02_Entry.png` | Level02 | `(0,1.7,0.5)` | Exterior ground, rocks, route posts, distant beacon |
| `Level02_Radio.png` | Level02 | `(0,1.8,3)` | Radio body, antenna, grille, screen and SOS details |
| `Level02_AccessKey.png` | Level02 | `(0,1.8,7)` | Access-card silhouette and emissive stripe |
| `Level02_Beacon.png` | Level02 | `(0,2,9)` | Mast, supports, status light and local rescue light |
| `Level02_SafeZone.png` | Level02 | `(0,2,15)` | Closed rescue platform boundary, perimeter route lights and signage |
| `PausePanel.png` | Level01 | gameplay camera | Pause controls and settings |
| `CompletionPanel.png` | Level02 | gameplay camera | Results hierarchy, score controls |

Imported mesh used in-scene: `Assets/StarterAssets/Environment/Prefabs/Box_350x250x200_Prefab.prefab`. Generated compositions are used where no credible dedicated imported backpack, radio, key, terminal, beacon or rescue-zone model exists.
