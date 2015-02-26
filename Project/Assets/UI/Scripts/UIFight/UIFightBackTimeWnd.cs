using UnityEngine;
using System.Collections;

public class UIFightBackTimeWnd : UIBaseWnd
{
    public UILabel txtTime;
    private int backTime = 30;
    //1.战斗开始2.战斗倒计时
    private int timeType = 0;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    private void StarBackTime()
    {
        if (BattleUIManager.BACK_TIME_FIGHT_PREPARE == timeType)
            txtTime.text = "战斗开始倒计时\n" + DateTimeUtil.PrettyFormatTimeSeconds(backTime);
        else if (BattleUIManager.BACK_TIME_FIGHT_OVER == timeType)
            txtTime.text = "战斗结束倒计时\n" + DateTimeUtil.PrettyFormatTimeSeconds(backTime);
        backTime--;
        if (backTime <= 0)
        {
            if (BattleUIManager.BACK_TIME_FIGHT_PREPARE == timeType || BattleUIManager.BACK_TIME_FIGHT_OVER == timeType)
            {
                this.CancelInvoke();
                BattleUIManager.Instance.BackTimeOver(timeType);
            }
        }
    }
    public void SetTimeAndType(int type, int backTime)
    {
        this.CancelInvoke();
        timeType = type;
        this.backTime = backTime;
        StarBackTime();
        this.InvokeRepeating("StarBackTime", 0, 1);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        this.CancelInvoke();
    }
}
