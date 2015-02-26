
using System;
using com.pureland.proto;
using UnityEngine;

public abstract class BaseResourceBuildingComponent : EntityComponent
{
    private int stealableCount;     //可以偷的数量
    private int stolenCount;        //被偷的数量
    private float stolenFloat;        //小数处理
    private float stealableCountPerDamage;//每一点伤害被偷的量

    protected float resourceStorage;  //当前存储量

    protected int viewIndex;
    public ResourceType ResourceType
    {
        get { return Entity.model.resourceType; }
    }
    public int StealableCount
    {
        get { return stealableCount; }
        private set { stealableCount = value; }
    }

    public override void Init()
    {
        base.Init();
        if (GameWorld.Instance.worldType == WorldType.Battle || GameWorld.Instance.worldType == WorldType.Replay)
        {
            stolenFloat = 0.1f;         //防止最后一点计算误差
            stealableCount = CalcStealableResourceCount();
            stealableCountPerDamage = (float)stealableCount / Entity.model.hp;
        }
        UpdateStorageView();
    }
    /// <summary>
    /// 根据存储量更新到5种现实状态
    /// </summary>
    protected void UpdateStorageView()
    {
        var storagePercent = CalcStoragePercent();
        viewIndex = Mathf.Clamp(Mathf.FloorToInt(storagePercent * 5) + 1, 1, 5);
        Entity.view.SwitchBody(viewIndex);//index 1..5
    }

    virtual public bool HandleOnTap()
    {
        return false;
    }

    /// <summary>
    /// 根据比例参数计算可以被偷的总量
    /// </summary>
    /// <param name="ratio"></param>
    /// <returns></returns>
    protected abstract int CalcStealableResourceCount();

    /// <summary>
    /// 计算当前存储状态 0..1
    /// </summary>
    /// <returns></returns>
    protected abstract float CalcStoragePercent();

    /// <summary>
    /// 根据伤害计算可以偷的数量
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    private int CalcStealableCountForDamage(float damage)
    {
        if (stealableCount == stolenCount)
            return 0;
        float stealableCountForDamage = stealableCountPerDamage * damage + stolenFloat;
        int intVal = Mathf.FloorToInt(stealableCountForDamage);
        stolenFloat = stealableCountForDamage - (float)intVal;
        if (stolenCount + intVal > stealableCount)
        {
            intVal = stealableCount - stolenCount;
        }
        stolenCount += intVal;
        return intVal;
    }

    public override void HandleMessage(EntityMessageType msg, object data = null)
    {
        switch (msg)
        {
            case EntityMessageType.MakeDamage:
            float damage = (float) data;
            int stolenCount = CalcStealableCountForDamage(damage);
            if (stolenCount > 0)
            {
                BattleManager.Instance.StolenResource(new ResourceVO() { resourceType = Entity.model.resourceType, resourceCount = stolenCount });
            }
            break;
        }
    }
}
