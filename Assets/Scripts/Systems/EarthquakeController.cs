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
    private Coroutine earthquakeRoutine;
    private bool isShuttingDown;

    private void Start() => earthquakeRoutine = StartCoroutine(RunEarthquake());

    private IEnumerator RunEarthquake()
    {
        if (profile == null) yield break;
        if (cameraEffectRoot != null) initialCameraLocalPosition = cameraEffectRoot.localPosition;
        float remaining = profile.PreparationCountdown;
        while (remaining > 0 && !isShuttingDown)
        {
            if (countdownText != null) countdownText.text = $"El simulacro comenzará en {Mathf.CeilToInt(remaining)}...";
            if (intensityText != null) intensityText.text = "Intensidad simulada: Preparación";
            remaining -= Time.deltaTime;
            yield return null;
        }
        if (isShuttingDown) yield break;
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        IsRunning = true;
        SimulationSession.Instance?.StartTimer();
        EarthquakeStarted?.Invoke();
        if (dust != null) dust.Play();
        while (elapsed < profile.Duration && !isShuttingDown)
        {
            elapsed += Time.deltaTime;
            CurrentIntensity = profile.Evaluate(elapsed);
            ApplyPresentation();
            yield return null;
        }
        if (isShuttingDown) yield break;
        CurrentIntensity = 0;
        RestorePresentation();
        IsRunning = false;
        earthquakeRoutine = null;
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

    private void StopSafely()
    {
        if (isShuttingDown) return;
        isShuttingDown = true;
        if (earthquakeRoutine != null) { StopCoroutine(earthquakeRoutine); earthquakeRoutine = null; }
        if (dust != null && dust.isPlaying) dust.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        IsRunning = false; CurrentIntensity = 0; RestorePresentation();
    }

    private void OnDisable() => StopSafely();
    private void OnDestroy() => StopSafely();
}
