using System;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public sealed class SimulationResearchRecorder : MonoBehaviour
{
    public const string DirectoryName="GuardianMistiResearch";
    public const string CsvHeader="anonymousRunId,difficulty,totalTime,level01Time,level02Time,timeToProtection,timeToLevel01Exit,timeToBeacon,timeToFinalSafeZone,incorrectInteractions,missingItemAttempts,hazardContacts,pauseCount,distanceTravelled,strongPhaseOutsideTime,objectiveOrderRespected,finalScore,objectiveTimestamps";
    public string Export(SimulationSession session)
    {
        if(session==null)return string.Empty;
        string directory=Path.Combine(Application.persistentDataPath,DirectoryName);Directory.CreateDirectory(directory);
        string runId=Guid.NewGuid().ToString("N");var data=CreateData(runId,session);
        File.WriteAllText(Path.Combine(directory,$"run-{runId}.json"),JsonUtility.ToJson(data,true));
        string csvPath=Path.Combine(directory,"simulation-runs.csv");if(!File.Exists(csvPath))File.WriteAllText(csvPath,CsvHeader+Environment.NewLine);
        File.AppendAllText(csvPath,CreateCsvRow(data)+Environment.NewLine);return runId;
    }
    public static SimulationResearchData CreateData(string runId,SimulationSession s)=>new(){anonymousRunId=runId,difficulty=s.Difficulty,totalTime=s.TotalTime,level01Time=s.Level01Time,level02Time=s.Level02Time,timeToProtection=s.TimeToProtection,timeToLevel01Exit=s.TimeToLevel01Exit,timeToBeacon=s.TimeToBeacon,timeToFinalSafeZone=s.TimeToFinalSafeZone,incorrectInteractions=s.IncorrectInteractions,missingItemAttempts=s.MissingItemAttempts,hazardContacts=s.HazardContacts,pauseCount=s.Pauses,distanceTravelled=s.DistanceTravelled,strongPhaseOutsideTime=s.StrongPhaseOutsideTime,objectiveOrderRespected=s.ObjectiveOrderRespected,finalScore=s.Score,objectiveTimestamps=s.ObjectiveTimestamps.ToArray(),positionSamples=s.PositionSamples.ToArray()};
    public static string CreateJson(string runId,SimulationSession session)=>JsonUtility.ToJson(CreateData(runId,session),true);
    public static string CreateCsvRow(SimulationResearchData d)
    {
        string F(float value)=>value.ToString("F3",CultureInfo.InvariantCulture);string Q(string value)=>"\""+(value??string.Empty).Replace("\"","\"\"")+"\"";
        return string.Join(",",Q(d.anonymousRunId),Q(d.difficulty),F(d.totalTime),F(d.level01Time),F(d.level02Time),F(d.timeToProtection),F(d.timeToLevel01Exit),F(d.timeToBeacon),F(d.timeToFinalSafeZone),d.incorrectInteractions,d.missingItemAttempts,d.hazardContacts,d.pauseCount,F(d.distanceTravelled),F(d.strongPhaseOutsideTime),d.objectiveOrderRespected?"true":"false",d.finalScore,Q(string.Join("|",d.objectiveTimestamps??Array.Empty<string>())));
    }
}
[Serializable]
public sealed class SimulationResearchData
{
    public string anonymousRunId,difficulty;public float totalTime,level01Time,level02Time,timeToProtection,timeToLevel01Exit,timeToBeacon,timeToFinalSafeZone,distanceTravelled,strongPhaseOutsideTime;public int incorrectInteractions,missingItemAttempts,hazardContacts,pauseCount,finalScore;public bool objectiveOrderRespected;public string[] objectiveTimestamps;public SimulationPositionSample[] positionSamples;
}
