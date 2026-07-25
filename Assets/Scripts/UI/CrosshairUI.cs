using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private Image crossHair;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactableColor = new(.15f, .85f, 1f);
    private bool state;

    public void SetInteractable(bool interactable)
    {
        if (this == null || crossHair == null || state == interactable) return;
        state = interactable;
        crossHair.color = interactable ? interactableColor : defaultColor;
    }
    private void OnDestroy() => crossHair = null;
}
