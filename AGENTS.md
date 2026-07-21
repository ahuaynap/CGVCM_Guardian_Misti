# Guardian Misti — Project Instructions

## 1. Project Goal

Guardian Misti is a short first-person Unity game focused on exploration, interaction, objectives and environmental progression.

The final deliverable must be a complete playable experience, not only a UI prototype.

The project must contain at least three functional scenes:

1. MainMenu
2. Level01
3. Level02

The player must be able to start the game from MainMenu, complete Level01, transition to Level02 and finish the game.

The project should feel cohesive, stable and presentable for an academic delivery and portfolio repository.

---

## 2. Unity Environment

- Unity version: 6000.5.3f1
- Platform: Ubuntu 24.04
- Language: C#
- Input system: Unity Input System
- UI text: TextMeshPro
- Version control: Git
- IDE: Visual Studio Code
- Do not upgrade the Unity version.
- Do not change render pipeline or major project packages unless explicitly requested.
- Preserve Linux compatibility.

---

## 3. Development Priorities

Prioritize work in this order:

1. Playable game flow
2. Stable scene transitions
3. Core gameplay systems
4. Objective progression
5. Interaction reliability
6. Player feedback
7. UI and visual polish
8. Optional enhancements

Do not spend excessive time polishing a visual element while the complete game flow is still unfinished.

A functional vertical slice is more important than adding many disconnected features.

---

## 4. Required Scene Flow

The required flow is:

MainMenu
    ->
Level01
    ->
Level02
    ->
Game completion screen or return to MainMenu

### MainMenu requirements

The MainMenu scene must contain:

- Game title
- Play button
- Exit button
- Optional credits or instructions
- Functional navigation
- Correct cursor visibility and lock state

### Level01 requirements

Level01 must:

- Introduce movement and interaction
- Present a clear primary objective
- Require the player to obtain or activate something
- Use the existing interaction and inventory systems
- End with a clear transition condition to Level02

Suggested narrative role:

- Explore an initial area
- Find an emergency backpack or required item
- Unlock or activate the route toward the next area

### Level02 requirements

Level02 must:

- Reuse the established mechanics
- Add a small increase in complexity
- Require at least one multi-step objective
- Include a final destination, safe point or rescue objective
- Trigger a completion screen or return to MainMenu

Suggested narrative role:

- Navigate through a more dangerous or complex area
- Use previously learned interaction mechanics
- Reach the final safe point
- Complete the mission

---

## 5. Architecture Principles

Follow these principles:

- Single Responsibility Principle
- Clear separation between gameplay logic and presentation
- Prefer composition over inheritance
- Avoid unnecessary global state
- Avoid unnecessary Singletons
- Avoid direct dependencies between unrelated systems
- Avoid FindObjectOfType, GameObject.Find and string-based object lookup in runtime gameplay code
- Use serialized references when dependencies are scene-specific
- Use interfaces when multiple gameplay objects share behavior
- Use ScriptableObjects for reusable static definitions and configuration
- Use events only when they meaningfully reduce coupling
- Do not introduce complex architecture for small isolated requirements

Keep the architecture proportional to the size of the project.

---

## 6. Existing Systems

Preserve and improve the current systems instead of replacing them without justification.

Expected systems include:

- InteractionSystem
- IInteractable
- InteractionUIController
- InteractionPromptUI
- CrosshairUI
- Inventory system
- InventoryItemDefinition
- Objective system
- NotificationUI
- Door or environmental interaction components
- Completion UI

Before modifying a system:

1. Inspect all related scripts.
2. Search for usages and dependencies.
3. Explain the intended change.
4. Make the smallest coherent change.
5. Validate that existing behavior is preserved.

Do not rename public classes or serialized fields casually because Unity scene and prefab references may depend on them.

---

## 7. Interaction System Rules

The interaction system must:

- Detect interactable objects continuously.
- Display the prompt before the interaction key is pressed.
- Keep the crosshair visible.
- Change crosshair feedback while aiming at an interactable.
- Store the current interactable.
- Execute interaction only when the input is pressed.
- Use IInteractable as the gameplay contract.
- Avoid type checks for individual interactable classes.

Expected interaction flow:

Detect target
    ->
Update current interactable
    ->
Update interaction UI
    ->
Player presses interaction key
    ->
Execute Interact()

---

## 8. Objective System Rules

Objectives must be explicit and observable.

Each level should have:

- A current objective displayed in the HUD
- A clear completion condition
- Feedback when the objective changes
- A final objective that triggers the next scene or completion flow

Do not encode objective progression only inside UI scripts.

Gameplay systems should update objective state; UI should only display it.

Prefer a simple level-specific objective controller over a large generic quest framework.

---

## 9. Inventory Rules

The inventory is intended for progression items, not a complex RPG inventory.

Use it for objects such as:

- Emergency backpack
- Key
- Radio
- Medical kit
- Access item

Inventory item definitions should use ScriptableObjects where appropriate.

Inventory behavior must support:

- Adding an item
- Checking whether an item exists
- Preventing unintended duplicate collection
- Updating inventory UI
- Showing item-acquired notification

Do not implement item dropping, equipment grids or weight systems unless explicitly requested.

---

## 10. Scene Management

Create a centralized and simple scene-loading flow.

Requirements:

- Scene names must not be duplicated as arbitrary strings throughout the codebase.
- Prefer constants, an enum-backed mapping or a SceneLoader component.
- Scene transitions must not happen from UI presentation scripts.
- Prevent repeated transition requests.
- Handle cursor lock state correctly when entering or leaving gameplay.
- All required scenes must be added to the Unity build profile/build settings.

Suggested scene names:

- MainMenu
- Level01
- Level02

Do not modify Unity scene YAML manually unless there is no safer alternative.

