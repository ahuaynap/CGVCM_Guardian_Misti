using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-200)]
public sealed class JumpInputGate : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs inputs;
    private bool wasHeld;

    private void Awake() => inputs ??= GetComponent<StarterAssetsInputs>();

    private void Update()
    {
        bool held = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
        if (inputs != null) inputs.JumpInput(held && !wasHeld);
        wasHeld = held;
    }

    private void OnDisable()
    {
        wasHeld = false;
        inputs?.JumpInput(false);
    }
}
