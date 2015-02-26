using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class UIFightWnd : UIBaseWnd
{
    public GameObject btnStartFight;
    public GameObject btnCancelFight;
    public GameObject btnSearch;
    public List<GameObject> stars = new List<GameObject>(3);
    public UILabel txtSearchConsume;
    public Transform soldierCon;
    public GameObject progressCon;
    private Dictionary<int, GameObject> dicSoldier = new Dictionary<int, GameObject>();
    private GameObject currentSelect;
    public GameObject searchCon;
    public UILabel txtProcess;
    //当前几颗星
    private int curStar = -1;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
        this.layer = UIMananger.UILayer.UI_FIXED_LAYER;
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnCancelFight).onClick += OnClickButton;
        UIEventListener.Get(btnSearch).onClick += OnClickButton;
        UIEventListener.Get(btnStartFight).onClick += OnClickButton;
        EventDispather.AddEventListener(BattleUIManager.BACK_TIME_FIGHT_START, OnBackTimeOver);
        EventDispather.AddEventListener(GameEvents.BATTLE_SPAWN, UpdateOneArmyById);
        EventDispather.AddEventListener(GameEvents.BATTLE_PROGRESS_CHANGE, UpdateFightProgress);
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
        if (1 == BattleManager.Instance.BattleStar && !stars[0].activeSelf)
        {
            PlayStarTween(1, stars[0]);
            curStar = 1;
        }
        else if (2 == BattleManager.Instance.BattleStar && !stars[1].activeSelf)
        {
            if (curStar < 1)
            {
                PlayStarTween(1, stars[0]);
                curStar = 1;
            }
            if (curStar < 2)
            {
                PlayStarTween(2, stars[1]);
                curStar = 2;
            }
        }
        else if (3 == BattleManager.Instance.BattleStar && !stars[2].activeSelf)
        {
            if (curStar < 1)
            {
                PlayStarTween(1, stars[0]);
                curStar = 1;
            }
            if (curStar < 2)
            {
                PlayStarTween(2, stars[1]);
                curStar = 2;
            }
            if (curStar < 3)
            {
                PlayStarTween(3, stars[2]);
                curStar = 3;
            }
        }
    }
    /// <summary>
    /// 飘星星
    /// </summary>
    /// <param name="starNumber"></param>
    /// <param name="original"></param>
    private void PlayStarTween(int starNumber, GameObject original)
    {
        if (original.activeSelf) return;
        //克隆对象
        GameObject tempObj = (GameObject)GameObject.Instantiate(original, Vector3.zero, Quaternion.identity);
        tempObj.transform.parent = UIMananger.Instance.uiLayer.transform;
        tempObj.transform.localScale = new Vector3(2, 2, 2);
        tempObj.SetLayerRecursively(LayerMask.NameToLayer("UILayerFront"));
        //位置动画
        TweenPosition position = tempObj.AddComponent<TweenPosition>();
        position.from = new Vector3(0, 100, 0);
        Vector3 tempVec = original.transform.localPosition;
        Transform transform = original.transform.parent;
        while (!transform.Equals(UIMananger.Instance.uiLayer.transform))
        {
            tempVec = tempVec + transform.localPosition;
            transform = transform.parent;
        }
        position.to = tempVec;
        EventDelegate eventDelegate = new EventDelegate(this, "PlayStarTweenOver");
        eventDelegate.parameters[0] = new EventDelegate.Parameter(original);
        eventDelegate.parameters[1] = new EventDelegate.Parameter(tempObj);
        eventDelegate.oneShot = true;
        position.onFinished.Add(eventDelegate);
        //缩放动画
        TweenScale scale = tempObj.AddComponent<TweenScale>();
        scale.from = new Vector3(2, 2, 2);
        scale.to = new Vector3(1, 1, 1);
        tempObj.SetActive(true);
        //position.PlayForward();
    }
    /// <summary>
    /// 星星动画结束
    /// </summary>
    /// <param name="gameObj"></param>
    /// <param name="tempObj"></param>
    private void PlayStarTweenOver(GameObject gameObj, GameObject tempObj)
    {
        gameObj.SetActive(true);
        GameObject.Destroy(tempObj);
    }
    /// <summary>
    /// 战斗进攻前倒计时结束
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="obj"></param>
    private void OnBackTimeOver(string eventType, object obj)
    {
        btnStartFight.SetActive(false);
        searchCon.SetActive(false);
        progressCon.SetActive(true);
        UpdateArmies();
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnStartFight))
        {
            btnStartFight.SetActive(false);
            UpdateArmies();
            BattleManager.Instance.StartBattle();
        }
        else if (go.Equals(btnCancelFight))
        {
            //返回营地
            if (!BattleUIManager.Instance.hasUseArmy)
                BattleManager.Instance.BackToMyCamp();
            else
                BattleManager.Instance.ForceBattleEnd();
        }
        else if (go.Equals(btnSearch))
        {
            //搜索下一个目标
            BattleUIManager.Instance.SearchNextBattle();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnCancelFight).onClick -= OnClickButton;
        UIEventListener.Get(btnSearch).onClick -= OnClickButton;
        UIEventListener.Get(btnStartFight).onClick -= OnClickButton;
        EventDispather.RemoveEventListener(BattleUIManager.BACK_TIME_FIGHT_START, OnBackTimeOver);
        EventDispather.RemoveEventListener(GameEvents.BATTLE_SPAWN, UpdateOneArmyById);
        EventDispather.RemoveEventListener(GameEvents.BATTLE_PROGRESS_CHANGE, UpdateFightProgress);
        foreach (Transform child in soldierCon)
        {
            UIEventListener.Get(child.gameObject).onClick -= OnClickArmyHandler;
            GameObject.Destroy(child.gameObject);
        }
        dicSoldier.Clear();
        //恢复到原来显示
        curStar = -1;
        btnStartFight.SetActive(true);
        searchCon.SetActive(true);
        progressCon.SetActive(false);
    }

    public void SetFightData()
    {
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }
        txtProcess.text = "0%";
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
            UIEventListener.Get(obj).onClick += OnClickArmyHandler;
            if (i == 0)
            {
                OnClickArmyHandler(obj);
            }
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
            UIEventListener.Get(obj).onClick += OnClickArmyHandler;
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
            UIEventListener.Get(obj).onClick += OnClickArmyHandler;
        }
    }
    /// <summary>
    /// 选中某个兵
    /// </summary>
    /// <param name="go"></param>
    private void OnClickArmyHandler(GameObject go)
    {
        UIFightSoldierInfo info;
        if (currentSelect)
        {
            info = currentSelect.GetComponent<UIFightSoldierInfo>();
            info.SetSelect(false);
        }
        info = go.GetComponent<UIFightSoldierInfo>();
        info.SetSelect(true);
        currentSelect = go;
        (GameWorld.Instance.CurrentWorldMode as IsoWorldModeAttack).SetSpawnId(info.ArmyData.cid);
    }
    /// <summary>
    /// 派出士兵时更新士兵数据
    /// </summary>
    /// <param name="armyId"></param>
    private void UpdateOneArmyById(string eventType, object obj)
    {
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
        if (vo.amount <= 0)
        {
            UIEventListener.Get(armyObject).onClick -= OnClickArmyHandler;
        }
        BattleUIManager.Instance.hasUseArmy = true;
    }
}