Prefer creating scripts and then giving clear Unity Editor setup instructions for scene and prefab wiring.

---

## 11. UI Responsibilities

UI scripts must only manage presentation and user input related to UI.

The Canvas should remain organized by responsibility:

Canvas
├── HUD
│   ├── Crosshair
│   ├── InteractionPrompt
│   ├── ObjectivePanel
│   └── InventoryPanel
├── Notifications
│   └── NotificationPanel
└── Screens
    ├── PausePanel
    └── CompletionPanel

MainMenu should use its own Canvas.

Imported UI packages should provide visual assets, not dictate the game architecture.

It is acceptable to reuse:

- Sprites
- Background panels
- Icons
- Animations
- Fonts
- Modal visual components

Avoid depending on imported UI managers when the existing project architecture already controls the behavior.

---

## 12. Imported Assets

Use imported assets selectively.

Modern UI Pack V4:

- Main menu
- HUD panels
- Notifications
- Completion screen
- Buttons and modal visuals

Off Screen Indicator:

- Emergency backpack
- Safe point
- Important remote objectives

Curved UI:

- Only for optional world-space terminals or in-world screens
- Do not use it as the primary FPS HUD

Do not import or enable additional large packages without approval.

---

## 13. Player Experience

The player should always understand:

- What the current objective is
- What can be interacted with
- What item was obtained
- Why an action failed
- Where to go next when direction is necessary
- When a level has been completed

Use feedback such as:

- Prompt text
- Crosshair state
- Notifications
- Objective updates
- Off-screen objective indicators
- Completion screen

Avoid excessive tutorial text.

---

## 14. Code Style

Use English for:

- Class names
- Method names
- Variable names
- Folder names
- Code comments

Player-facing UI text may be Spanish.

C# conventions:

- PascalCase for classes, methods and public properties
- camelCase for local variables and parameters
- `_camelCase` for private fields when practical
- `[SerializeField] private` instead of public fields
- One primary MonoBehaviour class per file
- File name must match class name
- Use explicit access modifiers
- Avoid deeply nested conditionals
- Prefer early returns
- Do not suppress warnings without justification

Example:

```csharp
[SerializeField] private InteractionUIController interactionUI;

private IInteractable currentInteractable;
```

---

## 15. Unity Safety Rules
Unity serialization is fragile.

Therefore:

- Do not rename serialized fields without using FormerlySerializedAs or providing migration instructions.
- Do not change a MonoBehaviour namespace casually.
- Do not delete scripts referenced by scenes or prefabs without verifying usages.
- Do not manually edit .meta identifiers.
- Keep .meta files under version control.
- Do not commit generated folders such as Library, Temp, Logs or obj.
- Preserve prefab and scene references.
- Avoid mass formatting of Unity YAML files.

Before changing scenes, prefabs or ScriptableObjects, explain whether the task requires manual Unity Editor work.

---

## 16. Testing and Validation

After each code change:

1. Check C# compilation.
2. Search for broken references.
3. Review the diff.
4. Report which Unity Editor references must be assigned.
5. Report how to test the feature in Play Mode.

When possible, run non-Unity static checks.

Do not claim that a scene or prefab works unless it has actually been validated in Unity.

For every task, provide a concise manual test checklist.

Example:

- Open Level01.
- Enter Play Mode.
- Aim at the backpack.
- Confirm the prompt appears.
- Press E.
- Confirm the notification appears.
- Confirm the objective advances.

---

## 17. Git Workflow

Before major changes:

- Inspect git status.
- Do not overwrite unrelated uncommitted work.
- Keep changes focused.
- Show the final diff summary.
- Recommend a commit message.

Do not execute destructive Git commands.

Never run:

- git reset --hard
- git clean -fd
- force push

unless explicitly requested.

Suggested commit style:

- feat: add main menu scene flow
- feat: implement level transition system
- fix: prevent repeated item collection
- refactor: decouple interaction UI
- polish: improve objective feedback

---

## 18. Codex Working Method

For any non-trivial task:

1. Inspect the repository.
2. Identify relevant scripts, scenes and prefabs.
3. Summarize the current architecture.
4. Propose a short implementation plan.
5. Wait for confirmation when the change affects multiple systems, scenes or serialized data.
6. Implement focused code changes.
7. Review the diff.
8. Provide Unity Editor setup steps.
9. Provide a Play Mode validation checklist.
10. Suggest the next highest-priority task.

Do not implement unrelated improvements during a focused task.

Do not silently rewrite systems that already work.

---

## 19. Current Delivery Scope

The minimum delivery scope is:

- MainMenu scene
- Level01 scene
- Level02 scene
- Play button
- Exit button
- Functional player movement
- Functional camera
- Interaction system
- At least three interactable object types
- Progression inventory
- Objectives in both levels
- Item-acquired notification
- Scene transitions
- Final completion screen
- Basic visual and audio feedback
- Stable Play Mode execution
- Build configuration containing all required scenes

Preferred enhancements after the minimum scope is functional:

- Pause menu
- Off-screen objective indicators
- Audio mixer and volume controls
- Simple environmental audio
- Screen fade transitions
- Restart level
- Main menu return
- Basic checkpoints
- Improved lighting
- README documentation
- Screenshots or gameplay GIFs

---

## 20. Definition of Done

The project is done when:

- It launches into MainMenu.
- Play loads Level01.
- Level01 has a complete objective flow.
- Completing Level01 loads Level02.
- Level02 has a complete objective flow.
- Completing Level02 shows the final completion state.
- The player can return to MainMenu or exit.
- No blocking console errors occur.
- All required scenes are included in the build configuration.
- Core interactions work consistently.
- The repository includes clear setup and play instructions.