using TMPro;
using UnityEngine;
public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;
    public void Show(string prompt) { gameObject.SetActive(true); if (promptText != null) promptText.text = $"[E]  {prompt}"; }
    public void Hide() => gameObject.SetActive(false);
}
