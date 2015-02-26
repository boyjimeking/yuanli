using UnityEngine;
using System.Collections;

public class UIGameSetWnd : UIBaseWnd
{
    //完善信息
    public GameObject btnCompleteInfo;
    //切换角色
    public GameObject btnChangePlayer;
    //切换账号
    public GameObject btnChangeAccount;
    //制作团队
    public GameObject btnDevelopTeam;
    //官网
    public GameObject btnOfficalWeb;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnCompleteInfo).onClick += OnClickButton;
        UIEventListener.Get(btnChangePlayer).onClick += OnClickButton;
        UIEventListener.Get(btnChangeAccount).onClick += OnClickButton;
        UIEventListener.Get(btnDevelopTeam).onClick += OnClickButton;
        UIEventListener.Get(btnOfficalWeb).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnCompleteInfo))
        {
            GameObject complete = UIMananger.Instance.ShowWin("PLG_CompleteInfo", "UICompleteInfoPanel");
            complete.transform.localPosition = Vector3.zero;
            complete.SetActive(true);
        }
        else if (go.Equals(btnChangePlayer))
        {
            //请求玩家列表
            LoginManager.Instance.RequestPlayerList();
        }
        else if (go.Equals(btnChangeAccount))
        {
            //切换新账号，显示选择账号界面
            LoginManager.Instance.RequestChangeAccount();
        }
        else if (go.Equals(btnDevelopTeam))
        {

        }
        else if (go.Equals(btnOfficalWeb))
        {
            Application.OpenURL("www.baidu.com");
        }
        base.CloseWin();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnCompleteInfo).onClick -= OnClickButton;
        UIEventListener.Get(btnChangePlayer).onClick -= OnClickButton;
        UIEventListener.Get(btnChangeAccount).onClick -= OnClickButton;
        UIEventListener.Get(btnDevelopTeam).onClick -= OnClickButton;
        UIEventListener.Get(btnOfficalWeb).onClick -= OnClickButton;
    }
}
