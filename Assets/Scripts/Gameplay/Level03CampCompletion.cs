using TMPro;
using UnityEngine;

public sealed class Level03CampCompletion : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private GameObject completionPanel;
    private bool completed;

    private void OnTriggerEnter(Collider other)
    {
        if (completed || !other.CompareTag("Player")) return;
        completed = true;
        if (objectiveText != null) objectiveText.text = "Campamento médico alcanzado.";
        if (completionPanel != null) completionPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("[Level03] Emergency camp objective completed.", this);
    }
}
