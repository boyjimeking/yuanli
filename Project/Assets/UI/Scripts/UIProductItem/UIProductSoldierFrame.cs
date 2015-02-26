using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIProductSoldierFrame : UIProductItemBaseFrame
{
    protected override void OnEnable()
    {
        base.OnEnable();
        EventDispather.AddEventListener(ItemOperationManager.UPDATE_SOLDIER, UpdateSoldierFrame);
        EventDispather.AddEventListener(ItemOperationManager.DISPLAY_ITEM_DES, ShowSoldierDesFrame);
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
        //判断兵营
        if (DataCenter.Instance.SpaceUsed + module.currentProductFactory.CurrentQueueSize > DataCenter.Instance.TotalSpace)
        {
            EntityModel model = DataCenter.Instance.FindEntityModel((RaceType)DataCenter.Instance.Defender.player.raceType, "Barracks", 1);
            GameTipsManager.Instance.ShowGameTips(EnumTipsID.ItemOper_10205, new string[] { model.nameForView });
            return;
        }
        module.currentProductFactory.CompleteProductionImmediately();
        productingItemCon.GetComponent<UIProductingItemCon>().ClearProductingItem();
        itemStoreCon.GetComponent<UIProductSoldierStoreCon>().ResetAllItemInfo();
        SetPanelName();
        GameTipsManager.Instance.ShowGameTips(EnumTipsID.ItemOper_10206);
    }
    private void UpdateSoldierFrame(string eventType, object obj)
    {
        itemStoreCon.GetComponent<UIProductSoldierStoreCon>().UpdateItemStore();
        productingItemCon.GetComponent<UIProductingItemCon>().UpdateProductingItemFrame();
    }
    private void ShowSoldierDesFrame(string eventType, object obj)
    {
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventDispather.RemoveEventListener(ItemOperationManager.UPDATE_SOLDIER, UpdateSoldierFrame);
        EventDispather.RemoveEventListener(ItemOperationManager.DISPLAY_ITEM_DES, ShowSoldierDesFrame);
    }
    public override void ShowProductItemFrame()
    {
        base.ShowProductItemFrame();
    }
    protected override void AddOneProductItemComplete()
    {
        //添加完之后如果当前训练营满了，界面改变
        //itemStoreCon.GetComponent<UIProductSoldierStoreCon>().CheckCurrentCapacity();
    }
    protected override void RemoveOneProductItemComplete(ProductionItemVO vo)
    {
        //itemStoreCon.GetComponent<UIProductSoldierStoreCon>().ChangeOneItemInfo(vo);
    }
    protected override int GetItemSpace(EntityModel model)
    {
        return model.spaceUse;
    }
    //public override void Clear()
    //{
    //    base.Clear();
    //    //清理预备兵种容器
    //    //itemStoreCon.GetComponent<UIProductSoldierStoreCon>().ClearItemStore();
    //}

    protected override void SetPanelName()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        txtPanelName.text = "当前容量:" + module.currentProductFactory.CurrentQueueSize + "/" + module.currentProductFactory.MaxQueueSize;
    }

    protected override void ShowFullTips()
    {
        return;
    }
}
