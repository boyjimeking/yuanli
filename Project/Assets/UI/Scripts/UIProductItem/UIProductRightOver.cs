using UnityEngine;
using System.Collections;
using System;
using com.pureland.proto;

public class UIProductRightOver : MonoBehaviour
{
    public UILabel txtTotalTime;
    public UILabel txtConsume;
    public GameObject btnRightOver;
    void OnEnable()
    {
        PanelUtil.SetUIRectColor(null, Color.white, false, btnRightOver);
        this.InvokeRepeating("UpdateTime", 0.1f, 1);
        UIEventListener.Get(btnRightOver).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        EventDispather.DispatherEvent(ItemOperationManager.PRODUCT_OVER_RIGHTNOW);
    }
    void OnDisable()
    {
        this.CancelInvoke();
        UIEventListener.Get(btnRightOver).onClick -= OnClickButton;
    }
    private void UpdateTime()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        if (module.currentProductFactory.TimeLeft <= 0 && DataCenter.Instance.SpaceUsed >= DataCenter.Instance.TotalSpace)
        {
            this.gameObject.SetActive(false);
            return;
        }
        TimeSpan tt = new TimeSpan(0, 0, module.currentProductFactory.TotalTimeLeft);
        string textstr = "";
        if (tt.Days > 0)
            textstr += tt.Days + "天 " + tt.Hours + "时 ";
        else if (tt.Hours > 0)
            textstr += tt.Hours + "时 " + tt.Minutes + "分 ";
        else if (tt.Minutes > 0)
            textstr += tt.Minutes + "分 " + tt.Seconds + "秒 ";
        else if (tt.Seconds > 0)
            textstr += tt.Seconds + "秒 ";
        txtTotalTime.text = textstr;
        //判断钱
        int count = GameDataAlgorithm.TimeToGem(module.currentProductFactory.TotalTimeLeft);
        if (count > DataCenter.Instance.GetResource(ResourceType.Diamond))
        {
            txtConsume.text = "[FF0000]" + count + "[-]";
        }
        else
        {
            txtConsume.text = GameDataAlgorithm.TimeToGem(module.currentProductFactory.TotalTimeLeft).ToString();
        }
    }
    public bool SoldierSpaceFull
    {
        set
        {
            PanelUtil.SetUIRectColor(null, value ? PanelUtil.greyColor : Color.white, false, btnRightOver);
        }
    }
}
