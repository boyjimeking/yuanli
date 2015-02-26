
using com.pureland.proto;

public class TrapRefillCommand : NetCommand
{
    private TrapRefillReq.RefillType type;
    private BuildingVO building;

    public TrapRefillCommand(TrapRefillReq.RefillType type, BuildingVO building=null)
    {
        this.type = type;
        this.building = building;
    }
    public override ReqWrapper Execute()
    {
        if (type == TrapRefillReq.RefillType.All)
        {
            foreach (var buildingVo in DataCenter.Instance.Defender.buildings)
            {
                if (buildingVo.trapBuildingVO != null && buildingVo.trapBuildingVO.broken)
                {
                    var model = DataCenter.Instance.FindEntityModelById(buildingVo.cid);
                    DataCenter.Instance.RemoveResource(ResourceType.Gold,model.refillCostResourceCount);
                    buildingVo.trapBuildingVO.broken = false;
                }
            }
        }
        else if(type == TrapRefillReq.RefillType.Single)
        {
            var model = DataCenter.Instance.FindEntityModelById(building.cid);
            DataCenter.Instance.RemoveResource(ResourceType.Gold, model.refillCostResourceCount);
            building.trapBuildingVO.broken = false;
        }
        EventDispather.DispatherEvent(GameEvents.TRAP_REFILL,building);
        return new ReqWrapper()
        {
            requestType = ReqWrapper.RequestType.TrapRefill,
            trapRefillReq = new TrapRefillReq()
            {
                refillType = type,
                sid = building == null ? 0 : building.sid,
            }
        };
    }
}
