using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class UIMailContentFrame : MonoBehaviour
{
    //玩家菜单操作按钮
    public GameObject btnPlayerMenu;
    //玩家名字
    public UILabel txtPlayerName;
    //联盟图标
    public GameObject allianceIcon;
    //联盟名字
    public UILabel txtAllianceName;
    //对方获得积分
    public UILabel txtOtherJiFen;
    //分享视频
    public GameObject btnShareVideo;
    //获得星币数量
    public UILabel txtOtherXingBi;
    //获得钛晶的数量
    public UILabel txtOtherTaiJing;
    //时间
    public UILabel txtTime;
    //结果
    public UILabel txtResult;
    //进度
    public UILabel txtFightProgress;
    public List<GameObject> stars = new List<GameObject>(3);
    //回放
    public GameObject btnBackPlay;
    //复仇
    public GameObject btnRevenge;
    //自己获得的奖牌
    public UILabel txtMyJiFen;
    //信息内容
    private BattleResultVO resultVO;
    //用兵容器
    public Transform soldierCon;
    // Use this for initialization
    void Start()
    {
    }


    void OnEnable()
    {
        soldierCon.GetComponent<UIPanel>().depth = soldierCon.parent.GetComponentInParent<UIPanel>().depth + 1;
        UIEventListener.Get(btnPlayerMenu).onClick += OnClickButton;
        UIEventListener.Get(btnShareVideo).onClick += OnClickButton;
        UIEventListener.Get(btnBackPlay).onClick += OnClickButton;
        UIEventListener.Get(btnRevenge).onClick += OnClickButton;
    }
    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnPlayerMenu))
        {
        }
        else if (go.Equals(btnShareVideo))
        {

        }
        else if (go.Equals(btnRevenge))
        {
        }
        else if (go.Equals(btnBackPlay))
        {
            GameWorld.Instance.ChangeLoading(WorldType.Replay, null, resultVO.battleReplayId);
        }
    }

    void OnDisable()
    {
        UIEventListener.Get(btnPlayerMenu).onClick -= OnClickButton;
        UIEventListener.Get(btnShareVideo).onClick -= OnClickButton;
        UIEventListener.Get(btnBackPlay).onClick -= OnClickButton;
        UIEventListener.Get(btnRevenge).onClick -= OnClickButton;
    }
    public void SetMailInfo(BattleResultVO resultVO, int type)
    {
        while (soldierCon.childCount > 0)
        {
            GameObject.DestroyImmediate(soldierCon.GetChild(0).gameObject);
        }
        this.resultVO = resultVO;
        txtPlayerName.text = resultVO.peerName;
        txtAllianceName.text = resultVO.peerClanName;
        //对方积分
        txtOtherJiFen.text = resultVO.peerCrown.ToString();
        //自己获得的积分
        txtMyJiFen.text = resultVO.rewardCrown * (resultVO.star > 0 ? 1 : -1) + "";
        txtOtherXingBi.text = GetHasAttackResourceCount(ResourceType.Gold, resultVO.stolenResources).ToString();
        txtOtherTaiJing.text = GetHasAttackResourceCount(ResourceType.Oil, resultVO.stolenResources).ToString();
        txtFightProgress.text = resultVO.percentage + "%";
        if (0 == type)
        {
            if (resultVO.star > 0)
            {
                txtResult.text = "您的防守失败了";
            }
            else
            {
                txtResult.text = "您的防守成功了";
            }
        }
        else if (1 == type)
        {
            if (resultVO.star > 0)
            {
                txtResult.text = "您获胜了";
            }
            else
            {
                txtResult.text = "您失败了";
            }
        }
        for (int i = 0; i < resultVO.star; i++)
        {
            stars[i].SetActive(true);
        }
        float posX = 0;
        GameObject tempObj;
        for (int i = 0, imax = resultVO.usedArmies.Count; i < imax; i++)
        {
            tempObj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
            tempObj.transform.parent = soldierCon;
            tempObj.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            posX = -135 + i * (tempObj.GetComponent<UISprite>().width * 0.7f + 1);
            tempObj.transform.localPosition = new Vector3(posX, 0);
            tempObj.SetActive(true);
            tempObj.AddMissingComponent<UILogicFightItem>().SetItemInfo(resultVO.usedArmies[i].cid, resultVO.usedArmies[i].amount);
        }
        //for (int i = 0, imax = resultVO.usedSkills.Count; i < imax; i++)
        //{
        //    tempObj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
        //    tempObj.transform.parent = soldierCon;
        //    tempObj.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        //    posX = posX + tempObj.GetComponent<UISprite>().width * 0.7f + 1;
        //    tempObj.transform.localPosition = new Vector3(posX, 0);
        //    tempObj.SetActive(true);
        //    tempObj.AddMissingComponent<UILogicFightItem>().SetItemInfo(resultVO.usedSkills[i].cid,resultVO.usedSkills[i].amount);
        //}
        if (resultVO.useDonatedArmy)
        {
            tempObj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
            tempObj.transform.parent = soldierCon;
            tempObj.transform.localScale = new Vector3(0.7f, 0.7f, 1);
            posX = posX + (tempObj.GetComponent<UISprite>().width * 0.7f + 1);
            tempObj.transform.localPosition = new Vector3(posX, 0);
            tempObj.SetActive(true);
            tempObj.AddMissingComponent<UILogicFightItem>().SetItemInfo(Constants.DENOTED_ARMY_ID, 1);
        }
        txtTime.text = DateTimeUtil.PrettyFormatTimeSeconds((int)(ServerTime.Instance.Now() - DateTimeUtil.UnixTimestampMSToDateTime(resultVO.timestamp)).TotalSeconds, 2) + "前";
    }
    private int GetHasAttackResourceCount(ResourceType type, List<ResourceVO> resources)
    {
        foreach (var resourceVo in resources)
        {
            if (resourceVo.resourceType == type)
            {
                return resourceVo.resourceCount;
            }
        }
        return 0;
    }
}
