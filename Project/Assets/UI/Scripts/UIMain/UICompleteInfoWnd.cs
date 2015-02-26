using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class UICompleteInfoWnd : UIBaseWnd
{
    public UILabel txtAccount;
    public UILabel txtPwd;
    public UILabel txtConfirmPwd;
    public UILabel txtPhone;
    public UILabel txtMail;
    public GameObject btnComplete;
    //验证码容器
    public GameObject checkCodeCon;
    //重设密码手机
    public GameObject btnResetPhone;
    //重设密码手机文本
    public UILabel txtResetPhone;
    //重设密码邮箱
    public GameObject btnResetMail;
    //重设密码邮箱
    public UILabel txtResetMail;
    //验证码
    public UILabel txtCode;
    //验证按钮
    public GameObject btnCheck;
    //倒计时
    private int displayTime;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnComplete).onClick += OnClickButton;
        UIEventListener.Get(btnResetPhone).onClick += OnClickButton;
        UIEventListener.Get(btnResetMail).onClick += OnClickButton;
        UIEventListener.Get(btnCheck).onClick += OnClickButton;
        if (LoginManager.Instance.account != "")
        {
            txtAccount.text = LoginManager.Instance.account;
        }
        if (LoginManager.Instance.pwd != "")
        {
            txtPwd.text = LoginManager.Instance.pwd;
            txtConfirmPwd.text = LoginManager.Instance.pwd;
        }
        if (LoginManager.Instance.phone != "")
        {
            txtPhone.text = LoginManager.Instance.phone;
        }
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnComplete))
        {
            //判断密码是否一致
            //if (txtPwd.text.Length < 6)
            //{
            //    return;
            //}
            if (txtPwd.text != txtConfirmPwd.text)
            {
                return;
            }
            if (!IsPhoneNumber(txtPhone.text.Trim()))
            {
                GameTipsManager.Instance.ShowGameTips("这是错误的手机号");
                return;
            }
            LoginManager.Instance.SetPlayerCompleteInfo(txtAccount.text, txtPwd.text, txtPhone.text, txtMail.text);
        }
        else if (go.Equals(btnResetPhone))
        {
            checkCodeCon.SetActive(true);
            displayTime = 90;
            this.InvokeRepeating("CountDownTimePhone", 0.1f, 1);
            btnResetPhone.GetComponent<UIButton>().isEnabled = false;
            btnResetMail.GetComponent<UIButton>().isEnabled = false;
        }
        else if (go.Equals(btnResetMail))
        {
            checkCodeCon.SetActive(true);
            displayTime = 90;
            this.InvokeRepeating("CountDownTimeMail", 0.1f, 1);
            btnResetPhone.GetComponent<UIButton>().isEnabled = false;
            btnResetMail.GetComponent<UIButton>().isEnabled = false;
        }
        else if (go.Equals(btnCheck))
        {

        }
    }
    private bool IsPhoneNumber(string phoneNum)
    {
        Regex dianxin = new Regex(@"^1[3578][01379]\d{8}$");
        Regex liantong = new Regex(@"^1[34578][01256]\d{8}$");
        Regex yidong = new Regex(@"^(134[012345678]\d{7}|1[34578][012356789]\d{8})$");
        return dianxin.IsMatch(phoneNum) || liantong.IsMatch(phoneNum) || yidong.IsMatch(phoneNum);
    }
    private void CountDownTimePhone()
    {
        displayTime--;
        if (displayTime <= 0)
        {
            this.CancelInvoke();
            txtResetPhone.text = "重设密码(手机)";
            btnResetPhone.GetComponent<UIButton>().isEnabled = true;
            btnResetMail.GetComponent<UIButton>().isEnabled = true;
        }
        else
        {
            txtResetPhone.text = displayTime + "秒";
        }
    }
    private void CountDownTimeMail()
    {
        displayTime--;
        if (displayTime <= 0)
        {
            this.CancelInvoke();
            txtResetMail.text = "重设密码(邮箱)";
            btnResetPhone.GetComponent<UIButton>().isEnabled = true;
            btnResetMail.GetComponent<UIButton>().isEnabled = true;
        }
        else
        {
            txtResetMail.text = displayTime + "秒";
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnComplete).onClick -= OnClickButton;
        UIEventListener.Get(btnResetPhone).onClick -= OnClickButton;
        UIEventListener.Get(btnResetMail).onClick -= OnClickButton;
        UIEventListener.Get(btnCheck).onClick -= OnClickButton;
        this.CancelInvoke();
        txtResetPhone.text = "重设密码(手机)";
        txtResetMail.text = "重设密码(邮箱)";
        checkCodeCon.SetActive(false);
    }
}
