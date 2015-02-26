using UnityEngine;
using System.Collections;
using com.pureland.proto;

public abstract class UIProductItemBaseFrame : MonoBehaviour
{
    //正在建造容器
    public GameObject prefabProductingItemCon;
    //面板名字
    public UILabel txtPanelName;
    //正在生产的容器
    protected GameObject productingItemCon;
    //容器
    public Transform itemStoreCon;
    //建造容器显示
    protected GameObject itemDisplayFrame;
    //返回按钮
    public GameObject btnReturn;
    //物品描述
    protected GameObject itemDesFrame;
    protected virtual void OnEnable()
    {
        UIEventListener.Get(btnReturn).onClick += OnClickButton;
        EventDispather.AddEventListener(ItemOperationManager.PRODUCT_ONE_ITEM, AddOneProductItem);
        EventDispather.AddEventListener(ItemOperationManager.REMOVE_ONE_ITEM, RemoveProductItem);
        EventDispather.AddEventListener(ItemOperationManager.PRODUCT_OVER_RIGHTNOW, RightNowOverProduct);
    }

    protected abstract void RightNowOverProduct(string eventType, object obj);
    protected virtual void OnDisable()
    {
        UIEventListener.Get(btnReturn).onClick -= OnClickButton;
        EventDispather.RemoveEventListener(ItemOperationManager.PRODUCT_ONE_ITEM, AddOneProductItem);
        EventDispather.RemoveEventListener(ItemOperationManager.REMOVE_ONE_ITEM, RemoveProductItem);
        EventDispather.RemoveEventListener(ItemOperationManager.PRODUCT_OVER_RIGHTNOW, RightNowOverProduct);
    }
    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnReturn))
        {
            itemDisplayFrame.SetActive(true);
            itemDesFrame.SetActive(false);
        }
    }
    public virtual void ShowProductItemFrame()
    {
        //添加正在生产容器
        if (null == productingItemCon)
        {
            productingItemCon = (GameObject)GameObject.Instantiate(prefabProductingItemCon, Vector3.zero, Quaternion.identity);
            productingItemCon.transform.parent = this.transform;
            productingItemCon.transform.localPosition = new Vector3(0, 175, 0);
            productingItemCon.transform.localScale = new Vector3(1, 1, 1);
            productingItemCon.SetActive(true);
        }
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        module.currentProductFactory.EventComplete -= CompleteOneItem;
        module.currentProductFactory.EventComplete += CompleteOneItem;
        //刷新预备的物品
        itemStoreCon.GetComponent<UIProductBaseItemStoreCon>().UpdateItemStore();
        //刷新正在生产的物品
        productingItemCon.GetComponent<UIProductingItemCon>().UpdateProductingItemFrame();
        SetPanelName();
    }
    private void CompleteOneItem(ProductionItemVO vo)
    {
        RemoveProductByProductItemVO(vo);
    }
    public virtual void AddOneProductItem(string eventType, object obj)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        UIProductItemInfo info = ((GameObject)obj).GetComponent<UIProductItemInfo>();
        ProductionItemVO itemVO = info.ItemVO;
        EntityModel model = DataCenter.Instance.FindEntityModelById(itemVO.cid);
        string name = DataCenter.Instance.NeedToLocalName(model.buildNeedType, model.buildNeedLevel);
        if (info.OpenLevel > 0)
        {
            GameTipsManager.Instance.ShowGameTips(EnumTipsID.ItemOper_10201, new string[] { model.buildNeedLevel.ToString(), name });
            return;
        }
        if (model.trainCostResourceCount > DataCenter.Instance.GetResource(model.trainCostResourceType))
        {
            //弹出资源和钱的转换框
            ShopManager.Instance.BuyResource(model.trainCostResourceType, model.trainCostResourceCount - DataCenter.Instance.GetResource(model.trainCostResourceType), BuyResComplete, info);
            return;
        }
        CheckFactorySpace(info);
    }
    private void BuyResComplete(bool isSuccess, object obj)
    {
        if (!isSuccess) return;
        CheckFactorySpace((UIProductItemInfo)obj);
    }
    private void CheckFactorySpace(UIProductItemInfo info)
    {
        ProductionItemVO itemVO = info.ItemVO;
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        EntityModel model = DataCenter.Instance.FindEntityModelById(itemVO.cid);
        //判断训练营的训练总量
        if (GetItemSpace(model) > module.currentProductFactory.MaxQueueSize - module.currentProductFactory.CurrentQueueSize)
        {
            ShowFullTips();
            return;
        }
        //改变资源数量
        DataCenter.Instance.RemoveResource(model.trainCostResourceType, model.trainCostResourceCount);
        if (null == ItemOperationManager.Instance.GetProductionItemVOById(itemVO.cid))
        {
            itemVO.count = 1;
            module.currentProductFactory.AddProduction(itemVO);
        }
        else
        {
            module.currentProductFactory.AddProduction(new ProductionItemVO() { cid = itemVO.cid, count = 1 });
        }
        info.ItemVO = itemVO;
        itemStoreCon.GetComponent<UIProductBaseItemStoreCon>().CheckCurrentCapacity();
        productingItemCon.GetComponent<UIProductingItemCon>().AddOneProductingItem(itemVO);
        SetPanelName();
        AddOneProductItemComplete();
    }
    protected abstract void AddOneProductItemComplete();
    public virtual void RemoveProductItem(string eventType, object obj)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        UIProductingItem info = ((GameObject)obj).GetComponent<UIProductingItem>();
        ProductionItemVO itemVO = info.ItemVO;
        EntityModel model = DataCenter.Instance.FindEntityModelById(itemVO.cid);
        //改变资源数量
        DataCenter.Instance.AddResource(new ResourceVO() { resourceType = model.trainCostResourceType, resourceCount = model.trainCostResourceCount });
        module.currentProductFactory.RemoveProduction(new ProductionItemVO() { cid = itemVO.cid, count = 1 });
        RemoveProductByProductItemVO(itemVO);
    }

    private void RemoveProductByProductItemVO(ProductionItemVO itemVO)
    {
        productingItemCon.GetComponent<UIProductingItemCon>().RemoveOneProductingItem(itemVO);
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        SetPanelName();
        itemStoreCon.GetComponent<UIProductBaseItemStoreCon>().ChangeOneItemInfo(itemVO);
        RemoveOneProductItemComplete(itemVO);
    }
    protected abstract void RemoveOneProductItemComplete(ProductionItemVO itemVO);
    protected abstract int GetItemSpace(EntityModel model);
    protected abstract void SetPanelName();
    protected abstract void ShowFullTips();
    /// <summary>
    /// 生产立即完成
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="obj"></param>
    public virtual void Clear()
    {
        //移调造兵完成事件
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        module.currentProductFactory.EventComplete -= CompleteOneItem;
        itemStoreCon.GetComponent<UIProductBaseItemStoreCon>().ClearItemStore();
        if (productingItemCon)
            productingItemCon.GetComponent<UIProductingItemCon>().ClearProductingItem();
    }
}
