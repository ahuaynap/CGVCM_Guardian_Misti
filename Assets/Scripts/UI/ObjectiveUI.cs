using TMPro;
using UnityEngine;
public class ObjectiveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectiveText;
    public void Refresh(Objective objective) { if (objectiveText != null) objectiveText.text = objective == null ? string.Empty : objective.Description; }
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
