using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class PlayerListCommand : LoginCommand
{
    public override com.pureland.proto.ReqWrapper Execute()
    {
        PlayerListReq playerListReq = new PlayerListReq();
        return new ReqWrapper() { requestType = ReqWrapper.RequestType.PlayerList, playerListReq = playerListReq };
    }
    public override void OnResponse(BaseResp resp)
    {
        if (resp.errorType > 0)
        {
            GameTipsManager.Instance.ShowGameTips("请求角色列表失败");
            return;
        }
        LoginManager.Instance.LoginSimpleVOs = resp.respWrapper.playerListResp.loginSimpleVOs;
    }
}
