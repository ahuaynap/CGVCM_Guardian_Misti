using System;
using UnityEngine;

public enum EarthquakeState { Inactive, Preparing, Light, Moderate, Strong, Decreasing, Finished }

public sealed class EarthquakeController : MonoBehaviour
{
    [SerializeField] private EarthquakeProfile profile;
    [SerializeField] private Transform cameraEffectRoot;
    [SerializeField] private Light[] emergencyLights;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ObjectivesManager objectivesManager;
    public event Action<EarthquakeState> StateChanged;
    public event Action<string> CountdownChanged;
    public event Action EarthquakeStarted;
    public event Action EarthquakeFinished;
    public EarthquakeState State { get; private set; } = EarthquakeState.Inactive;
    public bool IsRunning => State is EarthquakeState.Light or EarthquakeState.Moderate or EarthquakeState.Strong or EarthquakeState.Decreasing;
    public float CurrentIntensity { get; private set; }
    public int DisplayedCountdown { get; private set; }
    private Vector3 initialCameraLocalPosition;
    private float preparationRemaining, earthquakeElapsed, startBannerRemaining;
    private bool sequenceStarted, protectionReached, isShuttingDown;

    private void Start() => BeginSequence();
    private void Update() => Tick(Time.deltaTime);
    public bool BeginSequence()
    {
        if (sequenceStarted || isShuttingDown || profile == null) return false;
        sequenceStarted = true; preparationRemaining = Mathf.Max(0f, profile.PreparationCountdown); earthquakeElapsed = 0f; startBannerRemaining = 0f; protectionReached = false; CurrentIntensity = 0f;
        if (cameraEffectRoot != null) initialCameraLocalPosition = cameraEffectRoot.localPosition;
        SetState(EarthquakeState.Preparing); PublishCountdown(true); return true;
    }
    public void Tick(float deltaTime)
    {
        if (!sequenceStarted || isShuttingDown || deltaTime <= 0f) return;
        if (State == EarthquakeState.Preparing) { preparationRemaining = Mathf.Max(0f, preparationRemaining - deltaTime); PublishCountdown(false); if (preparationRemaining <= 0f) StartEarthquake(); return; }
        if (!IsRunning) return;
        if (startBannerRemaining > 0f) { startBannerRemaining = Mathf.Max(0f, startBannerRemaining - deltaTime); if (startBannerRemaining <= 0f) CountdownChanged?.Invoke(string.Empty); }
        earthquakeElapsed = Mathf.Min(profile.Duration, earthquakeElapsed + deltaTime); CurrentIntensity = profile.Evaluate(earthquakeElapsed);
        SetState(StateForProgress(profile.Duration <= 0f ? 1f : earthquakeElapsed / profile.Duration)); ApplyPresentation();
        if (earthquakeElapsed >= profile.Duration) FinishEarthquake();
    }
    public void MarkProtectionReached() { if (State is EarthquakeState.Preparing or EarthquakeState.Inactive) return; protectionReached = true; TryAdvanceProtectionObjective(); }
    public void ResetSequence() { sequenceStarted = false; protectionReached = false; preparationRemaining = earthquakeElapsed = startBannerRemaining = CurrentIntensity = 0f; RestorePresentation(); SetState(EarthquakeState.Inactive); }
    public static EarthquakeState StateForProgress(float p) => p < .25f ? EarthquakeState.Light : p < .5f ? EarthquakeState.Moderate : p < .75f ? EarthquakeState.Strong : EarthquakeState.Decreasing;
    private void PublishCountdown(bool force) { int value = Mathf.Clamp(Mathf.CeilToInt(preparationRemaining), 1, 3); if (!force && DisplayedCountdown == value) return; DisplayedCountdown = value; CountdownChanged?.Invoke($"El simulacro comenzará en {value}..."); }
    private void StartEarthquake() { DisplayedCountdown = 0; CountdownChanged?.Invoke("¡SISMO!"); startBannerRemaining = .75f; SetState(EarthquakeState.Light); objectivesManager?.TryCompleteObjective(GameIds.Level01Preparation); SimulationSession.Instance?.StartTimer(); EarthquakeStarted?.Invoke(); if (dust != null && !dust.isPlaying) dust.Play(); }
    private void FinishEarthquake() { CurrentIntensity = 0f; RestorePresentation(); SetState(EarthquakeState.Finished); CountdownChanged?.Invoke(string.Empty); if (dust != null && dust.isPlaying) dust.Stop(true, ParticleSystemStopBehavior.StopEmitting); TryAdvanceProtectionObjective(); EarthquakeFinished?.Invoke(); }
    private void TryAdvanceProtectionObjective() { if (State == EarthquakeState.Finished && protectionReached) objectivesManager?.TryCompleteObjective(GameIds.Level01Protect); }
    private void SetState(EarthquakeState next) { if (State == next) return; State = next; StateChanged?.Invoke(State); }
    private void ApplyPresentation() { if (cameraEffectRoot != null) { float t=Time.unscaledTime*profile.CameraShakeFrequency; Vector3 n=new(Mathf.PerlinNoise(t,0)-.5f,Mathf.PerlinNoise(0,t)-.5f,0); cameraEffectRoot.localPosition=initialCameraLocalPosition+n*(profile.CameraShakeIntensity*CurrentIntensity); } foreach(Light light in emergencyLights??Array.Empty<Light>()) if(light!=null) light.enabled=Mathf.PerlinNoise(Time.unscaledTime*9,light.GetHashCode())>profile.LightFlickerIntensity*CurrentIntensity; }
    private void RestorePresentation() { if(cameraEffectRoot!=null)cameraEffectRoot.localPosition=initialCameraLocalPosition; foreach(Light light in emergencyLights??Array.Empty<Light>())if(light!=null)light.enabled=true; }
    private void StopSafely() { if(isShuttingDown)return; isShuttingDown=true;sequenceStarted=false;if(dust!=null&&dust.isPlaying)dust.Stop(true,ParticleSystemStopBehavior.StopEmitting);CurrentIntensity=0;RestorePresentation(); }
    private void OnDisable()=>StopSafely();
    private void OnDestroy()=>StopSafely();
}
