using UnityEngine;
using System.Collections;

public class UIShopTypeFrame : MonoBehaviour
{
    [HideInInspector]
    public int shopOrder = 0;
    public UISprite backSprite;
    public UIButton button;
    public UILabel txtShopName;
    public void SetShopInfo(int shopOrder, string shopIcon,string shopName)
    {
        this.shopOrder = shopOrder;
        this.txtShopName.text = shopName;
        backSprite.spriteName = shopIcon;
        button.normalSprite = shopIcon;
    }
}
