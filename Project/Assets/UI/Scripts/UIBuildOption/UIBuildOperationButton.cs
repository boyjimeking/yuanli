using UnityEngine;
using System.Collections;
using System;
using com.pureland.proto;

public class UIBuildOperationButton : MonoBehaviour
{
    //信息
    private TileEntity entity;
    //点击按钮触发反应
    public event Action<bool> ClickButton;
    //按钮文本
    public UILabel btnText;
    //按钮上的消耗文本
    public UILabel txtConsume;
    //消耗资源GameObject
    public GameObject consumeObj;
    //按钮图标
    public UISprite iconButton;
    private bool isUpdate = false;

    void OnEnable()
    {
        UIEventListener.Get(this.gameObject).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        bool isExcute = true;
        if (this.gameObject.name == OperationButtonUtil.COLLECTRESOURCE)
        {
            if (entity.GetComponent<GatherResourceBuildingComponent>().CalculateResourceFromLastGather(ServerTime.Instance.Now()) < 6)
                return;
            isExcute = false;
        }
        if (isExcute && ClickButton != null)
        {
            ClickButton(true);
        }
        BuildOptManager.Instance.ExcuteOperationByName(this.gameObject.name, entity);
    }
    void OnDisable()
    {
        UIEventListener.Get(this.gameObject).onClick -= OnClickButton;
        ClickButton = null;
        entity = null;
        isUpdate = false;
    }
    public TileEntity Entity
    {
        set
        {
            this.entity = value;
            ExcuteByEntityType();
        }
    }

    private void ExcuteByEntityType()
    {
        if (EntityTypeUtil.IsGatherResourceBuilding(entity.model) && this.gameObject.name == OperationButtonUtil.COLLECTRESOURCE)
        {
            isUpdate = true;
            iconButton.spriteName = entity.model.resourceType.ToString();
            RefreshGatherResource();
        }
        if (this.gameObject.name == OperationButtonUtil.BUILDLEVELUP)
        {
            if (this.entity.buildingVO.buildingStatus == BuildingVO.BuildingStatus.Upgrade)
            {
                btnText.text = "取消";
                consumeObj.SetActive(false);
                iconButton.spriteName = "UI_Mbutton_quxiao";
            }
            else if (this.entity.buildingVO.buildingStatus == BuildingVO.BuildingStatus.On)
            {
                iconButton.spriteName = "UI_Mbutton_uplevel";
                btnText.text = "升级";
                consumeObj.SetActive(true);
                consumeObj.GetComponent<UIControlResourceIcon>().txtResourceCount.text = DataCenter.Instance.FindEntityModelById(entity.model.upgradeId).costResourceCount.ToString();
                consumeObj.GetComponent<UIControlResourceIcon>().iconResource.spriteName = DataCenter.Instance.FindEntityModelById(entity.model.upgradeId).costResourceType.ToString();
            }
        }
        else if (this.gameObject.name == OperationButtonUtil.COMPLETERIGHTNOW)
        {
            txtConsume.text = GameDataAlgorithm.TimeToGem(entity.GetComponent<ConstructBuildingComponent>().TimeLeft).ToString();
        }
        else if (this.gameObject.name == OperationButtonUtil.RESETALLXIANJING)
        {
            ResourceVO allXianJingVO = BuildOptManager.Instance.GetRefillCostAll();
            consumeObj.GetComponent<UIControlResourceIcon>().txtResourceCount.text = allXianJingVO.resourceCount.ToString();
            consumeObj.GetComponent<UIControlResourceIcon>().iconResource.spriteName = allXianJingVO.resourceType.ToString();
        }
        else if (this.gameObject.name == OperationButtonUtil.RESETXIANJING)
        {
            ResourceVO xianJingVO = entity.GetComponent<TrapComponent>().RefillCost;
            consumeObj.GetComponent<UIControlResourceIcon>().txtResourceCount.text = xianJingVO.resourceCount.ToString();
            consumeObj.GetComponent<UIControlResourceIcon>().iconResource.spriteName = xianJingVO.resourceType.ToString();
        }
    }
    void Update()
    {
        if (!isUpdate) return;
        ExcuteByEntityType();
    }
    /// <summary>
    /// 刷新收集资源
    /// </summary>
    private void RefreshGatherResource()
    {
        if (entity.GetComponent<GatherResourceBuildingComponent>().CalculateResourceFromLastGather(ServerTime.Instance.Now()) > 6)
        {
            PanelUtil.SetUIRectColor(null, Color.white, false, this.gameObject, 1);
        }
        else
        {
            PanelUtil.SetUIRectColor(null, PanelUtil.greyColor, false, this.gameObject, 0.1f);
        }
    }
}
