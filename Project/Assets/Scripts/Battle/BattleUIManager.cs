using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class BattleUIManager : Singleton<BattleUIManager>
{
    //战斗准备期倒计时结束
    public const string BACK_TIME_FIGHT_START = "back_time_start";
    //战斗准备期倒计时
    public const int BACK_TIME_FIGHT_PREPARE = 1;
    //战斗结束倒计时
    public const int BACK_TIME_FIGHT_OVER = 2;
    public bool hasUseArmy = false;
    /// <summary>
    /// 显示战斗的UI
    /// </summary>
    public void ShowBattleUI()
    {
        UIMananger.Instance.CloseWinByType(UICloseOrHideType.CLOSE_WORLD_TYPE_BATTLE);
        UIPanel panel;
        UISprite sprite;
        //战斗界面下方窗体
        GameObject fightPanel = UIMananger.Instance.ShowWin("PLG_Fight", "UIFightPanel");
        panel = fightPanel.GetComponent<UIPanel>();
        panel.baseClipRegion = new Vector4(0, 0, 0, 150);
        fightPanel.transform.localPosition = new Vector3(0, -(Constants.UI_HEIGHT - panel.baseClipRegion.w) * 0.5f, 0);
        panel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
        PanelUtil.SetPanelAnchors(panel, UIMananger.Instance.uiLayer.transform, new Vector4(0, 1, 0, 0), new Vector4(0, 0, 0, panel.baseClipRegion.w));
        fightPanel.GetComponent<UIFightWnd>().SetFightData();
        //战斗内部信息
        GameObject fightInfo = UIMananger.Instance.ShowWin("PLG_Fight", "UIFightInfoPanel");
        sprite = fightInfo.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0, 0, 1, 1), new Vector4(0, sprite.width, -sprite.height, 0));
        fightInfo.GetComponent<UIFightInfoWnd>().UpdateFightInfo(null, null);
        //战斗中掠夺资源
        GameObject personMoneyWin = UIMananger.Instance.ShowWin("PLG_MainUI", "UIPersonMoneyPanel");
        sprite = personMoneyWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(1, 1, 1, 1), new Vector4(-sprite.width, 0, -sprite.height, 0));
        personMoneyWin.GetComponent<UIPersonMoneyWnd>().SetPlayerMoney(OwnerType.Attacker);
        //战斗计时
        GameObject backTimeWin = UIMananger.Instance.ShowWin("PLG_Fight", "UIFightBackTimePanel");
        sprite = backTimeWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0.5f, 0.5f, 1, 1), new Vector4(-sprite.width * 0.5f, sprite.width * 0.5f, -sprite.height, 0));
        backTimeWin.GetComponent<UIFightBackTimeWnd>().SetTimeAndType(BACK_TIME_FIGHT_PREPARE, 10);
        EventDispather.AddEventListener(GameEvents.BATTLE_SPAWN, OnFightRealBegin);
        EventDispather.AddEventListener(GameEvents.BATTLE_END, OnFightRealEnd);
        hasUseArmy = false;
    }

    private void OnFightRealEnd(string eventType, object obj)
    {
        EventDispather.RemoveEventListener(GameEvents.BATTLE_END, OnFightRealEnd);
        UIMananger.Instance.CloseWin("UIFightBackTimePanel");
        UIMananger.Instance.CloseWin("UIFightReplayBackTimePanel");
        if (GameWorld.Instance.worldType == WorldType.Battle)
        {
            DelayManager.Instance.AddDelayCall(ShowFightResultWin, 1.2f);
        }
        else if (GameWorld.Instance.worldType == WorldType.Replay)
        {
            DelayManager.Instance.AddDelayCall(ShowFightReplayResultWin, 1.2f);
        }
    }
    /// <summary>
    /// 在倒计时时间内直接出兵，战斗开始
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="obj"></param>
    private void OnFightRealBegin(string eventType, object obj)
    {
        EventDispather.RemoveEventListener(GameEvents.BATTLE_SPAWN, OnFightRealBegin);
        EventDispather.DispatherEvent(BACK_TIME_FIGHT_START);
        GameObject backTimeWin = UIMananger.Instance.ShowWin("PLG_Fight", "UIFightBackTimePanel");
        UISprite sprite = backTimeWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0.5f, 0.5f, 1, 1), new Vector4(-sprite.width * 0.5f, sprite.width * 0.5f, -sprite.height, 0));
        backTimeWin.GetComponent<UIFightBackTimeWnd>().SetTimeAndType(2, 180);
    }
    public void BackTimeOver(int type)
    {
        if (BACK_TIME_FIGHT_PREPARE == type)
        {
            //战斗准备期结束
            GameObject backTimeWin = UIMananger.Instance.ShowWin("PLG_Fight", "UIFightBackTimePanel");
            UISprite sprite = backTimeWin.GetComponent<UISprite>();
            PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0.5f, 0.5f, 1, 1), new Vector4(-sprite.width * 0.5f, sprite.width * 0.5f, -sprite.height, 0));
            backTimeWin.GetComponent<UIFightBackTimeWnd>().SetTimeAndType(2, 180);
            EventDispather.DispatherEvent(BACK_TIME_FIGHT_START);
            //战斗默认开始了
            BattleManager.Instance.StartBattle();
            //倒计时正常结束，不需要监听本事件
            EventDispather.RemoveEventListener(GameEvents.BATTLE_SPAWN, OnFightRealBegin);
        }
        else if (BACK_TIME_FIGHT_OVER == type)
        {
            //战斗结束
            UIMananger.Instance.CloseWin("UIBackTimePanel");
            BattleManager.Instance.ForceBattleEnd();
            ShowFightResultWin();
        }
    }
    /// <summary>
    /// 得到能够掠夺的资源数量
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetCanAttackResourceCount(ResourceType type)
    {
        List<ResourceVO> resources = BattleManager.Instance.stealableResources;
        foreach (var resourceVo in resources)
        {
            if (resourceVo.resourceType == type)
            {
                return resourceVo.resourceCount;
            }
        }
        return 0;
    }
    /// <summary>
    /// 掠夺的资源数量
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetHasAttackResourceCount(ResourceType type, List<ResourceVO> resources)
    {
        if (null == resources)
            resources = BattleManager.Instance.stolenResources;
        foreach (var resourceVo in resources)
        {
            if (resourceVo.resourceType == type)
            {
                return resourceVo.resourceCount;
            }
        }
        return 0;
    }
    /// <summary>
    /// 根据军队id获得军队数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ArmyVO GetArmyVOById(int id)
    {
        if (id == Constants.DENOTED_ARMY_ID)
        {
            if (BattleManager.Instance.UseDonatedArmy)
                return new ArmyVO() { cid = Constants.DENOTED_ARMY_ID, amount = 0 };
            else
                return new ArmyVO() { cid = Constants.DENOTED_ARMY_ID, amount = 1 };
        }
        List<ArmyVO> listArmy = DataCenter.Instance.Attacker.armies;
        foreach (ArmyVO vo in listArmy)
        {
            if (vo.cid == id)
            {
                return vo;
            }
        }
        List<SkillVO> listSkill = DataCenter.Instance.Attacker.skills;
        foreach (SkillVO vo in listSkill)
        {
            if (vo.cid == id)
            {
                return new ArmyVO() { cid = vo.cid, amount = vo.amount };
            }
        }
        return null;
    }
    public bool HasLeftArmy()
    {
        List<ArmyVO> listArmy = DataCenter.Instance.Attacker.armies;
        foreach (ArmyVO vo in listArmy)
        {
            if (vo.amount > 0)
            {
                return true;
            }
        }
        List<SkillVO> listSkill = DataCenter.Instance.Attacker.skills;
        foreach (SkillVO vo in listSkill)
        {
            if (vo.amount > 0)
            {
                return true;
            }
        }
        if (BattleManager.Instance.UseDonatedArmy)
            return false;
        return false;
    }
    private void ShowFightResultWin()
    {
        //关闭战斗下栏
        UIMananger.Instance.CloseWin("UIFightPanel");
        //打开战斗结果面板
        GameObject fightResultWin = UIMananger.Instance.ShowWin("PLG_Fight", "UIFightResultPanel");
        fightResultWin.GetComponent<UIFightResultWnd>().SetFightResultData();
    }
    public void SearchNextBattle()
    {
        UIMananger.Instance.CloseAllWin();
        GameWorld.Instance.ChangeLoading(WorldType.Battle, null, new[] { (int)GameWorld.Instance.CurSearchType, -1 });
    }
    /// <summary>
    /// 显示回放的UI
    /// </summary>
    public void ShowReplayUI()
    {
        UIMananger.Instance.CloseWinByType(UICloseOrHideType.CLOSE_WORLD_TYPE_REPLAY);
        UIPanel panel;
        UISprite sprite;
        //回放防御者信息
        GameObject defenderPanel = UIMananger.Instance.ShowWin("PLG_FightReplay", "UIFightReplayDefenderPanel");
        sprite = defenderPanel.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0, 0, 1, 1), new Vector4(0, sprite.width, -sprite.height, 0));
        defenderPanel.GetComponent<UIFightReplayDefenderWnd>().UpdateDefenderInfo(null, null);
        //回放攻击者信息
        GameObject attackerPanel = UIMananger.Instance.ShowWin("PLG_FightReplay", "UIFightReplayAttackerPanel");
        sprite = attackerPanel.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(1, 1, 1, 1), new Vector4(-sprite.width, 0, -sprite.height, 0));
        attackerPanel.GetComponent<UIFightReplayAttackerWnd>().UpdateAttackerInfo(null, null);
        //回放战斗面板
        GameObject replayPanel = UIMananger.Instance.ShowWin("PLG_FightReplay", "UIFightReplayPanel");
        panel = replayPanel.GetComponent<UIPanel>();
        panel.baseClipRegion = new Vector4(0, 0, 0, 150);
        replayPanel.transform.localPosition = new Vector3(0, -(Constants.UI_HEIGHT - panel.baseClipRegion.w) * 0.5f, 0);
        panel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
        PanelUtil.SetPanelAnchors(panel, UIMananger.Instance.uiLayer.transform, new Vector4(0, 1, 0, 0), new Vector4(0, 0, 0, panel.baseClipRegion.w));
        replayPanel.GetComponent<UIFightReplayWnd>().SetFightData();
        //回放倒计时
        GameObject backTimeWin = UIMananger.Instance.ShowWin("PLG_FightReplay", "UIFightReplayBackTimePanel");
        sprite = backTimeWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0.5f, 0.5f, 1, 1), new Vector4(-sprite.width * 0.5f, sprite.width * 0.5f, -sprite.height, 0));
        backTimeWin.GetComponent<UIFightReplayBackTimeWnd>().SetFightReplayTime();
        EventDispather.AddEventListener(GameEvents.BATTLE_END, OnFightRealEnd);
    }
    private void ShowFightReplayResultWin()
    {
        UIMananger.Instance.CloseWin("UIFightReplayPanel");
        //打开战斗结果面板
        GameObject fightResultWin = UIMananger.Instance.ShowWin("PLG_FightReplay", "UIFightReplayResultPanel");
        UIPanel panel = fightResultWin.GetComponent<UIPanel>();
        panel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
        PanelUtil.SetPanelAnchors(panel, UIMananger.Instance.uiLayer.transform, new Vector4(0, 1, 0, 1), new Vector4(0, 0, 0, 0));
        fightResultWin.GetComponent<UIFightReplayResultWnd>().UpDateFightReplayResultInfo();
    }
}
