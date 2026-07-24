using TMPro;
using UnityEngine;
using UnityEngine.UI;
public sealed class InventorySlotView : MonoBehaviour
{
 [SerializeField] private string itemId;
 [SerializeField] private Image icon;
 [SerializeField] private TMP_Text itemName;
 [SerializeField] private GameObject checkMark;
 [SerializeField] private Image accent;
 [SerializeField] private Color emptyColor=new(.32f,.38f,.42f,.65f);
 [SerializeField] private Color obtainedColor=new(.12f,.82f,.72f,1f);
 public string ItemId=>itemId;
 public bool IsObtained{get;private set;}
 public void SetObtained(bool obtained){IsObtained=obtained;if(icon!=null)icon.color=obtained?Color.white:emptyColor;if(itemName!=null)itemName.color=obtained?Color.white:new Color(.65f,.7f,.73f,1);if(checkMark!=null)checkMark.SetActive(obtained);if(accent!=null)accent.color=obtained?obtainedColor:new Color(.12f,.17f,.2f,.9f);}
}
