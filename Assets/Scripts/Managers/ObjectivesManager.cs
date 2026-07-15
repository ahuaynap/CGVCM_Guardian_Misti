using UnityEngine;
using System.Collections.Generic;

public class ObjectivesManager : MonoBehaviour
{
    private readonly List<Objective> objectives = new();

    [SerializeField]
    private ObjectiveUI objectiveUI;

    public static ObjectivesManager Instance { get; private set; }

    private int currentObjectiveIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // InitializeObjectives();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeObjectives();
        objectiveUI.Refresh(GetCurrentObjective());
    }

    private void InitializeObjectives()
    {
        objectives.Add(
            new Objective("Sal de la habitacion")
        );

        objectives.Add(
            new Objective("Recoge la mochila de emergencia")
        );

        objectives.Add(
            new Objective("Dirigete al punto seguro")
        );
        Debug.Log("Ya agregue");
        Debug.Log($"data{objectives.Count}index{currentObjectiveIndex}");
    }

    public Objective GetCurrentObjective()
    {
        Debug.Log($"data{objectives.Count}index{currentObjectiveIndex}");
        return objectives[currentObjectiveIndex];
    }

    public void CompleteCurrentObjective()
    {
        if (currentObjectiveIndex >= objectives.Count - 1)
        {
            return;
        }

        currentObjectiveIndex++;
        objectiveUI.Refresh(GetCurrentObjective());
    }
}
