using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildPropertyUtil : MonoBehaviour
{
    public const string HP = "hp";
    public const string SPACEPROVIDE = "spaceProvide";
    public const string QUEUESIZE = "queueSize";
    public const string MAXRESOURCESTORAGE = "maxResourceStorage";
    public const string RESOURCEPERSECONDFORVIEW = "resourcePerSecondForView";

    public static List<string> GetPropertyList(TileEntity tileEntity)
    {
        List<string> returnList = new List<string>();
        //信息
        returnList.Add(HP);
        if (EntityTypeUtil.IsCenterBuilding(tileEntity.model))
        {
            //大本营
        }
        else if (EntityTypeUtil.IsBarracks(tileEntity.model))
        {
            //兵营
            returnList.Add(SPACEPROVIDE);
        }
        else if (EntityTypeUtil.IsGatherResourceBuilding(tileEntity.model))
        {
            //资源收集
            returnList.Add(MAXRESOURCESTORAGE);
            returnList.Add(RESOURCEPERSECONDFORVIEW);
        }
        else if (EntityTypeUtil.IsArmyShop(tileEntity.model))
        {
            //兵工厂
            returnList.Add(QUEUESIZE);
        }
        else if (EntityTypeUtil.IsSkillShop(tileEntity.model))
        {
            //实验室
        }
        else if (EntityTypeUtil.IsResearch(tileEntity.model))
        {
            //研究所
        }
        else if (EntityTypeUtil.IsAnyTrap(tileEntity.model))
        {
            //陷阱
        }
        return returnList;
    }
}
