using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIFightInfoWnd : UIBaseWnd
{
    public UISprite playerIcon;
    public UILabel txtPlayerLevel;
    public UILabel txtPlayerName;
    public UILabel txtXingBi;
    public UILabel txtTaiJing;
    public UILabel txtJiFenSuc;
    public UILabel txtJiFenFail;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.closeOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_HOME;
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        EventDispather.AddEventListener(GameEvents.STOLEN_RESOURCE, UpdateFightInfo);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventDispather.RemoveEventListener(GameEvents.STOLEN_RESOURCE, UpdateFightInfo);
    }
    public void UpdateFightInfo(string eventType, object data)
    {
        if (null != data)
        {
            if ((data as ResourceVO).resourceType == ResourceType.Gold)
            {
                txtXingBi.text = BattleUIManager.Instance.GetCanAttackResourceCount(ResourceType.Gold).ToString();
            }
            else if ((data as ResourceVO).resourceType == ResourceType.Oil)
            {
                txtTaiJing.text = BattleUIManager.Instance.GetCanAttackResourceCount(ResourceType.Oil).ToString();
            }
            return;
        }
        PlayerVO defenceVO = DataCenter.Instance.Defender.player;
        txtPlayerLevel.text = defenceVO.level.ToString();
        txtPlayerName.text = defenceVO.name;
        txtXingBi.text = BattleUIManager.Instance.GetCanAttackResourceCount(ResourceType.Gold).ToString();
        txtTaiJing.text = BattleUIManager.Instance.GetCanAttackResourceCount(ResourceType.Oil).ToString();
        txtJiFenSuc.text = BattleManager.Instance.GetBattleRewardCrown(true, 3).ToString();
        txtJiFenFail.text = "-" + BattleManager.Instance.GetBattleRewardCrown(false, 3);
    }
}
