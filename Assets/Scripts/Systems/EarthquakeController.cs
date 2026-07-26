using System;
using System.Linq;
using UnityEngine;

public enum EarthquakeState { Inactive, Preparing, Light, Moderate, Strong, Decreasing, Finished }

public sealed class EarthquakeController : MonoBehaviour
{
    [SerializeField] private EarthquakeProfile profile;
    [SerializeField] private Transform cameraEffectRoot;
    [SerializeField] private Light[] stableLights;
    [SerializeField] private Light[] emergencyLights;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ParticleSystem debris;
    [SerializeField] private AudioSource rumbleSource;
    [SerializeField] private AudioSource alarmSource;
    [SerializeField] private AudioSource impactSource;
    [SerializeField] private EarthquakeReactiveProp[] reactiveProps;
    [SerializeField] private ObjectivesManager objectivesManager;
    [SerializeField] private NotificationUI notificationUI;
    [SerializeField, Min(.25f)] private float failureMessageDelay = 1.5f;
    [SerializeField, Min(1f)] private float recoveryGraceSeconds = 3f;
    public event Action<EarthquakeState> StateChanged;
    public event Action<string> CountdownChanged;
    public event Action EarthquakeStarted;
    public event Action EarthquakeFinished;
    public EarthquakeState State { get; private set; } = EarthquakeState.Inactive;
    public bool IsRunning => State is EarthquakeState.Light or EarthquakeState.Moderate or EarthquakeState.Strong or EarthquakeState.Decreasing;
    public float CurrentIntensity { get; private set; }
    public int DisplayedCountdown { get; private set; }
    public int ActivePhysicsProps => (reactiveProps??Array.Empty<EarthquakeReactiveProp>()).Count(p=>p!=null&&p.CanReceiveForces);
    public Vector3 CameraPresentationOffset => cameraEffectRoot==null?Vector3.zero:cameraEffectRoot.localPosition-initialCameraLocalPosition;
    public bool ProtectionReached => validProtectionEntered;
    public bool ProtectionDwellSatisfied => dwellCompleted;
    public bool ProtectionEntered => protectionEntered;
    public bool ValidProtectionEntered => validProtectionEntered;
    public bool ProtectionSucceeded => protectionSucceeded;
    public bool ProtectionFailed => protectionFailed;
    public bool EarthquakeFinishedState => earthquakeFinished;
    public bool ProtectionResolved => protectionResolved;
    public bool HasFailureRecovery => failureMessageDelay > 0f && recoveryGraceSeconds > 0f;
    public bool IsProtectionPhase => State is EarthquakeState.Light or EarthquakeState.Moderate or EarthquakeState.Strong;
    private Vector3 initialCameraLocalPosition;
    private float[] stableIntensities=Array.Empty<float>(), emergencyIntensities=Array.Empty<float>();
    private bool[] emergencyEnabled=Array.Empty<bool>();
    private float preparationRemaining, earthquakeElapsed, startBannerRemaining, finishedElapsed, failureDelayRemaining;
    private bool sequenceStarted, protectionEntered, validProtectionEntered, dwellCompleted, protectionSucceeded, earthquakeFinished, protectionFailed, protectionResolved, insideProtection, isShuttingDown;

