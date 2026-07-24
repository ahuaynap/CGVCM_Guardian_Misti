# Guardian Misti

Guardian Misti is a first-person educational earthquake-response simulation built with Unity 6000.5.3f1. The playable flow is `MainMenu → Level01 → Level02 → results`.

Guardian Misti is an educational simulation and is not an official emergency-response certification tool.

## Controls

- WASD: move
- Mouse: look
- E: interact
- Esc: pause/resume

## Game flow

In Level01, complete the countdown and earthquake protection objective, then leave the room, collect the emergency backpack, activate the evacuation terminal, and reach the exit. In Level02, collect the emergency radio and access key, activate the gated emergency beacon, and reach the safe zone. The completion screen can restart Level02, return to MainMenu, or exit.

The cross-scene timer starts with the earthquake and pauses with the simulation. Results include level and total times, mistakes, hazards, a transparent 1000-point score, grade and locally stored best time. Difficulty profiles (Básico, Intermedio, Avanzado) tune duration, shake and target time. Optional anonymous research JSON is written locally under `Application.persistentDataPath/GuardianMistiResearch`; it contains no personal data, performs no upload, and supports analysis of evacuation time, navigation efficiency, objective errors and visual guidance.

## Architecture

Project-owned runtime systems provide raycast interaction through `IInteractable`, guarded per-level objective sequences, a progression-only inventory, centralized scene loading, pause/cursor control, notifications, and completion state. Starter Assets supplies first-person movement and Input System integration. Presentation remains in project-owned HUD/menu controllers.

All required scenes, prefabs, ScriptableObjects, materials, object references, UI events, and build settings are generated idempotently by `Assets/Editor/GuardianMistiGameBuilder.cs`. Validation is implemented by `Assets/Editor/GuardianMistiProjectValidator.cs`.

## Open and play

Open the repository with Unity 6000.5.3f1, open `Assets/Scenes/MainMenu.unity`, and enter Play Mode. No Inspector setup is required.

## Automated generation

```bash
/home/alferhp/Unity/Hub/Editor/6000.5.3f1/Editor/Unity -batchmode -nographics -quit -projectPath /home/alferhp/Guardian-Misti -executeMethod GuardianMistiGameBuilder.BuildCompleteGame -logFile /home/alferhp/Guardian-Misti/Logs/guardian-misti-build.log
```

The same action is available in Unity at **Guardian Misti > Build Complete Game**.

## Validation and tests

```bash
/home/alferhp/Unity/Hub/Editor/6000.5.3f1/Editor/Unity -batchmode -nographics -quit -projectPath /home/alferhp/Guardian-Misti -executeMethod GuardianMistiProjectValidator.ValidateProject -logFile /home/alferhp/Guardian-Misti/Logs/guardian-misti-validation.log

/home/alferhp/Unity/Hub/Editor/6000.5.3f1/Editor/Unity -batchmode -nographics -projectPath /home/alferhp/Guardian-Misti -runTests -testPlatform EditMode -testResults /home/alferhp/Guardian-Misti/Logs/editmode-results.xml -logFile /home/alferhp/Guardian-Misti/Logs/guardian-misti-editmode-tests.log
```

## Linux build

```bash
/home/alferhp/Unity/Hub/Editor/6000.5.3f1/Editor/Unity -batchmode -nographics -quit -projectPath /home/alferhp/Guardian-Misti -buildLinux64Player /home/alferhp/Guardian-Misti/Builds/Linux/GuardianMisti/GuardianMisti.x86_64 -logFile /home/alferhp/Guardian-Misti/Logs/guardian-misti-linux-build.log
```

Generated build binaries are excluded from version control.

## Final stabilization and presentation

The final builder creates lifecycle-safe interaction UI, centralized reversible gameplay input, tuned CharacterController/camera settings, composed emergency props, industrial Level01 dressing, volcanic Level02 landmarks, realtime guidance lights, URP post-processing profiles, atmospheric dust, coherent pause/completion UI, mouse sensitivity and master-volume controls. No audio files suitable for the game were present, so audio hooks remain optional and null-safe.

GPU review screenshots are generated under `Artifacts/Screenshots` with:

```bash
/home/alferhp/Unity/Hub/Editor/6000.5.3f1/Editor/Unity -batchmode -quit -projectPath /home/alferhp/Guardian-Misti -executeMethod GuardianMistiScreenshotCapture.CaptureScreenshots -logFile /home/alferhp/Guardian-Misti/Logs/guardian-misti-screenshots-gpu.log
```

`-nographics` must not be used for screenshots because it selects Unity's NullGfxDevice.

## Known limitations

The repository contains no dedicated emergency-object meshes or usable audio, so required objects are project-owned multi-part low-poly compositions and audio hooks remain null-safe. Seismic intensity is an educational animation curve, not a Richter-scale or structural-physics model.
