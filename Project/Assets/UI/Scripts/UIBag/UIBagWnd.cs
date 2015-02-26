using UnityEngine;
using System.Collections;

public class UIBagWnd : UIBaseWnd
{
    public GameObject btnReturn;
    public GameObject btnResetHomeLand;
    public GameObject btnSaveHomeLand;
    protected override void Awake()
    {
        base.Awake();
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnReturn).onClick += OnClickButton;
        UIEventListener.Get(btnResetHomeLand).onClick += OnClickButton;
        UIEventListener.Get(btnSaveHomeLand).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnReturn))
        {
            UIMananger.Instance.CloseWin(this.name);
        }
        else if (go.Equals(btnResetHomeLand))
        {

        }
        else if (go.Equals(btnSaveHomeLand))
        {

        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnReturn).onClick -= OnClickButton;
        UIEventListener.Get(btnResetHomeLand).onClick -= OnClickButton;
        UIEventListener.Get(btnSaveHomeLand).onClick -= OnClickButton;
    }
}
