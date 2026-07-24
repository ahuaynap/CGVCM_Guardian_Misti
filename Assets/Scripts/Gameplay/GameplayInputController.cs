using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameplayInputState { Gameplay, Paused, Completed, Transitioning }

public class GameplayInputController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private FirstPersonController movement;
    [SerializeField] private StarterAssetsInputs starterInputs;
    [SerializeField] private InteractionSystem interaction;
    public GameplayInputState State { get; private set; }

    private void Start() => EnterGameplay();

    public void EnterGameplay()
    {
        State = GameplayInputState.Gameplay;
        Time.timeScale = 1f;
        if (playerInput != null) playerInput.ActivateInput();
        if (movement != null) movement.enabled = true;
        if (interaction != null) { interaction.enabled = true; interaction.SetPresentationAvailable(true); }
        if (starterInputs != null) { starterInputs.cursorLocked = true; starterInputs.cursorInputForLook = true; }
        CursorState.ApplyGameplayMode();
    }

    public void EnterPause()
    {
        if (State == GameplayInputState.Completed) return;
        State = GameplayInputState.Paused;
        SuspendInput(); Time.timeScale = 0f; CursorState.ApplyMenuMode();
    }

    public void EnterCompletion()
    {
        State = GameplayInputState.Completed;
        Time.timeScale = 1f; SuspendInput(); CursorState.ApplyMenuMode();
    }

    private void SuspendInput()
    {
        if (starterInputs != null)
        {
            starterInputs.MoveInput(Vector2.zero); starterInputs.LookInput(Vector2.zero);
            starterInputs.JumpInput(false); starterInputs.SprintInput(false);
            starterInputs.cursorLocked = false; starterInputs.cursorInputForLook = false;
        }
        if (interaction != null) { interaction.SetPresentationAvailable(false); interaction.enabled = false; }
        if (movement != null) movement.enabled = false;
        if (playerInput != null) playerInput.DeactivateInput();
    }

    private void OnApplicationFocus(bool focused)
    {
        if (focused && State == GameplayInputState.Gameplay) EnterGameplay();
    }

    private void OnDestroy()
    {
        playerInput = null; movement = null; starterInputs = null; interaction = null;
        Time.timeScale = 1f;
    }
}
