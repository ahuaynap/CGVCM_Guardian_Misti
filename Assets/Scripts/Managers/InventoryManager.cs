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

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        inventoryUI.Refresh(null, ItemsCount);
    }

    public void AddItem(InventoryItem item)
    {
        items.Add(item);

        inventoryUI.Refresh(item, ItemsCount);
        notificationUI.Show(
            item
        );
    }

    public bool HasItem(string itemId)
    {
        return items.Exists(item => item.Id == itemId);
    }

    public int ItemsCount
    {
        get
        {
            return items.Count;
        }
    }

    public IReadOnlyList<InventoryItem> Items { get; }
}
