using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SimulationSession : MonoBehaviour
{
    public const string BestTimeKey = "GuardianMisti.BestTimeSeconds";
    public static SimulationSession Instance { get; private set; }
    public float TotalTime { get; private set; }
    public float Level01Time { get; private set; }
    public float Level02Time { get; private set; }
    public int IncorrectInteractions { get; private set; }
    public int HazardContacts { get; private set; }
    public int MissingItemAttempts { get; private set; }
    public int Pauses { get; private set; }
    public float TimeToProtection { get; private set; } = -1f;
    public float StrongPhaseOutsideTime { get; private set; }
    public bool ProtectionReached { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsFinished { get; private set; }
    public readonly List<string> ObjectiveTimestamps = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { if (Application.isPlaying) Destroy(gameObject); else DestroyImmediate(gameObject); return; }
        if (transform.parent != null) transform.SetParent(null, true);
        Instance = this; ResetRun(); if (Application.isPlaying) DontDestroyOnLoad(gameObject); SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Update()
    {
        if (!IsRunning || IsFinished || Time.timeScale <= 0) return;
        TotalTime += Time.unscaledDeltaTime;
        if (SceneManager.GetActiveScene().name == SceneNames.Level01) Level01Time += Time.unscaledDeltaTime;
        else if (SceneManager.GetActiveScene().name == SceneNames.Level02) Level02Time += Time.unscaledDeltaTime;
    }
    public void StartTimer() { IsRunning = true; IsFinished = false; }
    public void ResetRun()
    {
        TotalTime = 0; Level01Time = 0; Level02Time = 0; IncorrectInteractions = 0; HazardContacts = 0; MissingItemAttempts = 0; Pauses = 0;
        IsRunning = false; IsFinished = false; TimeToProtection = -1f; StrongPhaseOutsideTime = 0f; ProtectionReached = false; ObjectiveTimestamps.Clear();
    }
    public void StopTimer()
    {
        if (!IsRunning || IsFinished) return;
        IsFinished = true; IsRunning = false;
        float best = PlayerPrefs.GetFloat(BestTimeKey, float.MaxValue);
        if (TotalTime < best) { PlayerPrefs.SetFloat(BestTimeKey, TotalTime); PlayerPrefs.Save(); }
        if (PlayerPrefs.GetInt("GuardianMisti.ResearchEnabled", 0) == 1) ExportResearch();
    }
    public void RecordObjective(string id) => ObjectiveTimestamps.Add($"{id}:{TotalTime.ToString("F3", CultureInfo.InvariantCulture)}");
    public void RecordIncorrectInteraction() => IncorrectInteractions++;
    public void RecordMissingItemAttempt() => MissingItemAttempts++;
    public void RecordHazard() => HazardContacts++;
    public void RecordProtection(float elapsed) { if (ProtectionReached) return; ProtectionReached = true; TimeToProtection = Mathf.Max(0, elapsed); }
    public void RecordStrongOutside(float deltaTime) => StrongPhaseOutsideTime += Mathf.Max(0, deltaTime);
    public void RecordPause() => Pauses++;
    public int Score => PerformanceScoreCalculator.Calculate(TotalTime, IncorrectInteractions, HazardContacts, MissingItemAttempts, Pauses);
    public string Grade => PerformanceScoreCalculator.Grade(Score);
    public static string FormatTime(float seconds) => TimeSpan.FromSeconds(Mathf.Max(0, seconds)).ToString(@"mm\:ss\.fff");
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => HandleSceneLoaded(scene.name);
    public void HandleSceneLoaded(string sceneName)
    {
        if (sceneName != SceneNames.MainMenu) return;
        if (Application.isPlaying) Destroy(gameObject); else DestroyImmediate(gameObject);
    }
    private void ExportResearch()
    {
        string directory = Path.Combine(Application.persistentDataPath, "GuardianMistiResearch");
        Directory.CreateDirectory(directory);
        string runId = Guid.NewGuid().ToString("N");
        string json = JsonUtility.ToJson(new ResearchRun(runId, this), true);
        File.WriteAllText(Path.Combine(directory, $"run-{runId}.json"), json);
    }
    private void OnDestroy() { SceneManager.sceneLoaded -= OnSceneLoaded; if (Instance == this) Instance = null; }
    [Serializable] private sealed class ResearchRun
    {
        public string anonymousRunId; public float totalTime, timeToProtection, strongPhaseOutsideTime; public bool protectionReached; public int score, incorrectInteractions, hazards; public string[] objectives;
        public ResearchRun(string id, SimulationSession s) { anonymousRunId=id;totalTime=s.TotalTime;timeToProtection=s.TimeToProtection;strongPhaseOutsideTime=s.StrongPhaseOutsideTime;protectionReached=s.ProtectionReached;score=s.Score;incorrectInteractions=s.IncorrectInteractions;hazards=s.HazardContacts;objectives=s.ObjectiveTimestamps.ToArray(); }
    }
}
