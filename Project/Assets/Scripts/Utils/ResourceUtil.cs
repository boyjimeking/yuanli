using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class ResourceUtil
{
    public static string GetResNameByResType(ResourceType type)
    {
        string returnStr = "";
        switch (type)
        {
            case ResourceType.Gold:
                returnStr = "金币";
                break;
            case ResourceType.Oil:
                returnStr = "太阳油";
                break;
            case ResourceType.Diamond:
                returnStr = "钻石";
                break;
            case ResourceType.Medal:
                returnStr = "金牌";
                break;
        }
        return returnStr;
    }
    public static string GetItemIconByModel(EntityModel model)
    {
        if (EntityTypeUtil.IsSkill(model))
        {
            //技能
            return "UI_IconSkill_" + model.nameForResource;
        }
        else if (EntityTypeUtil.IsAnyActor(model.entityType))
        {
            //兵种
            return model.nameForResource.Substring(0, model.nameForResource.Length - 2) + "_IconSmall";
        }
        return "";
    }
}
