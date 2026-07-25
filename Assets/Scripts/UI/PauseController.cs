using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private GameplayStateController stateController;
    public bool IsPaused => stateController != null && stateController.State == GameplayState.Paused;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
    }
    public void TogglePause() { if (IsPaused) Resume(); else Pause(); }
    public void Pause()
    {
        if (stateController == null || stateController.State is GameplayState.Completed or GameplayState.Transitioning) return;
        SimulationSession.Instance?.RecordPause(); stateController.RequestState(GameplayState.Paused);
    }
    public void Resume()
    {
        if (!IsPaused) return; stateController.RequestState(GameplayState.Playing);
    }
    public void Reload() => sceneLoader?.ReloadCurrentScene();
    public void MainMenu() => sceneLoader?.LoadMainMenu();
}
