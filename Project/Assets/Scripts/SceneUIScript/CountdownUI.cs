using UnityEngine;
using System.Collections;
using System;

public class CountdownUI : BaseSceneUI
{
    //总的时间
    private long totalTime = 100;
    //当前剩余时间
    private long leftTime = 100;
    //时间文本
    public UILabel txtTime;
    //时间条
    public UISlider progressTime;
    //文本字符串
    private string textstr = "";
    //完成函数
    public event Action<bool> OnCompleteEvent;
    void Start()
    {
        //this.InvokeRepeating("OnTimerBack", 0, 1);
    }
    public long TotalTime
    {
        set
        {
            this.totalTime = value;
        }
    }
    public long LeftTime
    {
        set
        {
            if (value <= 0)
                leftTime = 0;
            leftTime = value;
            if (leftTime > 0)
                this.InvokeRepeating("OnTimerBack", 0, 1);
        }
        get
        {
            return this.leftTime;
        }
    }
    private void OnTimerBack()
    {
        leftTime--;
        if (leftTime <= 0)
        {
            this.CancelInvoke("OnTimerBack");
            OnCompleteEvent(true);
            return;
        }
        TimeSpan tt = new TimeSpan(leftTime * 10000000);
        txtTime.text = DateTimeUtil.PrettyFormatTimeSpan(tt);
        progressTime.value = leftTime * 1.0f / totalTime;
    }

}
