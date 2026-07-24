using System.Collections.Generic;
using UnityEngine;
public class InventoryUI : MonoBehaviour
{
 [SerializeField] private InventorySlotView[] slots;
 public void Refresh(IReadOnlyList<InventoryItem> items){foreach(var slot in slots??System.Array.Empty<InventorySlotView>()){bool obtained=false;if(items!=null)for(int i=0;i<items.Count;i++)if(items[i]?.Id==slot.ItemId){obtained=true;break;}slot.SetObtained(obtained);}}
 public void Refresh(InventoryItem item,int itemAmount){if(item==null){Refresh((IReadOnlyList<InventoryItem>)null);return;}foreach(var slot in slots??System.Array.Empty<InventorySlotView>())if(slot.ItemId==item.Id)slot.SetObtained(true);}
 public void Show()=>gameObject.SetActive(true);
 public void Hide()=>gameObject.SetActive(false);
}
