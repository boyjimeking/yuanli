using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UISearchWnd : UIBaseWnd
{
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.isCloseWinClickLockScreen = false;
        this.isLockScreen = true;
        this.closeOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_BATTLE;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
}
