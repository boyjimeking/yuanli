using UnityEngine;
using System.Collections;

public class ModelUtil
{
    /// <summary>
    /// 获得下一等级的EntityModel
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static EntityModel GetNextLevelModel(int id)
    {
        EntityModel model = DataCenter.Instance.FindEntityModelById(id);
        if (model.upgradeId != 0)
            return DataCenter.Instance.FindEntityModelById(model.upgradeId);
        return null;
    }
    public static EntityModel GetEntityModel(int id)
    {
        return DataCenter.Instance.FindEntityModelById(id);
    }
}
