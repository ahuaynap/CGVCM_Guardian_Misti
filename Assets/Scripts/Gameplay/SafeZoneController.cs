using UnityEngine;

public class SafeZoneController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

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
}
