using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;
using System;

public class UIProductingItemCon : MonoBehaviour
{
    //正在建造
    public GameObject productingItem;
    //立即完成
    public GameObject rightNowOver;
    //正在建造信息界面
    private List<GameObject> listProducting = new List<GameObject>();
    /// <summary>
    /// 更新正在生产的兵种信息
    /// </summary>
    public void UpdateProductingItemFrame()
    {
        //切换训练营时先清空正在生产的单位
        foreach (GameObject obj in listProducting)
        {
            GameObject.Destroy(obj);
        }
        listProducting.Clear();
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        GameObject tempProducting;
        int index = 0;
        //该训练营所有正在生产的兵种
        foreach (ProductionItemVO vo in module.currentProductFactory.ProductionItems)
        {
            tempProducting = FindProductByItemId(vo.cid);
            if (null == tempProducting)
            {
                tempProducting = (GameObject)GameObject.Instantiate(productingItem, this.transform.localPosition, Quaternion.identity);
                tempProducting.transform.parent = this.transform;
                tempProducting.transform.localScale = new Vector3(1, 1, 1);
                tempProducting.transform.localPosition = new Vector3(120 - index * (tempProducting.GetComponent<UISprite>().width + 2), 0, 0);
                tempProducting.SetActive(true);
                listProducting.Add(tempProducting);
            }
            tempProducting.GetComponent<UIProductingItem>().ItemVO = vo;
            index++;
        }
        if (null != module.currentProductFactory.CurrentProduction)
        {
            FindProductByItemId(module.currentProductFactory.CurrentProduction.cid).GetComponent<UIProductingItem>().IsCurrentProduct = true;
            CheckCanRightOver(module.currentProductFactory.CurrentProduction.cid);
        }
    }
    /// <summary>
    /// 添加一个生产单位
    /// </summary>
    /// <param name="itemVO"></param>
    public void AddOneProductingItem(ProductionItemVO itemVO)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        GameObject tempProducting = FindProductByItemId(itemVO.cid);
        if (null == tempProducting)
        {
            tempProducting = NGUITools.AddChild(gameObject, productingItem.gameObject);
            tempProducting.transform.localPosition = new Vector3(120 - listProducting.Count * (tempProducting.GetComponent<UISprite>().width + 20), 0, 0);
            tempProducting.SetActive(true);
            listProducting.Add(tempProducting);
        }
        UIProductingItem product = tempProducting.GetComponent<UIProductingItem>();
        product.ItemVO = itemVO;
        if (null != module.currentProductFactory.CurrentProduction)
        {
            if (product.ItemVO.cid == module.currentProductFactory.CurrentProduction.cid)
            {
                product.IsCurrentProduct = true;
            }
        }
        CheckCanRightOver(itemVO.cid);
    }
    /// <summary>
    /// 兵种检查兵营是否满了
    /// </summary>
    /// <param name="cid"></param>
    private void CheckCanRightOver(int cid)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        if (module.currentProductFactory.TimeLeft > 0)
        {
            rightNowOver.SetActive(true);
            if (EntityTypeUtil.IsAnyActor(ModelUtil.GetEntityModel(cid).entityType))
            {
                if (DataCenter.Instance.SpaceUsed + module.currentProductFactory.ProductionItems.Count >= DataCenter.Instance.TotalSpace)
                {
                    rightNowOver.GetComponent<UIProductRightOver>().SoldierSpaceFull = true;
                }
                else
                {
                    rightNowOver.GetComponent<UIProductRightOver>().SoldierSpaceFull = false;
                }
            }
        }
        else
        {
            rightNowOver.SetActive(false);
        }
    }
    /// <summary>
    /// 删除一个正在生产的单位
    /// </summary>
    /// <param name="vo"></param>
    public void RemoveOneProductingItem(ProductionItemVO vo)
    {
        GameObject obj = FindProductByItemId(vo.cid);
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        UIProductingItem info = obj.GetComponent<UIProductingItem>();
        ProductionItemVO itemVO = info.ItemVO;
        info.ItemVO = itemVO;
        if (itemVO.count <= 0)
        {
            listProducting.Remove(obj);
            GameObject.Destroy(obj);
            UpdateProductOrder();
        }
        if (null != module.currentProductFactory.CurrentProduction)
        {
            FindProductByItemId(module.currentProductFactory.CurrentProduction.cid).GetComponent<UIProductingItem>().IsCurrentProduct = true;
        }
        CheckCanRightOver(vo.cid);
    }
    /// <summary>
    /// 通过兵种id找到正在生产的兵种
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private GameObject FindProductByItemId(int id)
    {
        UIProductingItem productSoldier;
        foreach (GameObject obj in listProducting)
        {
            productSoldier = obj.GetComponent<UIProductingItem>();
            if (null != productSoldier.ItemVO && productSoldier.ItemVO.cid == id)
                return obj;
        }
        return null;
    }
    /// <summary>
    /// 删除完了一种类型排序
    /// </summary>
    private void UpdateProductOrder()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        int index = 0;
        foreach (GameObject obj in listProducting)
        {
            obj.transform.localPosition = new Vector3(120 - index * (obj.GetComponent<UISprite>().width + 20), 0, 0);
            index++;
        }
    }
    public void ClearProductingItem()
    {
        foreach (GameObject tempObj in listProducting)
        {
            GameObject.Destroy(tempObj);
        }
        listProducting.Clear();
        rightNowOver.SetActive(false);
    }
}
