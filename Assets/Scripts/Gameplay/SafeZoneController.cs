using UnityEngine;

public class SafeZoneController : MonoBehaviour
{
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private string objectiveId = GameIds.Level02ReachSafeZone;
    [SerializeField] private GameCompletionUI completionUI;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string nextScene;
    private bool completed;
    private void OnTriggerEnter(Collider other)
    {
        if (completed || !other.CompareTag("Player") || objectivesManager == null) return;
        completed = objectivesManager.TryCompleteObjective(objectiveId);
        if (completed && objectivesManager.IsSimulationCompleted)
        {
            if (!string.IsNullOrWhiteSpace(nextScene) && sceneLoader != null) sceneLoader.TryLoadSceneByName(nextScene);
            else { SimulationSession.Instance?.StopTimer(); completionUI?.Show(); }
        }
    }
}
