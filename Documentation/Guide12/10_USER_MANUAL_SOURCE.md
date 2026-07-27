# Manual de usuario — texto fuente

## 1. Portada

**Guardian Misti — Manual de usuario**  
Autor(es): [COMPLETAR] · Institución/curso: [COMPLETAR] · Versión/fecha: [COMPLETAR]

## 2. Introducción y propósito

Guardian Misti presenta una simulación breve en primera persona orientada a practicar instrucciones básicas de protección sísmica y evacuación. No reemplaza capacitación oficial.

## 3. Requisitos

Se requiere un equipo capaz de ejecutar la build final de Unity, teclado y ratón. Los requisitos mínimos no fueron medidos y deben completarse tras probar el ejecutable.

## 4. Instalación e inicio

Descargue el paquete oficial, extráigalo sin separar sus archivos y ejecute `Guardian-Misti`. En Linux, habilite permiso de ejecución si la distribución lo solicita. Verifique la fuente oficial antes de ejecutar.

[FIGURA MU-01: menú principal completo con título y botones]

## 5. Menú principal

Use el ratón para elegir Jugar, ajustes disponibles o Salir. Jugar inicia la simulación.

## 6. Controles

| Control | Función |
|---|---|
| WASD | movimiento |
| Ratón | mirar |
| E | interactuar |
| Ctrl izquierdo mantenido | agacharse |
| Espacio | saltar |
| Shift izquierdo | correr |
| Esc | pausar/reanudar |
| F3 | diagnóstico para evaluación |

[FIGURA MU-02: panel de instrucciones del menú]

## 7. Interfaz

El panel de objetivo indica la tarea actual. El prompt contextual aparece al mirar un objeto utilizable. Las notificaciones informan logros o errores. El inventario muestra ítems recogidos. Durante sismos aparecen intensidad, avisos y progreso de protección.

[FIGURA MU-03: HUD con objetivo, interacción e inventario identificados]

## 8. Recorrido Level01

1. Espere la preparación y localice la mesa marcada.
2. Durante el sismo mantenga Ctrl y entre bajo la mesa.
3. Permanezca dentro hasta completar la protección.
4. Cuando termine, abra la puerta con E.
5. Recoja la mochila.
6. Active el terminal y siga la salida.

[FIGURA MU-04: jugador agachado bajo la mesa con progreso]  
[FIGURA MU-05: puerta habilitada y mochila visible]

Si no alcanza la protección, recibirá penalización, pero podrá continuar.

## 9. Recorrido Level02

1. Encuentre la radio.
2. Recoja la llave.
3. Active la baliza.
4. Durante la réplica aléjese de estructuras y zonas marcadas.
5. Use torre y generador como referencias.
6. Entre por la abertura de la tienda médica y alcance la zona segura.

[FIGURA MU-06: advertencia de réplica y señal de riesgo]  
[FIGURA MU-07: aproximación abierta a la tienda y zona segura]

## 10. Resultados

Al entrar en la zona final aparece la pantalla de misión completada con la información disponible de la sesión.

[FIGURA MU-08: pantalla final completa]

## 11. Pausa

Pulse Esc. El cursor queda disponible. Reanude desde el panel; la orientación y sensibilidad deben conservarse.

## 12. Solución de problemas

- Cámara inmóvil: reanude, haga clic en el área de juego y confirme que no hay menú abierto.
- No aparece E: mire directamente el objeto y acérquese.
- No completa protección: permanezca agachado dentro del borde hasta 100%.
- No puede abrir puerta: espere a que termine y se resuelva el sismo.
- Sin sonido de réplica: la versión analizada no incluye clip local; no impide jugar.
- Ruta final: aproxímese a la tienda por su entrada frontal.

## 13. Limitaciones, créditos y licencias

Mando no probado físicamente; audio incompleto; sin requisitos mínimos medidos. Completar autores, licencias Unity/terceros, AssetHub y términos de modelos antes de publicar.

## Tabla de figuras

| ID | Escena | Posición/estado | Propósito |
|---|---|---|---|
| MU-01 | MainMenu | frontal, menú inicial | inicio |
| MU-02 | MainMenu | instrucciones abiertas | controles |
| MU-03 | Level01 | inicio/HUD visible | interfaz |
| MU-04 | Level01 | bajo mesa, dwell | protección |
| MU-05 | Level01 | corredor pos-sismo | progresión |
| MU-06 | Level02 | réplica activa | riesgo |
| MU-07 | Level02 | frente a tienda | destino |
| MU-08 | Level02 | final completado | resultados |
