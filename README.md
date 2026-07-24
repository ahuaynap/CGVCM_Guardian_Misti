# Guardian Misti

Guardian Misti is a short first-person emergency-preparation and survival game built with Unity 6000.5.3f1. The playable flow is `MainMenu → Level01 → Level02 → completion screen`.

## Controls

- WASD: move
- Mouse: look
- E: interact
- Esc: pause/resume

## Game flow

In Level01, leave the room, collect the emergency backpack, activate the evacuation terminal, and reach the exit. In Level02, collect the emergency radio and access key, activate the gated emergency beacon, and reach the safe zone. The completion screen can restart Level02, return to MainMenu, or exit.

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
