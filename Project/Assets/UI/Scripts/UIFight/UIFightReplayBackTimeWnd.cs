using UnityEngine;
using System.Collections;

public class UIFightReplayBackTimeWnd : UIBaseWnd
{
    public UILabel txtTime;
    private int backTime = 30;
    protected override void Awake()
    {
        base.Awake();
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
    }
    /// <summary>
    /// 设置回放时间
    /// </summary>
    public void SetFightReplayTime()
    {
        if (null == GameRecord.GetRecordData())
            return;
        backTime = GameRecord.GetRecordData().battleDuration / Constants.LOGIC_FPS;
        this.InvokeRepeating("StarBackTime", 0, 1);
    }
    private void StarBackTime()
    {
        txtTime.text = "离回放结束还有\n" + DateTimeUtil.PrettyFormatTimeSeconds(backTime);
        backTime--;
        if (backTime <= 0)
        {
            this.CancelInvoke();
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        this.CancelInvoke();
    }
}
