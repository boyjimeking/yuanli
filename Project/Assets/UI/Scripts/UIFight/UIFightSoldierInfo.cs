using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UIFightSoldierInfo : MonoBehaviour
{
    //数据信息
    private ArmyVO armyData;
    //无兵时闪烁
    public TweenColor tweenColor;
    //是否还有剩余的兵
    private bool hasLeftSolider = true;
    //控件脚本
    private UIControlFightItem fightItem;
    void OnEnable()
    {
        fightItem = this.transform.GetComponent<UIControlFightItem>();
        tweenColor = fightItem.iconSelect.GetComponent<TweenColor>();
    }
    public ArmyVO ArmyData
    {
        set
        {
            if (!hasLeftSolider)
            {
                StartTween();
                if (!BattleUIManager.Instance.HasLeftArmy())
                    GameTipsManager.Instance.ShowGameTips(EnumTipsID.Fight_10302);
                else
                    GameTipsManager.Instance.ShowGameTips(EnumTipsID.Fight_10303);
                return;
            }
            armyData = value;
            if (armyData.amount <= 0)
            {
                fightItem.txtItemCount.text = "X0";
                fightItem.iconItem.color = new Color(0, 1, 1, 1);
                hasLeftSolider = false;
            }
            else
            {
                fightItem.txtItemCount.text = "X" + armyData.amount;
            }
            if (armyData.cid == Constants.DENOTED_ARMY_ID)
            {
                fightItem.levelCon.SetActive(false);
            }
            else
            {
                EntityModel model = DataCenter.Instance.FindEntityModelById(armyData.cid);
                fightItem.txtItemLevel.text = model.level.ToString();
                fightItem.iconItem.spriteName = ResourceUtil.GetItemIconByModel(model);
            }
        }
        get
        {
            return this.armyData;
        }
    }
    public void SetSelect(bool isSelect)
    {
        fightItem.iconSelect.SetActive(isSelect);
    }
    public void StartTween()
    {
        EventDelegate eventDelete = new EventDelegate(this, "OnTweenFinish");
        eventDelete.oneShot = true;
        tweenColor.onFinished.Add(eventDelete);
        tweenColor.PlayForward();
    }
    public void OnTweenFinish()
    {
        tweenColor.PlayReverse();
    }
}
