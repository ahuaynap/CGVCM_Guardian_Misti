using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySettingsController : MonoBehaviour
{
    [SerializeField] private FirstPersonController controller;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider volumeSlider;
    private void Start()
    {
        if (sensitivitySlider != null) { sensitivitySlider.SetValueWithoutNotify(controller == null ? 1f : controller.RotationSpeed); sensitivitySlider.onValueChanged.AddListener(SetSensitivity); }
        if (volumeSlider != null) { volumeSlider.SetValueWithoutNotify(AudioListener.volume); volumeSlider.onValueChanged.AddListener(SetVolume); }
    }
    public void SetSensitivity(float value) { if (controller != null) controller.RotationSpeed = Mathf.Clamp(value, .25f, 3f); }
    public void SetVolume(float value) => AudioListener.volume = Mathf.Clamp01(value);
    private void OnDestroy() { sensitivitySlider?.onValueChanged.RemoveListener(SetSensitivity); volumeSlider?.onValueChanged.RemoveListener(SetVolume); }
}
