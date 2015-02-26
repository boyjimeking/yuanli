using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;
using System;

public class UIProductSoldierStoreCon : UIProductBaseItemStoreCon
{
    /// <summary>
    /// 更新兵种信息
    /// </summary>
    public override void UpdateItemStore()
    {
        base.UpdateItemStore();
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        ProductionItemVO itemVO;
        GameObject tempSolider;
        int index = 0;
        EntityModel model;
        foreach (KeyValuePair<int, ProductionItemVO> keyValuePair in module.dicAllSolider)
        {
            itemVO = keyValuePair.Value;
            model = ModelUtil.GetEntityModel(itemVO.cid);
            tempSolider = dicItem.ContainsKey(model.subType) ? dicItem[model.subType] : null;
            if (null == tempSolider)
            {
                tempSolider = (GameObject)GameObject.Instantiate(item, this.transform.localPosition, Quaternion.identity);
                tempSolider.transform.parent = this.transform;
                tempSolider.transform.localScale = new Vector3(1, 1, 1);
                tempSolider.transform.localPosition = new Vector3(-301 + (index / 2) * (tempSolider.GetComponent<UISprite>().width + 20), 83 - Mathf.Floor(index % 2) * (tempSolider.GetComponent<UISprite>().height + 20), 0);
                tempSolider.SetActive(true);
                tempSolider.GetComponent<UIDragScrollView>().scrollView = this.transform.GetComponent<UIScrollView>();
                dicItem.Add(model.subType, tempSolider);
            }
            tempSolider.GetComponent<UIProductItemInfo>().ItemVO = itemVO;
            //可以用兵种需要的训练营id可当前的训练营id来判断
            if (model.buildNeedLevel <= module.currentProductFactoryLevel)
            {
                tempSolider.GetComponent<UIProductItemInfo>().OpenLevel = -1;
            }
            else
            {
                //提示玩家多少等级开放
                tempSolider.GetComponent<UIProductItemInfo>().OpenLevel = model.buildNeedLevel;
                tempSolider.GetComponent<UIRepeatedButton>().IsRepeated = false;
            }
            index++;
        }
        //刷新兵种的ItemVO
        foreach (ProductionItemVO vo in module.currentProductFactory.ProductionItems)
        {
            dicItem[ModelUtil.GetEntityModel(vo.cid).subType].GetComponent<UIProductItemInfo>().ItemVO = vo;
        }
        //如果当前训练营满了则变黑
        CheckCurrentCapacity();
    }
    /// <summary>
    /// 全部信息重置
    /// </summary>
    public override void ResetAllItemInfo()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        ProductionItemVO itemVO;
        GameObject tempSolider;
        foreach (KeyValuePair<int, ProductionItemVO> keyValuePair in module.dicAllSolider)
        {
            itemVO = keyValuePair.Value;
            itemVO.count = 0;
            tempSolider = dicItem.ContainsKey(ModelUtil.GetEntityModel(itemVO.cid).subType) ? dicItem[ModelUtil.GetEntityModel(itemVO.cid).subType] : null;
            if (null != tempSolider)
                tempSolider.GetComponent<UIProductItemInfo>().ItemVO = itemVO;
        }
        CheckCurrentCapacity();
    }

    protected override int GetItemSpace(EntityModel model)
    {
        return model.spaceUse;
    }
}
