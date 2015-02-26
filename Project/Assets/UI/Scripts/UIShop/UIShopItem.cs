using UnityEngine;
using System.Collections;

public class UIShopItem : MonoBehaviour
{
    //商店物品
    public GameObject shopItem;
    //商店物品动画
    public TweenScale shopItemScale;
    //商店物品说明
    public GameObject shopItemDes;
    //商店物品说明动画
    public TweenScale shopItemDesScale;
    //金钱的Item
    public GameObject shopMoney;
    //物品信息
    private ShopModel itemData;

    void OnEnable()
    {
        UIEventListener.Get(shopItemDes).onClick += OnClickButton;
        UIEventListener.Get(shopItem).onClick += OnClickButton;
        UIEventListener.Get(shopMoney).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(shopMoney))
        {
            ShopManager.Instance.ExcuteByItem(go.GetComponent<UIShopMoney>().ItemData);
        }
        else if (go.Equals(shopItemDes))
        {
            DisPlayShopItem(true);
        }
        if (go.Equals(shopItem))
        {
            if (go.GetComponent<UIShopItemDetail>().OpenLevel > 0)
            {
                var model = DataCenter.Instance.FindEntityModelById(itemData.baseId);
                var name =
                    DataCenter.Instance.NeedToLocalName(model.buildNeedType,model.buildNeedLevel);
                GameTipsManager.Instance.ShowGameTips(EnumTipsID.ShopTip_10102, new string[] { name, model.buildNeedLevel.ToString() });
            }
            else if (go.GetComponent<UIShopItemDetail>().IsMax)
            {
                //当前最大数量
                ShopManager.Instance.CanHaveMoreBuilding(itemData.baseId);
            }
            else if (go.GetComponent<UIShopItemDetail>().ItemData.shopType == ShopType.ShopE)
            {

            }
            else
            {
                ShopManager.Instance.ExcuteByItem(itemData);
            }
        }
    }
    /**
     * 显示物品的描述
     * 物品先缩之后物品描述出来
     * */
    public void DisplayShopItemDes(bool isAddEvent)
    {
        if (isAddEvent)
        {
            //shopItem先隐藏并派发出事件
            EventDelegate eventDelegate = new EventDelegate(this, "DisPlayShopItem");
            eventDelegate.oneShot = true;
            eventDelegate.parameters[0] = new EventDelegate.Parameter(!isAddEvent);
            shopItemScale.onFinished.Add(eventDelegate);
            shopItemScale.PlayForward();
        }
        else
        {
            shopItemScale.PlayReverse();
        }
    }
    /**
     * 显示物品
     * 物品描述先缩之后物品出来
     * */
    public void DisPlayShopItem(bool isAddEvent)
    {
        if (isAddEvent)
        {
            //shopItemDes隐藏并派发事件
            EventDelegate eventDelegate = new EventDelegate(this, "DisplayShopItemDes");
            eventDelegate.oneShot = true;
            eventDelegate.parameters[0] = new EventDelegate.Parameter(!isAddEvent);
            shopItemDesScale.onFinished.Add(eventDelegate);
            shopItemDesScale.PlayReverse();
        }
        else
        {
            shopItemDesScale.PlayForward();
        }
    }
    void OnDisable()
    {
        UIEventListener.Get(shopItemDes).onClick -= OnClickButton;
        UIEventListener.Get(shopItem).onClick -= OnClickButton;
        if (1 != shopItem.transform.localScale.x)
        {
            shopItem.transform.localScale = new Vector3(1, 1, 1);
            shopItemScale.tweenFactor = 0;
        }
        if (0 != shopItemDes.transform.localScale.x)
        {
            shopItemDes.transform.localScale = new Vector3(0, 1, 1);
            shopItemDesScale.tweenFactor = 0;
        }
    }
    public ShopModel ItemData
    {
        set
        {
            this.itemData = value;
            //金钱的
            if (itemData.shopType == ShopType.ShopA)
            {
                shopItem.SetActive(false);
                shopItemDes.SetActive(false);
                shopMoney.SetActive(true);
                shopMoney.GetComponent<UIShopMoney>().ItemData = this.itemData;
            }
            else
            {
                shopItem.SetActive(true);
                shopItemDes.SetActive(true);
                shopMoney.SetActive(false);
                shopItem.GetComponent<UIShopItemDetail>().ItemData = this.itemData;
                shopItem.GetComponent<UIShopItemDetail>().OnClickButtonDes += DisplayShopItemDes;
                shopItemDes.GetComponent<UIShopItemDes>().ItemData = this.itemData;
            }
        }
    }
}
