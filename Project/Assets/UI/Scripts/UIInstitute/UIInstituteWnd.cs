using UnityEngine;
using System.Collections;

public class UIInstituteWnd : UIBaseWnd
{
    //面板名字
    public UILabel txtPanelName;
    public GameObject instituteSoldierFrame;
    public GameObject instituteSkillFrame;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.isLockScreen = true;
        this.closeOrHideType = UICloseOrHideType.CLOSE_DIAMOND_TIP;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        instituteSoldierFrame.GetComponent<UIInstituteSoldierFrame>().ClearSoldierFrame();
        instituteSkillFrame.GetComponent<UIInstituteSkillFrame>().ClearSkillFrame();
    }
    public void ShowFrame(bool isSkill)
    {
        if (isSkill)
        {
            instituteSoldierFrame.SetActive(false);
            instituteSkillFrame.SetActive(true);
            instituteSkillFrame.GetComponent<UIInstituteSkillFrame>().UpdateSkillFrame();
        }
        else
        {
            instituteSoldierFrame.SetActive(true);
            instituteSkillFrame.SetActive(false);
            instituteSoldierFrame.GetComponent<UIInstituteSoldierFrame>().UpdateSoldierFrame();
        }
    }
}
