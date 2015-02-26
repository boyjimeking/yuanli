using UnityEngine;
using System.Collections;

public class UILogin : MonoBehaviour
{
    //账号
    public UILabel txtAccount;
    //密码
    public UILabel txtPwd;
    //开始游戏按钮
    public GameObject btnStartGame;
    //向手机发信息
    public GameObject btnPhone;
    //向邮件发信息
    public GameObject btnMail;
    void OnEnable()
    {
        UIEventListener.Get(btnStartGame).onClick += OnClickButton;
        UIEventListener.Get(btnPhone).onClick += OnClickButton;
        UIEventListener.Get(btnMail).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnStartGame))
        {
            if (txtAccount.text == "" || txtPwd.text == "")
                return;
            //向服务器发送登陆请求
            LoginManager.Instance.RequestAuthLogin(txtAccount.text, txtPwd.GetComponent<UIInput>().value);
            //LoginManager.Instance.ShowSelectPlayer();
        }
        else if (go.Equals(btnMail))
        {

        }
        else if (go.Equals(btnPhone))
        {

        }
    }
    void OnDisable()
    {
        UIEventListener.Get(btnStartGame).onClick -= OnClickButton;
        UIEventListener.Get(btnPhone).onClick -= OnClickButton;
        UIEventListener.Get(btnMail).onClick -= OnClickButton;
    }
}
