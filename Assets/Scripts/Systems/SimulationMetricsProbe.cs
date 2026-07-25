using UnityEngine;
public sealed class SimulationMetricsProbe : MonoBehaviour
{
    private void Update(){SimulationSession.Instance?.ReportPlayerPosition(transform.position);}
}