    private void Start()=>BeginSequence();
    private void Update(){Tick(Time.deltaTime);TickProtectionResolution(Time.deltaTime);}
    public bool BeginSequence()
    {
        if(sequenceStarted||isShuttingDown||profile==null)return false;
        sequenceStarted=true;preparationRemaining=Mathf.Max(0f,profile.PreparationCountdown);earthquakeElapsed=0f;startBannerRemaining=finishedElapsed=failureDelayRemaining=0f;protectionEntered=validProtectionEntered=dwellCompleted=protectionSucceeded=earthquakeFinished=protectionFailed=protectionResolved=insideProtection=false;CurrentIntensity=0f;
        if(cameraEffectRoot!=null)initialCameraLocalPosition=cameraEffectRoot.localPosition;
        CacheLights();SetParticleRate(dust,0);SetParticleRate(debris,0);SetState(EarthquakeState.Preparing);PublishCountdown(true);return true;
    }
    public void Tick(float deltaTime)
    {
        if(!sequenceStarted||isShuttingDown||deltaTime<=0f)return;
        if(State==EarthquakeState.Preparing){preparationRemaining=Mathf.Max(0f,preparationRemaining-deltaTime);PublishCountdown(false);if(preparationRemaining<=0f)StartEarthquake();return;}
        if(!IsRunning)return;
        if(startBannerRemaining>0f){startBannerRemaining=Mathf.Max(0f,startBannerRemaining-deltaTime);if(startBannerRemaining<=0f)CountdownChanged?.Invoke(string.Empty);}
        earthquakeElapsed=Mathf.Min(profile.Duration,earthquakeElapsed+deltaTime);CurrentIntensity=profile.Evaluate(earthquakeElapsed);SetState(StateForProgress(profile.Progress(earthquakeElapsed)));
        if(State==EarthquakeState.Strong&&!insideProtection)SimulationSession.Instance?.RecordStrongOutside(deltaTime);
        ApplyPresentation();if(earthquakeElapsed>=profile.Duration)FinishEarthquake();
    }
    public void MarkProtectionTriggerEntered(){protectionEntered=true;Debug.Log("[Protection] Player entered protection trigger. Phase="+State,this);}
    public bool TryMarkProtectionEntered(){protectionEntered=true;if(!IsProtectionPhase)return false;insideProtection=true;if(!validProtectionEntered){validProtectionEntered=true;SimulationSession.Instance?.RecordProtection(earthquakeElapsed);Debug.Log("[Protection] Valid protection entry. Dwell started. Phase="+State,this);}return true;}
    public void MarkProtectionDwellSatisfied(){if(!IsProtectionPhase||!insideProtection||dwellCompleted)return;dwellCompleted=true;protectionSucceeded=true;Debug.Log("[Protection] Dwell completed. Protection succeeded.",this);}
    public void MarkProtectionExited(){insideProtection=false;Debug.Log("[Protection] Player exited protection trigger. Phase="+State,this);}
    public void ResetSequence(){sequenceStarted=false;protectionEntered=validProtectionEntered=dwellCompleted=protectionSucceeded=earthquakeFinished=protectionFailed=protectionResolved=insideProtection=false;preparationRemaining=earthquakeElapsed=startBannerRemaining=finishedElapsed=failureDelayRemaining=CurrentIntensity=0f;RestorePresentation();SetState(EarthquakeState.Inactive);}
    public static EarthquakeState StateForProgress(float p)=>p<.25f?EarthquakeState.Light:p<.5f?EarthquakeState.Moderate:p<.75f?EarthquakeState.Strong:EarthquakeState.Decreasing;
    private void PublishCountdown(bool force){int value=Mathf.Clamp(Mathf.CeilToInt(preparationRemaining),1,3);if(!force&&DisplayedCountdown==value)return;DisplayedCountdown=value;CountdownChanged?.Invoke($"El simulacro comenzará en {value}...");}
    private void StartEarthquake(){DisplayedCountdown=0;CountdownChanged?.Invoke("¡SISMO!");startBannerRemaining=.75f;SetState(EarthquakeState.Light);objectivesManager?.TryCompleteObjective(GameIds.Level01Preparation);SimulationSession.Instance?.StartTimer();EarthquakeStarted?.Invoke();PlayOptional(rumbleSource);PlayOptional(alarmSource);PlayParticles(dust);PlayParticles(debris);}
    private void FinishEarthquake(){CurrentIntensity=0f;SetParticleRate(dust,0);SetParticleRate(debris,0);StopOptional(alarmSource);if(rumbleSource!=null&&rumbleSource.clip!=null)rumbleSource.volume=0;foreach(var prop in reactiveProps??Array.Empty<EarthquakeReactiveProp>())prop?.StopForces();RestorePresentation();SetState(EarthquakeState.Finished);earthquakeFinished=true;finishedElapsed=0f;failureDelayRemaining=failureMessageDelay;CountdownChanged?.Invoke(string.Empty);StopParticles(dust);StopParticles(debris);Debug.Log("[Protection] Earthquake finished. Awaiting protection resolution.",this);if(protectionSucceeded)ResolveProtection(true,false);EarthquakeFinished?.Invoke();}
    public void TickProtectionResolution(float deltaTime)
    {
        if(!earthquakeFinished||protectionResolved||deltaTime<=0f)return;
        finishedElapsed+=deltaTime;
        if(protectionSucceeded){ResolveProtection(true,false);return;}
        failureDelayRemaining=Mathf.Max(0f,failureDelayRemaining-deltaTime);
        if(failureDelayRemaining<=0f){ResolveProtection(false,false);return;}
        if(finishedElapsed>=recoveryGraceSeconds&&objectivesManager!=null&&objectivesManager.IsCurrentObjective(GameIds.Level01Protect)){Debug.LogWarning("[Protection] Recovery guard resolved a stuck protection objective.",this);ResolveProtection(false,true);}
    }
    public bool ResolveProtection(bool succeeded,bool recovery)
    {
        if(!earthquakeFinished||protectionResolved)return false;
        protectionResolved=true;protectionSucceeded=succeeded;protectionFailed=!succeeded;
        if(succeeded)notificationUI?.ShowMessage("Protección completada.","El sismo ha finalizado. Evacúa con precaución.");
        else{SimulationSession.Instance?.RecordProtectionFailure();notificationUI?.ShowMessage("Protección no alcanzada","No alcanzaste la zona de protección a tiempo.");}
        bool advanced=objectivesManager!=null&&objectivesManager.IsCurrentObjective(GameIds.Level01Protect)&&objectivesManager.TryCompleteObjective(GameIds.Level01Protect);
        Debug.Log($"[Protection] Resolved as {(succeeded?"success":"failure")}. Objective advanced={advanced}. Recovery={recovery}",this);return advanced;
    }

