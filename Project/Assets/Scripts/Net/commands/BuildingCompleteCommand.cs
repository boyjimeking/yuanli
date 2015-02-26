
using com.pureland.proto;
using Org.BouncyCastle.Utilities.IO;

public class BuildingCompleteCommand : NetCommand
{
    private long sid;
    private TileEntity entity;
    private bool immediately;
    public BuildingCompleteCommand(TileEntity entity, bool immediately)
    {
        this.sid = entity.buildingVO.sid;

        this.entity = entity;
        this.immediately = immediately;
    }
    public override ReqWrapper Execute()
    {
        //占用工人数量
        DataCenter.Instance.FreeWorker += 1;
        EntityModel replaceModel = null;
        if (entity.buildingVO.buildingStatus == BuildingVO.BuildingStatus.Construct)
        {
            replaceModel = entity.model;
            //判断是否是军营
            if (EntityTypeUtil.IsBarracks(entity.model))
            {
                DataCenter.Instance.TotalSpace += entity.model.spaceProvide;
            }
        }
        else if (entity.buildingVO.buildingStatus == BuildingVO.BuildingStatus.Upgrade)//升级
        {
            replaceModel = DataCenter.Instance.FindEntityModelById(entity.model.upgradeId);
            //判断是否是军营
            if (EntityTypeUtil.IsBarracks(entity.model))
            {
                DataCenter.Instance.TotalSpace += (replaceModel.spaceProvide - entity.model.spaceProvide);//军营空间升级
            }
            else if (EntityTypeUtil.IsCenterBuilding(entity.model))
            {
                DataCenter.Instance.Defender.player.baseId = replaceModel.baseId;
            }
        }
        else
        {
            Assert.Fail("building status error!");
        }
        DataCenter.Instance.AddExp(replaceModel.buildExp);

        //替换成建造或者升级完成后的建筑
        entity.buildingVO.buildingStatus = BuildingVO.BuildingStatus.On;
        entity.buildingVO.cid = replaceModel.baseId;
        var newEntity = entity.ReplaceWith(replaceModel, entity.buildingVO);
        ((IsoWorldModeBuilder)GameWorld.Instance.CurrentWorldMode).SelectBuilding(newEntity);

        EventDispather.DispatherEvent(GameEvents.BUILDING_COMPLETE, entity.buildingVO);

        var buildingCompleteReq = new BuildingCompleteReq();
        buildingCompleteReq.sid = sid;
        if (immediately)
        {
            buildingCompleteReq.completeType = BuildingCompleteReq.CompleteType.CompleteImmediately;
            var now = ServerTime.Instance.Now();
            buildingCompleteReq.timestamp = DateTimeUtil.DateTimeToUnixTimestampMS(now);

            int deltaTime = (int)(DateTimeUtil.UnixTimestampMSToDateTime(entity.buildingVO.endTime) - now).TotalSeconds;
            DataCenter.Instance.RemoveResource(ResourceType.Diamond, GameDataAlgorithm.TimeToGem(deltaTime));
        }
        else
        {
            buildingCompleteReq.completeType = BuildingCompleteReq.CompleteType.Normal;
        }
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.BuildingComplete, buildingCompleteReq = buildingCompleteReq };
    }
}
