using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UILogicFightItem : MonoBehaviour
{
    private UIControlFightItem fightItem;
    void OnEnable()
    {
        fightItem = this.transform.GetComponent<UIControlFightItem>();
    }
    public void SetItemInfo(int cid, int amount)
    {
        fightItem.txtItemCount.text = "X" + amount;
        if (cid == Constants.DENOTED_ARMY_ID)
        {
            fightItem.levelCon.SetActive(false);
        }
        else
        {
            EntityModel model = DataCenter.Instance.FindEntityModelById(cid);
            fightItem.txtItemLevel.text = model.level.ToString();
            fightItem.iconItem.spriteName = ResourceUtil.GetItemIconByModel(model);
        }
    }
}
