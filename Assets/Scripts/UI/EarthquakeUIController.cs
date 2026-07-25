using TMPro;
using UnityEngine;
public sealed class EarthquakeUIController : MonoBehaviour
{
 [SerializeField] private EarthquakeController earthquakeController;
 [SerializeField] private TMP_Text countdownText;
 [SerializeField] private TMP_Text intensityText;
 [SerializeField] private float completedDisplaySeconds=2f;
 private float hideIntensityAt=-1f;
 public bool CountdownVisible=>countdownText!=null&&countdownText.gameObject.activeSelf;
 private void OnEnable(){if(earthquakeController==null)return;earthquakeController.StateChanged+=PresentState;earthquakeController.CountdownChanged+=PresentCountdown;PresentState(earthquakeController.State);}
 private void Update(){if(hideIntensityAt<0f||Time.time<hideIntensityAt)return;hideIntensityAt=-1f;if(intensityText!=null)SetIntensityVisible(false);}
 public void PresentCountdown(string message){if(countdownText==null)return;countdownText.text=message;countdownText.gameObject.SetActive(!string.IsNullOrEmpty(message));}
 public void PresentState(EarthquakeState state){if(countdownText!=null&&(state is EarthquakeState.Inactive or EarthquakeState.Finished))countdownText.gameObject.SetActive(false);if(intensityText==null)return;SetIntensityVisible(state!=EarthquakeState.Inactive);intensityText.text=state switch{EarthquakeState.Preparing=>"Intensidad simulada: Preparación",EarthquakeState.Light=>"Intensidad simulada: Leve",EarthquakeState.Moderate=>"Intensidad simulada: Moderada",EarthquakeState.Strong=>"Intensidad simulada: Fuerte",EarthquakeState.Decreasing=>"Intensidad simulada: Disminuyendo",EarthquakeState.Finished=>"Sismo finalizado",_=>string.Empty};hideIntensityAt=state==EarthquakeState.Finished?Time.time+completedDisplaySeconds:-1f;}
 private void SetIntensityVisible(bool visible){var target=intensityText.transform.parent!=null?intensityText.transform.parent.gameObject:intensityText.gameObject;target.SetActive(visible);}
 private void OnDisable(){if(earthquakeController==null)return;earthquakeController.StateChanged-=PresentState;earthquakeController.CountdownChanged-=PresentCountdown;}
}
