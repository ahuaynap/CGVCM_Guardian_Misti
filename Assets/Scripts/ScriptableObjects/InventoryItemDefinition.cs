using UnityEngine;

[CreateAssetMenu(
    fileName = "New Iventory Item",
    menuName = "Guarding-Misti/Inventory/Inventory Item"
)]
public class InventoryItemDefinition : ScriptableObject
{
    [field: SerializeField]
    public Sprite Icon { get; private set; }

    [field: SerializeField]
    public string Id { get; private set; }

    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public string Description { get; private set; }

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            Debug.LogWarning("Inventory item definitions require a non-empty ID.", this);
        }
    }
}
