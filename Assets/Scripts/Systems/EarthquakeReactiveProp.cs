using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public sealed class EarthquakeReactiveProp : MonoBehaviour
{
    [SerializeField] private bool routeSafe = true;
    [SerializeField] private float maximumSpeed = 2.2f;
    [SerializeField] private float maximumAngularSpeed = 6f;
    private Rigidbody body;
    private bool moderateApplied, strongApplied, hazardReported;
    public bool RouteSafe => routeSafe;
    public bool CanReceiveForces => body != null && !body.isKinematic;
    private void Awake(){body=GetComponent<Rigidbody>();body.isKinematic=true;body.useGravity=true;body.maxAngularVelocity=maximumAngularSpeed;}
    public bool React(EarthquakeState phase,float impulse){if(body==null)body=GetComponent<Rigidbody>();bool apply=phase==EarthquakeState.Moderate&&!moderateApplied||phase==EarthquakeState.Strong&&!strongApplied;if(!apply||impulse<=0f)return false;if(phase==EarthquakeState.Moderate)moderateApplied=true;else strongApplied=true;body.isKinematic=false;body.collisionDetectionMode=CollisionDetectionMode.ContinuousSpeculative;float seed=transform.position.x*3.17f+transform.position.z*5.31f;Vector3 direction=new(Mathf.Sin(seed)*.7f,.18f,Mathf.Cos(seed)*.7f);body.AddForce(direction.normalized*impulse,ForceMode.Impulse);body.AddTorque(new Vector3(.4f,.7f,.3f)*impulse,ForceMode.Impulse);return true;}
    public void StopForces(){if(body==null)return;body.linearVelocity=Vector3.ClampMagnitude(body.linearVelocity,maximumSpeed);body.angularVelocity=Vector3.ClampMagnitude(body.angularVelocity,maximumAngularSpeed);}
    public void PreviewReaction(EarthquakeState phase){if(phase is EarthquakeState.Moderate or EarthquakeState.Strong)transform.localRotation=Quaternion.Euler(phase==EarthquakeState.Strong?72f:24f,0,phase==EarthquakeState.Strong?18f:7f);}
    private void FixedUpdate(){if(body==null||body.isKinematic)return;body.linearVelocity=Vector3.ClampMagnitude(body.linearVelocity,maximumSpeed);body.angularVelocity=Vector3.ClampMagnitude(body.angularVelocity,maximumAngularSpeed);}
    private void OnCollisionEnter(Collision collision){if(hazardReported||!collision.gameObject.CompareTag("Player"))return;hazardReported=true;SimulationSession.Instance?.RecordHazard();}
}
