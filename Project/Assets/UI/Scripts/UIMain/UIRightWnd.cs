using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIRightWnd : UIBaseWnd
{
    public GameObject btnWorker;
    public UILabel txtWorker;
    public GameObject btnHuDun;
    public UILabel txtHuDun;
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
        UIEventListener.Get(btnWorker).onClick += OnClickButton;
        UIEventListener.Get(btnHuDun).onClick += OnClickButton;
        EventDispather.AddEventListener(GameEvents.WORKER_CHANGE, UpDateRightPanelInfo);
        UpDateRightPanelInfo(null, null);
    }

    private void UpDateRightPanelInfo(string eventType, object obj)
    {
        PlayerVO playerVO = DataCenter.Instance.Defender.player;
        if (null != playerVO)
        {
            txtWorker.text = playerVO.freeWorker + "/" + playerVO.maxWorker;
        }
    }

    private void OnClickButton(GameObject go)
    {
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnWorker).onClick -= OnClickButton;
        UIEventListener.Get(btnHuDun).onClick -= OnClickButton;
        EventDispather.RemoveEventListener(GameEvents.WORKER_CHANGE, UpDateRightPanelInfo);
    }
}
