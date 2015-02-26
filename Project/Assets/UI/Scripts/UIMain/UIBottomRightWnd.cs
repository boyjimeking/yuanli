using UnityEngine;
using System.Collections;

public class UIBottomRightWnd : UIBaseWnd
{
    //商店按钮
    public GameObject btnShop;
    //星际按钮
    public GameObject btnBag;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_BATTLE;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_REPLAY;
    }
    // Use this for initialization
    protected override void Start()
    {
        InitWin();
    }

    protected override bool InitWin()
    {
        return base.InitWin();
    }
    protected override void OnEnable()
    {
        UIEventListener.Get(btnShop).onClick += OnClickButton;
        UIEventListener.Get(btnBag).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnShop))
        {
            ShopManager.Instance.ShowShopWin(true);
        }
        else if (go.Equals(btnBag))
        {
            HomeLandManager.Instance.ShowBagWin();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnShop).onClick -= OnClickButton;
        UIEventListener.Get(btnBag).onClick -= OnClickButton;
    }
}
