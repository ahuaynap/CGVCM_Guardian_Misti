using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-200)]
public sealed class JumpInputGate : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs inputs;
    private bool wasHeld;

    public bool EvaluatePress(bool held) { bool pressed = held && !wasHeld; wasHeld = held; return pressed; }

    private void Awake() => inputs ??= GetComponent<StarterAssetsInputs>();

    private void Update()
    {
        bool held = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
        if (inputs != null) inputs.JumpInput(EvaluatePress(held));
    }

    private void OnDisable()
    {
        wasHeld = false;
        inputs?.JumpInput(false);
    }
}
