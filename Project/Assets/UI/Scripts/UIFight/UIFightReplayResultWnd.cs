using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class UIFightReplayResultWnd : UIBaseWnd
{
    public GameObject attackerFrame;
    public GameObject defenderFrame;
    public UILabel txtPanelName;
    public List<GameObject> stars = new List<GameObject>(3);
    public Transform deadCon;
    public GameObject btnReturnHomeLand;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.isLockScreen = true;
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
    public void UpDateFightReplayResultInfo()
    {
        //防御者
        //defenderFrame.GetComponent<UIFightReplayHeadInfoFrame>().UpdateHeadInfo(2);
        //攻击者
        //attackerFrame.GetComponent<UIFightReplayHeadInfoFrame>().UpdateHeadInfo(1);
        //战斗结果
        BattleResultVO vo = BattleManager.Instance.GetBattleResult();
        txtPanelName.text = "战果:[B4E370]" + vo.percentage + "%[-]";
        for (int i = 0, imax = vo.star; i < imax; i++)
        {
            stars[i].SetActive(true);
        }
        SetDeadSoldier(vo);
    }
    private void SetDeadSoldier(BattleResultVO vo)
    {
        int width = (vo.usedArmies.Count + /*vo.usedSkills.Count*/ -1) * 126 + (vo.usedArmies.Count + /*vo.usedSkills.Count*/ -1) * 15;
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
        //for (int i = 0, imax = vo.usedSkills.Count; i < imax; i++)
        //{
        //    GameObject obj = (GameObject)GameObject.Instantiate(CommonUIPrefabManager.Instance.FightItem, Vector3.zero, Quaternion.identity);
        //    obj.transform.parent = deadCon;
        //    obj.transform.localScale = new Vector3(1, 1, 1);
        //    obj.transform.localPosition = new Vector3(posX, 10);
        //    posX = posX + (obj.GetComponent<UISprite>().width + 15);
        //    obj.SetActive(true);
        //    obj.AddMissingComponent<UIFightSoldierInfo>().ArmyData = new ArmyVO() { cid = vo.usedSkills[i].cid, amount = vo.usedSkills[i].amount };
        //}
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnReturnHomeLand).onClick -= OnClickButton;
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }
        for (int i = 0; i < deadCon.transform.childCount; i++)
        {
            GameObject.Destroy(deadCon.GetChild(i).gameObject);
        }
    }
}
