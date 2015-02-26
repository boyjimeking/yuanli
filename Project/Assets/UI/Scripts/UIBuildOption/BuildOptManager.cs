using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class BuildOptManager : Singleton<BuildOptManager>
{
    //属性字符串
    private Dictionary<string, string> dicBuildingPropertyDes = new Dictionary<string, string>();
    //属性Icon
    private Dictionary<string, string> dicBuildingPropertyIcon = new Dictionary<string, string>();
    public BuildOptManager()
    {
        InitBuildOptManager();
    }
    /// <summary>
    /// 初始化对应建筑要显示的按钮
    /// 建筑显示的数组信息
    /// </summary>
    private void InitBuildOptManager()
    {
        //属性描述字符串
        dicBuildingPropertyDes.Add(BuildPropertyUtil.HP, "生命值:");
        dicBuildingPropertyDes.Add(BuildPropertyUtil.SPACEPROVIDE, "总兵力:");
        dicBuildingPropertyDes.Add(BuildPropertyUtil.QUEUESIZE, "训练容纳量:");
        dicBuildingPropertyDes.Add(BuildPropertyUtil.MAXRESOURCESTORAGE, "最大容量:");
        dicBuildingPropertyDes.Add(BuildPropertyUtil.RESOURCEPERSECONDFORVIEW, "生产速度:每小时");
        //属性Icon
        dicBuildingPropertyIcon.Add(BuildPropertyUtil.HP, "UI_Icon_HP");
        dicBuildingPropertyIcon.Add(BuildPropertyUtil.SPACEPROVIDE, "UI_Icon_TroopNum");
        dicBuildingPropertyIcon.Add(BuildPropertyUtil.QUEUESIZE, "UI_Icon_TroopNum");
        dicBuildingPropertyIcon.Add(BuildPropertyUtil.MAXRESOURCESTORAGE, "UI_Icon_SWVol");
        dicBuildingPropertyIcon.Add(BuildPropertyUtil.RESOURCEPERSECONDFORVIEW, "UI_Icon_SWGain");
    }
    /// <summary>
    /// 通过属性名获得建筑显示的属性字符串
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public string GetBuildingPropertyDes(string property)
    {
        if (this.dicBuildingPropertyDes.ContainsKey(property))
            return this.dicBuildingPropertyDes[property];
        return "没字段，快添加";
    }
    public string GetBuildingPropertyIcon(string property)
    {
        if (this.dicBuildingPropertyIcon.ContainsKey(property))
            return this.dicBuildingPropertyIcon[property];
        return "UI_Icon_HP";
    }
    /// <summary>
    /// 获得某个建筑最大等级
    /// </summary>
    /// <param name="baseID"></param>
    /// <returns></returns>
    public EntityModel GetMaxLevelEntityModel(int baseID)
    {
        EntityModel model = DataCenter.Instance.FindEntityModelById(baseID);
        while (model.upgradeId != 0)
        {
            model = DataCenter.Instance.FindEntityModelById(model.upgradeId);
            if (null == model)
            {
                break;
            }
        }
        return model;
    }
    /// <summary>
    /// 获得某个属性的值
    /// </summary>
    /// <param name="model"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    public int GetPropertyValue(EntityModel model, string property)
    {
        if (null == model)
        {
            Debug.Log("BuildOptManager这个model为空");
            return 1;
        }
        if (property == BuildPropertyUtil.HP)
            return model.hp;
        else if (property == BuildPropertyUtil.SPACEPROVIDE)
            return model.spaceProvide;
        else if (property == BuildPropertyUtil.RESOURCEPERSECONDFORVIEW)
            return model.resourcePerSecondForView;
        else if (property == BuildPropertyUtil.MAXRESOURCESTORAGE)
            return model.maxResourceStorage;
        else if (property == BuildPropertyUtil.QUEUESIZE)
            return model.queueSize;
        return 0;
    }
    public void ShowBuildingOptWin(TileEntity entity)
    {
        GameObject buildOptWin = UIMananger.Instance.ShowWin("PLG_BuildOperation", "UIBuildOperationPanel");
        UISprite sprite = buildOptWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0.5f, 0.5f, 0, 0), new Vector4(-sprite.width * 0.5f, sprite.width * 0.5f, 5, sprite.height + 5));
        buildOptWin.GetComponent<UIBuildOperationWnd>().CurTileEntity = entity;
    }
    public void CloseBuildOptWin()
    {
        GameObject buildOptWin = UIMananger.Instance.GetWinByName("UIBuildOperationPanel");
        if (buildOptWin)
            buildOptWin.GetComponent<UIBuildOperationWnd>().CurTileEntity = null;
    }
    /// <summary>
    /// 点击按钮执行对应的操作
    /// </summary>
    /// <param name="buttonName"></param>
    /// <param name="entity"></param>
    public void ExcuteOperationByName(string buttonName, TileEntity entity)
    {
        switch (buttonName)
        {
            case OperationButtonUtil.BUILDINFO:
                GameObject buildInfo = UIMananger.Instance.ShowWin("PLG_BuildInfo", "UIBuildInfoPanel");
                buildInfo.transform.localPosition = Vector3.zero;
                buildInfo.GetComponent<UIBuildInfoWnd>().CurTileEntity = entity;
                break;
            case OperationButtonUtil.BUILDLEVELUP:
                if (entity.buildingVO.buildingStatus == com.pureland.proto.BuildingVO.BuildingStatus.Upgrade)
                {
                    GameManager.Instance.RequestUpgradeBuilding(entity, entity.model.costResourceType, true);
                    return;
                }
                GameObject buildLevelUP = UIMananger.Instance.ShowWin("PLG_BuildLevelUp", "UIBuildLevelupPanel");
                buildLevelUP.transform.localPosition = Vector3.zero;
                buildLevelUP.GetComponent<UIBuildLevelUpWnd>().CurTileEntity = entity;
                break;
            case OperationButtonUtil.TRAINARMY:
            case OperationButtonUtil.PRODUCTSKILL:
                ItemOperationManager.Instance.OpenProductFactory(entity);
                break;
            case OperationButtonUtil.COMPLETERIGHTNOW:
                entity.GetComponent<ConstructBuildingComponent>().CompleteImmediately();
                break;
            case OperationButtonUtil.RESEARCH_A:
                ItemOperationManager.Instance.OpenInstituteWin(entity, false);
                break;
            case OperationButtonUtil.RESEARCH_B:
                ItemOperationManager.Instance.OpenInstituteWin(entity, true);
                break;
            case OperationButtonUtil.RESETXIANJING:
                ResourceVO resourceTrap = entity.GetComponent<TrapComponent>().RefillCost;
                if (DataCenter.Instance.GetResource(resourceTrap.resourceType) < resourceTrap.resourceCount)
                {
                    ShopManager.Instance.BuyResource(resourceTrap.resourceType, resourceTrap.resourceCount - DataCenter.Instance.GetResource(resourceTrap.resourceType), BuyResComplete, entity.GetComponent<TrapComponent>());
                }
                else
                {
                    entity.GetComponent<TrapComponent>().Refill(false);
                }
                break;
            case OperationButtonUtil.RESETALLXIANJING:
                ResourceVO resourceTrapAll = GetRefillCostAll();
                GameTipsManager.Instance.ShowGameTips(EnumTipsID.BuildOpt_10402, new string[] { resourceTrapAll.resourceCount.ToString(), ResourceUtil.GetResNameByResType(resourceTrapAll.resourceType) }, null, ConfirmResetAllTrap);
                break;
            case OperationButtonUtil.COLLECTRESOURCE:
                entity.GetComponent<GatherResourceBuildingComponent>().GatherResource();
                break;
        }
    }
    /// <summary>
    /// 确定重置所有陷阱
    /// </summary>
    /// <param name="isConfirm"></param>
    /// <param name="arrayList"></param>
    private void ConfirmResetAllTrap(bool isConfirm, ArrayList arrayList)
    {
        if (isConfirm)
        {
            ResourceVO resource = GetRefillCostAll();
            if (DataCenter.Instance.GetResource(resource.resourceType) > resource.resourceCount)
            {
                GameManager.Instance.RequestRefillAllTrap();
            }
            else
            {
                ShopManager.Instance.BuyResource(resource.resourceType, resource.resourceCount - DataCenter.Instance.GetResource(resource.resourceType), BuyResComplete);
            }
        }
    }

    private void BuyResComplete(bool isSuccess, object obj)
    {
        if (!isSuccess) return;
        if (null == obj)
        {
            GameManager.Instance.RequestRefillAllTrap();
        }
        else
        {
            ((TrapComponent)obj).Refill(false);
        }
    }
    /// <summary>
    /// 得到解锁的建筑
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, int> GetUnLockContent(int baseId, int nextBaseID)
    {
        if (0 == nextBaseID) return null;
        Dictionary<int, int> curUnLock = new Dictionary<int, int>();
        Dictionary<int, int> nextUnLock = new Dictionary<int, int>();
        Dictionary<int, int> returnUnLock = new Dictionary<int, int>();
        List<BuildingLimitModel> buildingLimitData = DataCenter.Instance.buildingLimitModels;
        BuildingLimitModel limitData;

        for (int i = 0, imax = buildingLimitData.Count; i < imax; i++)
        {
            limitData = buildingLimitData[i];
            if (limitData.baseId == baseId)
            {
                if (!curUnLock.ContainsKey(limitData.buildingBaseId))
                    curUnLock.Add(limitData.buildingBaseId, limitData.buildingCount);
                else
                    Debug.Log("当前解锁中建筑ID（limitData.buildingBaseId）" + limitData.buildingBaseId + "重复");
            }
            if (limitData.baseId == nextBaseID)
            {
                if (!nextUnLock.ContainsKey(limitData.buildingBaseId))
                    nextUnLock.Add(limitData.buildingBaseId, limitData.buildingCount);
                else
                    Debug.Log("下级解锁中建筑ID（limitData.buildingBaseId）" + limitData.buildingBaseId + "重复");
            }
        }
        foreach (KeyValuePair<int, int> keyValuePair in nextUnLock)
        {
            if (curUnLock.ContainsKey(keyValuePair.Key))
            {
                if ((keyValuePair.Value - curUnLock[keyValuePair.Key]) > 0)
                    returnUnLock.Add(keyValuePair.Key, keyValuePair.Value - curUnLock[keyValuePair.Key]);
            }
            else
            {
                returnUnLock.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        if (returnUnLock.Count <= 0)
            return null;
        return returnUnLock;
    }
    /// <summary>
    /// 建筑是否可以升级（依赖大本营）
    /// </summary>
    /// <param name="buildId"></param>
    /// <returns></returns>
    public bool IsBuildCanLevelUp(int buildId)
    {
        EntityModel model = DataCenter.Instance.FindEntityModelById(buildId);
        return model.upgradeNeedLevel <= DataCenter.Instance.GetCenterBuildingModel().level;
    }
    /// <summary>
    /// 是否含有某个大类型的建筑
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public bool hasBrokenTrap()
    {
        List<TileEntity> entityList = IsoMap.Instance.GetAllEntitiesByOwner(OwnerType.Defender);
        foreach (TileEntity tileEntity in entityList)
        {
            if (tileEntity.model.entityType == EntityType.Trap && tileEntity.buildingVO.trapBuildingVO.broken)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 有没有其他坏的炸弹
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public bool hasOtherBrokenTrap(long sid)
    {
        List<TileEntity> entityList = IsoMap.Instance.GetAllEntitiesByOwner(OwnerType.Defender);
        foreach (TileEntity tileEntity in entityList)
        {
            if (tileEntity.buildingVO.sid != sid && tileEntity.model.entityType == EntityType.Trap && tileEntity.buildingVO.trapBuildingVO.broken)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 得到重置全部陷阱的钱
    /// </summary>
    /// <returns></returns>
    public ResourceVO GetRefillCostAll()
    {
        ResourceVO resource = new ResourceVO();
        foreach (var buildingVo in DataCenter.Instance.Defender.buildings)
        {
            if (buildingVo.trapBuildingVO != null && buildingVo.trapBuildingVO.broken)
            {
                var model = DataCenter.Instance.FindEntityModelById(buildingVo.cid);
                resource.resourceType = ResourceType.Gold;
                resource.resourceCount += model.refillCostResourceCount;
            }
        }
        return resource;
    }
}
