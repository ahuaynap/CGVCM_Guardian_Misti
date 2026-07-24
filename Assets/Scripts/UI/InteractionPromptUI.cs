using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;
    private string currentPrompt;

    public void Show(string prompt)
    {
        if (this == null || promptText == null) return;
        string formatted = $"[E]  {prompt}";
        if (currentPrompt != formatted) { promptText.text = formatted; currentPrompt = formatted; }
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (this == null) return;
        currentPrompt = null;
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    private void OnDestroy() { promptText = null; currentPrompt = null; }
}
