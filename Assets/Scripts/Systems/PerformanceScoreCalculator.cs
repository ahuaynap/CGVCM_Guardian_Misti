using UnityEngine;

public static class PerformanceScoreCalculator
{
    public static int Calculate(float seconds, int incorrect, int hazards, int missingItems, int pauses)
    {
        int timePenalty = Mathf.Max(0, Mathf.CeilToInt(seconds - 100f) * 2);
        return Mathf.Clamp(1000 - timePenalty - incorrect * 25 - hazards * 80 - missingItems * 35 - Mathf.Max(0, pauses - 2) * 10, 0, 1000);
    }
    public static string Grade(int score) => score >= 900 ? "Excelente" : score >= 750 ? "Bueno" : score >= 550 ? "Regular" : "Debe mejorar";
}
