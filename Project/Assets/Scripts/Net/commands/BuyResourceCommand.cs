using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class BuyResourceCommand : NetCommand
{
    private ResourceVO resourceVO;
    public BuyResourceCommand(ResourceVO resourceVO)
    {
        this.resourceVO = resourceVO;
    }
    public override com.pureland.proto.ReqWrapper Execute()
    {
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.BuyResource, buyResourceReq = new BuyResourceReq() { resourceVO = this.resourceVO } };
    }
}
