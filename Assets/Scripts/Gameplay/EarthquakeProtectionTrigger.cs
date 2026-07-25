using UnityEngine;
[RequireComponent(typeof(Collider))]
public sealed class EarthquakeProtectionTrigger : MonoBehaviour
{
 [SerializeField] private EarthquakeController earthquakeController;
 private void OnTriggerEnter(Collider other){if(!other.CompareTag("Player")||earthquakeController==null)return;if(earthquakeController.State is EarthquakeState.Inactive or EarthquakeState.Preparing)return;earthquakeController.MarkProtectionReached();}
 private void OnTriggerExit(Collider other){if(other.CompareTag("Player"))earthquakeController?.MarkProtectionExited();}
}
