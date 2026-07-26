using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 3.5f;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private InteractionUIController interactionUIController;
    private IInteractable currentInteractable;
    private bool presentationAvailable = true;

    private void Update()
    {
        DetectInteractable();
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentInteractable == null) SimulationSession.Instance?.RecordIncorrectInteraction();
            else currentInteractable.Interact();
        }
    }

    private void DetectInteractable()
    {
        IInteractable detected = null;
        if (playerCamera != null && Physics.Raycast(playerCamera.transform.position,
            playerCamera.transform.forward, out RaycastHit hit, interactionDistance,
            interactionMask, QueryTriggerInteraction.Ignore))
        {
            hit.collider.TryGetComponent(out detected);
            if (detected == null) detected = hit.collider.GetComponentInParent<IInteractable>();
        }
        if (ReferenceEquals(detected, currentInteractable))
        {
            if (detected != null && presentationAvailable && interactionUIController != null) interactionUIController.Refresh(detected);
            return;
        }
        currentInteractable = detected;
        if (!presentationAvailable || interactionUIController == null) return;
        if (detected == null) interactionUIController.Hide(); else interactionUIController.Show(detected);
    }

    public void SetPresentationAvailable(bool available)
    {
        presentationAvailable = available;
        if (!available) currentInteractable = null;
    }

    private void OnDisable()
    {
        // Scene teardown order is undefined. Never call scene-owned presentation here.
        currentInteractable = null;
        presentationAvailable = false;
    }

    private void OnEnable() => presentationAvailable = true;
    private void OnDestroy() { currentInteractable = null; interactionUIController = null; playerCamera = null; }
}
