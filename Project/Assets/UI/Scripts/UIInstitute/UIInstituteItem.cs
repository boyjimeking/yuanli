using UnityEngine;
using System.Collections;

public class UIInstituteItem : MonoBehaviour
{
    private GameObject item;
    private UIControlFightItem itemControl;
    public UILabel txtCount;
    public UISprite iconMoney;
    public UILabel txtSoldierCount;
    //士兵的id
    [HideInInspector]
    public int itemId = -1;
    void Awake()
    {
        //创建兵的展示
        item = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
        item.transform.parent = this.transform;
        item.transform.localPosition = new Vector3(0, 20, 0);
        item.transform.localScale = Vector3.one;
        Destroy(item.GetComponent<BoxCollider>());
        item.SetActive(true);
        itemControl = item.GetComponent<UIControlFightItem>();
    }
    /// <summary>
    /// 设置兵种的信息
    /// </summary>
    /// <param name="itemId"></param>
    public void SetItemInfo(int itemId)
    {

        this.itemId = itemId;
        EntityModel model = DataCenter.Instance.FindEntityModelById(itemId);
        itemControl.iconItem.spriteName = ResourceUtil.GetItemIconByModel(model);
        EntityModel nextModel = ModelUtil.GetNextLevelModel(itemId);
        if (EntityTypeUtil.IsSkill(model))
        {
            txtSoldierCount.gameObject.SetActive(false);
            txtCount.gameObject.SetActive(true);
            if (null != nextModel)
            {
                txtCount.text = nextModel.costResourceCount.ToString();
                iconMoney.spriteName = nextModel.costResourceType.ToString();
                iconMoney.gameObject.SetActive(true);
            }
            else
            {
                iconMoney.gameObject.SetActive(false);
                txtCount.text = "MAX";
            }
        }
        else if (EntityTypeUtil.IsAnyActor(model.entityType))
        {
            txtSoldierCount.gameObject.SetActive(true);
            txtCount.gameObject.SetActive(false);
            iconMoney.gameObject.SetActive(false);
            if (null != nextModel)
            {
                txtSoldierCount.text = "兵力:\n" + ItemOperationManager.Instance.GetXiaoHaoSoldierCount(itemId) + "/" + nextModel.costResourceCount;
            }
            else
            {
                txtSoldierCount.text = "MAX";
            }
        }
        itemControl.txtItemCount.gameObject.SetActive(false);
        itemControl.txtItemLevel.text = model.level.ToString();
        UpdateCanLevel();
    }
    /// <summary>
    /// 判断是否可以升级
    /// </summary>
    public void UpdateCanLevel()
    {
        EntityModel model = DataCenter.Instance.FindEntityModelById(itemId);
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        EntityModel maxBuildingEntityModel = null;
        if (EntityTypeUtil.IsSkill(model))
        {
            txtSoldierCount.gameObject.SetActive(false);
            txtCount.gameObject.SetActive(true);
            iconMoney.gameObject.SetActive(true);
            maxBuildingEntityModel = ItemOperationManager.Instance.GetMaxLevelProductSkillFactoryModel();
        }
        else if (EntityTypeUtil.IsAnyActor(model.entityType))
        {
            txtSoldierCount.gameObject.SetActive(true);
            txtCount.gameObject.SetActive(false);
            iconMoney.gameObject.SetActive(false);
            maxBuildingEntityModel = ItemOperationManager.Instance.GetMaxLevelProductSoldierFactoryModel();
        }
        itemControl.txtItemCount.gameObject.SetActive(false);
        itemControl.txtItemLevel.text = model.level.ToString();
        //兵种或者是技能是否开放
        if (model.buildNeedLevel > maxBuildingEntityModel.level)
        {
            txtCount.gameObject.SetActive(false);
            txtSoldierCount.gameObject.SetActive(false);
            iconMoney.gameObject.SetActive(false);
            PanelUtil.SetUIRectColor(null, PanelUtil.greyColor, false, item);
            this.transform.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            this.transform.GetComponent<BoxCollider>().enabled = true;
            //兵种是否达到升级条件
            if (model.upgradeNeedLevel > module.researchBuildingComponent.Entity.model.level)
            {
                PanelUtil.SetUIRectColor(null, PanelUtil.greyColor, false, item);
                itemControl.txtTip.gameObject.SetActive(true);
                itemControl.txtTip.text = "[FF0000]需要" + module.researchBuildingComponent.Entity.model.nameForView + model.upgradeNeedLevel + "级[-]";
            }
            else if (EntityTypeUtil.IsSkill(model) && module.researchBuildingComponent.CurrentResearchId > 0)
            {
                //当前是否有正在升级的技能
                PanelUtil.SetUIRectColor(null, PanelUtil.greyColor, false, item);
            }
            else
            {
                PanelUtil.SetUIRectColor(null, Color.white, false, item);
            }
        }
    }
    public void SetSoldierCount()
    {
        txtSoldierCount.text = "兵力:\n" + ItemOperationManager.Instance.GetXiaoHaoSoldierCount(itemId) + "/" + ModelUtil.GetNextLevelModel(itemId).costResourceCount;
    }
}
