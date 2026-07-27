# Controles y flujo

| Acción | Teclado/ratón | Gamepad declarado | Nota |
|---|---|---|---|
| Move | WASD/flechas | leftStick | menor velocidad agachado |
| Look | delta ratón | rightStick | inactivo en pausa |
| Interact | E | buttonNorth | prompt contextual |
| Crouch | Ctrl izquierdo mantenido | buttonEast | baja cápsula; sin salto |
| Jump | Espacio | buttonSouth | rechazado agachado |
| Sprint | Shift izquierdo | leftStickPress | definido |
| Pause | Esc | no confirmado | libera cursor |
| Diagnostics | F3 | ninguno | evaluador |
| UI | ratón/Enter/Cancel | esquema UI | menús |

Sensibilidad base 1,5, rango 0,25–4,0. Ratón sin `deltaTime`; stick con tiempo de frame.

Flujo: abrir → Jugar → HUD Level01 → Ctrl bajo mesa/E para objetos → Esc pausa → Level02 → radio/llave/baliza → tienda → resultados.

Mismatches: gamepad no probado físicamente; pausa de gamepad no confirmada; `Attack` existe sin mecánica; F3 no es contenido educativo.
