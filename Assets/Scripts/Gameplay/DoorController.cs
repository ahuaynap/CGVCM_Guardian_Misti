using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField]
    private float openAngle = 90f;

    [SerializeField]
    private float openSpeed = 90f;

    private bool isOpen;
    private bool isOpening;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    public string Prompt => "Abrir puerta";

    private void Awake()
    {
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (!isOpening)
        {
            return;
        }

        RotateDoor();
    }

    public void Interact()
    {
        if (isOpen || isOpening)
        {
            return;
        }

        if (ObjectivesManager.Instance != null &&
            !ObjectivesManager.Instance.IsCurrentObjective(GameIds.ExitRoomObjective))
        {
            return;
        }

        isOpening = true;
        targetRotation = initialRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    public void OpenDoor()
    {
        transform.rotation = initialRotation * Quaternion.Euler(0f, openAngle, 0f);
        isOpen = true;
    }

    public void RotateDoor()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            openSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            transform.rotation = targetRotation;
            isOpening = false;
            isOpen = true;
            NotifyObjectiveCompleted();
        }
    }

    private void NotifyObjectiveCompleted()
    {
        if (ObjectivesManager.Instance == null)
        {
            Debug.LogWarning("ObjectivesManager not found.", this);
            return;
        }

        ObjectivesManager.Instance.TryCompleteObjective(GameIds.ExitRoomObjective);
    }
}
