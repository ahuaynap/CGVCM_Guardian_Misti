using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private readonly List<InventoryItem> items = new();
    public static InventoryManager Instance { get; private set; }
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private NotificationUI notificationUI;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        inventoryUI?.Refresh(null, 0);
    }
    public bool AddItem(InventoryItem item)
    {
        if (item == null || item.Definition == null || string.IsNullOrWhiteSpace(item.Id)) return false;
        if (HasItem(item.Id)) return false;
        items.Add(item); inventoryUI?.Refresh(item, items.Count); notificationUI?.Show(item); return true;
    }
    public bool HasItem(string itemId) => !string.IsNullOrWhiteSpace(itemId) && items.Exists(item => item.Id == itemId);
    public int ItemsCount => items.Count;
    public IReadOnlyList<InventoryItem> Items => items.AsReadOnly();
    private void OnDestroy() { if (Instance == this) Instance = null; }
}
