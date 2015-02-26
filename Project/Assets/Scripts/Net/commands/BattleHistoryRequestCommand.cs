
using com.pureland.proto;

public class BattleHistoryRequestCommand : NetCommand
{
    public override ReqWrapper Execute()
    {
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.BattleHistory, battleHistoryReq = new BattleHistoryReq() };
    }

    public override void OnResponse(BaseResp resp)
    {
        DataCenter.Instance.SetBattleHistories(resp.respWrapper.battleHistoryResp.attackBattleHistories,resp.respWrapper.battleHistoryResp.defenseBattleHistories);
    }
}
