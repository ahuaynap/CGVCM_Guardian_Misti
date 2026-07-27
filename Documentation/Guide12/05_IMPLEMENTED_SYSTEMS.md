# Sistemas implementados

| Sistema | Scripts | Estado/limitación |
|---|---|---|
| Movimiento FPS | `FirstPersonController`, inputs | verificado |
| Cámara | `PlayerLookController` | verificado; mando pendiente |
| Agachado | `PlayerCrouchController` | verificado |
| Interacción | `InteractionSystem`, `IInteractable` | activo |
| Objetivos | `ObjectivesManager` | verificado |
| Inventario/colección | `InventoryManager`, `CollectibleItemController` | verificado |
| Puerta | `DoorController` | Level01 |
| Protección | `EarthquakeProtectionTrigger` | crouch + 2 s |
| Sismo | `EarthquakeController` | Level01 |
| Réplica/riesgos | `AftershockController`, `AftershockRiskZone` | Level02; sin clip |
| Zona segura | `SafeZoneController` | trigger verificado |
| Métricas | `SimulationSession`, metrics | runtime |
| UI | objetivo, inventario, prompts, pausa | TMP/uGUI |
| Escenas/resultados | `SceneLoader`, `GameCompletionUI` | activo |
| Audio | AudioSources/hook | clips ausentes |
| VFX | polvo, shake, luces, props | sin perfilado |
| Diagnóstico | `GameplayDiagnostics` | F3 |

PlayerInput alimenta control/interacción; objetivos condicionan objetos; sismo condiciona protección/puerta; inventario y baliza avanzan Level02; SafeZone cierra la sesión.
