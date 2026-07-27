using UnityEngine;

public class SafeZoneController : MonoBehaviour
{
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private string objectiveId = GameIds.Level02ReachSafeZone;
    [SerializeField] private GameCompletionUI completionUI;
    private bool completed; private bool playerInside;
    public bool IsCompleted=>completed;
    public bool IsPlayerInside=>playerInside;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))playerInside=true;
        if (completed || !other.CompareTag("Player") || objectivesManager == null) return;
        completed = objectivesManager.TryCompleteObjective(objectiveId);
        if (completed && objectivesManager.IsSimulationCompleted) { SimulationSession.Instance?.StopTimer(); completionUI?.Show(); }
    }
    private void OnTriggerExit(Collider other){if(other.CompareTag("Player"))playerInside=false;}
}
