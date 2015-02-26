using com.pureland.proto;
using UnityEngine;
using System.Collections;

public class AuthLoginCommand : LoginCommand
{
    private string account = "";
    private string pwd = "";
    public AuthLoginCommand(string account, string pwd)
    {
        this.account = account;
        this.pwd = pwd;
    }
    public override ReqWrapper Execute()
    {
        var authLoginReq = new AuthLoginReq();
        authLoginReq.machineId = GameManager.Instance.MachineId;
        authLoginReq.account = account;
        authLoginReq.passwd = pwd;
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.AuthLogin, authLoginReq = authLoginReq };
    }
    public override void OnResponse(BaseResp resp)
    {
        if (resp.errorType == BaseResp.ErrorType.AuthFailed)
        {
            GameTipsManager.Instance.ShowGameTips("认证登陆失败");
            return;
        }
        LoginManager.Instance.LoginSimpleVOs = resp.respWrapper.playerListResp.loginSimpleVOs;
    }
}
