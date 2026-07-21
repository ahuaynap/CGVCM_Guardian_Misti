using UnityEngine;

public class CollectibleItemController : MonoBehaviour, IInteractable
{

    [SerializeField]
    private InventoryItemDefinition definition;

    public string Prompt => $"Recoger {definition.Name}";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        InventoryItem item = new InventoryItem(definition);
        InventoryManager.Instance.AddItem(item);
        CollectBackpack();
        NotifyObjectiveCompleted();
    }

    private void NotifyObjectiveCompleted()
    {
        if(ObjectivesManager.Instance == null)
        {
            Debug.LogWarning("ObjectivesManager not found");
            return;
        }

        ObjectivesManager.Instance.CompleteCurrentObjective();
    }

    private void CollectBackpack()
    {
        gameObject.SetActive(false);
    }

}
