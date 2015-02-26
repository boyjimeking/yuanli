using UnityEngine;
using System.Collections;
using System;

public class UIInstitutingArea : MonoBehaviour
{
    //升级内容名字
    public UILabel txtSoldierName;
    //升级总时间
    public UILabel txtTotalTime;
    //立即完成按钮
    public GameObject btnRightOver;
    //立即完成花费
    public UILabel txtRightOverConsume;
    //图标Icon
    public UISprite iconItem;
    void OnEnable()
    {
        UpdateInstitutingArea();
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

    private void UpdateInstitutingArea()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        EntityModel model = DataCenter.Instance.FindEntityModelById(module.researchBuildingComponent.CurrentResearchId);
        EntityModel nextModel = ModelUtil.GetNextLevelModel(model.upgradeId);
        txtSoldierName.text = nextModel.nameForView;
        OnTimer();
        this.InvokeRepeating("OnTimer", 0.1f, 1);
    }
    private void OnTimer()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        TimeSpan tt = new TimeSpan(0, 0, module.researchBuildingComponent.TimeLeft);
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
        txtRightOverConsume.text = GameDataAlgorithm.TimeToGem(module.researchBuildingComponent.TimeLeft).ToString();
    }
}
