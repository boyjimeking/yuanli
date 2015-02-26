using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class ShopManager : Singleton<ShopManager>
{
    public delegate void CallBack(bool isSuccess, object obj);
    //商城表数据
    private List<ShopModel> shopDataList = new List<ShopModel>();
    public ShopManager()
    {
        GameModule.AddModule(new ModuleShop());
    }
    /// <summary>
    /// 解析商城表数据
    /// </summary>
    public void AnalysisShopData()
    {
        ShopModel shopModel;
        ModuleShop module = (ModuleShop)GameModule.GetModule(GameModule.MODULE_SHOP);
        RaceType myRaceType = (RaceType)DataCenter.Instance.Defender.player.raceType;
        //没有数据则解析
        if (shopDataList.Count <= 0)
        {
            shopDataList = DataCenter.Instance.shopModels;
            //筛选自己种族商店
            for (int i = 0, imax = shopDataList.Count; i < imax; i++)
            {
                shopModel = shopDataList[i];
                if (shopModel.raceType == myRaceType || shopModel.raceType == RaceType.None)
                {
                    module.shopData[shopModel.shopOrder - 1].Add(shopModel);
                }
            }
            module.SortShopData();
        }
    }
    /// <summary>
    /// 购买某物品
    /// </summary>
    /// <param name="model"></param>
    public void ExcuteByItem(ShopModel model)
    {
        EntityModel entityModel = DataCenter.Instance.FindEntityModelById(model.baseId);
        if (model.shopType == ShopType.ShopA)
        {
            //金钱的
            ArrayList listTitle = new ArrayList();
            ArrayList listContent = new ArrayList();
            if (EntityTypeUtil.IsDiamond(entityModel))
            {
                GameManager.Instance.RequestChargeDiamond(entityModel.hp);
            }
            else if (EntityTypeUtil.IsGold(entityModel))
            {
                BuyResource(ResourceType.Gold, DataCenter.Instance.GetMaxResourceStorage(ResourceType.Gold) * entityModel.hp / 100);
            }
            else if (EntityTypeUtil.IsOil(entityModel))
            {
                BuyResource(ResourceType.Oil, DataCenter.Instance.GetMaxResourceStorage(ResourceType.Oil) * entityModel.hp / 100);
            }
            else if (EntityTypeUtil.IsMedal(entityModel))
            {
                BuyResource(ResourceType.Medal, entityModel.hp);
            }
        }
        else
        {
            int hasMoney = DataCenter.Instance.GetResource(entityModel.costResourceType);
            if (hasMoney >= entityModel.costResourceCount)
            {
                BuyResourceComplete(true, entityModel.baseId);
            }
            else
            {
                ShopManager.Instance.BuyResource(entityModel.costResourceType, entityModel.costResourceCount - hasMoney, BuyResourceComplete, model.baseId);
            }
        }
    }

    private void BuyResourceComplete(bool isSuccess, object obj)
    {
        if (isSuccess)
        {
            GameManager.Instance.BuyBuilding((int)obj);
            UIMananger.Instance.CloseWin("UIShopPanel");
        }
    }
    /// <summary>
    /// 外部接口统一的资源兑换
    /// </summary>
    /// <param name="resType">ResourceType</param>
    /// <param name="count">数量</param>
    /// <param name="call">外部的回调函数</param>
    /// <param name="callParam">回调参数</param>
    public void BuyResource(ResourceType resType, int count, CallBack call = null, object callParam = null)
    {
        ArrayList list = new ArrayList();
        list.Add(ResourceUtil.GetResNameByResType(resType));
        list.Add(GameDataAlgorithm.ResourceToGem(count).ToString());
        ArrayList listCallBack = new ArrayList();
        listCallBack.Add(resType);
        listCallBack.Add(count);
        listCallBack.Add(call);
        listCallBack.Add(callParam);
        GameTipsManager.Instance.ShowGameTips(EnumTipsID.ShopTip_10101, new string[] { ResourceUtil.GetResNameByResType(resType), count.ToString() }, list, ConfirmBuyResource, listCallBack);
    }
    /// <summary>
    /// 确认资源兑换
    /// </summary>
    /// <param name="isConfirm"></param>
    /// <param name="arrayList"></param>
    private void ConfirmBuyResource(bool isConfirm, ArrayList arrayList)
    {
        if (isConfirm)
        {
            if (DataCenter.Instance.GetResource((ResourceType)arrayList[0]) + (int)arrayList[1] > DataCenter.Instance.GetMaxResourceStorage((ResourceType)arrayList[0]))
            {
                string name = GetResourceStorageName((ResourceType)arrayList[0]);
                GameTipsManager.Instance.ShowGameTips(EnumTipsID.ShopTip_10104, new string[] { name, name });
                return;
            }
            int needCount = GameDataAlgorithm.ResourceToGem((int)arrayList[1]);
            if (DataCenter.Instance.GetResource(ResourceType.Diamond) < needCount)
            {
                ShowDiamondNotEnough();
            }
            else
            {
                DataCenter.Instance.AddResource(new ResourceVO() { resourceType = (ResourceType)arrayList[0], resourceCount = (int)arrayList[1] });
                DataCenter.Instance.RemoveResource(ResourceType.Diamond, needCount);
                GameManager.Instance.RequestBuyResource(new ResourceVO() { resourceType = (ResourceType)arrayList[0], resourceCount = (int)arrayList[1] });
                if (null != arrayList[2])
                {
                    ((CallBack)arrayList[2])(true, arrayList[3]);
                }
            }
        }
        else
        {
            if (null != arrayList[2])
            {
                ((CallBack)arrayList[2])(false, arrayList[3]);
            }
        }
    }
    public void ShowDiamondNotEnough()
    {
        GameTipsManager.Instance.ShowGameTips(EnumTipsID.ShopTip_10105, null, null, OpenShopDiamondWin);
    }
    private void OpenShopDiamondWin(bool isConfirm, ArrayList arrayList)
    {
        if (isConfirm)
        {
            ShowShopWin(false, ShopType.ShopA);
        }
    }
    /// <summary>
    /// 获得资源存储罐的名字
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetResourceStorageName(ResourceType type)
    {
        List<EntityModel> entityModels = DataCenter.Instance.entityModels;
        foreach (var entityModel in entityModels)
        {
            if (entityModel.raceType == (RaceType)DataCenter.Instance.Defender.player.raceType && EntityTypeUtil.IsStorageResoruceBuilding(entityModel) && entityModel.resourceType == type)
            {
                return entityModel.nameForView;
            }
        }
        return "";
    }
    /// <summary>
    /// 打开商店面板
    /// </summary>
    /// <param name="isDefault">默认全部商店</param>
    /// <param name="type">商店类型</param>
    public void ShowShopWin(bool isDefault, ShopType type = ShopType.ShopA)
    {
        AnalysisShopData();
        GameObject shopWin = UIMananger.Instance.ShowWin("PLG_Shop", "UIShopPanel");
        shopWin.transform.localPosition = Vector3.zero;
        UIPanel panel = shopWin.GetComponent<UIPanel>();
        panel.clipping = UIDrawCall.Clipping.SoftClip;
        PanelUtil.SetPanelAnchors(panel, UIMananger.Instance.uiLayer.transform, new Vector4(0, 1, 0, 1), new Vector4(0, 0, 0, 0));
        if (isDefault)
        {
            shopWin.GetComponent<UIShopWnd>().AddShopTypeFrame();
        }
        else
        {
            shopWin.GetComponent<UIShopWnd>().showShopItemByType((int)type);
        }
    }
    /// <summary>
    /// 后续可否建造更多的建筑
    /// </summary>
    /// <param name="buildId"></param>
    /// <returns></returns>
    public bool CanHaveMoreBuilding(int buildId)
    {
        //解锁的建筑数据表
        List<BuildingLimitModel> buildingLimitModels = DataCenter.Instance.buildingLimitModels;
        //该建筑当前已有的数量
        int curMaxCount = DataCenter.Instance.FindBuildingLimitById(buildId);
        //当前的大本营
        EntityModel baseModel = DataCenter.Instance.GetCenterBuildingModel();
        while (baseModel.upgradeId != 0)
        {
            baseModel = DataCenter.Instance.FindEntityModelById(baseModel.upgradeId);
            if (GetNextLevelBuildingCount(buildId, baseModel.baseId, buildingLimitModels) - curMaxCount > 0)
            {
                GameTipsManager.Instance.ShowGameTips(EnumTipsID.ShopTip_10102, new string[] { baseModel.nameForView, baseModel.level.ToString() });
                return true;
            }
        }
        GameTipsManager.Instance.ShowGameTips(EnumTipsID.ShopTip_10103);
        return false;
    }
    /// <summary>
    /// BuildingLimitModel获得建筑个数
    /// </summary>
    /// <param name="buildingId"></param>
    /// <param name="nextBaseId"></param>
    /// <param name="buildingLimitModels"></param>
    /// <returns></returns>
    private int GetNextLevelBuildingCount(int buildingId, int nextBaseId, List<BuildingLimitModel> buildingLimitModels)
    {
        foreach (var buildingLimitModel in buildingLimitModels)
        {
            if (buildingLimitModel.baseId == nextBaseId && buildingLimitModel.buildingBaseId == buildingId)
            {
                return buildingLimitModel.buildingCount;
            }
        }
        return 0;
    }
}
