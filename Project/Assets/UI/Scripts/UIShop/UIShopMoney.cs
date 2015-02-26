using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIShopMoney : MonoBehaviour
{

    public UILabel txtItemName;
    public UILabel txtItemCount;
    public UISprite moneyIcon;
    public UISprite itemIcon;
    public UILabel txtConsume;
    private ShopModel itemData;
    public ShopModel ItemData
    {
        set
        {
            this.itemData = value;
            EntityModel model = DataCenter.Instance.FindEntityModelById(itemData.baseId);
            if (null == model)
            {
                Debug.Log("当前没有BaseID为" + itemData.baseId + "的数据");
            }
            else
            {
                txtItemName.text = model.nameForView;
                if (EntityTypeUtil.IsDiamond(model))
                {
                    txtItemCount.text = model.hp.ToString();
                    txtConsume.text = "￥ " + model.costResourceCount;
                }
                else if (EntityTypeUtil.IsGold(model))
                {
                    txtItemCount.text = Mathf.Ceil((DataCenter.Instance.GetMaxResourceStorage(ResourceType.Gold) * model.hp / 100)).ToString();
                }
                else if (EntityTypeUtil.IsOil(model))
                {
                    txtItemCount.text = Mathf.Ceil((DataCenter.Instance.GetMaxResourceStorage(ResourceType.Oil) * model.hp / 100)).ToString();
                }
                else if (EntityTypeUtil.IsMedal(model))
                {
                    txtItemCount.text = model.hp.ToString();
                    txtConsume.text = model.costResourceCount.ToString();
                }
                moneyIcon.spriteName = model.subType;
                itemIcon.spriteName = model.nameForResource;
            }
        }
        get
        {
            return this.itemData;
        }
    }
}
