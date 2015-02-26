using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class UIShopWnd : UIBaseWnd
{
    //返回按钮
    public GameObject btnReturn;
    //商店一级容器
    public GameObject shopFrameCon;
    //商店二级容器
    public GameObject shopSubFrameCon;
    //星币
    public UILabel txtXingBi;
    //钛晶
    public UILabel txtTaiJing;
    //星钻
    public UILabel txtXingZuan;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        this.gameObject.GetComponent<BoxCollider>().size = new Vector3(PanelUtil.GetPanelWidth(), Constants.UI_HEIGHT);
    }
    /// <summary>
    /// 添加大资源类型
    /// </summary>
    public void AddShopTypeFrame()
    {
        btnReturn.SetActive(false);
        shopFrameCon.SetActive(true);
        shopSubFrameCon.SetActive(false);
        shopFrameCon.GetComponent<UIShopFrameCon>().ClickShopType += OnClickShopTypeFrame;
        shopFrameCon.GetComponent<UIShopFrameCon>().SetShopTypeFrameData();
    }

    private void OnClickShopTypeFrame(int shopOrder)
    {
        shopFrameCon.GetComponent<UIShopFrameCon>().ClickShopType -= OnClickShopTypeFrame;
        AddShopItem(shopOrder);
    }
    /// <summary>
    /// 添加对应的资源信息
    /// </summary>
    private void AddShopItem(int shopOrder)
    {
        btnReturn.SetActive(true);
        shopFrameCon.SetActive(false);
        shopSubFrameCon.SetActive(true);
        shopSubFrameCon.GetComponent<UIShopSubFrameCon>().SetShopSubTypeFrameData(shopOrder);
    }

    protected override bool InitWin()
    {
        if (!base.InitWin())
        {
            return true;
        }
        return false;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        this.GetComponent<BoxCollider>().size = new Vector3(Screen.width * Constants.UI_WIDTH / Screen.height, Constants.UI_HEIGHT);
        UIEventListener.Get(btnReturn).onClick += OnClickButton;
        EventDispather.AddEventListener(GameEvents.RESOURCE_CHANGE, OnUpdateMyResource);
        OnUpdateMyResource(null, null);
    }

    private void OnUpdateMyResource(string eventType, object obj)
    {
        txtXingBi.text = DataCenter.Instance.GetResource(ResourceType.Gold).ToString();
        txtTaiJing.text = DataCenter.Instance.GetResource(ResourceType.Oil).ToString();
        txtXingZuan.text = DataCenter.Instance.GetResource(ResourceType.Diamond).ToString();
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnReturn))
        {
            AddShopTypeFrame();
        }
    }
    /// <summary>
    /// 通过类型显示商店物品（增加工人、星钻和护盾入口）
    /// </summary>
    public void showShopItemByType(int shopType)
    {
        btnReturn.SetActive(false);
        AddShopItem(shopType);
    }
    protected override void OnDisable()
    {
        UIEventListener.Get(btnReturn).onClick -= OnClickButton;
        EventDispather.RemoveEventListener(GameEvents.RESOURCE_CHANGE, OnUpdateMyResource);
        shopFrameCon.GetComponent<UIShopFrameCon>().ClearShopFrameCon();
    }
}
