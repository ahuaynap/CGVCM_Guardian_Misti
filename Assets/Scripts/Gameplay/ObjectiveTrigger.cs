using UnityEngine;

public sealed class ObjectiveTrigger : MonoBehaviour
{
    [SerializeField] private string objectiveId;
    [SerializeField] private NotificationUI notificationUI;
    [SerializeField] private string successMessage = "Zona segura alcanzada.";
    private bool completed;

    private void OnTriggerEnter(Collider other)
    {
        if (completed || !other.CompareTag("Player") || ObjectivesManager.Instance == null) return;
        completed = ObjectivesManager.Instance.TryCompleteObjective(objectiveId);
        if (completed) notificationUI?.ShowMessage("Objetivo completado", successMessage);
    }
}
