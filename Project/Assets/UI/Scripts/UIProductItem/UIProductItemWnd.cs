using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;
using System;

public class UIProductItemWnd : UIBaseWnd
{
    //面板名字
    public UILabel txtPanelName;
    //返回按钮
    public GameObject btnReturn;
    //左边按钮
    public GameObject btnLeft;
    //右边按钮
    public GameObject btnRight;
    //生产士兵界面
    public GameObject productSoldierFrame;
    //生产技能界面
    public GameObject productSkillFrame;
    //当前训练营ID
    private int productFactoryId = 0;
    //士兵描述信息面板
    public GameObject soldierDes;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.isLockScreen = true;
        this.closeOrHideType = UICloseOrHideType.CLOSE_DIAMOND_TIP;
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
    }

    /// <summary>
    /// 返回到士兵训练界面
    /// </summary>
    /// <param name="go"></param>
    private void OnClickButton(GameObject go)
    {
        btnReturn.SetActive(false);
        btnLeft.SetActive(true);
        btnRight.SetActive(true);
        productSoldierFrame.SetActive(true);
        soldierDes.SetActive(false);
    }

    public void OnTweenFinish(GameObject go)
    {
        //soldierCon.transform.localPosition = new Vector3(0, -60, 0);
        //UITweener[] tweens = soldierCon.GetComponents<UITweener>();
        //foreach (UITweener tween in tweens)
        //{
        //    tween.tweenFactor = 0;
        //}
        ////移除事件
        //ModuleProductItem module = (ModuleProductItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        //module.currentProductFactory.EventComplete -= RemoveProductByProductItemVO;
        ////刷新面板显示信息
        //if (go.Equals(btnLeft))
        //{
        //    ProductFactoryId = 0;
        //}
        //else if (go.Equals(btnRight))
        //{
        //    ProductFactoryId = 1;
        //}
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        productSoldierFrame.GetComponent<UIProductSoldierFrame>().Clear();
        productSkillFrame.GetComponent<UIProductSkillFrame>().Clear();
        productSkillFrame.SetActive(false);
        productSoldierFrame.SetActive(false);
        BuildOptManager.Instance.ShowBuildingOptWin(module.currentProductFactory.Entity);
        ItemOperationManager.Instance.ClearModuleOperateItem();
    }
    /// <summary>
    /// 设置要显示的训练营
    /// </summary>
    public int ProductFactoryId
    {
        set
        {
            this.productFactoryId = value;
            EntityModel model = DataCenter.Instance.FindEntityModelById(value);
            if (EntityTypeUtil.IsArmyShop(model))
            {
                productSkillFrame.SetActive(false);
                productSoldierFrame.SetActive(true);
                productSoldierFrame.GetComponent<UIProductSoldierFrame>().txtPanelName = txtPanelName;
                productSoldierFrame.GetComponent<UIProductSoldierFrame>().ShowProductItemFrame();
            }
            else if (EntityTypeUtil.IsSkillShop(model))
            {
                productSoldierFrame.SetActive(false);
                productSkillFrame.SetActive(true);
                productSkillFrame.GetComponent<UIProductSkillFrame>().txtPanelName = txtPanelName;
                productSkillFrame.GetComponent<UIProductSkillFrame>().ShowProductItemFrame();
            }
        }
        get
        {
            return this.productFactoryId;
        }
    }
    /// <summary>
    /// 显示士兵描述信息
    /// </summary>
    /// <param name="obj"></param>
    public void ShowSoldierDesFrame(GameObject obj)
    {
        btnReturn.SetActive(true);
        btnLeft.SetActive(false);
        btnRight.SetActive(false);
        productSoldierFrame.SetActive(false);
        soldierDes.SetActive(true);
        UIProductItemInfo info = obj.GetComponent<UIProductItemInfo>();
    }
    protected override void Clear()
    {
        base.Clear();
        productSoldierFrame.GetComponent<UIProductSoldierFrame>().Clear();
        productSkillFrame.GetComponent<UIProductSkillFrame>().Clear();
    }
}
