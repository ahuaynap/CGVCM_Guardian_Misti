using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameplaySettingsController : MonoBehaviour
{
 [SerializeField] private StarterAssets.FirstPersonController controller;
 [SerializeField] private PlayerLookController lookController;
 [SerializeField] private Slider sensitivitySlider,volumeSlider;
 [SerializeField] private TMP_Text sensitivityValue,volumeValue,researchExplanation;
 [SerializeField] private Toggle researchToggle;
 private void Start()
 {
  if(sensitivitySlider!=null){float value=lookController==null?PlayerLookController.DefaultMouseSensitivity:lookController.MouseSensitivity;sensitivitySlider.SetValueWithoutNotify(value);sensitivitySlider.onValueChanged.AddListener(SetSensitivity);SetSensitivity(value);}
  if(volumeSlider!=null){volumeSlider.SetValueWithoutNotify(AudioListener.volume);volumeSlider.onValueChanged.AddListener(SetVolume);SetVolume(volumeSlider.value);}
  if(researchToggle!=null){researchToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(SimulationSession.ResearchEnabledKey,0)==1);researchToggle.onValueChanged.AddListener(SetResearchEnabled);SetResearchEnabled(researchToggle.isOn);}
 }
 public void SetSensitivity(float value){value=PlayerLookController.RepairSensitivity(value,false);lookController?.SetMouseSensitivity(value);if(sensitivityValue!=null)sensitivityValue.text=value.ToString("0.00");}
 public void SetVolume(float value){value=Mathf.Clamp01(value);AudioListener.volume=value;if(volumeValue!=null)volumeValue.text=Mathf.RoundToInt(value*100)+"%";}
 public void SetResearchEnabled(bool enabled){PlayerPrefs.SetInt(SimulationSession.ResearchEnabledKey,enabled?1:0);PlayerPrefs.Save();if(researchExplanation!=null)researchExplanation.text=enabled?"Activado · datos anónimos guardados solo en este dispositivo":"Desactivado · no se guardarán datos del simulacro";}
 private void OnDestroy(){sensitivitySlider?.onValueChanged.RemoveListener(SetSensitivity);volumeSlider?.onValueChanged.RemoveListener(SetVolume);researchToggle?.onValueChanged.RemoveListener(SetResearchEnabled);}
}
