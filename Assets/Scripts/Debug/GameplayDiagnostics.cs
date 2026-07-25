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
    private Vector3 previousPosition;
    private float smoothedFps;
    public string Snapshot { get; private set; }

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
            $"overlaps {overlaps.Length}  blocker {runtimeLogger?.LastBlockingCollider ?? "ninguno"}  ground {ground}\nFPS {smoothedFps:F1}";
    }
}
