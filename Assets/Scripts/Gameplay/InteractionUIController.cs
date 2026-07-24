using UnityEngine;
public class InteractionUIController : MonoBehaviour
{
    [SerializeField] private InteractionPromptUI promptUI;
    [SerializeField] private CrosshairUI crosshairUI;
    public void Show(IInteractable interactable) { promptUI?.Show(interactable.Prompt); crosshairUI?.SetInteractable(true); }
    public void Hide() { promptUI?.Hide(); crosshairUI?.SetInteractable(false); }
}
