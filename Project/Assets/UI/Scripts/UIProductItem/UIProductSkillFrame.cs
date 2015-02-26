using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System;

public class UIProductSkillFrame : UIProductItemBaseFrame
{
    //已经有的技能容器
    public Transform hasSkillCon;
    //提示文字
    public UILabel txtHasItemTip;
    protected override void OnEnable()
    {
        base.OnEnable();
        EventDispather.AddEventListener(ItemOperationManager.UPDATE_SKILL, UpdateSkillFrame);
    }
    /// <summary>
    /// 建造军队立即完成
    /// </summary>
    protected override void RightNowOverProduct(string eventType, object obj)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        //钱
        int count = GameDataAlgorithm.TimeToGem(module.currentProductFactory.TotalTimeLeft);
        if (count > DataCenter.Instance.GetResource(ResourceType.Diamond))
        {
            ShopManager.Instance.ShowDiamondNotEnough();
            return;
        }
        module.currentProductFactory.CompleteProductionImmediately();
        productingItemCon.GetComponent<UIProductingItemCon>().ClearProductingItem();
        itemStoreCon.GetComponent<UIProductSkillStoreCon>().ResetAllItemInfo();
        hasSkillCon.GetComponent<UIProductHasSkillCon>().UpdateHasSkillFrame();
        SetPanelName();
        GameTipsManager.Instance.ShowGameTips(EnumTipsID.ItemOper_10207);
        txtHasItemTip.text = "准备好的法术:" + ItemOperationManager.Instance.GetSkillCount() + "/" + module.currentProductFactory.MaxQueueSize;
    }
    private void UpdateSkillFrame(string eventType, object obj)
    {
        itemStoreCon.GetComponent<UIProductSkillStoreCon>().UpdateItemStore();
        productingItemCon.GetComponent<UIProductingItemCon>().UpdateProductingItemFrame();
        hasSkillCon.GetComponent<UIProductHasSkillCon>().UpdateHasSkillFrame();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventDispather.RemoveEventListener(ItemOperationManager.UPDATE_SKILL, UpdateSkillFrame);
    }
    public override void ShowProductItemFrame()
    {
        base.ShowProductItemFrame();
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        ////刷新预备的技能
        //itemStoreCon.GetComponent<UIProductSkillStoreCon>().UpdateItemStore();
        //刷新已经有的技能
        hasSkillCon.GetComponent<UIProductHasSkillCon>().UpdateHasSkillFrame();
        txtHasItemTip.text = "准备好的法术:" + ItemOperationManager.Instance.GetSkillCount() + "/" + module.currentProductFactory.MaxQueueSize;
    }
    protected override void AddOneProductItemComplete()
    {
        //添加完之后如果当前训练营满了，界面改变
        //itemStoreCon.GetComponent<UIProductSkillStoreCon>().CheckCurrentCapacity();
    }
    protected override void RemoveOneProductItemComplete(ProductionItemVO vo)
    {
        //itemStoreCon.GetComponent<UIProductSkillStoreCon>().ChangeOneItemInfo(vo);
    }
    protected override int GetItemSpace(EntityModel model)
    {
        return 1 + ItemOperationManager.Instance.GetSkillCount();
    }
    public override void Clear()
    {
        base.Clear();
        //清理预备兵种容器
        //itemStoreCon.GetComponent<UIProductSkillStoreCon>().ClearItemStore();
        hasSkillCon.GetComponent<UIProductHasSkillCon>().Clear();
    }

    protected override void SetPanelName()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        txtPanelName.text = "配置法术:" + (ItemOperationManager.Instance.GetSkillCount() + module.currentProductFactory.CurrentQueueSize) + "/" + module.currentProductFactory.MaxQueueSize;
    }

    protected override void ShowFullTips()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        GameTipsManager.Instance.ShowGameTips(EnumTipsID.ItemOper_10202, new string[] { module.curOperEntity.model.nameForView });
    }
}
