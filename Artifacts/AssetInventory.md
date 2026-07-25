# Guardian Misti Asset Inventory

Generated from a complete `Assets` audit on 2026-07-25. The project contains 19 FBX model files, 0 OBJ files, 264 prefabs, 19 materials and 334 textures. Local, already licensed content was sufficient; this iteration performed no new download.

## Selected visual assets

| Exact asset path | Intended use | Scene | Collider strategy | Material compatibility | Origin |
|---|---|---|---|---|---|
| `Assets/ThirdParty/Kenney/FurnitureKit/Models/desk.fbx` via `Assets/Prefabs/Environment/Desk.prefab` | Operations desk | Level01 | Project wrapper BoxCollider; source mesh collider removed | Wrapper replaces source material with project URP/Lit material | Imported, Kenney Furniture Kit, CC0 1.0 |
| `Assets/ThirdParty/Kenney/FurnitureKit/Models/chairDesk.fbx` via `Assets/Prefabs/Environment/Chair.prefab` | Office chair | Level01 | Project wrapper BoxCollider | Project URP/Lit | Imported, Kenney Furniture Kit, CC0 1.0 |
| `Assets/ThirdParty/Kenney/FurnitureKit/Models/bookcaseOpen.fbx` via `Assets/Prefabs/Environment/Shelf.prefab` | Emergency equipment shelf | Level01 | Project wrapper BoxCollider | Project URP/Lit | Imported, Kenney Furniture Kit, CC0 1.0 |
| `Assets/ThirdParty/Kenney/FurnitureKit/Models/doorway.fbx` via `Assets/Prefabs/Environment/KenneyDoorFrame.prefab` | Architectural doorway detail | Level01 | Decorative source colliders removed; gameplay door owns collision | Project URP/Lit | Imported, Kenney Furniture Kit, CC0 1.0 |
| `Assets/ThirdParty/Kenney/SurvivalKit/Models/rock-a.fbx` via `Assets/Prefabs/Environment/KenneyRock.prefab` | Volcanic route landmarks | Level02 | Simple project BoxCollider, outside center route | Project URP/Lit | Imported, Kenney Survival Kit, CC0 1.0 |
| `Assets/ThirdParty/Kenney/SurvivalKit/Models/tent-canvas.fbx` via `Assets/Prefabs/Environment/KenneyRescueTent.prefab` | Rescue landmark | Level02 | Simple project BoxCollider, outside safe-zone approach | Project URP/Lit | Imported, Kenney Survival Kit, CC0 1.0 |
| `Assets/Prefabs/Gameplay/EmergencyBackpack.prefab` | Emergency backpack collectible | Level01 | One simple root BoxCollider; child visuals collider-free | Project URP/Lit and emissive label | Project-owned multipart composition |
| `Assets/Prefabs/Gameplay/EmergencyRadio.prefab` | Handheld radio collectible | Level02 | One simple root BoxCollider | Project URP/Lit and emissive screen/button | Project-owned multipart composition |
| `Assets/Prefabs/Gameplay/AccessKey.prefab` | Access badge collectible | Level02 | One simple root BoxCollider | Project URP/Lit with emissive pickup ring | Project-owned multipart composition |
| `Assets/Prefabs/Gameplay/InteractableDoor.prefab` | Operable framed exit door | Level01 | Separate root blocking BoxCollider disabled after opening; child visuals collider-free | Project URP/Lit | Project-owned multipart composition |
| `Assets/Prefabs/Gameplay/EvacuationTerminal.prefab` | Evacuation control terminal | Level01 | One simple root BoxCollider | Project URP/Lit with screen and indicator materials | Project-owned multipart composition |
| `Assets/Prefabs/Gameplay/EmergencyBeacon.prefab` | Rescue beacon | Level02 | One root CapsuleCollider; detailed children collider-free | Project URP/Lit with emissive status light | Project-owned multipart composition |
| `Assets/Prefabs/Gameplay/SafeZone.prefab` | Final rescue zone | Level02 | Root trigger separated from platform, symbol, perimeter lights and sign | Project URP/Lit and emissive markings | Project-owned multipart composition |
| `Assets/Prefabs/Environment/EmergencySign.prefab` | Route and rescue signage | Level02 | Visual-only multipart sign; placed away from route center | Project URP/Lit | Project-owned multipart composition |

## Verified scene use

The builder retains prefab links for every asset listed above. Level01 uses the gameplay backpack, door and terminal plus Desk, Chair and Shelf wrappers. Level02 uses the gameplay radio, access badge, beacon and safe-zone wrappers, EmergencySign, KenneyRock and KenneyRescueTent. `GuardianMistiProjectValidator.ValidateProject` checks the expected source prefab path, renderer count, materials and scale on each instantiated major object.

## Other audited local model assets

Starter Assets supplies ten greybox/structural FBX models (`Box`, `Ground`, `Ramp`, `Stairs`, `Structure`, `Tunnel`, `Wall`) used only where already appropriate for background structure or supply crates. Kenney also supplies `rock-b.fbx`, `rock-c.fbx` and `signpost.fbx`; these remain preserved source assets but are not claimed as selected scene visuals in this iteration.

Licenses, original URLs, authors, archive names and SHA256 values remain recorded in `Assets/ThirdParty/ATTRIBUTION.md` and `Artifacts/ExternalAssetManifest.json`.
