using UnityEngine;

public class LevelExitController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private GameScene nextScene = GameScene.Level02;
    [SerializeField] private bool requireObjectivesCompleted = true;
    [SerializeField] private ObjectivesManager objectivesManager;

    private bool transitionRequested;

    private void OnTriggerEnter(Collider other)
    {
        if (transitionRequested || !other.CompareTag("Player"))
        {
            return;
        }

        if (requireObjectivesCompleted &&
            (objectivesManager == null || !objectivesManager.IsSimulationCompleted))
        {
            return;
        }

        if (sceneLoader == null)
        {
            Debug.LogWarning("LevelExitController requires a SceneLoader reference.", this);
            return;
        }

        transitionRequested = sceneLoader.TryLoadScene(nextScene);
    }

    private void OnValidate()
    {
        if (sceneLoader == null)
        {
            Debug.LogWarning("LevelExitController requires a SceneLoader reference.", this);
        }

        if (requireObjectivesCompleted && objectivesManager == null)
        {
            Debug.LogWarning(
                "LevelExitController requires an ObjectivesManager when objective completion is required.",
                this);
        }
    }
}
