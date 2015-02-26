using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class ModuleOperateItem : IModule
{
    //当前点击的建筑
    public TileEntity curOperEntity;
    //所有的技能信息
    public Dictionary<int, ProductionItemVO> dicAllSkill = new Dictionary<int, ProductionItemVO>();
    //所有的兵种信息
    public Dictionary<int, ProductionItemVO> dicAllSolider = new Dictionary<int, ProductionItemVO>();
    //当前兵营或者是产生道具的建筑信息
    public BaseProductBuildingComponent currentProductFactory;
    //当前兵营的ID
    public int currentProductFactoryLevel;
    //当前研究所的建筑信息
    public ResearchBuildingComponent researchBuildingComponent;
    public ModuleOperateItem()
    {
    }
    /// <summary>
    /// 初始化兵种信息-添加一个兵种
    /// </summary>
    /// <param name="vo"></param>
    public void AddOneSoldier(ProductionItemVO vo)
    {
        if (!dicAllSolider.ContainsKey(vo.cid))
        {
            dicAllSolider.Add(vo.cid, vo);
        }
    }
    /// <summary>
    /// 初始化技能信息-添加一个技能
    /// </summary>
    /// <param name="vo"></param>
    public void AddOneSkill(ProductionItemVO vo)
    {
        if (!dicAllSkill.ContainsKey(vo.cid))
        {
            dicAllSkill.Add(vo.cid, vo);
        }
    }
    public string ModuleName()
    {
        return GameModule.MODULE_PRODUCTSOLDIER;
    }

    public void ClearModule()
    {
        curOperEntity = null;
        currentProductFactory = null;
        currentProductFactoryLevel = -1;
        dicAllSkill.Clear();
        dicAllSolider.Clear();
    }
}
