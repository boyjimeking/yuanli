
using com.pureland.proto;

public class BattleReplayCommand : NetCommand
{
    private long replayId;

    public BattleReplayCommand(long replayId)
    {
        this.replayId = replayId;
    }

    public override ReqWrapper Execute()
    {
        return new ReqWrapper()
        {
            requestType = ReqWrapper.RequestType.BattleReplay,
            battleReplayReq = new BattleReplayReq() { replayId = this.replayId }
        };
    }

    public override void OnResponse(BaseResp resp)
    {
        if (resp.errorType > 0)
        {
            GameTipsManager.Instance.ShowGameTips("请求回放失败");
            return;
        }
        var replayVO = resp.respWrapper.battleReplayResp.battleReplayVO;
        GameWorld.Instance.Create(WorldType.Replay, replayVO.defender, replayVO.attacker, replayVO);
    }
}
