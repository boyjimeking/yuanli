using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.pureland.proto;

public abstract class UIProductBaseItemStoreCon : MonoBehaviour
{
    //兵种
    public GameObject item;
    //所有的兵种信息界面
    protected Dictionary<string, GameObject> dicItem = new Dictionary<string, GameObject>();
    /// <summary>
    /// 更新兵种信息
    /// </summary>
    public virtual void UpdateItemStore()
    {
        foreach (KeyValuePair<string, GameObject> keyValuePair in dicItem)
        {
            GameObject.Destroy(keyValuePair.Value);
        }
        dicItem.Clear();
    }
    public virtual void CheckCurrentCapacity()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        EntityModel model;
        UIProductItemInfo info;
        int freeCount = module.currentProductFactory.MaxQueueSize - module.currentProductFactory.CurrentQueueSize;
        foreach (KeyValuePair<string, GameObject> keyValuePair in dicItem)
        {
            info = keyValuePair.Value.GetComponent<UIProductItemInfo>();
            model = DataCenter.Instance.FindEntityModelById(info.ItemVO.cid);
            keyValuePair.Value.GetComponent<UIProductItemInfo>().IsGrey = GetItemSpace(model) > freeCount;
        }
    }
    protected abstract int GetItemSpace(EntityModel model);
    /// <summary>
    /// 改变一个ITem的信息
    /// </summary>
    /// <param name="itemVO"></param>
    public void ChangeOneItemInfo(ProductionItemVO itemVO)
    {
        //改变下面士兵的信息
        GameObject tempItem = dicItem[ModelUtil.GetEntityModel(itemVO.cid).subType];
        UIProductItemInfo itemInfo = tempItem.GetComponent<UIProductItemInfo>();
        itemInfo.ItemVO = itemInfo.ItemVO;
        CheckCurrentCapacity();
    }
    /// <summary>
    /// 全部信息重置
    /// </summary>
    public abstract void ResetAllItemInfo();
    public void ClearItemStore()
    {
        foreach (KeyValuePair<string, GameObject> keyValuePair in dicItem)
        {
            GameObject.Destroy(keyValuePair.Value);
        }
        dicItem.Clear();
    }
}
