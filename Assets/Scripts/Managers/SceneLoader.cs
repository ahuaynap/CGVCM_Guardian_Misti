using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameplayStateController stateController;
    private bool isLoading;

    public void LoadMainMenu()
    {
        TryLoadScene(SceneNames.MainMenu);
    }

    public void LoadLevel01()
    {
        TryLoadScene(SceneNames.Level01);
    }

    public void LoadLevel02()
    {
        TryLoadScene(SceneNames.Level02);
    }

    public void LoadSimulation()
    {
        TryLoadScene(SceneNames.Simulation);
    }

    public void ReloadCurrentScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        TryLoadScene(activeScene.name);
    }

    public bool TryLoadScene(GameScene scene)
    {
        return TryLoadScene(SceneNames.GetName(scene));
    }

    private bool TryLoadScene(string sceneName)
    {
        if (isLoading)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Cannot load a scene with an empty name.", this);
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning(
                $"Scene '{sceneName}' is not available in the build configuration.",
                this);
            return false;
        }

        isLoading = true;
        Time.timeScale = 1f;
        stateController?.RequestState(GameplayState.Transitioning);
        SceneManager.LoadScene(sceneName);
        return true;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit requested. Application.Quit is ignored in the Unity Editor.", this);
#else
        Application.Quit();
#endif
    }
}
