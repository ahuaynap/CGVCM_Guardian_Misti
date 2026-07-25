using TMPro;
using UnityEngine;
public class ObjectiveUI : MonoBehaviour
{
 [SerializeField] private TextMeshProUGUI objectiveText;
 [SerializeField] private CanvasGroup canvasGroup;
 private float animationRemaining;
 public void Refresh(Objective objective){if(objectiveText==null)return;objectiveText.text=objective?.Description??string.Empty;gameObject.SetActive(objective!=null);animationRemaining=.25f;if(canvasGroup!=null)canvasGroup.alpha=.35f;}
 private void Update(){if(animationRemaining<=0f||canvasGroup==null)return;animationRemaining=Mathf.Max(0,animationRemaining-Time.unscaledDeltaTime);canvasGroup.alpha=Mathf.Lerp(1f,.35f,animationRemaining/.25f);}
 public void Show()=>gameObject.SetActive(true);
 public void Hide(){if(objectiveText!=null)objectiveText.text=string.Empty;gameObject.SetActive(false);}
}
