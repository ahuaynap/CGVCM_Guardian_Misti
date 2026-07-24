using System;
using System.Collections;
using TMPro;
using UnityEngine;

public sealed class EarthquakeController : MonoBehaviour
{
    [SerializeField] private EarthquakeProfile profile;
    [SerializeField] private Transform cameraEffectRoot;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text intensityText;
    [SerializeField] private Light[] emergencyLights;
    [SerializeField] private ParticleSystem dust;
    public event Action EarthquakeStarted;
    public event Action EarthquakeFinished;
    public bool IsRunning { get; private set; }
    public float CurrentIntensity { get; private set; }
    private Vector3 initialCameraLocalPosition;
    private float elapsed;

    private IEnumerator Start()
    {
        if (profile == null) yield break;
        if (cameraEffectRoot != null) initialCameraLocalPosition = cameraEffectRoot.localPosition;
        float remaining = profile.PreparationCountdown;
        while (remaining > 0)
        {
            if (countdownText != null) countdownText.text = $"El simulacro comenzará en {Mathf.CeilToInt(remaining)}...";
            if (intensityText != null) intensityText.text = "Intensidad simulada: Preparación";
            remaining -= Time.deltaTime;
            yield return null;
        }
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        IsRunning = true;
        SimulationSession.Instance?.StartTimer();
        EarthquakeStarted?.Invoke();
        dust?.Play();
        while (elapsed < profile.Duration)
        {
            elapsed += Time.deltaTime;
            CurrentIntensity = profile.Evaluate(elapsed);
            ApplyPresentation();
            yield return null;
        }
        CurrentIntensity = 0;
        RestorePresentation();
        IsRunning = false;
        EarthquakeFinished?.Invoke();
    }

    private void ApplyPresentation()
    {
        if (cameraEffectRoot != null)
        {
            float t = Time.unscaledTime * profile.CameraShakeFrequency;
            Vector3 noise = new(Mathf.PerlinNoise(t, 0) - .5f, Mathf.PerlinNoise(0, t) - .5f, 0);
            cameraEffectRoot.localPosition = initialCameraLocalPosition + noise * (profile.CameraShakeIntensity * CurrentIntensity);
        }
        if (intensityText != null)
        {
            string label = CurrentIntensity < .25f ? "Sismo leve" : CurrentIntensity < .65f ? "Sismo moderado" : "Sismo fuerte";
            intensityText.text = $"Intensidad simulada: {label}";
        }
        foreach (Light light in emergencyLights ?? Array.Empty<Light>())
            if (light != null) light.enabled = Mathf.PerlinNoise(Time.unscaledTime * 9, light.GetHashCode()) > profile.LightFlickerIntensity * CurrentIntensity;
    }

    private void RestorePresentation()
    {
        if (cameraEffectRoot != null) cameraEffectRoot.localPosition = initialCameraLocalPosition;
        if (intensityText != null) intensityText.text = "Intensidad simulada: Sismo finalizado";
        foreach (Light light in emergencyLights ?? Array.Empty<Light>()) if (light != null) light.enabled = true;
    }

    private void OnDisable() => RestorePresentation();
}
