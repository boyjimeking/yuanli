using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class ChargeDiamondCommand : NetCommand
{
    private int diamondCount;
    public ChargeDiamondCommand(int count)
    {
        this.diamondCount = count;
    }
    public override com.pureland.proto.ReqWrapper Execute()
    {
        ChargeDiamondReq chargeDiamondReq = new ChargeDiamondReq();
        chargeDiamondReq.diamondCount = this.diamondCount;
        DataCenter.Instance.AddResource(new ResourceVO() { resourceType = ResourceType.Diamond, resourceCount = this.diamondCount });
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.ChargeDiamond, chargeDiamondReq = chargeDiamondReq };
    }
}
