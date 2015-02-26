using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationButtonUtil : MonoBehaviour
{
    //信息按钮
    public const string BUILDINFO = "ButtonInfo";
    //升级按钮
    public const string BUILDLEVELUP = "ButtonLevelUp";
    //提取资源按钮
    public const string COLLECTRESOURCE = "ButtonCollect";
    //单独提速
    public const string SINGLESPEEDUP = "ButtonSingleSpeed";
    //同类型提速
    public const string SAMETYPESPEEDUP = "ButtonSameSpeed";
    //重置陷阱
    public const string RESETXIANJING = "ButtonResetXianJing";
    //重置全部陷阱
    public const string RESETALLXIANJING = "ButtonResetAllXianJing";
    //启动人工护盾
    public const string OPENHUDUN = "ButtonOpenHuDun";
    //训练军队
    public const string TRAINARMY = "ButtonTrainArmy";
    //加速完成
    public const string COMPLETERIGHTNOW = "ButtonRightOver";
    //生产技能道具
    public const string PRODUCTSKILL = "ButtonProductSkill";
    //研究A所
    public const string RESEARCH_A = "ButtonResearchA";
    //研究B所
    public const string RESEARCH_B = "ButtonResearchB";
    //增援
    public const string ZENGYUAN = "ButtonZengYuan";
    //联邦
    public const string FEDERAL = "ButtonFederal";
    //选择整行
    public const string SELECTROW = "ButtonSelectRow";
    public static List<string> GetButtonList(TileEntity tileEntity)
    {
        List<string> returnList = new List<string>();
        //信息
        returnList.Add(BUILDINFO);
        if (GameWorld.Instance.worldType == WorldType.Visit)
            return returnList;
        //升级
        if (tileEntity.model.upgradeId != 0)
            returnList.Add(BUILDLEVELUP);
        if (tileEntity.buildingVO.buildingStatus != com.pureland.proto.BuildingVO.BuildingStatus.On)
            returnList.Add(COMPLETERIGHTNOW);
        if (EntityTypeUtil.IsCenterBuilding(tileEntity.model))
        {
            //大本营判断陷阱
            if (BuildOptManager.Instance.hasBrokenTrap())
            {
                returnList.Add(RESETALLXIANJING);
            }
        }
        else if (EntityTypeUtil.IsGatherResourceBuilding(tileEntity.model))
        {
            //资源收集
            CheckCollectResourceBuilding(tileEntity, returnList);
        }
        else if (EntityTypeUtil.IsArmyShop(tileEntity.model))
        {
            CheckArmyBuilding(tileEntity, returnList);
        }
        else if (EntityTypeUtil.IsSkillShop(tileEntity.model))
        {
            CheckLaboratoryBuilding(tileEntity, returnList);
        }
        else if (EntityTypeUtil.IsResearch(tileEntity.model))
        {
            CheckResearchBuilding(tileEntity, returnList);
        }
        else if (EntityTypeUtil.IsAnyTrap(tileEntity.model))
        {
            CheckXianJingBuilding(tileEntity, returnList);
        }
        return returnList;
    }
    /// <summary>
    /// 检查研究
    /// </summary>
    /// <param name="tileEntity"></param>
    /// <param name="returnList"></param>
    private static void CheckResearchBuilding(TileEntity tileEntity, List<string> returnList)
    {
        returnList.Add(RESEARCH_A);
        returnList.Add(RESEARCH_B);
    }
    /// <summary>
    /// 检查陷阱的
    /// </summary>
    /// <param name="tileEntity"></param>
    /// <param name="returnList"></param>
    private static void CheckXianJingBuilding(TileEntity tileEntity, List<string> returnList)
    {
        if (tileEntity.buildingVO.trapBuildingVO.broken)
        {
            returnList.Add(RESETXIANJING);
        }
        if (BuildOptManager.Instance.hasOtherBrokenTrap(tileEntity.buildingVO.sid))
        {
            returnList.Add(RESETALLXIANJING);
        }
    }
    /// <summary>
    /// 研究法术的
    /// </summary>
    /// <param name="tileEntity"></param>
    /// <param name="returnList"></param>
    private static void CheckLaboratoryBuilding(TileEntity tileEntity, List<string> returnList)
    {
        returnList.Add(PRODUCTSKILL);
    }
    /// <summary>
    /// 生产军队建筑
    /// </summary>
    /// <param name="tileEntity"></param>
    /// <param name="returnList"></param>
    private static void CheckArmyBuilding(TileEntity tileEntity, List<string> returnList)
    {
        returnList.Add(SINGLESPEEDUP);
        returnList.Add(SAMETYPESPEEDUP);
        returnList.Add(TRAINARMY);
    }
    /// <summary>
    /// 收集资源类建筑
    /// </summary>
    /// <param name="tileEntity"></param>
    /// <param name="returnList"></param>
    private static void CheckCollectResourceBuilding(TileEntity tileEntity, List<string> returnList)
    {
        returnList.Add(SINGLESPEEDUP);
        returnList.Add(SAMETYPESPEEDUP);
        returnList.Add(COLLECTRESOURCE);
    }
}
