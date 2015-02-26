
using System.Collections.Generic;
using com.pureland.proto;

public class ProductionRequestCommand : NetCommand
{
    private ProductionReq.ProductionRequestType requestType;
    private ProductionItemVO productionItem;
    private int diamondCount;
    private BuildingVO building;

    public ProductionRequestCommand(ProductionReq.ProductionRequestType requestType,BuildingVO building, ProductionItemVO productionItem,int diamondCount=0)
    {
        this.requestType = requestType;
        this.productionItem = productionItem;
        this.diamondCount = diamondCount;
        this.building = building;
    }
    public override ReqWrapper Execute()
    {
        if (diamondCount > 0)
        {
            DataCenter.Instance.RemoveResource(ResourceType.Diamond,diamondCount);
        }

        var productionReq = new ProductionReq();

        productionReq.sid = building.sid;
        productionReq.productionRequestType = requestType;
        productionReq.productionItemVO = productionItem;
        productionReq.time = building.productionBuildingVO.endTime;
        productionReq.diamondCount = this.diamondCount;

        return new ReqWrapper() {productionReq = productionReq, requestType = ReqWrapper.RequestType.Production};
    }
}
