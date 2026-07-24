using UnityEngine;

public class EmergencyBeaconController : MonoBehaviour, IInteractable
{
    [SerializeField] private string objectiveId = GameIds.Level02ActivateBeacon;
    [SerializeField] private string radioItemId = GameIds.EmergencyRadio;
    [SerializeField] private string accessKeyItemId = GameIds.AccessKey;
    [SerializeField] private NotificationUI notificationUI;
    [SerializeField] private Renderer statusRenderer;
    private bool activated;
    private float nextFailureTime;
    public string Prompt => activated ? "Baliza activada" : "Activar baliza de emergencia";
    public bool HasRequirements(InventoryManager inventory) => inventory != null && inventory.HasItem(radioItemId) && inventory.HasItem(accessKeyItemId);
    public void Interact()
    {
        if (activated || ObjectivesManager.Instance == null || !ObjectivesManager.Instance.IsCurrentObjective(objectiveId)) return;
        if (!HasRequirements(InventoryManager.Instance))
        { if (Time.unscaledTime < nextFailureTime) return; nextFailureTime = Time.unscaledTime + 1.5f; notificationUI?.ShowMessage("Faltan suministros", "Necesitas la radio de emergencia y la llave de acceso."); return; }
        activated = ObjectivesManager.Instance.TryCompleteObjective(objectiveId);
        if (activated && statusRenderer != null) statusRenderer.material.color = Color.green;
    }
}
