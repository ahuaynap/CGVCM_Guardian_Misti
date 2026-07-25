using UnityEngine;

public class InteractionUIController : MonoBehaviour
{
    [SerializeField] private InteractionPromptUI promptUI;
    [SerializeField] private CrosshairUI crosshairUI;
    private IInteractable displayed;

    public void Show(IInteractable interactable)
    {
        if (interactable == null || this == null || !isActiveAndEnabled) return;
        if (ReferenceEquals(displayed, interactable)) return;
        displayed = interactable;
        if (promptUI != null) promptUI.Show(interactable.Prompt);
        if (crosshairUI != null) crosshairUI.SetInteractable(true);
    }

    public void Hide()
    {
        if (this == null) return;
        displayed = null;
        if (promptUI != null) promptUI.Hide();
        if (crosshairUI != null) crosshairUI.SetInteractable(false);
    }

    private void OnDisable() => displayed = null;
    private void OnDestroy() { displayed = null; promptUI = null; crosshairUI = null; }
}
