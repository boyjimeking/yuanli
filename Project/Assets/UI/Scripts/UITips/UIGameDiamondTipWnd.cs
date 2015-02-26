using UnityEngine;
using System.Collections;

public class UIGameDiamondTipWnd : UIBaseWnd
{
    //回调函数
    private GameTipsManager.CallBack callBackMethod;
    //回调参数
    private ArrayList callbackParam;
    //提示标题
    public UILabel txtTitle;
    //提示内容
    public UILabel txtTipContent;
    //按钮
    public GameObject btnConfirm;
    //按钮文本
    public UILabel btnText;
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
    }

    public void SetTipInfo(int tipsId, string content, ArrayList addParam, GameTipsManager.CallBack callBack, ArrayList callbackParam)
    {
        TipModel tipModel = DataCenter.Instance.FindTipModelById(tipsId);
        if (null == addParam)
        {
            return;
        }
        txtTitle.text = tipModel.title.Replace("{0}", (string)addParam[0]);
        txtTipContent.text = content;
        btnText.text = tipModel.btnText1.Replace("{0}", (string)addParam[1]);
        this.callBackMethod = callBack;
        this.callbackParam = callbackParam;
    }
    private void OnClickButton(GameObject go)
    {
        if (null != callBackMethod)
        {
            callBackMethod(true, this.callbackParam);
        }
        base.CloseWin();
    }
    protected override void CloseWin(GameObject obj = null)
    {
        if (null != callBackMethod)
        {
            callBackMethod(false, this.callbackParam);
        }
        base.CloseWin(obj);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnConfirm).onClick -= OnClickButton;
        callBackMethod = null;
    }
}
