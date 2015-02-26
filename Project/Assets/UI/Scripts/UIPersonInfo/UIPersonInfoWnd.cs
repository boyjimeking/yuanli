using UnityEngine;
using System.Collections;

public class UIPersonInfoWnd : UIBaseWnd
{
    public GameObject btnSet;
    public GameObject btnVisitHome;
    public GameObject btnClan;
    public UISprite playerIcon;
    public UILabel txtPlayerLevel;
    public UILabel txtPlayerName;
    public GameObject clanCon;
    public UILabel txtClanName;
    public UISprite iconRank;
    public UILabel txtRank;
    public UISprite iconJunXian;
    public UILabel txtJunXian;
    public UILabel txtJiFen;
    public GameObject soldierCon;
    public GameObject itemCon;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnSet).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnSet))
        {
            UIMananger.Instance.CloseWin(this.gameObject.name);
            GameObject gameSet = UIMananger.Instance.ShowWin("PLG_GameSet", "UIGameSetPanel");
            gameSet.transform.localPosition = Vector3.zero;
            gameSet.SetActive(true);
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnSet).onClick -= OnClickButton;
    }
    public void SetPlayerData()
    {
        //        soldierCon.GetComponent<UIPlayerSoldierCon>().SetSoldierData(DataCenter.Instance.Defender.player.armyShop);TODO
    }
}
