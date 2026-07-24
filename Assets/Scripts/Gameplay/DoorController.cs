using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private float openAngle = 95f;
    [SerializeField] private float openSpeed = 120f;
    [SerializeField] private string objectiveId = GameIds.Level01ExitRoom;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openClip;
    private bool isOpen, isOpening;
    private Quaternion initialRotation, targetRotation;
    public string Prompt => isOpen ? "Puerta abierta" : "Abrir puerta";
    private void Awake() { initialRotation = transform.rotation; }
    public void Interact()
    {
        if (isOpen || isOpening || ObjectivesManager.Instance == null || !ObjectivesManager.Instance.IsCurrentObjective(objectiveId)) return;
        isOpening = true; targetRotation = initialRotation * Quaternion.Euler(0, openAngle, 0);
        if (audioSource != null && openClip != null) audioSource.PlayOneShot(openClip);
    }
    private void Update()
    {
        if (!isOpening) return;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, openSpeed * Time.deltaTime);
        if (Quaternion.Angle(transform.rotation, targetRotation) > .1f) return;
        transform.rotation = targetRotation; isOpening = false; isOpen = true;
        ObjectivesManager.Instance?.TryCompleteObjective(objectiveId);
    }
    public void OpenDoor() { transform.rotation = initialRotation * Quaternion.Euler(0, openAngle, 0); isOpen = true; }
}
