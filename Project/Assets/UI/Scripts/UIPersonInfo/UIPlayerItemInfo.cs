using UnityEngine;
using System.Collections;

public class UIPlayerItemInfo : MonoBehaviour
{
    UIControlFightItem fightItem;
    void OnEnable()
    {
        fightItem = this.gameObject.GetComponent<UIControlFightItem>();
        fightItem.txtItemCount.gameObject.SetActive(false);
    }
    public void SetItemInfo(int itemId)
    {
        EntityModel model = DataCenter.Instance.FindEntityModelById(itemId);
        fightItem.txtItemLevel.text = model.level.ToString();
        fightItem.iconItem.spriteName = ResourceUtil.GetItemIconByModel(model);
    }
}
