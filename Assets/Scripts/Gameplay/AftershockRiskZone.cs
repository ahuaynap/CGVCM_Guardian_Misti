using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class AftershockRiskZone : MonoBehaviour
{
    [SerializeField] private AftershockController aftershock;
    [SerializeField] private TMP_Text sign;
    private bool inside;
    public bool IsInside=>inside;
    private void Awake(){GetComponent<BoxCollider>().isTrigger=true;}
    private void Update(){if(inside&&aftershock!=null&&aftershock.IsActive)SimulationSession.Instance?.RecordAftershockRiskTime(Time.deltaTime);}
    private void OnTriggerEnter(Collider other){if(!other.CompareTag("Player"))return;inside=true;if(aftershock!=null&&aftershock.IsActive)SimulationSession.Instance?.RecordAftershockUnsafeEntry();}
    private void OnTriggerExit(Collider other){if(other.CompareTag("Player"))inside=false;}
}
