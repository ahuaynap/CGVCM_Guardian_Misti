using UnityEngine;

public class CollectibleItemController : MonoBehaviour, IInteractable
{
    [SerializeField] private InventoryItemDefinition definition;
    [SerializeField] private string objectiveId;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectionClip;
    public string Prompt => definition == null ? "Recoger objeto" : $"Recoger {definition.Name.ToLowerInvariant()}";
    public void Interact()
    {
        if (definition == null || InventoryManager.Instance == null || ObjectivesManager.Instance == null) return;
        if (!ObjectivesManager.Instance.IsCurrentObjective(objectiveId)) return;
        if (!InventoryManager.Instance.AddItem(new InventoryItem(definition))) return;
        ObjectivesManager.Instance.TryCompleteObjective(objectiveId);
        if (audioSource != null && collectionClip != null) audioSource.PlayOneShot(collectionClip);
        gameObject.SetActive(false);
    }
}
