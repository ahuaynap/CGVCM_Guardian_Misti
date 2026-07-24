using UnityEngine;
using UnityEngine.UI;
public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private Image crossHair;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactableColor = new(0.2f, 1f, 0.45f);
    public void SetInteractable(bool interactable) { if (crossHair != null) crossHair.color = interactable ? interactableColor : defaultColor; }
}
