using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 3f;

    [Header("UI")]
    [SerializeField] private InteractionUIController interactionUIController;

    private IInteractable currentInteractable;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectInteractable();

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
        
    }

    private void DetectInteractable()
    {
        currentInteractable = null;
        interactionUIController.Hide();
        
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                currentInteractable = interactable;

                interactionUIController.Show(interactable);

                return;
            }
        }

    }

    private void Interact()
    {
        currentInteractable?.Interact();
    }

}
