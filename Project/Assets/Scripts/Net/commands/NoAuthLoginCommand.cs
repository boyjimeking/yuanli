using com.pureland.proto;
using UnityEngine;
using System.Collections;

public class NoAuthLoginCommand : LoginCommand
{
    private int raceType = 0;
    private string userName = "";
    public NoAuthLoginCommand(int raceType, string userName)
    {
        this.raceType = raceType;
        this.userName = userName;
    }
    public override ReqWrapper Execute()
    {
        var noAuthLoginReq = new NoAuthLoginReq();
        noAuthLoginReq.machineId = GameManager.Instance.MachineId;
        if (raceType != 0)
            noAuthLoginReq.raceType = raceType;//TODO
        if (userName != "")
            noAuthLoginReq.userName = this.userName;
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.NoAuthLogin, noAuthLoginReq = noAuthLoginReq };
    }
}
