using UnityEngine;

public class GameCompletionUI : MonoBehaviour
{
    [SerializeField] private GameCompletionController controller;
    public void Show() => controller?.EnterCompletionMode();
    public void Hide() { gameObject.SetActive(false); }
}
