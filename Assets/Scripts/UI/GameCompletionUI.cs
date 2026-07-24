using UnityEngine;

public class GameCompletionUI : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
        CursorState.ApplyMenuMode();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
