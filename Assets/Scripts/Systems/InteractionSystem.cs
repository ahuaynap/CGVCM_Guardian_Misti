using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 3.5f;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private InteractionUIController interactionUIController;
    private IInteractable currentInteractable;
    private void Update()
    {
        DetectInteractable();
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) currentInteractable?.Interact();
    }
    private void DetectInteractable()
    {
        IInteractable detected = null;
        if (playerCamera != null && Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,
            out RaycastHit hit, interactionDistance, interactionMask, QueryTriggerInteraction.Ignore))
        {
            hit.collider.TryGetComponent(out detected);
            if (detected == null) detected = hit.collider.GetComponentInParent<IInteractable>();
        }
        if (ReferenceEquals(detected, currentInteractable)) return;
        currentInteractable = detected;
        if (detected == null) interactionUIController?.Hide(); else interactionUIController?.Show(detected);
    }
    private void OnDisable() { currentInteractable = null; interactionUIController?.Hide(); }
}
