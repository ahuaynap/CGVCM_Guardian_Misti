using UnityEngine;
[RequireComponent(typeof(Collider))]
public sealed class EarthquakeProtectionTrigger : MonoBehaviour
{
 [SerializeField] private EarthquakeController earthquakeController;
 private bool reached;
 private void OnTriggerEnter(Collider other){if(reached||!other.CompareTag("Player")||earthquakeController==null)return;if(earthquakeController.State is EarthquakeState.Inactive or EarthquakeState.Preparing)return;reached=true;earthquakeController.MarkProtectionReached();}
}
