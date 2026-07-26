using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    [SerializeField] private List<Objective> objectives = new();
    [SerializeField] private ObjectiveUI objectiveUI;
    public static ObjectivesManager Instance { get; private set; }
    public event Action<Objective> ObjectiveChanged;
    public event Action AllObjectivesCompleted;
    private int currentObjectiveIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (objectiveUI == null) Debug.LogWarning("ObjectivesManager requires an ObjectiveUI reference.", this);
        objectiveUI?.Refresh(GetCurrentObjective());
    }

    public Objective GetCurrentObjective() => IsSimulationCompleted ? null : objectives[currentObjectiveIndex];
    public bool IsCurrentObjective(string objectiveId) => GetCurrentObjective() is Objective current && current.Id == objectiveId;
    public bool TryCompleteObjective(string objectiveId)
    {
        if (!IsCurrentObjective(objectiveId)) { SimulationSession.Instance?.RecordIncorrectInteraction(); SimulationSession.Instance?.RecordObjectiveOrderViolation(); return false; }
        string completedId = GetCurrentObjective().Id;
        SimulationSession.Instance?.RecordObjective(objectiveId);
        currentObjectiveIndex++;
        if (IsSimulationCompleted) { objectiveUI?.Hide(); AllObjectivesCompleted?.Invoke(); Debug.Log("[Objective] Advanced from " + completedId + "; simulation completed.", this); return true; }
        objectiveUI?.Refresh(GetCurrentObjective());
        ObjectiveChanged?.Invoke(GetCurrentObjective());
        Debug.Log("[Objective] Advanced from " + completedId + " to " + GetCurrentObjective().Id + ".", this);
        return true;
    }
    public bool IsSimulationCompleted => currentObjectiveIndex >= objectives.Count;
    public IReadOnlyList<Objective> Objectives => objectives;
    private void OnDestroy() { if (Instance == this) Instance = null; }
}
