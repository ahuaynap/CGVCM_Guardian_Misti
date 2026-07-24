using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class GameplayDiagnosticsOverlay : MonoBehaviour
{
    [SerializeField] private GameplayDiagnostics diagnostics;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text diagnosticText;
    private void Start() { if (panel != null) panel.SetActive(false); }
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f3Key.wasPressedThisFrame && panel != null) panel.SetActive(!panel.activeSelf);
        if (panel != null && panel.activeSelf && diagnosticText != null && diagnostics != null) diagnosticText.text = diagnostics.Snapshot;
    }
}
