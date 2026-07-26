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
    public AftershockState State {get;private set;}=AftershockState.Inactive;
    public bool IsActive=>State is AftershockState.Light or AftershockState.Moderate or AftershockState.Decreasing;
    public EarthquakeProfile Profile=>profile;
    public event Action<AftershockState> StateChanged;
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
        State=next;phaseElapsed=0;Debug.Log("[Aftershock] "+next,this);
        if(warningText!=null){warningText.gameObject.SetActive(next!=AftershockState.Finished);warningText.text=next==AftershockState.Warning?"RÉPLICA SIMULADA\nPosible réplica. Mantente alejado de estructuras inestables.":next==AftershockState.Finished?"":"RÉPLICA SIMULADA\nAléjate de estructuras y sigue la ruta señalizada.";}
        if(next==AftershockState.Light){dust?.Play();rumbleSource?.Play();React(EarthquakeState.Moderate,.8f);}
        if(next==AftershockState.Moderate)React(EarthquakeState.Strong,1.25f);
        if(next==AftershockState.Finished){dust?.Stop(true,ParticleSystemStopBehavior.StopEmitting);rumbleSource?.Stop();if(cameraEffectRoot!=null)cameraEffectRoot.localPosition=cameraBase;foreach(var p in reactiveProps??Array.Empty<EarthquakeReactiveProp>())p?.StopForces();}
        StateChanged?.Invoke(next);
    }
    private void React(EarthquakeState phase,float impulse){foreach(var p in reactiveProps??Array.Empty<EarthquakeReactiveProp>())p?.React(phase,impulse);}
    public void RecordClearAreaReached(){if(reactionRecorded)return;reactionRecorded=true;SimulationSession.Instance?.RecordAftershockClearArea(elapsed);}
    public static AftershockState NextPhase(AftershockState phase)=>phase==AftershockState.Finished?phase:(AftershockState)((int)phase+1);
    public bool PreservesStableTransforms()=>player==null||player.position==playerStart&&stableFloor!=null&&stableFloor.position==floorStart;
}
