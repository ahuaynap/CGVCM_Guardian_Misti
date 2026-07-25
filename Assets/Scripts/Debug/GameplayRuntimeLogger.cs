using System;
using System.Collections.Generic;
using System.IO;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class GameplayRuntimeLogger : MonoBehaviour
{
    [SerializeField] private GameplayStateController stateController;
    [SerializeField] private StarterAssetsInputs inputs;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private FirstPersonController movement;
    [SerializeField] private CharacterController characterController;
    private readonly Queue<string> buffer = new();
    private string logPath;
    private Vector3 previousPosition;
    private bool previousGrounded, moving;
    private float stationaryDuration, nextFlush;
    private string lastCollider = "ninguno";
    public string LastBlockingCollider => lastCollider;
    public bool ObstructionDetected { get; private set; }

    private void Awake()
    {
        string directory = Path.Combine(Application.persistentDataPath, "GuardianMistiLogs");
        Directory.CreateDirectory(directory);
        logPath = Path.Combine(directory, $"gameplay-runtime-{DateTime.Now:yyyyMMdd-HHmmss}.log");
        previousPosition = transform.position;
        Log($"Scene loaded: {SceneManager.GetActiveScene().name}");
    }
    private void OnEnable() { if (stateController != null) stateController.StateChanged += OnStateChanged; }
    private void OnDisable() { if (stateController != null) stateController.StateChanged -= OnStateChanged; Flush(); }
    private void OnDestroy() => Flush();
    private void Update()
    {
        bool nowMoving = inputs != null && inputs.move.magnitude > .1f;
        if (nowMoving != moving) { moving = nowMoving; Log(moving ? $"Movement input began {inputs.move}" : "Movement input ended"); }
        bool grounded = movement != null && movement.Grounded;
        if (grounded != previousGrounded) { Log(grounded ? "Landing/grounded detected" : "Grounded false / takeoff"); previousGrounded = grounded; }
        DetectObstruction();
        previousPosition = transform.position;
        if (Time.unscaledTime >= nextFlush) { nextFlush = Time.unscaledTime + 2f; Flush(); }
    }
    private void DetectObstruction()
    {
        bool eligible = stateController != null && stateController.State == GameplayState.Playing &&
            inputs != null && inputs.move.magnitude > .5f && playerInput != null && playerInput.enabled &&
            characterController != null && characterController.enabled;
        float displacement = Vector3.ProjectOnPlane(transform.position - previousPosition, Vector3.up).magnitude;
        stationaryDuration = eligible && displacement < .003f ? stationaryDuration + Time.unscaledDeltaTime : 0f;
        if (stationaryDuration < .5f) { ObstructionDetected = false; return; }
        if (ObstructionDetected) return;
        ObstructionDetected = true;
        Collider[] nearby = Physics.OverlapCapsule(transform.position + Vector3.up * .35f,
            transform.position + Vector3.up * 1.65f, .48f, ~0, QueryTriggerInteraction.Collide);
        string names = string.Join(", ", Array.ConvertAll(nearby, c => $"{c.name}[layer={c.gameObject.layer},trigger={c.isTrigger},bounds={c.bounds}]"));
        Vector3 attempted = transform.right * inputs.move.x + transform.forward * inputs.move.y;
        Warn($"Unexpected movement interruption input={inputs.move} position={transform.position} attempted={attempted.normalized} " +
             $"velocity={characterController.velocity} grounded={movement?.Grounded} nearby={names} closest={lastCollider} " +
             $"stepOffset={characterController.stepOffset:F3} skinWidth={characterController.skinWidth:F3}");
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        lastCollider = $"{hit.collider.name} layer={hit.gameObject.layer} tag={hit.gameObject.tag}";
        Log($"CharacterController collision: {lastCollider} normal={hit.normal} point={hit.point}");
    }
    private void OnStateChanged(GameplayState previous, GameplayState current) => Log($"Gameplay state changed: {previous} -> {current}");
    private void OnApplicationFocus(bool focused) => Log(focused ? "Application focus regained" : "Application focus lost");
    public void Log(string message)
    {
        buffer.Enqueue($"{DateTime.Now:O} {message}");
        while (buffer.Count > 256) buffer.Dequeue();
    }
    public void Warn(string message) { Log("ANOMALY " + message); Debug.LogWarning("[GameplayDiagnostics] " + message, this); }
    private void Flush()
    {
        if (string.IsNullOrEmpty(logPath) || buffer.Count == 0) return;
        File.AppendAllLines(logPath, buffer); buffer.Clear();
    }
    public static bool ShouldReportObstruction(Vector2 move, GameplayState state, bool inputEnabled, bool controllerEnabled, float displacement, float duration) =>
        move.magnitude > .5f && state == GameplayState.Playing && inputEnabled && controllerEnabled && displacement < .003f && duration >= .5f;
}
