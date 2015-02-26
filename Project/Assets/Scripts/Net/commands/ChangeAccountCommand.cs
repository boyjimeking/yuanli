using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class ChangeAccountCommand : NetCommand
{

    public override com.pureland.proto.ReqWrapper Execute()
    {
        ChangeAccountReq changeAccountReq = new ChangeAccountReq();
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.ChangeAccount, changeAccountReq = changeAccountReq };
    }
    public override void OnResponse(BaseResp resp)
    {
        if (resp.errorType > 0)
        {
            GameTipsManager.Instance.ShowGameTips("切换账号失败");
        }
        LoginManager.Instance.ShowLoginWin();
    }
}
