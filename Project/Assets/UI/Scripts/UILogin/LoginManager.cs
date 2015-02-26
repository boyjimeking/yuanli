using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class LoginManager : Singleton<LoginManager>
{
    //职业
    public int professionId = -1;
    //名字
    public string playerName = "";
    //账号
    public string account = "";
    //密码
    public string pwd = "";
    //手机号
    public string phone = "";
    //邮箱
    //public string mail = "";
    //玩家列表信息
    List<PlayerLoginSimpleVO> loginSimpleVOs;
    private GameObject ShowStartWin()
    {
        GameObject startWin = UIMananger.Instance.ShowWin("PLG_Login", "UIStartPanel");
        startWin.transform.localPosition = Vector3.zero;
        UIPanel panel = startWin.GetComponent<UIPanel>();
        panel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
        PanelUtil.SetPanelAnchors(panel, UIMananger.Instance.uiLayer.transform, new Vector4(0, 1, 0, 1), new Vector4(0, 0, 0, 0));
        return startWin;
    }
    /// <summary>
    /// 账号密码界面
    /// </summary>
    public void ShowLoginWin()
    {
        GameObject startWin = ShowStartWin();
        startWin.GetComponent<UIStartWnd>().ShowWinByType(1);
    }
    /// <summary>
    /// 创建角色-选择种族
    /// </summary>
    public void ShowLoginMainFrame()
    {
        GameObject startWin = ShowStartWin();
        startWin.GetComponent<UIStartWnd>().ShowWinByType(2);
    }
    /// <summary>
    /// 选择头像
    /// </summary>
    public void ShowCreateFace()
    {
        GameObject startWin = ShowStartWin();
        startWin.GetComponent<UIStartWnd>().ShowWinByType(3);
    }
    public void ShowProfessionDes(bool isShow)
    {
        return;
        GameObject startWin = ShowStartWin();
        startWin.GetComponent<UIStartWnd>().ShowProfessionDes(isShow);
    }
    /// <summary>
    /// 设置账号信息
    /// </summary>
    /// <param name="account"></param>
    /// <param name="pwd"></param>
    /// <param name="phone"></param>
    /// <param name="mail"></param>
    public void SetPlayerCompleteInfo(string account, string pwd, string phone, string mail)
    {
        this.account = account;
        this.pwd = pwd;
        this.phone = phone;
        //this.mail = mail;
        new CompleteUserInfoCommand(account, pwd, phone).ExecuteAndSend();

    }
    public void RequestAuthLogin(string account, string pwd)
    {
        new AuthLoginCommand(account, pwd).ExecuteAndSend();
    }
    /// <summary>
    /// 请求已有玩家列表
    /// </summary>
    public void RequestPlayerList()
    {
        new PlayerListCommand().ExecuteAndSend();
    }
    public void RequestChangeAccount()
    {
        new ChangeAccountCommand().ExecuteAndSend();
    }
    /// <summary>
    /// 请求进入游戏
    /// </summary>
    public void RequestEnterGame()
    {
        new NoAuthLoginCommand(professionId, "").ExecuteAndSend();
    }
    /// <summary>
    /// 请求创建角色并进入游戏
    /// </summary>
    public void RequestCreateRole()
    {
        new NoAuthLoginCommand(professionId, playerName).ExecuteAndSend();
    }
    /// <summary>
    /// 设置已有角色列表信息
    /// </summary>
    public List<PlayerLoginSimpleVO> LoginSimpleVOs
    {
        set
        {
            this.loginSimpleVOs = value;
            LoginManager.Instance.ShowLoginMainFrame();
        }
        get
        {
            return this.loginSimpleVOs;
        }
    }
    /// <summary>
    /// 如果玩家在战斗轮询查询
    /// </summary>
    public void RequestEnterGameReCycle()
    {
        DelayManager.Instance.AddDelayCall(RequestEnterGame, 10.0f);
    }
}
