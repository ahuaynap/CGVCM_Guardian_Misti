using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI inventoryText;

    [SerializeField]
    private Image inventoryImage;

    public void Refresh(InventoryItem item, int itemAmount)
    {
        inventoryText.text = $"x{itemAmount}";
        
        if(item != null )
        {
            inventoryImage.sprite = item.Icon;   
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
