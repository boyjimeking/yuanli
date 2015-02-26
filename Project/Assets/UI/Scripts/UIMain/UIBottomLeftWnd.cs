using UnityEngine;
using System.Collections;

public class UIBottomLeftWnd : UIBaseWnd
{
    //跳跃按钮
    public GameObject btnTiaoYue;
    //活动按钮
    public GameObject btnActivity;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_BATTLE;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_REPLAY;
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnTiaoYue).onClick += OnClickButton;
        UIEventListener.Get(btnActivity).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnTiaoYue))
        {
            //判断有没有兵
            if (DataCenter.Instance.Defender.armies.Count <= 0)
            {
                GameTipsManager.Instance.ShowGameTips(EnumTipsID.Fight_10304);
                return;
            }
            GameObject searchWin = UIMananger.Instance.ShowWin("PLG_FightSearch", "UISearchPanel");
            searchWin.transform.localPosition = Vector3.zero;
            UIPanel panel = searchWin.GetComponent<UIPanel>();
            panel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
            PanelUtil.SetPanelAnchors(panel, UIMananger.Instance.uiLayer.transform, new Vector4(0, 1, 0, 1), new Vector4(0, 0, 0, 0));
            searchWin.SetActive(true);
        }
        else if (go.Equals(btnActivity))
        {
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnActivity).onClick -= OnClickButton;
        UIEventListener.Get(btnTiaoYue).onClick -= OnClickButton;
    }
}
