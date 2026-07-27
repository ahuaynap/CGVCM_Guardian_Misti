using System.Linq;
using StarterAssets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class GameplayDiagnostics : MonoBehaviour
{
    [SerializeField] private GameplayStateController stateController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private StarterAssetsInputs inputs;
    [SerializeField] private FirstPersonController movement;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private InteractionSystem interaction;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private GameplayRuntimeLogger runtimeLogger;
    [SerializeField] private PlayerLookController lookController;
    [SerializeField] private PlayerCrouchController crouchController;
    [SerializeField] private EarthquakeController earthquakeController;
    [SerializeField] private Transform protectionDesk;
    [SerializeField] private EarthquakeProtectionTrigger protectionTrigger;
    private Vector3 previousPosition;
    private float smoothedFps;
    public string Snapshot { get; private set; }

    private void Start(){ValidateProtectionDesk();}
    private void ValidateProtectionDesk(){if(protectionDesk==null){Debug.LogWarning("[ProtectionDesk] No physical desk detected.",this);return;}var colliders=protectionDesk.GetComponentsInChildren<Collider>(true);if(colliders.Length==0)Debug.LogWarning("[ProtectionDesk] No physical collider found.",this);else if(colliders.All(c=>c.isTrigger))Debug.LogWarning("[ProtectionDesk] All desk colliders are triggers.",this);else if(Physics.GetIgnoreLayerCollision(gameObject.layer,protectionDesk.gameObject.layer))Debug.LogWarning("[ProtectionDesk] Desk layer does not collide with Player layer.",this);}
    private void Update()
    {
        float dt = Mathf.Max(Time.unscaledDeltaTime, .0001f);
        Vector3 delta = transform.position - previousPosition;
        previousPosition = transform.position;
        smoothedFps = Mathf.Lerp(smoothedFps, 1f / dt, .1f);
        Collider[] overlaps = characterController == null ? new Collider[0] :
            Physics.OverlapCapsule(transform.position + Vector3.up * .35f, transform.position + Vector3.up * 1.65f,
                characterController.radius * .95f, ~0, QueryTriggerInteraction.Collide);
        string ground = overlaps.FirstOrDefault(c => !c.isTrigger && c.gameObject != gameObject && c.bounds.max.y <= transform.position.y + .25f)?.name ?? "ninguno";
        Snapshot =
            $"ESCENA {SceneManager.GetActiveScene().name}\nESTADO {stateController?.State}\ntimeScale {Time.timeScale:F2}  cursor {Cursor.lockState}/{Cursor.visible}  focus {Application.isFocused}\n" +
            $"PlayerInput {playerInput?.enabled}  map {playerInput?.currentActionMap?.name ?? "ninguno"}\nmove {inputs?.move}  look {inputs?.look}  jump {inputs?.jump}  sprint {inputs?.sprint}\n" +
            $"Grounded {movement?.Grounded}  CC.grounded {characterController?.isGrounded}  CC.enabled {characterController?.enabled}\n" +
            $"velocity {characterController?.velocity}  verticalCalc {(delta.y / dt):F3}\nposition {transform.position}  delta {delta}  speed {(delta.magnitude / dt):F3}\n" +
            $"FPC {movement?.enabled}  Inputs {inputs?.enabled}  Interaction {interaction?.enabled}\n" +
            $"EventSystem {(EventSystem.current != null)}  selected {(EventSystem.current?.currentSelectedGameObject?.name ?? "ninguno")}\n" +
            $"Pause {pausePanel?.activeSelf}  Completion {completionPanel?.activeSelf}\n" +
            $"overlaps {overlaps.Length}  blocker {runtimeLogger?.LastBlockingCollider ?? "ninguno"}  ground {ground}\n" +
            $"\nCAMERA\nraw {lookController?.RawLookInput}  device {lookController?.InputDeviceName}\n" +
            $"mouse {lookController?.MouseSensitivity:F2}  gamepad {lookController?.GamepadSensitivity:F1}\n" +
            $"pitch {lookController?.Pitch:F1}  yaw {lookController?.Yaw:F1}  smoothing {lookController?.SmoothingEnabled}\n" +
            $"paused {stateController?.State != GameplayState.Playing}  cursor {Cursor.lockState}\n" +
            $"controller {lookController?.GetType().Name ?? "ninguno"}  active {PlayerLookController.CountActiveLookControllers(gameObject)}\n" +
             "hierarchy " + (lookController?.HierarchyDescription ?? "incompleta") + "\n" +
            "shake " + lookController?.ShakeOffset + "  crouchHeight " + lookController?.CrouchHeightOffset + "\n" +
            "crouching " + crouchController?.IsCrouching + "  quake " + earthquakeController?.State + "\n" +
            DeskDiagnostics() + "\nFPS " + smoothedFps.ToString("F1");
    }
    private string DeskDiagnostics(){if(protectionDesk==null)return "DESK missing";var cs=protectionDesk.GetComponentsInChildren<Collider>(true);Bounds b=cs.Length>0?cs[0].bounds:new Bounds(protectionDesk.position,Vector3.zero);foreach(var c in cs)b.Encapsulate(c.bounds);string names=string.Join(",",cs.Select(c=>c.name+":"+(c.enabled?"on":"off")+":"+(c.isTrigger?"trigger":"solid")));bool collide=!Physics.GetIgnoreLayerCollision(gameObject.layer,protectionDesk.gameObject.layer);return "DESK bounds "+b.size+" colliders "+cs.Length+" ["+names+"]\nlayers player "+gameObject.layer+" desk "+protectionDesk.gameObject.layer+" collide "+collide+" inside "+protectionTrigger?.IsInside+" crouched "+crouchController?.IsCrouching+" clearance 2.30x1.35x1.53";}
}
