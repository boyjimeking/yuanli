using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIFightReplayHeadInfoFrame : MonoBehaviour
{
    public UISprite playerIcon;
    public UILabel txtPlayerLevel;
    public UILabel txtPlayerName;
    public UILabel txtAllianceName;
    public UILabel txtXingBi;
    public UILabel txtTaiJing;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="actorType">1、攻击者2、防御者</param>
    public void UpdateHeadInfo(int actorType)
    {
        PlayerVO playerVO = null;
        if (1 == actorType)
        {
            playerVO = DataCenter.Instance.Attacker.player;
            txtXingBi.text = BattleUIManager.Instance.GetHasAttackResourceCount(ResourceType.Gold, BattleManager.Instance.stolenResources).ToString();
            txtTaiJing.text = BattleUIManager.Instance.GetHasAttackResourceCount(ResourceType.Oil, BattleManager.Instance.stolenResources).ToString();
        }
        else if (2 == actorType)
        {
            playerVO = DataCenter.Instance.Defender.player;
            txtXingBi.text = BattleUIManager.Instance.GetCanAttackResourceCount(ResourceType.Gold).ToString();
            txtTaiJing.text = BattleUIManager.Instance.GetCanAttackResourceCount(ResourceType.Oil).ToString();
        }
        txtPlayerName.text = playerVO.name;
        txtPlayerLevel.text = playerVO.level.ToString();
    }
}
