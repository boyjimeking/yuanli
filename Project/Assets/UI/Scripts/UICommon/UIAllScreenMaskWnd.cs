using UnityEngine;
using System.Collections;

public class UIAllScreenMaskWnd : UIBaseWnd
{
    public UISprite maskSprite;
    protected override void Awake()
    {
        base.Awake();
        this.layer = UIMananger.UILayer.UI_SCREENMASK_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_SCREENMASK);
    }
    protected override void Start()
    {
        base.Start();
        maskSprite.SetAnchor(UIMananger.Instance.uiLayer, 0, 0, 0, 0);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        this.GetComponent<BoxCollider>().size = new Vector3(PanelUtil.GetPanelWidth(), Constants.UI_HEIGHT);
        UIEventListener.Get(this.gameObject).onClick += OnClickMask;
    }

    private void OnClickMask(GameObject go)
    {
        EventDispather.DispatherEvent(GameEvents.CLICK_SCREEN_MASK);
    }
}
