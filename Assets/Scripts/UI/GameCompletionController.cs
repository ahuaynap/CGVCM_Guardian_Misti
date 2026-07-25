using UnityEngine;

public class GameCompletionController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private GameplayStateController stateController;
    [SerializeField] private PauseController pauseController;
    private bool applied;
    public void EnterCompletionMode()
    {
        if (applied) return; applied = true;
        stateController?.RequestState(GameplayState.Completed);
    }
    public void ReturnToMainMenu() => sceneLoader?.LoadMainMenu();
    public void RestartCurrentLevel() { SimulationSession.Instance?.ResetRun(); sceneLoader?.LoadLevel01(); }
    public void QuitGame() => sceneLoader?.QuitGame();
}
