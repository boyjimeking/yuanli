using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class UIFightReplayWnd : UIBaseWnd
{
    public GameObject btnReturnHome;
    public List<GameObject> stars = new List<GameObject>(3);
    public Transform soldierCon;
    public GameObject progressCon;
    private Dictionary<int, GameObject> dicSoldier = new Dictionary<int, GameObject>();
    public UILabel txtProcess;
    public UILabel txtPlayRate;
    public GameObject btnPlayRate;
    public UIToggle btnPlay;
    private int curPlayRate = 1;
    //当前几颗星
    private int curStar = -1;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
    }
    protected override void Start()
    {
        base.Start();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnReturnHome).onClick += OnClickButton;
        UIEventListener.Get(btnPlayRate).onClick += OnClickButton;
        EventDelegate.Add(btnPlay.onChange, OnToggleValueChange);
        EventDispather.AddEventListener(GameEvents.BATTLE_SPAWN, UpdateOneArmyById);
        EventDispather.AddEventListener(GameEvents.BATTLE_PROGRESS_CHANGE, UpdateFightProgress);
    }
    private void OnToggleValueChange()
    {
        UIToggle toggle = UIToggle.current;
        GameRunner.Instance.Paused = !toggle.value;
    }
    /// <summary>
    /// 更新战斗进度
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="obj"></param>
    private void UpdateFightProgress(string eventType, object obj)
    {
        string progress = (BattleManager.Instance.DestroyBuildingPercent * 100).ToString();
        int index = progress.LastIndexOf(".");
        txtProcess.text = progress.Substring(0, index + 3) + "%";
        for (int i = 0; i < BattleManager.Instance.BattleStar; i++)
        {
            stars[i].SetActive(true);
        }
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnReturnHome))
        {
            BattleManager.Instance.BackToMyCamp();
        }
        else if (go.Equals(btnPlayRate))
        {
            btnPlay.value = true;
            SetPlayRate();
        }
    }
    /// <summary>
    /// 设置播放速度
    /// </summary>
    private void SetPlayRate()
    {
        curPlayRate *= 2;
        if (curPlayRate > 8)
            curPlayRate = 1;
        txtPlayRate.text = "X" + curPlayRate;
        GameRunner.Instance.GameSpeed = curPlayRate;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnReturnHome).onClick -= OnClickButton;
        UIEventListener.Get(btnPlayRate).onClick -= OnClickButton;
        EventDispather.RemoveEventListener(GameEvents.BATTLE_SPAWN, UpdateOneArmyById);
        EventDispather.RemoveEventListener(GameEvents.BATTLE_PROGRESS_CHANGE, UpdateFightProgress);
        while (soldierCon.childCount > 0)
        {
            GameObject.DestroyImmediate(soldierCon.GetChild(0).gameObject);
        }
        dicSoldier.Clear();
        //恢复到原来显示
        curStar = -1;
        progressCon.SetActive(false);
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }
        btnPlay.value = true;
        curPlayRate = 1;
        txtPlayRate.text = "X" + 1;
        GameRunner.Instance.GameSpeed = curPlayRate;
    }

    public void SetFightData()
    {
        txtProcess.text = "0%";
        UpdateArmies();
    }
    /// <summary>
    /// 刷新军队数据
    /// </summary>
    private void UpdateArmies()
    {
        if (dicSoldier.Count > 0)
            return;
        List<ArmyVO> listArmy = DataCenter.Instance.Attacker.armies;
        GameObject obj = null;
        int posX = 0;
        for (int i = 0, imax = listArmy.Count; i < imax; i++)
        {
            obj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
            obj.transform.parent = soldierCon;
            obj.transform.localScale = new Vector3(1, 1, 1);
            posX = -417 + i * (obj.GetComponent<UISprite>().width + 15);
            obj.transform.localPosition = new Vector3(posX, 0);
            obj.SetActive(true);
            obj.GetComponent<UIDragScrollView>().scrollView = soldierCon.GetComponent<UIScrollView>();
            obj.AddMissingComponent<UIFightSoldierInfo>().ArmyData = listArmy[i];
            dicSoldier.Add(listArmy[i].cid, obj);
        }
        List<SkillVO> listSkill = DataCenter.Instance.Attacker.skills;
        for (int i = 0, imax = listSkill.Count; i < imax; i++)
        {
            obj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
            obj.transform.parent = soldierCon;
            obj.transform.localScale = new Vector3(1, 1, 1);
            posX = posX + (obj.GetComponent<UISprite>().width + 15);
            obj.transform.localPosition = new Vector3(posX, 0);
            obj.SetActive(true);
            obj.GetComponent<UIDragScrollView>().scrollView = soldierCon.GetComponent<UIScrollView>();
            obj.AddMissingComponent<UIFightSoldierInfo>().ArmyData = new ArmyVO() { cid = listSkill[i].cid, amount = listSkill[i].amount };
            dicSoldier.Add(listSkill[i].cid, obj);
        }
        if (0 < DataCenter.Instance.Attacker.donatedArmies.Count)
        {
            obj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
            obj.transform.parent = soldierCon;
            obj.transform.localScale = new Vector3(1, 1, 1);
            posX = posX + (obj.GetComponent<UISprite>().width + 15);
            obj.transform.localPosition = new Vector3(posX, 0);
            obj.SetActive(true);
            obj.GetComponent<UIDragScrollView>().scrollView = soldierCon.GetComponent<UIScrollView>();
            obj.AddMissingComponent<UIFightSoldierInfo>().ArmyData = new ArmyVO() { cid = Constants.DENOTED_ARMY_ID, amount = 1 };
            dicSoldier.Add(Constants.DENOTED_ARMY_ID, obj);
        }
    }
    /// <summary>
    /// 派出士兵时更新士兵数据
    /// </summary>
    /// <param name="armyId"></param>
    private void UpdateOneArmyById(string eventType, object obj)
    {
        if (!progressCon.activeSelf)
            progressCon.SetActive(true);
        int armyId = (int)obj;
        if (!dicSoldier.ContainsKey(armyId))
        {
            Debug.Log("当前数据错误，没有找到派出的士兵");
            return;
        }
        ArmyVO vo = BattleUIManager.Instance.GetArmyVOById(armyId);
        if (null == vo)
        {
            Debug.Log("没有找到进攻方该Id的数据，检查是否正确");
            return;
        }
        GameObject armyObject = dicSoldier[armyId];
        armyObject.GetComponent<UIFightSoldierInfo>().ArmyData = vo;
        BattleUIManager.Instance.hasUseArmy = true;
    }
}
