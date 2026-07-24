using UnityEngine;

public class EvacuationTerminalController : MonoBehaviour, IInteractable
{
    [SerializeField] private string objectiveId = GameIds.Level01ActivateEvacuation;
    [SerializeField] private string requiredItemId = GameIds.EmergencyBackpack;
    [SerializeField] private NotificationUI notificationUI;
    [SerializeField] private Renderer statusRenderer;
    [SerializeField] private Color activeColor = new(0.1f, 1f, 0.35f);
    private bool activated;
    private float nextFailureTime;
    public string Prompt => activated ? "Salida activada" : "Activar salida de evacuación";
    public void Interact()
    {
        if (activated || ObjectivesManager.Instance == null || !ObjectivesManager.Instance.IsCurrentObjective(objectiveId)) return;
        if (InventoryManager.Instance == null || !InventoryManager.Instance.HasItem(requiredItemId))
        { if (Time.unscaledTime < nextFailureTime) return; nextFailureTime = Time.unscaledTime + 1.5f; notificationUI?.ShowMessage("Acceso denegado", "Necesitas la mochila de emergencia."); return; }
        activated = ObjectivesManager.Instance.TryCompleteObjective(objectiveId);
        if (activated && statusRenderer != null) statusRenderer.material.color = activeColor;
    }
}
