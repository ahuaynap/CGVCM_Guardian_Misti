
using System;
using UnityEngine;

public class InventoryItem
{
    public InventoryItemDefinition Definition { get; }
    public string Id  => Definition.Id;
    public string Name => Definition.Name;
    public string Description  => Definition.Description;
    public Sprite Icon => Definition.Icon;

    public InventoryItem(InventoryItemDefinition definition)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }
        
        Definition = definition;
    }
}
