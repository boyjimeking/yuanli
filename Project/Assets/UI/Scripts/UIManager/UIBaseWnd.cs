using UnityEngine;
using System.Collections;
using AnimationOrTween;

public class UIBaseWnd : MonoBehaviour
{
    //关闭时是否彻底删除
    protected bool isDestroy = true;
    //是否有关闭按钮
    protected bool hasClose = false;
    //关闭按钮
    public GameObject btnClose;
    //二进制数据（面板关闭或者是打开的类型）
    protected int closeOrHideType;
    //是否锁屏
    protected bool isLockScreen = false;
    //点击锁屏是否关闭面板
    protected bool isCloseWinClickLockScreen = true;
    //面板动画
    public Animation panelAnimation;
    //面板所属的层
    protected UIMananger.UILayer layer;
    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {
    }
    protected virtual bool InitWin()
    {
        if (null == btnClose)
        {
            if (hasClose)
            {
                if (panelAnimation)
                {
                    btnClose = this.transform.Find("PanelAnimation/ButtonClose").gameObject;
                }
                else
                {
                    btnClose = this.transform.Find("ButtonClose").gameObject;
                }
            }
            return false;
        }
        return true;
    }
    protected virtual void OnEnable()
    {
        InitWin();
        if (btnClose)
        {
            UIEventListener.Get(btnClose).onClick += CloseWin;
        }
        if (isLockScreen)
        {
            UIMananger.Instance.ShowWin("PLG_ScreenMask", "UIAllScreenMaskPanel");
            EventDispather.AddEventListener(GameEvents.CLICK_SCREEN_MASK, OnClickScreenMask);
        }
        if (panelAnimation)
        {
            ActiveAnimation.Play(panelAnimation, null, Direction.Forward, EnableCondition.IgnoreDisabledState, DisableCondition.DoNotDisable);
        }
    }
    protected virtual void OnDisable()
    {
        if (btnClose)
        {
            //移除按钮监听事件
            UIEventListener.Get(btnClose).onClick -= CloseWin;
        }
        if (isLockScreen)
        {
            UIMananger.Instance.CloseWin("UIAllScreenMaskPanel");
            EventDispather.RemoveEventListener(GameEvents.CLICK_SCREEN_MASK, OnClickScreenMask);
        }
    }
    protected virtual void CloseWin(GameObject obj = null)
    {
        if (panelAnimation)
        {
            ActiveAnimation activeAnimation = ActiveAnimation.Play(panelAnimation, null, Direction.Reverse, EnableCondition.IgnoreDisabledState, DisableCondition.DoNotDisable);
            EventDelegate eventDelegate = new EventDelegate(ReallyCloseWin);
            eventDelegate.oneShot = true;
            activeAnimation.onFinished.Add(eventDelegate);
        }
        else
        {
            ReallyCloseWin();
        }
    }
    protected virtual void ReallyCloseWin()
    {
        UIMananger.Instance.CloseWin(this.gameObject.name);
    }
    public bool IsDestroy
    {
        get
        {
            return isDestroy;
        }
    }
    public int CloseOrHideType
    {
        set
        {
            this.closeOrHideType |= value;
        }
        get
        {
            return this.closeOrHideType;
        }
    }
    public bool CheckCloseOrHideType(int type)
    {
        return 0 != (this.closeOrHideType & type);
    }
    public bool IsLockScreen
    {
        set
        {
            this.isLockScreen = value;
        }
        get
        {
            return this.isLockScreen;
        }
    }

    private void OnClickScreenMask(string eventType, object obj)
    {
        if (isCloseWinClickLockScreen)
            CloseWin();
    }
    public UIMananger.UILayer Layer
    {
        get
        {
            return this.layer;
        }
    }
    protected virtual void OnDestroy()
    {

    }
}
