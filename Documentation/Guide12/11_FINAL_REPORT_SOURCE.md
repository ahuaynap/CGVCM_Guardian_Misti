# Informe final — texto fuente

## PORTADA

[INSTITUCIÓN] · [CURSO/SECCIÓN] · **Guardian Misti** · [AUTORES] · [DOCENTE] · [FECHA]

## RESUMEN

Guardian Misti es una simulación educativa breve desarrollada en Unity y presentada desde primera persona. Su experiencia combina protección interior durante un sismo y evacuación exterior durante una réplica. El jugador debe interpretar objetivos, agacharse bajo una mesa, interactuar con equipos de emergencia y llegar a una zona segura. La versión evaluada contiene menú, dos niveles y estado final. Las pruebas automatizadas disponibles aprobaron 87 de 87 casos; no se realizó estudio con usuarios ni perfilado formal.

**Palabras clave:** simulación educativa, sismo, evacuación, primera persona, Unity, interacción.

## 1. INTRODUCCIÓN

El proyecto utiliza un entorno interactivo para presentar decisiones básicas de autoprotección y evacuación. Busca reforzar instrucciones mediante acción contextual, sin afirmar que sustituye entrenamiento oficial.

[FIGURA IF-01: menú principal y título]

## 2. OBJETIVOS

### 2.1 General

Desarrollar una experiencia interactiva breve que permita practicar una secuencia básica de protección y evacuación sísmica.

### 2.2 Específicos

- Implementar control FPS, agachado e interacción.
- Representar sismo interior y réplica exterior sin quitar control.
- Guiar mediante objetivos, señales, iluminación y feedback.
- Registrar métricas de reacción y exposición.
- Integrar assets 3D reutilizables y verificar el flujo completo.

## 3. FUNDAMENTACIÓN Y PROYECCIÓN SOCIAL

### 3.1 Contexto y justificación

La preparación ante emergencias requiere instrucciones comprensibles y oportunidades de práctica. Guardian Misti presenta un escenario simulado que relaciona texto, espacio y acción.

### 3.2 Contribución

Se orienta a apoyar demostraciones educativas y discusión guiada. Su contribución es experiencial y técnica, no una eficacia educativa científicamente validada.

### 3.3 Alcance

Dos niveles, español y escritorio. Sin estudio formal, certificación ni generalización de resultados.

## 4. DESCRIPCIÓN DEL PROYECTO

### 4.1–4.3 Aplicación, público y flujo

Experiencia FPS corta para público académico/general no delimitado por edad. Flujo: menú → Level01 → Level02 → final.

[FIGURA IF-02: diagrama de escenas]

### 4.4 Nivel 1

Preparación, sismo, protección agachada durante 2 s, puerta, mochila, terminal y salida.

[FIGURA IF-03: protección bajo AssetHub_CommandDesk]

### 4.5 Nivel 2

Radio, llave, baliza, réplica, ruta señalizada, torre, generador y tienda médica final.

[FIGURA IF-04: Level02 con torre, generador y tienda]

### 4.6 Mecánicas

Movimiento, cámara, salto, agachado, interacción, inventario, objetivos, riesgo, métricas y resultados.

[TABLA IF-01: objetivos exactos e IDs de ambos niveles]

## 5. DISEÑO E IMPLEMENTACIÓN

### 5.1 Organización

Escenas, scripts runtime, prefabs, arte y configuraciones se mantienen separados.

### 5.2–5.5 Movimiento, cámara, interacción, objetivos e inventario

PlayerInput alimenta control FPS. El ratón produce giro directo; la interacción usa raycast e interfaz `IInteractable`. Objetivos ordenan la experiencia e inventario conserva ítems de progreso.

### 5.6 Protección sísmica

Valida fase activa, entrada, agachado y dwell continuo. La falla penaliza y permite continuar.

### 5.7 Sismo y réplica

El sismo principal incluye fases de intensidad; la réplica exterior usa Warning, Light, Moderate, Decreasing y Finished. Los efectos son visuales/aditivos y no mueven el jugador.

### 5.8–5.11 Zona segura, interfaz, audio y resultados

La zona final es un trigger accesible. HUD y notificaciones comunican estado. Hay arquitectura de audio, pero no clips locales confirmados. El panel final resume la sesión.

[FIGURA IF-05: HUD anotado]  
[TABLA IF-02: scripts y responsabilidades]

## 6. PRODUCCIÓN GRÁFICA Y MULTIMEDIA

### 6.1 Dirección visual

Paleta oscura con cian seguro y naranja/rojo de advertencia.

### 6.2–6.4 Modelos, AssetHub y Blender

El flujo de producción fue concepto → AssetHub → revisión en Blender → FBX → Unity. Los modelos se convirtieron en prefabs y recibieron escala, materiales y colliders funcionales. Blender requiere captura externa porque no existe `.blend` versionado.

[FIGURA IF-06: comparación referencia/resultado AssetHub]  
[FIGURA IF-07: modelo revisado en Blender]

### 6.5–6.8 Materiales, iluminación, VFX y sonido

Materiales URP/Lit, texturas de tienda, iluminación cian moderada, polvo y shake separado. No hay clips locales, por lo que el sonido es una limitación.

[TABLA IF-03: inventario de assets 3D]

## 7. TECNOLOGÍAS

Unity 6000.5.3f1, C#, URP 17.5.0, Input System 1.19.0, TextMeshPro/uGUI, Blender y AssetHub.

[TABLA IF-04: paquetes y versiones]

## 8. PRUEBAS Y RESULTADOS

### 8.1–8.3 Estrategia y resultados

Compilación, pruebas EditMode, fixtures con entrada a Play Mode, revisión de referencias y matriz manual. Resultado automatizado: 87/87.

### 8.4 Incidencias

Se corrigieron referencias de audio opcional y orientación del collider posterior de la tienda. No se deben convertir estas incidencias en afirmaciones de perfección.

### 8.5 Límites

Target PlayMode separado descubrió 0 pruebas; no hubo ensayo formal de usuario ni perfilado.

[TABLA IF-05: matriz de pruebas y estado]

## 9. LECCIONES APRENDIDAS

La coherencia entre señalización, colisión, objetivos y feedback es tan importante como la presencia del modelo visual. Los assets generados requieren adaptación funcional y revisión humana.

## 10. LIMITACIONES Y TRABAJO FUTURO

Audio, accesibilidad, gamepad, perfilado, estudio de usuarios y refinamiento de assets.

## 11. CONCLUSIONES

La versión actual implementa un flujo verificable de dos niveles que permite practicar acciones de protección y evacuación. La evidencia sostiene la funcionalidad técnica disponible, pero no una eficacia educativa medida.

## BIBLIOGRAFÍA

[COMPLETAR únicamente con fuentes reales consultadas: normas de protección civil, documentación Unity, licencias AssetHub/Blender.]

## ANEXOS

Inventarios CSV, matriz de evidencia, pruebas, capturas, licencia de assets y enlace al video.
