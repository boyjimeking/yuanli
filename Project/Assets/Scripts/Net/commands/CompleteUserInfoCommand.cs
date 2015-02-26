using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class CompleteUserInfoCommand : NetCommand
{
    private string account;
    private string pwd;
    private string phone;
    public CompleteUserInfoCommand(string account, string pwd, string phone)
    {
        this.account = account;
        this.pwd = pwd;
        this.phone = phone;
    }
    public override com.pureland.proto.ReqWrapper Execute()
    {
        CompleteInfoReq completeInfoReq = new CompleteInfoReq();
        completeInfoReq.account = this.account;
        completeInfoReq.passwd = pwd;
        completeInfoReq.phoneNum = phone;
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.CompleteInfo, completeInfoReq = completeInfoReq };
    }
    public override void OnResponse(BaseResp resp)
    {
        UIMananger.Instance.CloseWin("UICompleteInfoPanel");
    }
}
