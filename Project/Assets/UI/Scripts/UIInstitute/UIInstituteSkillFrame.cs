using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class UIInstituteSkillFrame : MonoBehaviour
{
    public GameObject btnReturn;
    //全部的兵种展示容器
    public GameObject allSkillCon;
    public Transform skillStoreArea;
    public GameObject prefabSkillItem;
    //技能详细信息
    private GameObject skillDes;
    //正在生产的技能区域
    public GameObject institutingArea;
    //正在生产的技能提示
    public GameObject institutingTip;
    //list
    private List<GameObject> listObj = new List<GameObject>();

    void OnEnable()
    {
        EventDispather.AddEventListener(ItemOperationManager.PRODUCT_OVER_RIGHTNOW, RightNowOverProduct);
        UIEventListener.Get(btnReturn).onClick += OnClickButton;
    }
    /// <summary>
    /// 立即完成
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="obj"></param>
    private void RightNowOverProduct(string eventType, object obj)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        if (DataCenter.Instance.GetResource(ResourceType.Diamond) < GameDataAlgorithm.TimeToGem(module.researchBuildingComponent.TimeLeft))
        {
            //钻石不足
        }
        //升级完成——>Server
        module.researchBuildingComponent.CompleteResearchImmediately();
        //界面清理
        LevelUpComplete();
    }
    /// <summary>
    /// 升级技能界面初始化
    /// </summary>
    public void UpdateSkillFrame()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        module.researchBuildingComponent.EventComplete -= LevelUpComplete;
        module.researchBuildingComponent.EventComplete += LevelUpComplete;
        //预备兵种
        RefreshSkillArea();
        //正在升级的兵种
        institutingArea.SetActive(module.researchBuildingComponent.CurrentResearchId > 0);
        institutingTip.SetActive(module.researchBuildingComponent.CurrentResearchId <= 0);
    }
    /// <summary>
    /// 刷新待选区域兵种
    /// </summary>
    private void RefreshSkillArea()
    {
        foreach (GameObject tempObj in listObj)
        {
            GameObject.Destroy(tempObj);
        }
        listObj.Clear();
        PlayerVO playerVO = DataCenter.Instance.Defender.player;
        int index = 0;
        foreach (int id in playerVO.skillShop)
        {
            GameObject skillItem = (GameObject)GameObject.Instantiate(prefabSkillItem, Vector3.zero, Quaternion.identity);
            skillItem.transform.parent = skillStoreArea;
            skillItem.transform.localScale = new Vector3(1, 1, 1);
            skillItem.transform.localPosition = new Vector3(-347 + index * (skillItem.GetComponent<UISprite>().width + 20), 0, 0);
            skillItem.SetActive(true);
            skillItem.GetComponent<UIDragScrollView>().scrollView = skillStoreArea.GetComponent<UIScrollView>();
            skillItem.GetComponent<UIInstituteItem>().SetItemInfo(id);
            listObj.Add(skillItem);
            index++;
            UIEventListener.Get(skillItem).onClick += OnClickButton;
        }
    }
    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnReturn))
        {
            allSkillCon.SetActive(true);
            btnReturn.SetActive(false);
            if (skillDes)
            {
                skillDes.SetActive(false);
            }
        }
        else
        {
            ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
            int needLevel = ModelUtil.GetEntityModel(go.GetComponent<UIInstituteItem>().itemId).upgradeNeedLevel;
            if (needLevel > module.researchBuildingComponent.Entity.model.level)
            {
                GameTipsManager.Instance.ShowGameTips(EnumTipsID.ItemOper_10204, new string[] { module.researchBuildingComponent.Entity.model.nameForView, needLevel.ToString() });
                return;
            }
            allSkillCon.SetActive(false);
            btnReturn.SetActive(true);
            if (null == skillDes)
            {
                skillDes = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/SkillDes");
                skillDes.transform.parent = this.transform;
                skillDes.transform.localScale = Vector3.one;
                skillDes.transform.localPosition = Vector3.zero;
            }
            skillDes.SetActive(true);
            skillDes.AddMissingComponent<UILogicSkillDes>().SetSoldierInfo(go.GetComponent<UIInstituteItem>().itemId, true);
            skillDes.AddMissingComponent<UILogicSkillDes>().OnClickRightOver += ClickRightOver;
            skillDes.AddMissingComponent<UILogicSkillDes>().OnClickLevelUp += ClickLevelUp;
        }
    }
    public void ClearSkillFrame()
    {
        foreach (GameObject tempObj in listObj)
        {
            GameObject.Destroy(tempObj);
        }
        listObj.Clear();
        if (skillDes)
        {
            GameObject.Destroy(skillDes);
        }
        UIEventListener.Get(btnReturn).onClick -= OnClickButton;
        EventDispather.RemoveEventListener(ItemOperationManager.PRODUCT_OVER_RIGHTNOW, RightNowOverProduct);
    }
    private void ClickRightOver(int itemId)
    {
        EntityModel model = DataCenter.Instance.FindEntityModelById(itemId);
        EntityModel nextModel = null;
        if (model.upgradeId != 0)
        {
            nextModel = DataCenter.Instance.FindEntityModelById(model.upgradeId);
        }
        int diamond = GameDataAlgorithm.TimeToGem(nextModel.buildTime);
        if (DataCenter.Instance.GetResource(ResourceType.Diamond) < diamond)
        {

        }
        else
        {
            //升级完成
            CheckResearchCapacity();
        }
    }
    private void ClickLevelUp(int itemId)
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        if (module.researchBuildingComponent.CurrentResearchId > 0)
        {
            GameTipsManager.Instance.ShowGameTips(EnumTipsID.ItemOper_10203);
            return;
        }
        EntityModel nextModel = ModelUtil.GetNextLevelModel(itemId);
        int hasCount = DataCenter.Instance.GetResource(nextModel.costResourceType);
        if (hasCount >= nextModel.costResourceCount)
        {
            ResearchSkill(true, itemId);
        }
        else
        {
            ShopManager.Instance.BuyResource(nextModel.costResourceType, nextModel.costResourceCount - hasCount, ResearchSkill, itemId);
        }
    }
    private void ResearchSkill(bool isConfirm, object obj)
    {
        if (!isConfirm) return;
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        //可以升级
        module.researchBuildingComponent.Research((int)obj);
        allSkillCon.SetActive(true);
        skillDes.SetActive(false);
        btnReturn.SetActive(false);
        institutingArea.SetActive(module.researchBuildingComponent.CurrentResearchId > 0);
        institutingTip.SetActive(module.researchBuildingComponent.CurrentResearchId <= 0);
        CheckResearchCapacity();
    }
    private void CheckResearchCapacity()
    {
        foreach (GameObject tempObj in listObj)
        {
            tempObj.GetComponent<UIInstituteItem>().UpdateCanLevel();
        }
    }
    private void LevelUpComplete()
    {
        institutingArea.SetActive(false);
        institutingTip.SetActive(true);
        RefreshSkillArea();
        CheckResearchCapacity();
    }
}
