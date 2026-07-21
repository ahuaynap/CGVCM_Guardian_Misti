using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;

    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private Animator animator;

    private Coroutine notificationCoroutine;

    public void Show(InventoryItem item)
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
            animator.Rebind();
        }

        icon.sprite = item.Icon;
        titleText.text = "Objecto obtenido";
        descriptionText.text = item.Name;

        notificationCoroutine = StartCoroutine(PlayNotification());
    }

    public IEnumerator PlayNotification()
    {
        animator.Play("In", 0, 0);

        yield return new WaitForSeconds(0.3f);

        animator.Play("Loop", 0, 0);

        yield return new WaitForSeconds(0.6f);

        animator.Play("Out", 0, 0);

        yield return new WaitForSeconds(0.3f);

        notificationCoroutine = null;
    }

}
