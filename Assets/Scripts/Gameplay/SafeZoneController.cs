using UnityEngine;

public class SafeZoneController : MonoBehaviour
{
    private bool hasCompletedObjective;

    private void OnTriggerEnter(Collider other)
    {
        if (hasCompletedObjective || !other.CompareTag("Player"))
        {
            return;
        }

        if (ObjectivesManager.Instance == null)
        {
            Debug.LogWarning("ObjectivesManager not found.", this);
            return;
        }

        hasCompletedObjective = ObjectivesManager.Instance.TryCompleteObjective(
            GameIds.ReachSafeZoneObjective);
    }
}
