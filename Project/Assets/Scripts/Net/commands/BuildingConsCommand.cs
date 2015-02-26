using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class BuildingConsCommand : NetCommand
{
    private ResourceType resourceType;
    private TileEntity entity;

    public BuildingConsCommand(TileEntity entity)
    {
        resourceType = entity.model.costResourceType;
        this.entity = entity;
    }
    public override com.pureland.proto.ReqWrapper Execute()
    {
        var buildTime = entity.model.buildTime;
        var buildingVO = new BuildingVO();
        buildingVO.buildingStatus = buildTime == 0 ? BuildingVO.BuildingStatus.On : BuildingVO.BuildingStatus.Construct;
        buildingVO.cid = entity.model.baseId;
        buildingVO.sid = DataCenter.Instance.CreateNextItemSid();
        buildingVO.endTime = ServerTime.Instance.GetTimestamp(buildTime);
        buildingVO.x = entity.GetTilePos().x;
        buildingVO.y = entity.GetTilePos().y;

        if (EntityTypeUtil.IsGatherResourceBuilding(entity.model))
        {
            buildingVO.resourceBuildingVO = new ResourceBuildingVO() { lastGatherTime = buildingVO.endTime };//建造完成开始计算产量
        }
        //替换为有建筑动画的对象
        var newEntity = entity.ReplaceWith(entity.model, buildingVO);
        IsoMap.Instance.ForceAddEntity(newEntity);

        if (buildTime != 0)
        {
            //占用工人数量
            DataCenter.Instance.FreeWorker -= 1;
        }
        //添加到建筑vo,方便计数等
        DataCenter.Instance.AddBuilding(buildingVO);
        //消耗资源
        DataCenter.Instance.AddResource(new ResourceVO() { resourceType = entity.model.costResourceType, resourceCount = -entity.model.costResourceCount });


        var buildingConsReq = new BuildingConsReq();
        buildingConsReq.buildingVO = buildingVO;
        buildingConsReq.resourceType = resourceType;
        return new ReqWrapper() {requestType = ReqWrapper.RequestType.BuildingCons, buildingConsReq = buildingConsReq};
    }
}
