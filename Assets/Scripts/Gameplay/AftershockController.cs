using System;
using TMPro;
using UnityEngine;

public enum AftershockState { Inactive, Warning, Light, Moderate, Decreasing, Finished }

public sealed class AftershockController : MonoBehaviour
{
    [SerializeField] private EarthquakeProfile profile;
    [SerializeField] private Transform player;
    [SerializeField] private Transform stableFloor;
    [SerializeField] private Transform cameraEffectRoot;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private EarthquakeReactiveProp[] reactiveProps;
    [SerializeField] private AudioSource rumbleSource;
    [SerializeField] private float orientationSeconds=4f, warningSeconds=3f, lightSeconds=3f, moderateSeconds=4f, decreasingSeconds=2f;
    private float elapsed, phaseElapsed; private Vector3 playerStart,floorStart,cameraBase; private bool reactionRecorded;
    private bool missingClipWarningIssued, missingSourceWarningIssued;
    public AftershockState State {get;private set;}=AftershockState.Inactive;
    public bool IsActive=>State is AftershockState.Light or AftershockState.Moderate or AftershockState.Decreasing;
    public EarthquakeProfile Profile=>profile;
    public AudioSource RumbleSource=>rumbleSource;
    public float StateElapsed=>phaseElapsed;
    public event Action<AftershockState> StateChanged;
    private void Awake()=>EnsureAudioReferences();
    private void Start(){playerStart=player==null?Vector3.zero:player.position;floorStart=stableFloor==null?Vector3.zero:stableFloor.position;cameraBase=cameraEffectRoot==null?Vector3.zero:cameraEffectRoot.localPosition;}
    private void Update()
    {
        elapsed+=Time.deltaTime;
        if(State==AftershockState.Inactive&&elapsed>=orientationSeconds)SetState(AftershockState.Warning);
        else if(State!=AftershockState.Inactive&&State!=AftershockState.Finished){phaseElapsed+=Time.deltaTime;float limit=State switch{AftershockState.Warning=>warningSeconds,AftershockState.Light=>lightSeconds,AftershockState.Moderate=>moderateSeconds,_=>decreasingSeconds};if(phaseElapsed>=limit)SetState((AftershockState)((int)State+1));}
        if(IsActive&&cameraEffectRoot!=null){float strength=State==AftershockState.Moderate?.035f:.015f;cameraEffectRoot.localPosition=cameraBase+new Vector3(Mathf.Sin(Time.time*19),Mathf.Sin(Time.time*23),0)*strength;}
    }
    private void SetState(AftershockState next)
    {
        if(State==next)return;
        State=next;phaseElapsed=0;Debug.Log("[Aftershock] "+next,this);
        if(warningText!=null){warningText.gameObject.SetActive(next!=AftershockState.Finished);warningText.text=next==AftershockState.Warning?"RÉPLICA SIMULADA\nPosible réplica. Mantente alejado de estructuras inestables.":next==AftershockState.Finished?"":"RÉPLICA SIMULADA\nAléjate de estructuras y sigue la ruta señalizada.";}
        if(next==AftershockState.Light){dust?.Play();PlayRumble();React(EarthquakeState.Moderate,.8f);}
        if(next==AftershockState.Moderate)React(EarthquakeState.Strong,1.25f);
        if(next==AftershockState.Finished){dust?.Stop(true,ParticleSystemStopBehavior.StopEmitting);StopRumble();if(cameraEffectRoot!=null)cameraEffectRoot.localPosition=cameraBase;foreach(var p in reactiveProps??Array.Empty<EarthquakeReactiveProp>())p?.StopForces();}
        StateChanged?.Invoke(next);
    }
    public AudioSource EnsureAudioReferences()
    {
        if(rumbleSource!=null)return rumbleSource;
        Transform audioRoot=transform.Find("Audio");
        Transform dedicated=audioRoot==null?null:audioRoot.Find("RumbleSource");
        rumbleSource=dedicated==null?GetComponent<AudioSource>():dedicated.GetComponent<AudioSource>();
        if(rumbleSource!=null)return rumbleSource;
        if(audioRoot==null){var audioObject=new GameObject("Audio");audioObject.transform.SetParent(transform,false);audioRoot=audioObject.transform;}
        var sourceObject=new GameObject("RumbleSource");sourceObject.transform.SetParent(audioRoot,false);
        rumbleSource=sourceObject.AddComponent<AudioSource>();ConfigureRumbleSource(rumbleSource);return rumbleSource;
    }
    private static void ConfigureRumbleSource(AudioSource source){source.playOnAwake=false;source.loop=true;source.spatialBlend=0f;source.volume=.45f;}
    public void PlayRumble(){AudioSource source=EnsureAudioReferences();if(source==null){if(!missingSourceWarningIssued){Debug.LogWarning("[Aftershock] RumbleSource could not be resolved; continuing without audio.",this);missingSourceWarningIssued=true;}return;}if(source.clip==null){if(!missingClipWarningIssued){Debug.LogWarning("[Aftershock] No rumble clip is assigned; continuing without aftershock audio.",this);missingClipWarningIssued=true;}return;}if(!source.isPlaying)source.Play();}
    public void StopRumble(){if(rumbleSource!=null&&rumbleSource.isPlaying)rumbleSource.Stop();}
    private void OnDisable(){StopRumble();if(cameraEffectRoot!=null)cameraEffectRoot.localPosition=cameraBase;}
    private void OnDestroy()=>StopRumble();
    private void React(EarthquakeState phase,float impulse){foreach(var p in reactiveProps??Array.Empty<EarthquakeReactiveProp>())p?.React(phase,impulse);}
    public void RecordClearAreaReached(){if(reactionRecorded)return;reactionRecorded=true;SimulationSession.Instance?.RecordAftershockClearArea(elapsed);}
    public static AftershockState NextPhase(AftershockState phase)=>phase==AftershockState.Finished?phase:(AftershockState)((int)phase+1);
    public bool PreservesStableTransforms()=>player==null||player.position==playerStart&&stableFloor!=null&&stableFloor.position==floorStart;
}
