using UnityEngine;
public class GameplayCursorController : MonoBehaviour
{
    private void OnEnable() { if (Time.timeScale > 0f) CursorState.ApplyGameplayMode(); }
}
