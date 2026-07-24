using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    private void OnEnable()
    {
        CursorState.ApplyMenuMode();
    }

    public void StartGame()
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("MainMenuController requires a SceneLoader reference.", this);
            return;
        }

        sceneLoader.LoadLevel01();
    }

    public void QuitGame()
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("MainMenuController requires a SceneLoader reference.", this);
            return;
        }

        sceneLoader.QuitGame();
    }

    private void OnValidate()
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("MainMenuController requires a SceneLoader reference.", this);
        }
    }
}
