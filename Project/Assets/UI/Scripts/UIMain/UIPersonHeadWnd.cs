using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIPersonHeadWnd : UIBaseWnd
{
    //玩家名字
    public UILabel txtPlayerName;
    //玩家头像容器
    public GameObject playerIconCon;
    //玩家头像
    public UISprite playerIcon;
    //玩家等级
    public UILabel txtPlayerLevel;
    //经验条
    public UISlider progressExp;
    //经验显示数字
    public UILabel txtExp;
    //排名
    public GameObject btnRank;
    public UILabel txtRank;
    //成就
    public GameObject btnHonour;
    public UILabel txtHonour;
    //积分
    public UILabel txtJiFen;
    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_BATTLE;
        this.CloseOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_REPLAY;
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override bool InitWin()
    {
        return base.InitWin();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnHonour).onClick += OnClickButton;
        UIEventListener.Get(btnRank).onClick += OnClickButton;
        UIEventListener.Get(playerIconCon).onClick += OnClickButton;
        EventDispather.AddEventListener(GameEvents.LEVEL_UP, UpdatePersonInfo);
        EventDispather.AddEventListener(GameEvents.EXP_CHANGE, UpdatePersonInfo);
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnRank))
        {

        }
        else if (go.Equals(btnHonour))
        {
            GameObject honorWin = UIMananger.Instance.ShowWin("PLG_Honor", "UIHonorPanel");
            honorWin.transform.localPosition = Vector3.zero;
        }
        else if (go.Equals(playerIconCon))
        {
            GameObject personInfo = UIMananger.Instance.ShowWin("PLG_PersonInfo", "UIPersonInfoPanel");
            personInfo.transform.localPosition = Vector3.zero;
            personInfo.GetComponent<UIPersonInfoWnd>().SetPlayerData();
        }
    }

    public void UpdatePersonInfo(string eventType, object obj)
    {
        PlayerVO playerVO = DataCenter.Instance.Defender.player;
        if (null != playerVO)
        {
            txtPlayerName.text = playerVO.name;
            txtPlayerLevel.text = playerVO.level.ToString();
            txtExp.text = playerVO.experience.ToString();
            RankModel nextLevel;
            RankModel curLevel;
            DataCenter.Instance.FindRankModel(RankType.Level, playerVO.experience, out curLevel, out nextLevel);
            if (nextLevel != null)
                progressExp.value = playerVO.experience * 1.0f / nextLevel.exp;
            else
            {
                progressExp.value = 1.0f;
            }
            txtRank.text = "";
            txtHonour.text = "";
            txtJiFen.text = playerVO.crown.ToString();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnHonour).onClick -= OnClickButton;
        UIEventListener.Get(btnRank).onClick -= OnClickButton;
        EventDispather.RemoveEventListener(GameEvents.LEVEL_UP, UpdatePersonInfo);
        EventDispather.RemoveEventListener(GameEvents.EXP_CHANGE, UpdatePersonInfo);
    }
}
