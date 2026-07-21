using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text promptText;

    public void Show(string prompt)
    {
        gameObject.SetActive(true);

        promptText.text = $"[E]\n {prompt}";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
