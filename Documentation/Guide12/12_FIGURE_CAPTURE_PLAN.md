# Plan de figuras

| ID | Destino | Escena/momento | UI/cámara/objetos | Demuestra | Ahora/bloqueo |
|---|---|---|---|---|---|
| F01 | manual/informe | MainMenu inicial | frontal, título/botones | inicio | sí |
| F02 | informe | Level01 inicio | HUD, vista general | composición | sí |
| F03 | informe | sismo activo | intensidad/polvo | evento | sí |
| F04 | ambos | bajo mesa | crouch+dwell, señal | protección | sí |
| F05 | anexo | prefab mesa | Scene view colliders/trigger | colisión | sí |
| F06 | ambos | mochila | prompt+inventario | colección | sí |
| F07 | ambos | puerta antes/después | prompt/hoja abierta | gate | sí |
| F08 | informe | área protegida | luces cian sin sobreexposición | guía | sí |
| F09 | informe | Level02 entrada | ruta exterior | nivel | sí |
| F10 | ambos | réplica Moderate | warning/polvo/props | réplica | sí |
| F11 | informe | comunicaciones | torre+generador | landmarks | sí |
| F12 | ambos | aproximación final | entrada tienda abierta | destino | sí |
| F13 | ambos | dentro SafeZone | trigger/landmark | final | sí |
| F14 | ambos | panel final | resultados completos | cierre | sí |
| F15 | anexo | prefab representativo | hierarchy Visual/Colliders | reutilización | sí |
| F16 | informe | referencia y modelo | comparación AssetHub | producción | requiere fuente |
| F17 | informe | Blender | viewport con escala/material | revisión | requiere archivo/captura |
| F18 | manual | instrucciones | controles visibles | operación | sí |
| F19 | informe | HUD objetivo | panel legible | guía | sí |
| F20 | informe | inventario | ítems adquiridos | progreso | sí |

Encuadre recomendado: 16:9, sin diagnóstico salvo F05, resolución final de entrega, textos completos, cursor/crosshair sin solapar. Capturar también consola limpia al final.

Diagramas separados:

- D01: `MainMenu → Level01 → Level02 → final`.
- D02: flujo jugable de objetivos por nivel.
- D03: PlayerInput, control, interacción, objetivos, métricas y UI.
- D04: `concepto → AssetHub → Blender → Unity`.
