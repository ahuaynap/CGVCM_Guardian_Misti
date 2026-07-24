using TMPro;
using UnityEngine;

public sealed class SimulationResultsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text metricsText;
    private void OnEnable()
    {
        SimulationSession.Instance?.StopTimer();
        var s = SimulationSession.Instance;
        if (s == null || metricsText == null) return;
        float best = PlayerPrefs.GetFloat(SimulationSession.BestTimeKey, s.TotalTime);
        metricsText.text = $"TIEMPO TOTAL  {SimulationSession.FormatTime(s.TotalTime)}\nTIEMPO NIVEL 1  {SimulationSession.FormatTime(s.Level01Time)}\nTIEMPO NIVEL 2  {SimulationSession.FormatTime(s.Level02Time)}\nINTERACCIONES INCORRECTAS  {s.IncorrectInteractions}\nRIESGOS ENCONTRADOS  {s.HazardContacts}\nPUNTAJE FINAL  {s.Score}\nCALIFICACIÓN  {s.Grade}\nMEJOR TIEMPO  {SimulationSession.FormatTime(best)}";
    }
}
