using TMPro;
using UnityEngine;

public sealed class SimulationHud : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    private void Update() { if (timerText != null) timerText.text = SimulationSession.FormatTime(SimulationSession.Instance?.TotalTime ?? 0); }
}
