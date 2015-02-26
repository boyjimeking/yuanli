
using System.Collections.Generic;
using com.pureland.proto;

public class MoveBuildingCommand : NetCommand
{
    private List<BuildingVO> buildings; 
    public MoveBuildingCommand(List<BuildingVO> buildings)
    {
        this.buildings = buildings;
    }
    public override ReqWrapper Execute()
    {
        var moveBuildingReq = new MoveBuildingReq();
        moveBuildingReq.building.AddRange(buildings);
        return new ReqWrapper()
        {
            requestType = ReqWrapper.RequestType.MoveBuilding,
            moveBuildingReq = moveBuildingReq
        };
    }
}