    private void SetState(EarthquakeState next){if(State==next)return;State=next;if(next is EarthquakeState.Moderate or EarthquakeState.Strong)ReactProps(next);StateChanged?.Invoke(State);}
    private void ReactProps(EarthquakeState phase){float force=profile.MaximumPropForce*profile.EvaluateCurve(profile.PropForceCurve,earthquakeElapsed);int limit=Mathf.Min(profile.MaximumActivePhysicsProps,reactiveProps?.Length??0);for(int i=0;i<limit;i++)if(reactiveProps[i]!=null&&reactiveProps[i].React(phase,force)&&impactSource!=null&&impactSource.clip!=null)impactSource.PlayOneShot(impactSource.clip,Mathf.Clamp01(CurrentIntensity));}
    private void ApplyPresentation()
    {
        float shake=profile.EvaluateCurve(profile.CameraShakeCurve,earthquakeElapsed);if(cameraEffectRoot!=null){float t=earthquakeElapsed*profile.CameraShakeFrequency;Vector3 noise=new(Mathf.PerlinNoise(t,.17f)-.5f,Mathf.PerlinNoise(.31f,t)-.5f,0);cameraEffectRoot.localPosition=initialCameraLocalPosition+noise*(profile.CameraShakeIntensity*shake*2f);}
        float flicker=profile.EvaluateCurve(profile.LightFlickerCurve,earthquakeElapsed)*profile.LightFlickerIntensity;for(int i=0;i<(stableLights?.Length??0);i++)if(stableLights[i]!=null){float wave=.5f+.5f*Mathf.Sin(earthquakeElapsed*7f+i*1.7f);stableLights[i].enabled=true;stableLights[i].intensity=stableIntensities[i]*(1f-flicker*wave);}
        bool emergency=State is EarthquakeState.Moderate or EarthquakeState.Strong or EarthquakeState.Decreasing;for(int i=0;i<(emergencyLights?.Length??0);i++)if(emergencyLights[i]!=null){emergencyLights[i].enabled=emergency;emergencyLights[i].intensity=emergencyIntensities[i]*(emergency?(.75f+.25f*CurrentIntensity):1f);}
        SetParticleRate(dust,profile.MaximumDustEmission*profile.EvaluateCurve(profile.DustEmissionCurve,earthquakeElapsed));SetParticleRate(debris,Mathf.Min(3f,profile.MaximumDustEmission*.45f)*profile.EvaluateCurve(profile.DustEmissionCurve,earthquakeElapsed));
        float audio=profile.AudioIntensity*profile.EvaluateCurve(profile.AudioVolumeCurve,earthquakeElapsed);SetOptionalVolume(rumbleSource,audio);SetOptionalVolume(alarmSource,State==EarthquakeState.Strong?audio:audio*.55f);
    }
    private void CacheLights(){stableIntensities=(stableLights??Array.Empty<Light>()).Select(l=>l==null?0:l.intensity).ToArray();emergencyIntensities=(emergencyLights??Array.Empty<Light>()).Select(l=>l==null?0:l.intensity).ToArray();emergencyEnabled=(emergencyLights??Array.Empty<Light>()).Select(l=>l!=null&&l.enabled).ToArray();}
    private void RestorePresentation(){if(cameraEffectRoot!=null)cameraEffectRoot.localPosition=initialCameraLocalPosition;for(int i=0;i<(stableLights?.Length??0);i++)if(stableLights[i]!=null){stableLights[i].enabled=true;if(i<stableIntensities.Length)stableLights[i].intensity=stableIntensities[i];}for(int i=0;i<(emergencyLights?.Length??0);i++)if(emergencyLights[i]!=null){emergencyLights[i].enabled=i<emergencyEnabled.Length&&emergencyEnabled[i];if(i<emergencyIntensities.Length)emergencyLights[i].intensity=emergencyIntensities[i];}}
    private static void PlayParticles(ParticleSystem value){if(value!=null&&!value.isPlaying)value.Play();}private static void StopParticles(ParticleSystem value){if(value!=null&&value.isPlaying)value.Stop(true,ParticleSystemStopBehavior.StopEmitting);}private static void SetParticleRate(ParticleSystem value,float rate){if(value==null)return;var emission=value.emission;emission.rateOverTime=Mathf.Max(0,rate);}private static void PlayOptional(AudioSource source){if(source!=null&&source.clip!=null&&!source.isPlaying)source.Play();}private static void StopOptional(AudioSource source){if(source!=null&&source.isPlaying)source.Stop();}private static void SetOptionalVolume(AudioSource source,float volume){if(source!=null&&source.clip!=null)source.volume=Mathf.Clamp01(volume);}
    private void StopSafely(){if(isShuttingDown)return;isShuttingDown=true;sequenceStarted=false;SetParticleRate(dust,0);SetParticleRate(debris,0);StopParticles(dust);StopParticles(debris);StopOptional(rumbleSource);StopOptional(alarmSource);CurrentIntensity=0;RestorePresentation();}
    private void OnDisable()=>StopSafely();private void OnDestroy()=>StopSafely();
}
