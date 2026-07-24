using UnityEngine;

public class GameCompletionController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private GameplayInputController inputController;
    [SerializeField] private PauseController pauseController;
    private bool applied;
    public void EnterCompletionMode()
    {
        if (applied) return; applied = true;
        if (pauseController != null) pauseController.enabled = false;
        inputController?.EnterCompletion();
    }
    public void ReturnToMainMenu() { Time.timeScale = 1f; sceneLoader?.LoadMainMenu(); }
    public void RestartCurrentLevel() { Time.timeScale = 1f; sceneLoader?.ReloadCurrentScene(); }
    public void QuitGame() { Time.timeScale = 1f; sceneLoader?.QuitGame(); }
}
