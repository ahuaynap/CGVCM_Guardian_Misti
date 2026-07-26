using System;
using System.Linq;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public sealed class PlayerLookController : MonoBehaviour
{
    public const string SensitivityKey = "GuardianMisti.MouseSensitivity";
    public const float MinimumSensitivity = .25f;
    public const float MaximumSensitivity = 4f;
    public const float DefaultMouseSensitivity = 1.5f;
    public const float DefaultGamepadSensitivity = 180f;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private StarterAssetsInputs starterInputs;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform cameraHeightPivot;
    [SerializeField] private Transform cameraLookPivot;
    [SerializeField] private Transform cameraShakePivot;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float mouseSensitivity = DefaultMouseSensitivity;
    [SerializeField] private float gamepadSensitivity = DefaultGamepadSensitivity;
    [SerializeField] private float verticalSensitivityMultiplier = 1f;
    [SerializeField] private bool invertY;
    [SerializeField] private float minimumPitch = -85f;
    [SerializeField] private float maximumPitch = 85f;
    [SerializeField] private bool smoothingEnabled;

    private InputAction lookAction;
    private bool inputEnabled = true;
    private bool discardNextInput;
    private float pitch;

    public Vector2 RawLookInput { get; private set; }
    public string InputDeviceName { get; private set; } = "None";
    public float MouseSensitivity => mouseSensitivity;
    public float GamepadSensitivity => gamepadSensitivity;
    public float Pitch => pitch;
    public float MinimumPitch => minimumPitch;
    public float MaximumPitch => maximumPitch;
    public float Yaw => playerRoot == null ? 0f : playerRoot.eulerAngles.y;
    public bool SmoothingEnabled => smoothingEnabled;
    public bool InputEnabled => inputEnabled;
    public Transform CameraHeightPivot => cameraHeightPivot;
    public Transform CameraLookPivot => cameraLookPivot;
    public Transform CameraShakePivot => cameraShakePivot;
    public Camera MainCamera => mainCamera;
    public Vector3 ShakeOffset => cameraShakePivot == null ? Vector3.zero : cameraShakePivot.localPosition;
    public Vector3 CrouchHeightOffset => cameraHeightPivot == null ? Vector3.zero : cameraHeightPivot.localPosition;
    public string HierarchyDescription => cameraHeightPivot == null || cameraLookPivot == null || cameraShakePivot == null || mainCamera == null
        ? "incompleta"
        : cameraHeightPivot.name + "/" + cameraLookPivot.name + "/" + cameraShakePivot.name + "/" + mainCamera.name;

    private void Awake()
    {
        lookAction = playerInput?.actions?.FindAction("Look", false);
        mouseSensitivity = RepairSensitivity(PlayerPrefs.GetFloat(SensitivityKey, mouseSensitivity), true);
        pitch = cameraLookPivot == null ? 0f : NormalizePitch(cameraLookPivot.localEulerAngles.x);
        WarnForInvalidSetup();
    }

    private void Update()
    {
        if (!inputEnabled || lookAction == null) { RawLookInput = Vector2.zero; return; }
        RawLookInput = lookAction.ReadValue<Vector2>();
        InputDeviceName = lookAction.activeControl?.device is Mouse ? "Mouse" : lookAction.activeControl?.device is Gamepad ? "Gamepad" : "Other";
    }

    private void LateUpdate()
    {
        if (starterInputs != null) starterInputs.LookInput(Vector2.zero);
        if (!inputEnabled || discardNextInput) { discardNextInput = false; return; }
        ApplyLookDelta(RawLookInput, InputDeviceName == "Mouse", Time.deltaTime);
    }

    public void ApplyLookDelta(Vector2 input, bool mouse, float deltaTime)
    {
        float scale = mouse ? mouseSensitivity : gamepadSensitivity * Mathf.Max(0f, deltaTime);
        float yawDelta = input.x * scale;
        float pitchDelta = input.y * scale * verticalSensitivityMultiplier * (invertY ? -1f : 1f);
        if (playerRoot != null) playerRoot.Rotate(Vector3.up, yawDelta, Space.Self);
        pitch = ClampPitch(pitch - pitchDelta, minimumPitch, maximumPitch);
        if (cameraLookPivot != null) cameraLookPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        RawLookInput = Vector2.zero;
        if (starterInputs != null) starterInputs.LookInput(Vector2.zero);
        if (enabled) discardNextInput = true;
    }

    public void SetMouseSensitivity(float value, bool persist = true)
    {
        mouseSensitivity = RepairSensitivity(value, false);
        if (!persist) return;
        PlayerPrefs.SetFloat(SensitivityKey, mouseSensitivity);
        PlayerPrefs.Save();
    }

    public static float RepairSensitivity(float value, bool logRepair)
    {
        bool invalid = float.IsNaN(value) || float.IsInfinity(value) || value < MinimumSensitivity || value > MaximumSensitivity;
        if (!invalid) return value;
        if (logRepair) Debug.LogWarning($"[Camera] Invalid sensitivity {value}; restored to {DefaultMouseSensitivity}.");
        return DefaultMouseSensitivity;
    }

    public static float ClampPitch(float value, float minimum, float maximum) => Mathf.Clamp(value, minimum, maximum);
    public static Vector2 CalculateRotationDelta(Vector2 input, bool mouse, float mouseSensitivity, float gamepadSensitivity, float deltaTime) =>
        input * (mouse ? mouseSensitivity : gamepadSensitivity * Mathf.Max(0f, deltaTime));

    public static int CountActiveLookControllers(GameObject player) =>
        player == null ? 0 : player.GetComponentsInChildren<PlayerLookController>(true).Count(c => c.isActiveAndEnabled);

    private void WarnForInvalidSetup()
    {
        if (playerRoot == null || cameraHeightPivot == null || cameraLookPivot == null || cameraShakePivot == null || mainCamera == null)
            Debug.LogWarning("[Camera] Camera hierarchy reference is missing.", this);
        int count = CountActiveLookControllers(gameObject);
        if (count > 1) Debug.LogWarning($"[Camera] Duplicate look controllers found: {count}.", this);
    }

    private static float NormalizePitch(float degrees) => degrees > 180f ? degrees - 360f : degrees;
}
