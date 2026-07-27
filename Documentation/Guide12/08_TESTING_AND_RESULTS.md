# Pruebas y resultados

El 27-07-2026: compilación 0 errores; validación exitosa; suite Unity Editor 87/87, 0 fallidas/omitidas. Fixtures integradas usan `EnterPlayMode`. Target PlayMode separado descubrió 0; no añade cobertura.

| Caso | Esperado | Observado | Estado/evidencia |
|---|---|---|---|
| MainMenu | título/botones | escena build | pendiente/MU-01 |
| Movimiento/cámara/crouch | respuesta | tests | verificado/clip |
| Mesa/protección | colisión+dwell | tests | verificado/captura |
| Puerta | abre/paso | tests | verificado/video |
| Mochila/inventario | una vez | tests | verificado/UI |
| Luz segura | legible | valores | parcial/captura |
| Transición L1 | carga L2 | lógica | parcial/video |
| Réplica | fases sin error | tests | verificado/video |
| Rumble | sin excepción | fuente | verificado; sin clip |
| Ruta L2 | libre | barrido | verificado/recorrido |
| Torre/generador | visibles | presentes | parcial/captura |
| Tienda | abierta | collider | verificado/captura |
| SafeZone/resultados | completa/panel | lógica | parcial/video |
| Pausa/descarga | estable | tests/logs | parcial/clip |

No se realizó una medición formal de rendimiento.
