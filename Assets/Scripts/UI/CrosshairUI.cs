using System;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{

    [SerializeField] private Image crossHair;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactableColor = Color.green;

    public void SetInteractable(bool interactable)
    {
        crossHair.color = interactable ? interactableColor : defaultColor;
    }
    

}
