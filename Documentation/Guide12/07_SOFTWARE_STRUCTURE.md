# Estructura de software

Carpetas: `Scenes`, `Scripts`, `Prefabs`, `Art`, `ScriptableObjects`, `Settings`.

```text
PlayerInput → movimiento / cámara / agachado / interacción
InteractionSystem → IInteractable → puerta / ítems / baliza
EarthquakeController → ProtectionTrigger → ObjectivesManager → HUD
AftershockController → RiskZone
InventoryManager → Collectibles
SimulationSession → ResultsUI
```

Flujo: `MainMenu → Level01 → Level02 → GameCompletionUI`.

`IInteractable` separa detección y objetos. ScriptableObjects contienen perfiles e ítems. Prefabs separan presentación reusable y lógica.

Scripts principales: `PlayerLookController`, `PlayerCrouchController`, `InteractionSystem`, `ObjectivesManager`, `InventoryManager`, `EarthquakeController`, `EarthquakeProtectionTrigger`, `AftershockController`, `DoorController`, `SafeZoneController`, `SimulationSession`.

Prefabs: `GameplayPlayer`, `GameplaySystems`, `GameplayHUD`, `InteriorProtectionZone`, `FacilityExitDoor`, `EmergencyBackpack`, `SafeZone` y siete visuales AssetHub.
