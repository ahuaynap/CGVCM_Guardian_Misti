# Escenas y jugabilidad

## MainMenu

`Assets/Scenes/MainMenu.unity`. Presenta título, instrucciones, ajustes, inicio y salida. `Jugar` carga Level01.

## Level01

Sala interior y corredor. El sismo usa Light, Moderate y Strong. La protección bajo `AssetHub_CommandDesk` exige agachado y 2 s continuos; salir o levantarse reinicia. Éxito o falla permiten continuar. Luego: puerta, mochila, terminal y salida.

| Orden | ID | Texto |
|---|---|---|
| 1 | `level01_preparation` | Prepárate para el inicio del simulacro. |
| 2 | `level01_protect` | Protégete durante el sismo. |
| 3 | `level01_exit_room` | Sal de la habitación. |
| 4 | `level01_collect_backpack` | Encuentra la mochila de emergencia. |
| 5 | `level01_activate_evacuation` | Activa la salida de evacuación. |
| 6 | `level01_reach_exit` | Dirígete al punto de salida. |

`Preparación → Protección → Resolución → Puerta → Mochila → Terminal → Salida → Level02`

## Level02

Ruta exterior con torre, generador, cajas y tienda médica. Réplica: orientación, Warning, Light, Moderate, Decreasing y Finished; no quita movimiento/cámara. Riesgos registran exposición sin bloquear.

| Orden | ID | Texto |
|---|---|---|
| 1 | `level02_collect_radio` | Encuentra la radio de emergencia. |
| 2 | `level02_collect_access_key` | Encuentra la llave de acceso. |
| 3 | `level02_activate_beacon` | Activa la baliza de emergencia. |
| 4 | `level02_reach_safe_zone` | Llega a la zona segura. |

`Radio → Llave → Baliza → Ruta/réplica → Tienda médica → Zona segura → Resultado`

La ruta se validó con cápsula. El trigger final no es sólido. `RumbleSource` existe, pero no hay clip.

## Resultado

No hay escena Results separada. `GameCompletionUI` aparece dentro de Level02 al completar `SafeZoneController`.
