
using com.pureland.proto;

public class ResearchRequestCommand : NetCommand
{
    private int cid;
    private ResearchReq.ResearchRequestType type;
    private int diamondCount;
    private ResearchBuildingVO researchBuilding;

    public ResearchRequestCommand(ResearchBuildingVO researchBuilding, ResearchReq.ResearchRequestType type, int cid, int diamondCount)
    {
        this.cid = cid;

        this.type = type;
        this.diamondCount = diamondCount;

        this.researchBuilding = researchBuilding;
    }
    public override ReqWrapper Execute()
    {
        var model = DataCenter.Instance.FindEntityModelById(cid);
        var nextLevelModel = DataCenter.Instance.FindEntityModelById(model.upgradeId);
        var now = ServerTime.Instance.Now();
        if (type == ResearchReq.ResearchRequestType.Research)
        {
            //升级信息写在了下一等级
            DataCenter.Instance.RemoveResource(nextLevelModel.costResourceType, nextLevelModel.costResourceCount);
            researchBuilding.cid = cid;
            researchBuilding.endTime = DateTimeUtil.DateTimeToUnixTimestampMS(now.AddSeconds(nextLevelModel.buildTime));
        }
        else if (type == ResearchReq.ResearchRequestType.CompleteImmediately || type == ResearchReq.ResearchRequestType.Complete)
        {
            DataCenter.Instance.RemoveResource(ResourceType.Diamond, diamondCount);
            //改变skillshop
            DataCenter.Instance.ChangeSkillShop(cid);
            researchBuilding.cid = 0;
            researchBuilding.endTime = 0;
        }

        return new ReqWrapper()
        {
            requestType = ReqWrapper.RequestType.Research,
            researchReq = new ResearchReq()
            {
                cid = cid,
                currentTime = DateTimeUtil.DateTimeToUnixTimestampMS(now),
                researchRequestType = type,
                diamondCount = diamondCount
            }
        };
    }
}
