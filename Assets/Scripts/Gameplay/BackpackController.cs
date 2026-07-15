using UnityEngine;

public class BackpackController : MonoBehaviour, IInteractable
{

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
