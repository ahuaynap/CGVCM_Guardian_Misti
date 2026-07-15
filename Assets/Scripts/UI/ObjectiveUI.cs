using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI objectiveText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh(Objective objective)
    {
        Debug.Log("Here");

        objectiveText.text = $"Objectivo\n{objective.Description}";
    }
}
