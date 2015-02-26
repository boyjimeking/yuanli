using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIPersonMoneyWnd : UIBaseWnd
{
    //星币数量
    public UILabel txtXingBi;
    //星币进度条
    public UISlider progressXingBi;
    //钛晶数量
    public UILabel txtTaiJing;
    //钛晶进度条
    public UISlider progressTaiJing;
    //金牌数量
    public UILabel txtJinPai;
    //星钻数量
    public UILabel txtXingZuan;
    //增加星钻按钮
    public GameObject btnAddXingZuan;
    public UILabel txtMaxXingBi;
    public UILabel txtMaxTaiJing;
    //当前要显示家园数据的类型（进攻方）
    private OwnerType ownerType;
    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_REPLAY;
    }
    protected override void OnEnable()
    {
        UIEventListener.Get(btnAddXingZuan).onClick += OnClickButton;
        EventDispather.AddEventListener(GameEvents.RESOURCE_CHANGE, UpdatePersonMoney);
        EventDispather.AddEventListener(GameEvents.STOLEN_RESOURCE, UpdatePersonMoney);
        EventDispather.AddEventListener(GameEvents.BUILDING_COMPLETE, UpdateMaxResource);
    }

    private void UpdateMaxResource(string eventType, object obj)
    {
        if (null != obj)
        {
            BuildingVO buildVO = obj as BuildingVO;
            EntityModel model = DataCenter.Instance.FindEntityModelById(buildVO.cid);
            if (!EntityTypeUtil.IsStorageResoruceBuilding(model))
            {
                return;
            }
        }
        int maxXingBi = DataCenter.Instance.GetMaxResourceStorage(ResourceType.Gold);
        int maxTaiJing = DataCenter.Instance.GetMaxResourceStorage(ResourceType.Oil);
        txtMaxXingBi.text = "最大储量:" + maxXingBi;
        txtMaxTaiJing.text = "最大储量:" + maxTaiJing;
        //progressXingBi.value = DataCenter.Instance.GetResource(ResourceType.Gold, ownerType) / maxXingBi;
        //progressTaiJing.value = DataCenter.Instance.GetResource(ResourceType.Oil, ownerType) / maxTaiJing;
    }

    private void UpdatePersonMoney(string eventType, object data)
    {
        if (null != data)
        {
            if ((data as ResourceVO).resourceType == ResourceType.Gold)
            {
                txtXingBi.text = DataCenter.Instance.GetResource(ResourceType.Gold, ownerType).ToString();
            }
            else if ((data as ResourceVO).resourceType == ResourceType.Oil)
            {
                txtTaiJing.text = DataCenter.Instance.GetResource(ResourceType.Oil, ownerType).ToString();
            }
            else if ((data as ResourceVO).resourceType == ResourceType.Medal)
            {
                txtJinPai.text = DataCenter.Instance.GetResource(ResourceType.Medal, ownerType).ToString();
            }
            else if ((data as ResourceVO).resourceType == ResourceType.Diamond)
            {
                txtXingZuan.text = DataCenter.Instance.GetResource(ResourceType.Diamond, ownerType).ToString();
            }
            return;
        }
        //刷新全部的金钱
        txtJinPai.text = DataCenter.Instance.GetResource(ResourceType.Medal, ownerType).ToString();
        txtTaiJing.text = DataCenter.Instance.GetResource(ResourceType.Oil, ownerType).ToString();
        txtXingBi.text = DataCenter.Instance.GetResource(ResourceType.Gold, ownerType).ToString();
        txtXingZuan.text = DataCenter.Instance.GetResource(ResourceType.Diamond, ownerType).ToString();
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnAddXingZuan))
        {
            ShopManager.Instance.ShowShopWin(false, ShopType.ShopA);
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnAddXingZuan).onClick -= OnClickButton;
        EventDispather.RemoveEventListener(GameEvents.RESOURCE_CHANGE, UpdatePersonMoney);
        EventDispather.RemoveEventListener(GameEvents.STOLEN_RESOURCE, UpdatePersonMoney);
        EventDispather.RemoveEventListener(GameEvents.BUILDING_COMPLETE, UpdateMaxResource);
    }
    public void SetPlayerMoney(OwnerType type)
    {
        this.ownerType = type;
        UpdatePersonMoney(null, null);
        UpdateMaxResource(null, null);
    }
}
