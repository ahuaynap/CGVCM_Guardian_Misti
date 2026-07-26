using UnityEngine;

public class InteractionUIController : MonoBehaviour
{
    [SerializeField] private InteractionPromptUI promptUI;
    [SerializeField] private CrosshairUI crosshairUI;
    private IInteractable displayed;
    private string displayedPrompt;

    public void Show(IInteractable interactable)
    {
        if (interactable == null || this == null || !isActiveAndEnabled) return;
        if (ReferenceEquals(displayed, interactable)) { Refresh(interactable); return; }
        displayed = interactable;
        displayedPrompt = interactable.Prompt;
        if (promptUI != null) promptUI.Show(displayedPrompt);
        if (crosshairUI != null) crosshairUI.SetInteractable(true);
    }

    public void Refresh(IInteractable interactable)
    {
        if (interactable == null || !ReferenceEquals(displayed, interactable)) return;
        string prompt = interactable.Prompt;
        if (prompt == displayedPrompt) return;
        displayedPrompt = prompt; promptUI?.Show(prompt); Debug.Log("[Interaction] Prompt changed: " + prompt, this);
    }

    public void Hide()
    {
        if (this == null) return;
        displayed = null;
        displayedPrompt = null;
        if (promptUI != null) promptUI.Hide();
        if (crosshairUI != null) crosshairUI.SetInteractable(false);
    }

    private void OnDisable() => displayed = null;
    private void OnDestroy() { displayed = null; promptUI = null; crosshairUI = null; }
}
