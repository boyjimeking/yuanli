using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModuleShop : IModule
{
    //商店资源数据
    public List<ArrayList> shopData = new List<ArrayList>(7);
    public ModuleShop()
    {
        for (int i = 0; i < shopData.Capacity; i++)
        {
            shopData.Add(new ArrayList());
        }
    }
    public string ModuleName()
    {
        return GameModule.MODULE_SHOP;
    }
    /// <summary>
    /// 对每种商店类型下的物品按顺序排序
    /// </summary>
    public void SortShopData()
    {
        for (int i = 0; i < shopData.Count; i++)
        {
            shopData[i].Sort(new SortClass());
        }
    }
    /// <summary>
    /// 排序比较类
    /// </summary>
    private class SortClass : IComparer
    {
        public int Compare(object x, object y)
        {
            return (x as ShopModel).itemOrder.CompareTo((y as ShopModel).itemOrder);
        }
    }

    public void ClearModule()
    {
        throw new System.NotImplementedException();
    }
}
