using System;
using UnityEngine;

[Serializable]
public readonly struct PerformanceScoreInput
{
    public readonly float TotalTime,TimeToProtection,DistanceTravelled;
    public readonly int IncorrectInteractions,HazardContacts,MissingItemAttempts;
    public readonly bool ObjectiveOrderRespected;
    public PerformanceScoreInput(float totalTime,int incorrect,int hazards,int missing,float protection,bool ordered,float distance){TotalTime=totalTime;IncorrectInteractions=incorrect;HazardContacts=hazards;MissingItemAttempts=missing;TimeToProtection=protection;ObjectiveOrderRespected=ordered;DistanceTravelled=distance;}
}
public static class PerformanceScoreCalculator
{
    public const int MinimumScore=0,MaximumScore=1100;
    public static int Calculate(PerformanceScoreInput input)
    {
        int score=1000;
        score-=Mathf.Max(0,Mathf.CeilToInt(input.TotalTime-180f))*2;
        score-=input.IncorrectInteractions*25;
        score-=input.HazardContacts*80;
        score-=input.MissingItemAttempts*35;
        if(input.TimeToProtection<0)score-=180;else if(input.TimeToProtection>15f)score-=Mathf.CeilToInt(input.TimeToProtection-15f)*8;
        if(input.TimeToProtection>=0&&input.TimeToProtection<=8f)score+=75;
        if(input.ObjectiveOrderRespected)score+=75;
        if(input.HazardContacts==0)score+=50;
        if(input.DistanceTravelled>0&&input.DistanceTravelled<=180f)score+=25;
        return Mathf.Clamp(score,MinimumScore,MaximumScore);
    }
    public static int Calculate(float seconds,int incorrect,int hazards,int missingItems,int pauses){int timePenalty=Mathf.Max(0,Mathf.CeilToInt(seconds-100f)*2);return Mathf.Clamp(1000-timePenalty-incorrect*25-hazards*80-missingItems*35-Mathf.Max(0,pauses-2)*10,MinimumScore,1000);}
    public static string Grade(int score)=>score>=900?"Excelente":score>=750?"Bueno":score>=550?"Regular":"Debe mejorar";
}
