# Gráficos, multimedia y assets

La dirección visual usa bases oscuras, cian seguro, blanco de apoyo y naranja/rojo de advertencia. Level01 focaliza la mesa; Level02 emplea landmarks exteriores.

Flujo multimedia documentable: concepto → AssetHub → revisión/preparación en Blender → FBX → Unity. Los FBX se encapsulan en prefabs, normalizan y reciben URP/Lit y colliders adaptados. No hay `.blend`: Blender requiere evidencia externa. La geometría generada puede tener topología, pivotes o texto imperfectos.

| Asset | Fuente → prefab | Uso | Collider/limitación |
|---|---|---|---|
| Mochila | `Medical_Backpack.fbx` → `EmergencyBackpack_Visual.prefab` | L1 | Box; lógica wrapper |
| Mesa | `Command_Desk.fbx` → `CommandDesk.prefab` | L1 | tablero+soportes |
| Tienda | `Green_Medical_Tent.fbx` → `MedicalTent.prefab` | L2 | paredes, entrada abierta |
| Torre | `Communications_Tower.fbx` → `CommunicationsTower.prefab` | L2 | base estrecha |
| Puerta | `Sliding_Hangar_Door.fbx` → `FacilityDoor.prefab` | reusable; uso funcional no confirmado | pivote limitado |
| Generador | `Portable_Generator.fbx` → `PortableGenerator.prefab` | L2 | Box |
| Caja | `Medical_Crate.fbx` → `MedicalCrate.prefab` | L1/L2 | Box |

Materiales: `Assets/Art/Materials/AssetHub/Generated/*.mat`, URP/Lit. Tienda: Color y NormalGL. No se confirmó PBR completo.

Iluminación: Level01 usa techo/emergencia; protección con fill cian 0,52/rango 3,2 y spot blanco 0,72/rango 4,2. Level02 usa luz exterior y emisión; luz interior de tienda no confirmada.

Audio: no se encontraron `.wav`, `.mp3`, `.ogg` o `.aiff`; existen fuentes pero no debe afirmarse reproducción. VFX: polvo, shake separado, props, avisos y luz de emergencia.
