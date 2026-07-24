# Guardian Misti 3D Asset Inventory

Inventory generated from the complete `Assets` tree. The repository contains 10 FBX models, no OBJ files, and no suitable gameplay audio clips.

## Imported mesh assets

| Asset path | Type | Intended use | URP | Collider strategy | Scene |
|---|---|---|---|---|---|
| `Assets/StarterAssets/Environment/Art/Models/Ground_Mesh.fbx` | FBX mesh | Modular ground reference | Compatible Starter Assets materials | Project-owned continuous BoxCollider | Level01/02 |
| `Assets/StarterAssets/Environment/Art/Models/Wall_Mesh.fbx` | FBX mesh | Facility modular wall reference | Compatible | Project-owned BoxCollider | Level01 |
| `Assets/StarterAssets/Environment/Art/Models/Structure_Mesh.fbx` | FBX mesh | Facility silhouette/menu dressing | Compatible | Removed on decorative instances | MainMenu/Level01 |
| `Assets/StarterAssets/Environment/Art/Models/Tunnel_Mesh.fbx` | FBX mesh | Architectural module available, not selected | Compatible | N/A | Not instantiated |
| `Assets/StarterAssets/Environment/Art/Models/Ramp_Mesh.fbx` | FBX mesh | Accessible route option | Compatible | Simple BoxCollider if used | Level02 |
| `Assets/StarterAssets/Environment/Art/Models/Ramp_100x100x200_Mesh.fbx` | FBX mesh | Route grade option | Compatible | Simple BoxCollider if used | Level02 |
| `Assets/StarterAssets/Environment/Art/Models/Stairs_200x100x200_Mesh.fbx` | FBX mesh | Available stairs, not selected for main route | Compatible | N/A | Not instantiated |
| `Assets/StarterAssets/Environment/Art/Models/Stairs_650_400_300_Mesh.fbx` | FBX mesh | Available stairs, not selected for main route | Compatible | N/A | Not instantiated |
| `Assets/StarterAssets/Environment/Art/Models/Box_350x250x200_Mesh.fbx` | FBX mesh/prefab | Emergency supply crates and facility dressing | Compatible | Imported colliders removed; decorative only | Level01 |
| `Assets/StarterAssets/Environment/Art/Models/Box_350x250x300_Mesh.fbx` | FBX mesh/prefab | Available large crate | Compatible | Removed if decorative | Not instantiated |

## Selected prefabs and project-owned compositions

| Asset path | Type | Intended use | Origin | URP | Collider strategy | Scene |
|---|---|---|---|---|---|---|
| `Assets/StarterAssets/Environment/Prefabs/Box_350x250x200_Prefab.prefab` | Imported mesh prefab | Supply crates | Imported | Compatible | Decorative colliders removed | Level01 |
| `Assets/StarterAssets/Environment/Prefabs/Structure_Prefab.prefab` | Imported mesh prefab | Facility/menu structural module | Imported | Compatible | Wrapper controls collision | MainMenu/Level01 |
| `Assets/Prefabs/Gameplay/EmergencyBackpack.prefab` | Multi-part composition | Backpack | Project-owned | Project URP materials | One pickup BoxCollider | Level01 |
| `Assets/Prefabs/Gameplay/EmergencyRadio.prefab` | Multi-part composition | Handheld radio | Project-owned | Project URP materials/emission | One pickup BoxCollider | Level02 |
| `Assets/Prefabs/Gameplay/AccessKey.prefab` | Multi-part composition | Access card | Project-owned | Project URP materials/emission | One pickup BoxCollider | Level02 |
| `Assets/Prefabs/Gameplay/EvacuationTerminal.prefab` | Multi-part composition | Evacuation control | Project-owned | Project URP materials | Simple housing BoxCollider | Level01 |
| `Assets/Prefabs/Gameplay/EmergencyBeacon.prefab` | Multi-part composition | Rescue beacon | Project-owned | Project URP materials/emission | Simple CapsuleCollider | Level02 |
| `Assets/Prefabs/Gameplay/SafeZone.prefab` | Platform, posts, lights and trigger | Rescue area | Project-owned | Project URP materials | Separate trigger; visuals have no colliders | Level02 |
| `Assets/Prefabs/Gameplay/InteractableDoor.prefab` | Frame/leaf/handle composition | Facility door | Project-owned | Project URP materials | Dedicated leaf BoxCollider | Level01 |

The imported packages also contain UI textures, sprites, fonts, materials, animations and demo prefabs. They are presentation resources rather than suitable replacements for the required gameplay models. Every selected asset above is instantiated by `GuardianMistiGameBuilder`; validator checks verify required renderers and asset paths.


## Verified external assets (2026-07-24)

Kenney Furniture Kit 1.0 (CC0): desk, desk chair, open bookcase and doorway FBX meshes are preserved under `Assets/ThirdParty/Kenney/FurnitureKit` and used through project-owned URP wrapper prefabs in Level01.

Kenney Survival Kit 2.0 (CC0): rock, signpost and rescue-tent FBX meshes are preserved under `Assets/ThirdParty/Kenney/SurvivalKit` and used through project-owned URP wrapper prefabs in Level02. Archive hashes, exact source URLs, licenses, prefab paths and scene usage are recorded in `Artifacts/ExternalAssetManifest.json`.
