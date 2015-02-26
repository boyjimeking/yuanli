using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class BuildingUpgradeCommand : NetCommand
{
    //建筑sid
    private long sid;
    //是否取消建筑
    private bool isCancel;

    private ResourceType resourceType;

    private TileEntity entity;
    public BuildingUpgradeCommand(TileEntity entity, ResourceType resourceType, bool isCancel)
    {
        this.resourceType = resourceType;
        this.sid = entity.buildingVO.sid;
        this.isCancel = isCancel;

        this.entity = entity;
    }

    public override com.pureland.proto.ReqWrapper Execute()
    {
        var upgradeModel = DataCenter.Instance.FindEntityModelById(entity.model.upgradeId);
        if (!isCancel)
        {
            //消耗资源,REMARK 升级消耗写在了下一级
            DataCenter.Instance.RemoveResource(upgradeModel.costResourceType, upgradeModel.costResourceCount);
            //判断是不是立即完成
            if (upgradeModel.buildTime == 0)
            {
                DataCenter.Instance.AddExp(upgradeModel.buildExp);
                entity.buildingVO.cid = upgradeModel.baseId;
            }
            else
            {
                DataCenter.Instance.FreeWorker -= 1;
                entity.buildingVO.buildingStatus = BuildingVO.BuildingStatus.Upgrade;
                entity.buildingVO.endTime = ServerTime.Instance.GetTimestamp(upgradeModel.buildTime);
            }
            //替换成建造或者升级完成后的建筑
            var newEntity = entity.ReplaceWith(entity.model, entity.buildingVO);
            ((IsoWorldModeBuilder)GameWorld.Instance.CurrentWorldMode).SelectBuilding(newEntity);
        }
        else//取消升级
        {
            //取消 消耗资源 返回50%
            DataCenter.Instance.AddResource(new ResourceVO() { resourceType = upgradeModel.costResourceType, resourceCount = Mathf.FloorToInt(upgradeModel.costResourceCount * 0.5f) });
            DataCenter.Instance.FreeWorker += 1;
            entity.buildingVO.buildingStatus = BuildingVO.BuildingStatus.On;
            //替换成升级前的建筑
            var newEntity = entity.ReplaceWith(entity.model, entity.buildingVO);
            ((IsoWorldModeBuilder)GameWorld.Instance.CurrentWorldMode).SelectBuilding(newEntity);
        }

        var buildingUpReq = new BuildingUpgradeReq();
        buildingUpReq.sid = sid;
        buildingUpReq.resourceType = resourceType;
        buildingUpReq.cancel = isCancel;
        buildingUpReq.endTime = entity.buildingVO.endTime;

        return new ReqWrapper() { requestType = ReqWrapper.RequestType.BuildingUpgrade, buildingUpReq = buildingUpReq };
    }
}
