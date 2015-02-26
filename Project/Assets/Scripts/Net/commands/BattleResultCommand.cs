
using com.pureland.proto;

public class BattleResultCommand : NetCommand
{
    private BattleResultVO battleResult;
    private BattleReplayVO battleReplay;

    public BattleResultCommand(BattleResultVO battleResult, BattleReplayVO battleReplay)
    {
        this.battleResult = battleResult;
        this.battleReplay = battleReplay;
    }

    public override ReqWrapper Execute()
    {
        var gold = new ResourceVO()
        {
            resourceType = ResourceType.Gold,
            resourceCount = battleResult.rewardGoldByCrownLevel
        };
        var oil = new ResourceVO()
        {
            resourceType = ResourceType.Oil, 
            resourceCount = battleResult.rewardOilByCrownLevel
        };
        DataCenter.Instance.AddResource(gold, OwnerType.Attacker);
        DataCenter.Instance.AddResource(oil, OwnerType.Attacker);
        EventDispather.DispatherEvent(GameEvents.STOLEN_RESOURCE, gold);
        EventDispather.DispatherEvent(GameEvents.STOLEN_RESOURCE, oil);
        DataCenter.Instance.AddCrown(battleResult.rewardCrown,OwnerType.Attacker);
        for (int i = DataCenter.Instance.Attacker.armies.Count - 1; i >= 0; i--)
        {
            if (DataCenter.Instance.Attacker.armies[i].amount == 0)
            {
                DataCenter.Instance.Attacker.armies.RemoveAt(i);
            }
        }

        return new ReqWrapper()
        {
            requestType = ReqWrapper.RequestType.BattleResult,
            battleResultReq = new BattleResultReq()
            {
                battleResultVO = battleResult,
                battleReplayVO = battleReplay
            }
        };
    }
}
