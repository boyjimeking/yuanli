
using System;
using com.pureland.proto;
using UnityEngine;

public class GatherResourceCommand : NetCommand
{
    private GatherResourceBuildingComponent gatherResourceComponent;

    public GatherResourceCommand(GatherResourceBuildingComponent gatherResourceComponent)
    {
        this.gatherResourceComponent = gatherResourceComponent;
    }

    public override ReqWrapper Execute()
    {
        GatherReq req = new GatherReq();
        req.sid = gatherResourceComponent.Entity.buildingVO.sid;
        var gatherTime = ServerTime.Instance.Now();
        gatherResourceComponent.Entity.buildingVO.resourceBuildingVO.lastGatherTime =
            DateTimeUtil.DateTimeToUnixTimestampMS(gatherTime);
        req.gatherTime = DateTimeUtil.DateTimeToUnixTimestampMS(gatherTime);
        var leftStorageSpace =
            DataCenter.Instance.GetMaxResourceStorage(gatherResourceComponent.Entity.model.resourceType) -
            DataCenter.Instance.GetResource(gatherResourceComponent.Entity.model.resourceType);
        var canGatherCount = gatherResourceComponent.CalculateResourceFromLastGather(gatherTime);
        req.resourceVO = new ResourceVO()
        {
            resourceType = gatherResourceComponent.Entity.model.resourceType,
            resourceCount = Mathf.Min(leftStorageSpace,canGatherCount),
        };
        DataCenter.Instance.AddResource(req.resourceVO);
        gatherResourceComponent.UpdateState(gatherTime,canGatherCount - req.resourceVO.resourceCount);
        return new ReqWrapper() {requestType = ReqWrapper.RequestType.Gather, gatherReq = req};
    }
}
