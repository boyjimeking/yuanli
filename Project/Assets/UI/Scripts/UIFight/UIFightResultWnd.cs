using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class UIFightResultWnd : UIBaseWnd
{
    public List<GameObject> stars = new List<GameObject>(3);
    public UILabel txtPanelName;
    public GameObject btnReturnHomeLand;
    public Transform deadCon;
    public UILabel txtGetXingBi;
    public UILabel txtGetTaiJing;
    public UILabel txtGetJiFen;
    public UILabel txtGetJinPai;
    public UILabel txtRewardXingBi;
    public UILabel txtRewardTaiJing;
    public GameObject otherCon;

    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.IsLockScreen = true;
        this.isCloseWinClickLockScreen = false;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
        this.closeOrHideType = UICloseOrHideType.CLOSE_WORLD_TYPE_HOME;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnReturnHomeLand).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnReturnHomeLand))
        {
            BattleManager.Instance.BackToMyCamp();
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnReturnHomeLand).onClick -= OnClickButton;
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }
        otherCon.SetActive(false);
        for (int i = 0; i < deadCon.transform.childCount; i++)
        {
            GameObject.Destroy(deadCon.GetChild(i).gameObject);
        }
    }
    public void SetFightResultData()
    {
        BattleResultVO vo = BattleManager.Instance.GetBattleResult();
        for (int i = 0, imax = vo.star; i < imax; i++)
        {
            stars[i].SetActive(true);
        }
        txtPanelName.text = "战果:[B4E370]" + vo.percentage + "%[-]";
        txtGetXingBi.text = BattleUIManager.Instance.GetHasAttackResourceCount(ResourceType.Gold, vo.stolenResources).ToString();
        txtGetTaiJing.text = BattleUIManager.Instance.GetHasAttackResourceCount(ResourceType.Oil, vo.stolenResources).ToString();
        txtGetJiFen.text = vo.star > 0 ? vo.rewardCrown.ToString() : (-vo.rewardCrown).ToString();
        //txtGetJinPai.text = vo.rewardMedal.ToString();
        txtRewardXingBi.text = vo.rewardGoldByCrownLevel.ToString();
        txtRewardTaiJing.text = vo.rewardOilByCrownLevel.ToString();
        SetDeadSoldier(vo);
    }

    private void SetDeadSoldier(BattleResultVO vo)
    {
        int width = (vo.usedArmies.Count + vo.usedSkills.Count - 1) * 126 + (vo.usedArmies.Count + vo.usedSkills.Count - 1) * 15;
        float posX = -width * 0.5f;
        for (int i = 0, imax = vo.usedArmies.Count; i < imax; i++)
        {
            GameObject obj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
            obj.transform.parent = deadCon;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(posX, 10);
            posX = posX + (obj.GetComponent<UISprite>().width + 15);
            obj.SetActive(true);
            obj.AddMissingComponent<UIFightSoldierInfo>().ArmyData = vo.usedArmies[i];
        }
        for (int i = 0, imax = vo.usedSkills.Count; i < imax; i++)
        {
            GameObject obj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
            obj.transform.parent = deadCon;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(posX, 10);
            posX = posX + (obj.GetComponent<UISprite>().width + 15);
            obj.SetActive(true);
            obj.AddMissingComponent<UIFightSoldierInfo>().ArmyData = new ArmyVO() { cid = vo.usedSkills[i].cid, amount = vo.usedSkills[i].amount };
        }
        if (vo.useDonatedArmy)
        {
            GameObject obj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
            obj.transform.parent = deadCon;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(posX, 10);
            posX = posX + (obj.GetComponent<UISprite>().width + 15);
            obj.SetActive(true);
            obj.AddMissingComponent<UIFightSoldierInfo>().ArmyData = new ArmyVO() { cid = Constants.DENOTED_ARMY_ID, amount = 1 };
        }
    }
}
