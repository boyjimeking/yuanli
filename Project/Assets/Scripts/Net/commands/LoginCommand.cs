
using com.pureland.proto;

public abstract class LoginCommand : NetCommand
{
    public override void OnResponse(BaseResp resp)
    {
        if (resp.errorType > 0)
        {
            GameTipsManager.Instance.ShowGameTips("登陆失败");
            return;
        }
        ServerTime.Instance.SetTimestamp(resp.respWrapper.loginResp.currentTime);
        DataCenter.Instance.authToken = resp.respWrapper.loginResp.authToken;
        DataCenter.Instance.myUserId = resp.respWrapper.loginResp.userId;
        DataCenter.Instance.nextItemSid = resp.respWrapper.loginResp.nextItemSid;
        if (DataCenter.Instance.playerLocalDataVO == null)
        {
            DataCenter.Instance.playerLocalDataVO = new PlayerLocalDataVO();
            DataCenter.Instance.playerLocalDataVO.machineId = GameManager.Instance.MachineId;
            DataCenter.Instance.SavePlayerLocalData();
        }
        //关掉登陆窗体
        UIMananger.Instance.CloseWin("UIStartPanel");
        GameManager.Instance.RequestHomeLandData(DataCenter.Instance.myUserId, WorldType.Home);
    }
}
