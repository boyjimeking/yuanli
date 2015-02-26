using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIFightReplayDefenderWnd : UIBaseWnd
{
    public UISprite playerIcon;
    public UILabel txtPlayerLevel;
    public UILabel txtPlayerName;
    public UILabel txtAllianceName;
    public UILabel txtXingBi;
    public UILabel txtTaiJing;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.closeOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_HOME;
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        EventDispather.AddEventListener(GameEvents.STOLEN_RESOURCE, UpdateDefenderInfo);
    }
    public void UpdateDefenderInfo(string eventType, object data)
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
    }
}
