using com.pureland.proto;
using UnityEngine;

public class LoadHomelandCommand : NetCommand
{
    private long userId;
    private WorldType worldType;

    public LoadHomelandCommand(long userId, WorldType worldType)
    {
        this.userId = userId;
        this.worldType = worldType;
    }

    public override ReqWrapper Execute()
    {
        var campReq = new CampReq();
        campReq.userId = userId;
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.Camp, campReq = campReq };
    }

    public override void OnResponse(BaseResp resp)
    {
        DataCenter.Instance.originDefenderData = ProtoBuf.Serializer.DeepClone(resp.respWrapper.campResp.campVO);
        DataCenter.Instance.SetHomelandData(resp.respWrapper.campResp.campVO);

        if (worldType == WorldType.Home)//TODO 考虑等待一定时间再加载,考虑缓存
        {
            //GameManager.Instance.RequestBattleHistory();
        }

        GameWorld.Instance.Create(worldType);
    }
}