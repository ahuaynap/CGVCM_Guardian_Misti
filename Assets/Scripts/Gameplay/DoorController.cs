using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private float openAngle = 95f;
    [SerializeField] private float openSpeed = 120f;
    [SerializeField] private string objectiveId = GameIds.Level01ExitRoom;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openClip;
    [SerializeField] private Transform doorLeaf;
    [SerializeField] private Collider blockingCollider;
    private bool isOpen, isOpening;
    private Quaternion initialRotation, targetRotation;
    public string Prompt => isOpen ? "Puerta abierta" : "Abrir puerta";
    private void Awake() { if (doorLeaf == null) doorLeaf = transform; if (blockingCollider == null) blockingCollider = GetComponent<Collider>(); initialRotation = doorLeaf.localRotation; }
    public void Interact()
    {
        if (isOpen || isOpening || ObjectivesManager.Instance == null || !ObjectivesManager.Instance.IsCurrentObjective(objectiveId)) return;
        isOpening = true; targetRotation = initialRotation * Quaternion.Euler(0, openAngle, 0);
        if (audioSource != null && openClip != null) audioSource.PlayOneShot(openClip);
    }
    private void Update()
    {
        if (!isOpening) return;
        doorLeaf.localRotation = Quaternion.RotateTowards(doorLeaf.localRotation, targetRotation, openSpeed * Time.deltaTime);
        if (Quaternion.Angle(doorLeaf.localRotation, targetRotation) > .1f) return;
        doorLeaf.localRotation = targetRotation; isOpening = false; isOpen = true;
        if (blockingCollider != null) blockingCollider.enabled = false;
        ObjectivesManager.Instance?.TryCompleteObjective(objectiveId);
    }
    public void OpenDoor() { if (doorLeaf == null) doorLeaf = transform; doorLeaf.localRotation = initialRotation * Quaternion.Euler(0, openAngle, 0); if (blockingCollider != null) blockingCollider.enabled = false; isOpen = true; }
}
