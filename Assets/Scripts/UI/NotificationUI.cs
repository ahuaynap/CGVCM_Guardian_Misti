using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Animator animator;
    private Coroutine routine;
    public void Show(InventoryItem item)
    {
        if (icon != null) { icon.enabled = item.Icon != null; icon.sprite = item.Icon; }
        ShowMessage("Objeto obtenido", item.Name);
    }
    public void ShowMessage(string title, string description)
    {
        if (routine != null) StopCoroutine(routine);
        titleText.text = title; descriptionText.text = description; gameObject.SetActive(true);
        routine = StartCoroutine(HideLater());
    }
    private IEnumerator HideLater() { yield return new WaitForSecondsRealtime(2.4f); gameObject.SetActive(false); routine = null; }
}
