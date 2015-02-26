using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMessageWnd : UIBaseWnd
{
    //左边三个按钮的Toogle数组
    public List<UIToggle> toggleList = new List<UIToggle>(2);
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.IsLockScreen = true;
        this.closeOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_REPLAY;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
    }
    protected override bool InitWin()
    {
        if (!base.InitWin())
        {
            return false;
        }
        return true;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (UIToggle toggle in toggleList)
        {
            //EventDelegate.Add(toggle.onChange, OnToggleValueChange);
        }
        //请求信息
        GameManager.Instance.RequestBattleHistory();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (UIToggle toggle in toggleList)
        {
            //EventDelegate.Remove(toggle.onChange, OnToggleValueChange);
        }
    }
    /// <summary>
    /// 按照标签显示对应内容
    /// </summary>
    /// <param name="index"></param>
    public void showWinByType(int index)
    {
        UIToggle toggle = toggleList[index];
        toggle.value = true;
    }
    //private void OnToggleValueChange()
    //{
    //    UIToggle toggle = UIToggle.current;
    //    if (!toggle.value) return;
    //    int index = toggleList.IndexOf(toggle, 0);
    //    if (-1 != index)
    //    {
    //        mailFrame.ShowContentByIndex(index);
    //    }
    //}
}
