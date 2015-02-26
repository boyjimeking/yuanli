using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class UIBuildLevelUpWnd : UIBaseWnd
{
    //解锁内容框
    public GameObject unlockContent;
    //升级加成容器
    public Transform propertyCon;
    //解锁内容容器
    public Transform unlockArea;
    //建筑Icon
    public Transform buildIconArea;
    private GameObject buildModel;
    //确定升级按钮
    public GameObject btnLevelUp;
    //数据信息
    private TileEntity tileEntity;
    //升级时间
    public UILabel txtLevelTime;
    //升级所需要的钱
    public UILabel txtConsume;
    //升级所需要的钱的Icon
    public UISprite moneyIcon;
    //面板名字
    public UILabel txtPanelName;
    public GameObject unlockCon;
    //时间容器
    public GameObject levelUpTimeCon;
    //建筑描述
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.isLockScreen = true;
        this.closeOrHideType = UICloseOrHideType.CLOSE_DIAMOND_TIP;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnLevelUp).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (!BuildOptManager.Instance.IsBuildCanLevelUp(tileEntity.model.baseId))
        {
            string name = DataCenter.Instance.NeedToLocalName(tileEntity.model.upgradeNeedType, tileEntity.model.upgradeNeedLevel);
            GameTipsManager.Instance.ShowGameTips(EnumTipsID.BuildOpt_10401, new string[] { name, tileEntity.model.upgradeNeedLevel.ToString() });
            return;
        }
        EntityModel nextModel = ModelUtil.GetNextLevelModel(tileEntity.model.baseId);
        int hasCount = DataCenter.Instance.GetResource(tileEntity.model.costResourceType);
        if (hasCount >= nextModel.costResourceCount)
        {
            ConfirmLevelUp(true, null);
        }
        else
        {
            if (tileEntity.model.costResourceType == ResourceType.Diamond)
            {
                ShopManager.Instance.ShowDiamondNotEnough();
            }
            else
            {
                ShopManager.Instance.BuyResource(tileEntity.model.costResourceType, nextModel.costResourceCount - hasCount, ConfirmLevelUp);
            }
        }
    }
    private void ConfirmLevelUp(bool isSuccess, object obj)
    {
        if (!isSuccess) return;
        GameManager.Instance.RequestUpgradeBuilding(tileEntity, tileEntity.model.costResourceType, false);//TODO 玩家使用什么类型资源进行升级
        base.CloseWin();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnLevelUp).onClick -= OnClickButton;
        for (int i = 0; i < propertyCon.transform.childCount; i++)
        {
            GameObject.Destroy(propertyCon.transform.GetChild(i).gameObject);
        }
        GameObject.Destroy(buildModel);
        for (int i = 0, imax = unlockArea.childCount; i < imax; i++)
        {
            GameObject.Destroy(unlockArea.transform.GetChild(i).gameObject);
        }
    }
    protected override void CloseWin(GameObject obj = null)
    {
        base.CloseWin(obj);
        BuildOptManager.Instance.ShowBuildingOptWin(tileEntity);
        this.tileEntity = null;
    }
    public TileEntity CurTileEntity
    {
        set
        {
            this.tileEntity = value;
            UpdatePanelByData();
        }
    }
    private void UpdatePanelByData()
    {
        buildModel = (GameObject)TileEntity.LoadAndCreate(tileEntity.model);
        if (buildModel != null)
        {
            var view = buildModel.AddMissingComponent<EntityViewComponent>();
            view.Init(false);
            //创建模型
            buildModel.transform.parent = buildIconArea;
            buildModel.SetLayerRecursively(LayerMask.NameToLayer("UILayerMiddle"));
            buildModel.transform.localScale = new Vector3(20, 20, 1);
            buildModel.transform.localPosition = new Vector3(0, -20, 0);
        }
        //添加最高级判断
        if (0 == tileEntity.model.upgradeId)
        {
            Debug.Log("已经升到最高级了");
            return;
        }
        EntityModel nextModel = ModelUtil.GetNextLevelModel(tileEntity.model.baseId);
        txtPanelName.text = "升级到" + nextModel.level + "级";
        if (nextModel.buildTime <= 0)
        {
            txtLevelTime.text = "升级时间:无";
        }
        else
        {
            txtLevelTime.text = "升级时间:" + DateTimeUtil.PrettyFormatTimeSeconds(nextModel.buildTime, 2);
        }
        UpdateBuildInfo();
        txtConsume.text = nextModel.costResourceCount.ToString();
        moneyIcon.spriteName = nextModel.costResourceType.ToString();
    }
    private void UpdateBuildInfo()
    {
        if (null == tileEntity)
            return;
        EntityModel nextModel = ModelUtil.GetNextLevelModel(tileEntity.model.baseId);
        if (null == nextModel)
            return;
        //属性信息
        List<string> list = BuildPropertyUtil.GetPropertyList(tileEntity);
        for (int i = 0, imax = list.Count; i < imax; i++)
        {
            GameObject obj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/Property");
            obj.transform.parent = propertyCon.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(0, 80 - i * (obj.GetComponent<UISprite>().height + 2));
            obj.SetActive(true);
            EntityModel maxEntityModel = BuildOptManager.Instance.GetMaxLevelEntityModel(tileEntity.model.baseId);
            string desStr = BuildOptManager.Instance.GetBuildingPropertyDes(list[i]);
            obj.AddMissingComponent<UILogicLevelUpProperty>().SetPropertyIcon(GetPropertyIcon(list[i]));
            obj.AddMissingComponent<UILogicLevelUpProperty>().SetPropertyInfo(GetPropertyValue(tileEntity.model, list[i]), GetPropertyValue(nextModel, list[i]), GetPropertyValue(maxEntityModel, list[i]), desStr);
        }
        unlockCon.gameObject.SetActive(false);
        if (EntityTypeUtil.IsCenterBuilding(tileEntity.model) || EntityTypeUtil.IsArmyShop(tileEntity.model))
        {
            Dictionary<int, int> unlock = BuildOptManager.Instance.GetUnLockContent(tileEntity.model.baseId, tileEntity.model.upgradeId);
            if ((null != unlock) && unlock.Count > 0)
            {
                unlockCon.gameObject.SetActive(true);
                int index = 0;
                foreach (KeyValuePair<int, int> keyValuePair in unlock)
                {
                    GameObject unlockObj = (GameObject)GameObject.Instantiate(unlockContent, Vector3.zero, Quaternion.identity);
                    unlockObj.transform.parent = unlockArea;
                    unlockObj.transform.localScale = new Vector3(1, 1, 1);
                    unlockObj.transform.localPosition = new Vector3(-343 + index * (unlockObj.GetComponent<UISprite>().width + 5), 0);
                    unlockObj.SetActive(true);
                    unlockObj.AddMissingComponent<UIUnlockContent>().SetUnlockInfo(keyValuePair.Key, keyValuePair.Value);
                    index++;
                }
            }
        }
    }
    private int GetPropertyValue(EntityModel model, string property)
    {
        return BuildOptManager.Instance.GetPropertyValue(model, property);
    }
    private string GetPropertyIcon(string property)
    {
        return BuildOptManager.Instance.GetBuildingPropertyIcon(property);
    }
}
