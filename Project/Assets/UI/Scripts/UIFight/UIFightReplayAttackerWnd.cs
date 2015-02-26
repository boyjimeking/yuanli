using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIFightReplayAttackerWnd : UIBaseWnd
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
        EventDispather.AddEventListener(GameEvents.STOLEN_RESOURCE, UpdateAttackerInfo);
    }
    public void UpdateAttackerInfo(string eventType, object data)
    {
        if (null != data)
        {
            if ((data as ResourceVO).resourceType == ResourceType.Gold)
            {
                txtXingBi.text = BattleUIManager.Instance.GetHasAttackResourceCount(ResourceType.Gold, BattleManager.Instance.stolenResources).ToString();
            }
            else if ((data as ResourceVO).resourceType == ResourceType.Oil)
            {
                txtTaiJing.text = BattleUIManager.Instance.GetHasAttackResourceCount(ResourceType.Oil, BattleManager.Instance.stolenResources).ToString();
            }
            return;
        }
        PlayerVO attackVO = DataCenter.Instance.Attacker.player;
        txtPlayerLevel.text = attackVO.level.ToString();
        txtPlayerName.text = attackVO.name;
        txtXingBi.text = BattleUIManager.Instance.GetHasAttackResourceCount(ResourceType.Gold, BattleManager.Instance.stolenResources).ToString();
        txtTaiJing.text = BattleUIManager.Instance.GetHasAttackResourceCount(ResourceType.Oil, BattleManager.Instance.stolenResources).ToString();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventDispather.RemoveEventListener(GameEvents.STOLEN_RESOURCE, UpdateAttackerInfo);
    }
}
