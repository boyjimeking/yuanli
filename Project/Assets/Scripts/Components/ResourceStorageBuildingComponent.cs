
using UnityEngine;

public class ResourceStorageBuildingComponent : BaseResourceBuildingComponent
{
    public int resourceStorage;
    public int maxResourceStorage;
    public override void Init()
    {
        maxResourceStorage = Entity.model.maxResourceStorage;
        base.Init();
    }

    protected override int CalcStealableResourceCount()
    {
        return Mathf.FloorToInt(Mathf.Min(resourceStorage * Constants.STEALABLE_RATIO_STORAGE, Constants.MAX_STEALABLE_STORAGE) * BattleManager.Instance.GetBattleRewardRatio());
    }

    protected override float CalcStoragePercent()
    {
        return (float) resourceStorage / maxResourceStorage;
    }
}

