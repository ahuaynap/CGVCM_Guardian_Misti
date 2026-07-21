using System.Collections.Generic;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    private readonly List<Objective> objectives = new();

    [SerializeField]
    private ObjectiveUI objectiveUI;

    [SerializeField]
    private InventoryUI inventoryUI;

    [SerializeField]
    private GameCompletionUI gameCompletionUI;

    public static ObjectivesManager Instance { get; private set; }

    private int currentObjectiveIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeObjectives();

        if (objectiveUI == null)
        {
            Debug.LogWarning("ObjectivesManager requires an ObjectiveUI reference.", this);
            return;
        }

        objectiveUI.Refresh(GetCurrentObjective());
    }

    private void InitializeObjectives()
    {
        objectives.Add(
            new Objective(GameIds.ExitRoomObjective, "Sal de la habitacion")
        );

        objectives.Add(
            new Objective(
                GameIds.CollectEmergencyBackpackObjective,
                "Recoge la mochila de emergencia")
        );

        objectives.Add(
            new Objective(GameIds.ReachSafeZoneObjective, "Dirigete al punto seguro")
        );
    }

    public Objective GetCurrentObjective()
    {
        if (IsSimulationCompleted)
        {
            return null;
        }

        return objectives[currentObjectiveIndex];
    }

    public bool IsCurrentObjective(string objectiveId)
    {
        Objective currentObjective = GetCurrentObjective();
        return currentObjective != null && currentObjective.Id == objectiveId;
    }

    public bool TryCompleteObjective(string objectiveId)
    {
        if (!IsCurrentObjective(objectiveId))
        {
            return false;
        }

        currentObjectiveIndex++;

        if (IsSimulationCompleted)
        {
            objectiveUI?.Hide();
            inventoryUI?.Hide();
            gameCompletionUI?.Show();
            return true;
        }

        objectiveUI?.Refresh(GetCurrentObjective());
        return true;
    }

    public bool IsSimulationCompleted => currentObjectiveIndex >= objectives.Count;

    private void OnValidate()
    {
        if (objectiveUI == null)
        {
            Debug.LogWarning("ObjectivesManager requires an ObjectiveUI reference.", this);
        }

        if (inventoryUI == null)
        {
            Debug.LogWarning("ObjectivesManager requires an InventoryUI reference.", this);
        }

        if (gameCompletionUI == null)
        {
            Debug.LogWarning("ObjectivesManager requires a GameCompletionUI reference.", this);
        }
    }
}
