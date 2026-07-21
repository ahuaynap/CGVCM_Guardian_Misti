using UnityEngine;

public class CollectibleItemController : MonoBehaviour, IInteractable
{
    [SerializeField]
    private InventoryItemDefinition definition;

    public string Prompt => definition == null ? "Recoger objeto" : $"Recoger {definition.Name}";

    public void Interact()
    {
        if (definition == null)
        {
            Debug.LogWarning("CollectibleItemController requires an item definition.", this);
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager not found.", this);
            return;
        }

        if (ObjectivesManager.Instance != null &&
            !ObjectivesManager.Instance.IsCurrentObjective(
                GameIds.CollectEmergencyBackpackObjective))
        {
            return;
        }

        InventoryItem item = new InventoryItem(definition);
        if (!InventoryManager.Instance.AddItem(item))
        {
            return;
        }

        CollectBackpack();

        if (ObjectivesManager.Instance == null)
        {
            Debug.LogWarning("ObjectivesManager not found.", this);
            return;
        }

        ObjectivesManager.Instance.TryCompleteObjective(
            GameIds.CollectEmergencyBackpackObjective);
    }

    private void CollectBackpack()
    {
        gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (definition == null)
        {
            Debug.LogWarning("CollectibleItemController requires an item definition.", this);
        }
    }
}
