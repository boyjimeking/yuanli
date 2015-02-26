using UnityEngine;
using System.Collections;

public class UIShopItemDes : MonoBehaviour
{
    public UILabel txtItemName;
    public UILabel txtItemDes;
    public ShopModel ItemData
    {
        set
        {
            EntityModel model = DataCenter.Instance.FindEntityModelById(value.baseId);
            if (null == model)
            {
                Debug.Log("当前没有BaseID为" + value.baseId + "的数据");
                return;
            }
            txtItemName.text = model.nameForView;
            txtItemDes.text = model.desc;
        }
    }
    public bool IsGrey
    {
        set
        {
            UISprite[] sprites = this.transform.GetComponentsInChildren<UISprite>();
            foreach (UISprite sprite in sprites)
            {
                sprite.color = Color.black;
            }
        }
    }
}
