using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class ItemOperationManager : Singleton<ItemOperationManager>
{
    public const string PRODUCT_ONE_ITEM = "PRODUCT_ONE_ITEM";
    public const string REMOVE_ONE_ITEM = "REMOVE_ONE_ITEM";
    public const string PRODUCT_OVER_RIGHTNOW = "PRODUCT_OVER_RIGHTNOW";
    public const string UPDATE_SKILL = "UPDATE_SKILL";
    public const string UPDATE_SOLDIER = "UPDATE_SOLDIER";
    public const string DISPLAY_ITEM_DES = "DISPLAY_ITEM_DES";
    public ItemOperationManager()
    {
        GameModule.AddModule(new ModuleOperateItem());
    }
    public void OpenProductFactory(TileEntity entity)
    {
        AddEventListeners();
        CreateAllSoldierData(null, null);
        CreateAllSkillData(null, null);
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        module.curOperEntity = entity;
        module.currentProductFactory = entity.GetComponent<BaseProductBuildingComponent>();
        GameObject product = UIMananger.Instance.ShowWin("PLG_Product", "UIProductItemPanel");
        product.transform.localPosition = Vector3.zero;
        module.currentProductFactoryLevel = entity.model.level;
        product.GetComponent<UIProductItemWnd>().ProductFactoryId = entity.model.baseId;
    }
    private void AddEventListeners()
    {
        EventDispather.AddEventListener(GameEvents.SKILL_UP, CreateAllSkillData);
        EventDispather.AddEventListener(GameEvents.SOLDIER_UP, CreateAllSoldierData);
    }
    private void RemoveEventListeners()
    {
        EventDispather.RemoveEventListener(GameEvents.SKILL_UP, CreateAllSkillData);
        EventDispather.RemoveEventListener(GameEvents.SOLDIER_UP, CreateAllSoldierData);
    }
    /// <summary>
    /// 创建士兵数据信息
    /// </summary>
    public void CreateAllSoldierData(string eventType, object data)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        module.dicAllSolider.Clear();
        PlayerVO playerVO = DataCenter.Instance.Defender.player;
        if (null == playerVO)
        {
            Debug.Log("还没有玩家数据");
            return;
        }
        foreach (var armyExpVo in playerVO.armyShop)
        {
            module.AddOneSoldier(new ProductionItemVO() { cid = armyExpVo.cid, count = 0 });
        }
        if (null != data)
        {
            EventDispather.DispatherEvent(UPDATE_SOLDIER);
        }
    }
    public void CreateAllSkillData(string eventType, object data)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        module.dicAllSkill.Clear();
        PlayerVO playerVO = DataCenter.Instance.Defender.player;
        if (null == playerVO)
        {
            Debug.Log("还没有玩家数据");
            return;
        }
        foreach (int baseId in playerVO.skillShop)
        {
            module.AddOneSkill(new ProductionItemVO() { cid = baseId, count = 0 });
        }
        if (null != data)
        {
            EventDispather.DispatherEvent(UPDATE_SKILL);
        }
    }
    /// <summary>
    /// 根据id得到生产VO
    /// </summary>
    /// <param name="cid"></param>
    /// <returns></returns>
    public ProductionItemVO GetProductionItemVOById(int cid)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        for (int i = 0, imax = module.currentProductFactory.ProductionItems.Count; i < imax; i++)
        {
            if (module.currentProductFactory.ProductionItems[i].cid == cid)
            {
                return module.currentProductFactory.ProductionItems[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 得到已有技能个数
    /// </summary>
    /// <returns></returns>
    public int GetSkillCount()
    {
        int count = 0;
        foreach (var skillVo in DataCenter.Instance.Defender.skills)
        {
            count += skillVo.amount;
        }
        return count;
    }
    /// <summary>
    /// 打开升级兵种面板
    /// </summary>
    /// <param name="entity"></param>
    public void OpenInstituteWin(TileEntity entity, bool isSkill)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        module.curOperEntity = entity;
        module.researchBuildingComponent = entity.GetComponent<ResearchBuildingComponent>();
        GameObject product = UIMananger.Instance.ShowWin("PLG_Research", "UIInstitutePanel");
        product.transform.localPosition = Vector3.zero;
        product.GetComponent<UIInstituteWnd>().ShowFrame(isSkill);
    }
    /// <summary>
    /// 获得技能工厂最大等级的Model
    /// </summary>
    /// <returns></returns>
    public EntityModel GetMaxLevelProductSkillFactoryModel()
    {
        List<ProductSkillBuildingComponent> skillBuildings = IsoMap.Instance.GetComponents<ProductSkillBuildingComponent>(OwnerType.Defender);
        EntityModel tempModel = null;
        if (skillBuildings.Count > 0)
        {
            tempModel = skillBuildings[0].Entity.model;
            foreach (ProductSkillBuildingComponent skillBuilding in skillBuildings)
            {
                if (skillBuilding.Entity.model.level > tempModel.level)
                    tempModel = skillBuilding.Entity.model;
            }
        }
        return tempModel;
    }
    /// <summary>
    /// 获得兵种工厂最大等级的Model
    /// </summary>
    /// <returns></returns>
    public EntityModel GetMaxLevelProductSoldierFactoryModel()
    {
        List<ProductSoldierBuildingComponent> soldierBuildings = IsoMap.Instance.GetComponents<ProductSoldierBuildingComponent>(OwnerType.Defender);
        EntityModel tempModel = null;
        if (soldierBuildings.Count > 0)
        {
            tempModel = soldierBuildings[0].Entity.model;
            foreach (ProductSoldierBuildingComponent skillBuilding in soldierBuildings)
            {
                if (skillBuilding.Entity.model.level > tempModel.level)
                    tempModel = skillBuilding.Entity.model;
            }
        }
        return tempModel;
    }
    public bool IsItemCanLevelUp(int itemId)
    {
        EntityModel model = DataCenter.Instance.FindEntityModelById(itemId);
        List<ResearchBuildingComponent> researchBuildings = IsoMap.Instance.GetComponents<ResearchBuildingComponent>(OwnerType.Defender);
        if (researchBuildings.Count > 0)
        {
            return researchBuildings[0].Entity.model.level >= model.upgradeNeedLevel;
        }
        return true;
    }
    public int GetXiaoHaoSoldierCount(int id)
    {
        List<ArmyExpVO> list = DataCenter.Instance.Defender.player.armyShop;
        foreach (ArmyExpVO vo in list)
        {
            if (vo.cid == id)
            {
                return vo.exp;
            }
        }
        return 0;
    }

    public void ClearModuleOperateItem()
    {
        RemoveEventListeners();
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        module.ClearModule();
    }
}
