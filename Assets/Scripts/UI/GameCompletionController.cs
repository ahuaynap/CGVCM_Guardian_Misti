using StarterAssets;
using UnityEngine;

public class GameCompletionController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private Behaviour[] gameplayBehaviours;
    [SerializeField] private PauseController pauseController;
    [SerializeField] private StarterAssetsInputs starterInputs;
    private bool applied;
    public void EnterCompletionMode()
    {
        if (applied) return; applied = true; Time.timeScale = 1;
        if (pauseController != null) pauseController.enabled = false;
        if (gameplayBehaviours != null) foreach (Behaviour behaviour in gameplayBehaviours) if (behaviour != null && behaviour != this) behaviour.enabled = false;
        if (starterInputs != null) { starterInputs.cursorLocked = false; starterInputs.cursorInputForLook = false; }
        CursorState.ApplyMenuMode();
    }
    public void ReturnToMainMenu() => sceneLoader.LoadMainMenu();
    public void RestartCurrentLevel() => sceneLoader.ReloadCurrentScene();
    public void QuitGame() => sceneLoader.QuitGame();
}
