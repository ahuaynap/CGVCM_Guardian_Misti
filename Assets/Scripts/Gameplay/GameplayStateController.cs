using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameplayState { Playing, Paused, Completed, Transitioning }

public class GameplayStateController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private FirstPersonController movement;
    [SerializeField] private StarterAssetsInputs starterInputs;
    [SerializeField] private InteractionSystem interaction;
    [SerializeField] private PlayerCrouchController crouch;
    [SerializeField] private PlayerLookController lookController;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject completionPanel;
    public GameplayState State { get; private set; } = GameplayState.Transitioning;
    public event Action<GameplayState, GameplayState> StateChanged;

    private void Start() => RequestState(GameplayState.Playing);

    public bool RequestState(GameplayState requested)
    {
        if (State == GameplayState.Completed && requested != GameplayState.Transitioning) return false;
        if (State == requested) { ApplyState(); return true; }
        GameplayState previous = State;
        State = requested;
        ApplyState();
        StateChanged?.Invoke(previous, State);
        return true;
    }

    private void ApplyState()
    {
        bool playing = State == GameplayState.Playing;
        Time.timeScale = State == GameplayState.Paused ? 0f : 1f;
        if (playing) playerInput?.ActivateInput(); else playerInput?.DeactivateInput();
        if (movement != null) movement.enabled = playing;
        crouch?.SetInputEnabled(playing);
        lookController?.SetInputEnabled(playing);
        if (interaction != null)
        {
            interaction.enabled = playing;
            interaction.SetPresentationAvailable(playing);
        }
        if (starterInputs != null)
        {
            if (!playing) ClearGameplayInput();
            starterInputs.cursorLocked = playing || State == GameplayState.Transitioning;
            starterInputs.cursorInputForLook = playing;
        }
        if (pausePanel != null) pausePanel.SetActive(State == GameplayState.Paused);
        if (completionPanel != null) completionPanel.SetActive(State == GameplayState.Completed);
        if (playing || State == GameplayState.Transitioning) CursorState.ApplyGameplayMode();
        else CursorState.ApplyMenuMode();
    }

    public void ClearGameplayInput()
    {
        if (starterInputs == null) return;
        starterInputs.MoveInput(Vector2.zero);
        starterInputs.LookInput(Vector2.zero);
        starterInputs.JumpInput(false);
        starterInputs.SprintInput(false);
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused) return;
        ApplyState();
    }
}
