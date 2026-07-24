using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private Behaviour[] gameplayBehaviours;
    [SerializeField] private StarterAssetsInputs starterInputs;
    private bool paused;
    public bool IsPaused => paused;
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
    }
    public void TogglePause() { if (paused) Resume(); else Pause(); }
    public void Pause()
    {
        if (paused) return; paused = true; Time.timeScale = 0; SetGameplay(false); pausePanel.SetActive(true); CursorState.ApplyMenuMode();
    }
    public void Resume()
    {
        if (!paused) return; paused = false; Time.timeScale = 1; pausePanel.SetActive(false); SetGameplay(true); CursorState.ApplyGameplayMode();
    }
    public void Reload() { Time.timeScale = 1; sceneLoader.ReloadCurrentScene(); }
    public void MainMenu() { Time.timeScale = 1; sceneLoader.LoadMainMenu(); }
    private void SetGameplay(bool enabled)
    {
        if (gameplayBehaviours != null) foreach (Behaviour behaviour in gameplayBehaviours) if (behaviour != null) behaviour.enabled = enabled;
        if (starterInputs != null) { starterInputs.cursorLocked = enabled; starterInputs.cursorInputForLook = enabled; }
    }
}
