using UnityEngine;

public class LevelExitController : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private GameScene nextScene = GameScene.Level02;
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private string objectiveId = GameIds.Level01ReachExit;
    private bool transitionRequested;
    private void OnTriggerEnter(Collider other)
    {
        if (transitionRequested || !other.CompareTag("Player") || objectivesManager == null) return;
        if (!objectivesManager.TryCompleteObjective(objectiveId)) return;
        transitionRequested = sceneLoader != null && sceneLoader.TryLoadScene(nextScene);
    }
}
