using UnityEngine;
using System.Collections;

public class UIGameAlertTipWnd : UIBaseWnd
{

    //回调函数
    private GameTipsManager.CallBack callBackMethod;
    //回调参数
    private ArrayList callbackParam;
    //提示标题
    public UILabel txtTitle;
    //提示内容
    public UILabel txtTipContent;
    //确定按钮
    public GameObject btnConfirm;
    //取消按钮
    public GameObject btnCancel;
    //按钮文本1
    public UILabel btnTextConfirm;
    //按钮文本2
    public UILabel btnTextCancel;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.isLockScreen = true;
        this.layer = UIMananger.UILayer.UI_TIPS_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_TIP);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnConfirm).onClick += OnClickButton;
        UIEventListener.Get(btnCancel).onClick += OnClickButton;
    }

    public void SetTipInfo(int tipsId, string content, ArrayList addParam, GameTipsManager.CallBack callBack, ArrayList callbackParam)
    {
        TipModel tipModel = DataCenter.Instance.FindTipModelById(tipsId);
        if (null == tipModel)
        {
            return;
        }
        if (null != addParam)
        {
            txtTitle.text = tipModel.title.Replace("{0}", (string)addParam[0]);
        }
        else
        {
            txtTitle.text = tipModel.title;
        }
        txtTipContent.text = content;
        btnTextConfirm.text = tipModel.btnText2;
        btnTextCancel.text = tipModel.btnText1;
        this.callBackMethod = callBack;
        this.callbackParam = callbackParam;
    }
    private void OnClickButton(GameObject go)
    {
        if (null != callBackMethod)
        {
            callBackMethod(go.Equals(btnConfirm), this.callbackParam);
        }
        base.CloseWin();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnConfirm).onClick -= OnClickButton;
        UIEventListener.Get(btnCancel).onClick -= OnClickButton;
        callBackMethod = null;
    }
}
