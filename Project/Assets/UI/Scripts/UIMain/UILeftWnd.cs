using UnityEngine;
using System.Collections;

public class UILeftWnd : UIBaseWnd
{
    //排行榜
    public GameObject btnRank;
    //信息
    public GameObject btnMessage;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_BATTLE;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_REPLAY;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnRank).onClick += OnClickButton;
        UIEventListener.Get(btnMessage).onClick += OnClickButton;
    }
    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnRank))
        {
        }
        else if (go.Equals(btnMessage))
        {
            GameObject message = UIMananger.Instance.ShowWin("PLG_Message", "UIMessagePanel");
            message.transform.localPosition = Vector3.zero;
            message.SetActive(true);
            message.GetComponent<UIMessageWnd>().showWinByType(1);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnRank).onClick -= OnClickButton;
        UIEventListener.Get(btnMessage).onClick -= OnClickButton;
    }
}
