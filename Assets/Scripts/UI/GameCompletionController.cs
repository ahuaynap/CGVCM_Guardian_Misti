using UnityEngine;

public class GameCompletionController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private Behaviour[] gameplayBehaviours;

    private bool completionModeApplied;

    private void OnEnable()
    {
        EnterCompletionMode();
    }

    public void EnterCompletionMode()
    {
        if (completionModeApplied)
        {
            return;
        }

        completionModeApplied = true;

        if (gameplayBehaviours != null)
        {
            foreach (Behaviour gameplayBehaviour in gameplayBehaviours)
            {
                if (gameplayBehaviour != null && gameplayBehaviour != this)
                {
                    gameplayBehaviour.enabled = false;
                }
            }
        }

        CursorState.ApplyMenuMode();
    }

    public void ReturnToMainMenu()
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("GameCompletionController requires a SceneLoader reference.", this);
            return;
        }

        sceneLoader.LoadMainMenu();
    }

    public void RestartCurrentLevel()
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("GameCompletionController requires a SceneLoader reference.", this);
            return;
        }

        sceneLoader.ReloadCurrentScene();
    }

    public void QuitGame()
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("GameCompletionController requires a SceneLoader reference.", this);
            return;
        }

        sceneLoader.QuitGame();
    }

    private void OnValidate()
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("GameCompletionController requires a SceneLoader reference.", this);
        }
    }
}
