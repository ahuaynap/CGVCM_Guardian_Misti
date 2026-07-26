using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SimulationSession : MonoBehaviour
{
    public const string BestTimeKey = "GuardianMisti.BestTimeSeconds";
    public const string ResearchEnabledKey = "GuardianMisti.ResearchEnabled";
    public const string DifficultyKey = "GuardianMisti.Difficulty";
    public static SimulationSession Instance { get; private set; }
    public float TotalTime { get; private set; }
    public float Level01Time { get; private set; }
    public float Level02Time { get; private set; }
    public float TimeToProtection { get; private set; } = -1f;
    public float TimeToLevel01Exit { get; private set; } = -1f;
    public float TimeToBeacon { get; private set; } = -1f;
    public float TimeToFinalSafeZone { get; private set; } = -1f;
    public float DistanceTravelled { get; private set; }
    public float StrongPhaseOutsideTime { get; private set; }
    public float AftershockRiskTime { get; private set; }
    public float AftershockReactionTime { get; private set; } = -1f;
    public int AftershockUnsafeEntries { get; private set; }
    public bool AftershockReachedClearArea { get; private set; }
    public int IncorrectInteractions { get; private set; }
    public int HazardContacts { get; private set; }
    public int MissingItemAttempts { get; private set; }
    public int Pauses { get; private set; }
    public bool ProtectionReached { get; private set; }
    public bool ProtectionFailed { get; private set; }
    public bool ObjectiveOrderRespected { get; private set; } = true;
    public bool IsRunning { get; private set; }
    public bool IsFinished { get; private set; }
    public int StopCount { get; private set; }
    public string Difficulty { get; private set; } = "Intermedio";
    public readonly List<string> ObjectiveTimestamps = new();
    public readonly List<SimulationPositionSample> PositionSamples = new();
    private Vector3 lastPlayerPosition;
    private bool hasPlayerPosition;
    private float nextPositionSampleTime;

    private void Awake()
    {
        if (Instance != null && Instance != this) { if (Application.isPlaying) Destroy(gameObject); else DestroyImmediate(gameObject); return; }
        if (transform.parent != null) transform.SetParent(null, true);
        Instance = this;
        ResetRun();
        if (Application.isPlaying) DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Update()
    {
        if (!IsRunning || IsFinished || Time.timeScale <= 0f) return;
        float delta = Time.unscaledDeltaTime;
        TotalTime += delta;
        string scene = SceneManager.GetActiveScene().name;
        if (scene == SceneNames.Level01) Level01Time += delta;
        else if (scene == SceneNames.Level02) Level02Time += delta;
    }
    public void StartTimer() { if (IsFinished) return; IsRunning = true; }
    public void ResetRun()
    {
        TotalTime=0;Level01Time=0;Level02Time=0;TimeToProtection=-1;TimeToLevel01Exit=-1;TimeToBeacon=-1;TimeToFinalSafeZone=-1;DistanceTravelled=0;StrongPhaseOutsideTime=0;
        AftershockRiskTime=0;AftershockReactionTime=-1;AftershockUnsafeEntries=0;AftershockReachedClearArea=false;IncorrectInteractions=0;HazardContacts=0;MissingItemAttempts=0;Pauses=0;ProtectionReached=false;ProtectionFailed=false;ObjectiveOrderRespected=true;IsRunning=false;IsFinished=false;StopCount=0;
        Difficulty=PlayerPrefs.GetString(DifficultyKey,"Intermedio");ObjectiveTimestamps.Clear();PositionSamples.Clear();hasPlayerPosition=false;nextPositionSampleTime=0;
    }
    public bool StopTimer()
    {
        if (!IsRunning || IsFinished) return false;
        IsFinished=true;IsRunning=false;StopCount++;
        if (IsValidRun && IsBetterTime(TotalTime,PlayerPrefs.GetFloat(BestTimeKey,float.MaxValue))) { PlayerPrefs.SetFloat(BestTimeKey,TotalTime);PlayerPrefs.Save(); }
        if (PlayerPrefs.GetInt(ResearchEnabledKey,0)==1) GetComponent<SimulationResearchRecorder>()?.Export(this);
        return true;
    }
    public void RecordObjective(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || ObjectiveTimestamps.Any(x=>x.StartsWith(id+":",StringComparison.Ordinal))) { ObjectiveOrderRespected=false;return; }
        ObjectiveTimestamps.Add($"{id}:{TotalTime.ToString("F3",CultureInfo.InvariantCulture)}");
        if(id==GameIds.Level01ReachExit)TimeToLevel01Exit=TotalTime;
        else if(id==GameIds.Level02ActivateBeacon)TimeToBeacon=TotalTime;
        else if(id==GameIds.Level02ReachSafeZone)TimeToFinalSafeZone=TotalTime;
    }
    public void RecordObjectiveOrderViolation(){ObjectiveOrderRespected=false;}
    public void RecordIncorrectInteraction(){IncorrectInteractions++;}
    public void RecordMissingItemAttempt(){MissingItemAttempts++;}
    public void RecordHazard(){HazardContacts++;}
    public void RecordProtection(float elapsed){if(ProtectionReached)return;ProtectionReached=true;TimeToProtection=Mathf.Max(0,elapsed);}
    public void RecordProtectionFailure(){if(ProtectionFailed)return;ProtectionFailed=true;HazardContacts++;}
    public void RecordStrongOutside(float deltaTime){if(Time.timeScale>0)StrongPhaseOutsideTime+=Mathf.Max(0,deltaTime);}
    public void RecordAftershockRiskTime(float deltaTime){AftershockRiskTime+=Mathf.Max(0,deltaTime);}
    public void RecordAftershockUnsafeEntry(){AftershockUnsafeEntries++;HazardContacts++;}
    public void RecordAftershockClearArea(float reactionTime){if(AftershockReachedClearArea)return;AftershockReachedClearArea=true;AftershockReactionTime=Mathf.Max(0,reactionTime);}
    public void RecordPause(){Pauses++;}
    public void ReportPlayerPosition(Vector3 position)
    {
        if(!IsRunning||IsFinished)return;
        if(hasPlayerPosition)DistanceTravelled+=Vector3.Distance(lastPlayerPosition,position);
        lastPlayerPosition=position;hasPlayerPosition=true;
        if(TotalTime+0.0001f<nextPositionSampleTime)return;
        PositionSamples.Add(new SimulationPositionSample(TotalTime,position));nextPositionSampleTime=TotalTime+1f;
    }
    public bool IsValidRun=>IsFinished&&ProtectionReached&&ObjectiveOrderRespected&&TimeToFinalSafeZone>=0;
    public int Score=>PerformanceScoreCalculator.Calculate(new PerformanceScoreInput(TotalTime,IncorrectInteractions,HazardContacts,MissingItemAttempts,TimeToProtection,ObjectiveOrderRespected,DistanceTravelled));
    public string Grade=>PerformanceScoreCalculator.Grade(Score);
    public static bool IsBetterTime(float candidate,float currentBest)=>candidate>0&&candidate<currentBest;
    public static string FormatTime(float seconds)=>TimeSpan.FromSeconds(Mathf.Max(0,seconds)).ToString(@"mm\:ss\.fff");
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)=>HandleSceneLoaded(scene.name);
    public void HandleSceneLoaded(string sceneName){Time.timeScale=1f;if(sceneName==SceneNames.MainMenu){if(Application.isPlaying)Destroy(gameObject);else DestroyImmediate(gameObject);}}
    private void OnDestroy(){SceneManager.sceneLoaded-=OnSceneLoaded;if(Instance==this)Instance=null;}
}

[Serializable]
public struct SimulationPositionSample
{
    public float time,x,y,z;
    public SimulationPositionSample(float timestamp,Vector3 position){time=timestamp;x=position.x;y=position.y;z=position.z;}
}
