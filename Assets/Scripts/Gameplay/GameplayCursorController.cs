using UnityEngine;

public class GameplayCursorController : MonoBehaviour
{
    private void OnEnable()
    {
        CursorState.ApplyGameplayMode();
    }
}
