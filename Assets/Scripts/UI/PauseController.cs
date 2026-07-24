using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private GameplayInputController inputController;
    public bool IsPaused => inputController != null && inputController.State == GameplayInputState.Paused;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
    }
    public void TogglePause() { if (IsPaused) Resume(); else Pause(); }
    public void Pause()
    {
        if (inputController == null || inputController.State == GameplayInputState.Completed) return;
        inputController.EnterPause(); if (pausePanel != null) pausePanel.SetActive(true);
    }
    public void Resume()
    {
        if (!IsPaused) return; if (pausePanel != null) pausePanel.SetActive(false); inputController.EnterGameplay();
    }
    public void Reload() { Time.timeScale = 1f; sceneLoader?.ReloadCurrentScene(); }
    public void MainMenu() { Time.timeScale = 1f; sceneLoader?.LoadMainMenu(); }
}
