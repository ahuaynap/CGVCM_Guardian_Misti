using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private readonly List<InventoryItem> items = new();

    public static InventoryManager Instance { get; private set; }

    [SerializeField]
    private InventoryUI inventoryUI;

    [SerializeField]
    private NotificationUI notificationUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryManager requires an InventoryUI reference.", this);
            return;
        }

        inventoryUI.Refresh(null, ItemsCount);
    }

    public bool AddItem(InventoryItem item)
    {
        if (item == null || item.Definition == null)
        {
            Debug.LogWarning("Cannot add an inventory item without a definition.", this);
            return false;
        }

        if (string.IsNullOrWhiteSpace(item.Id))
        {
            Debug.LogWarning("Cannot add an inventory item with an empty ID.", item.Definition);
            return false;
        }

        if (HasItem(item.Id))
        {
            return false;
        }

        items.Add(item);
        inventoryUI?.Refresh(item, ItemsCount);

        if (notificationUI != null)
        {
            notificationUI.Show(item);
        }
        else
        {
            Debug.LogWarning("InventoryManager requires a NotificationUI reference.", this);
        }

        return true;
    }

    public bool HasItem(string itemId)
    {
        return items.Exists(item => item.Id == itemId);
    }

    public int ItemsCount => items.Count;

    public IReadOnlyList<InventoryItem> Items => items.AsReadOnly();

    private void OnValidate()
    {
        if (inventoryUI == null)
        {
            Debug.LogWarning("InventoryManager requires an InventoryUI reference.", this);
        }

        if (notificationUI == null)
        {
            Debug.LogWarning("InventoryManager requires a NotificationUI reference.", this);
        }
    }
}
