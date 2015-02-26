using System;
using System.IO;
using com.pureland.proto;
using Org.BouncyCastle.Security;
using UnityEngine;
using System.Collections.Generic;

public class BattleManager : Singleton<BattleManager>
{
    public CampVO attackerData;                //备份的进攻方数据
    public CampVO defenderData;                //备份的防守方数据

    private int totalDefenderBuildingCount;     //防守的总建筑数量 不包括墙等
    private int defenderBuildingCount;          //被摧毁的防守的数据 不包括墙等
    private int attackerArmyCount;              //进攻的士兵数量,(已经出兵的和未出兵的)
    private int attackerCurerCount;             //进攻方的医疗数量

    public List<ResourceVO> stolenResources = new List<ResourceVO>();       //偷取的资源量

    public List<ResourceVO> stealableResources = new List<ResourceVO>();    //可以被偷的资源量 
    private bool hasGotHalfDestroyStar;         //是否获取了摧毁50%的星星奖励
    private bool useDonatedArmy;                //使用了援军

    private List<ArmyVO> killedDefenderDonatedArmies = new List<ArmyVO>();
    private List<long> brokenTraps = new List<long>(); //防守方使用掉的陷阱

#if UNITY_EDITOR
    public static List<string> logDamageList = new List<string>();
    public static List<string> logMoveList = new List<string>();
    public static List<string> logFireBullet = new List<string>();
    public static List<string> logTryLockTargeter = new List<string>();
    public static List<string> logWaitTime = new List<string>();
    public static void logClear()
    {
        logDamageList.Clear();
        logMoveList.Clear();
        logFireBullet.Clear();
        logTryLockTargeter.Clear();
        logWaitTime.Clear();
    }
#endif  //  UNITY_EDITOR

    /// <summary>
    /// 战斗开始时间,各种资源建筑的时间结算点等.
    /// </summary>
    public DateTime BattleStartTime { get; set; }
    public int BattleStar { get; private set; }
    /// <summary>
    /// 摧毁比例 0..1
    /// </summary>
    public float DestroyBuildingPercent { get; private set; }
    /// <summary>
    /// 战斗是否开始了
    /// </summary>
    public bool IsBattleStarted
    {
        get;
        private set;
    }
    /// <summary>
    /// 战斗是否结束了
    /// </summary>
    public bool IsBattleEnded { get; private set; }

    private BattleResultVO result;
    private BattleManager()
    {
    }

    public void PlayerPlaceSoldierOrSkill(int entityId, int x, int y, bool record = true)
    {
        if (!IsBattleStarted)
            return;
        var model = DataCenter.Instance.FindEntityModelById(entityId);
        //是否是使用的援军
        if (entityId == Constants.DENOTED_ARMY_ID)
        {
            if (!useDonatedArmy)
            {
                if (IsoMap.Instance.CanPlaceSoldier(x, y))
                {
                    useDonatedArmy = true;
                    Debug.Log("使用的援军");
                    //  战斗模式 [录像] 记录操作数据
                    if (record)
                    {
                        GameRecord.RecordPlaceSoldier(x, y, Constants.DENOTED_ARMY_ID);
                    }
                    var spawnHelper = new SpawnDonatedArmyHelper(DataCenter.Instance.Attacker.donatedArmies.Clone(), x, y);
                    UpdateManager.Instance.AddUpdate(spawnHelper);

                    DataCenter.Instance.Attacker.donatedArmies.Clear();
                }
                else
                {
                    GameTipsManager.Instance.ShowGameTips(EnumTipsID.Fight_10301);
                    IsoMap.Instance.ShowGuardAreaMap(true);
                }
            }
        }
        else if (EntityTypeUtil.IsSkill(model))
        {
            if (IsoMap.Instance.CanPlaceSkill(x, y))
            {
                foreach (var skillVo in DataCenter.Instance.Attacker.skills)
                {
                    if (skillVo.cid == model.baseId && skillVo.amount > 0)
                    {
                        if (record)
                        {
                            GameRecord.RecordPlaceSoldier(x, y, model.baseId);
                        }
                        GameSkillManager.Instance.AddSkill(model, x, y);
                        skillVo.amount--;
                        break;
                    }
                }
            }
            else
            {
                GameTipsManager.Instance.ShowGameTips(EnumTipsID.Fight_10301);
            }
        }
        else
        {
            if (IsoMap.Instance.CanPlaceSoldier(x, y))
            {
                foreach (var armyVo in DataCenter.Instance.Attacker.armies)
                {
                    if (armyVo.cid == model.baseId && armyVo.amount > 0)
                    {
                        //  战斗模式 [录像] 记录操作数据
                        if (record)
                        {
                            GameRecord.RecordPlaceSoldier(x, y, model.baseId);
                        }
                        //  创建对象
                        IsoMap.Instance.CreateEntityAt(OwnerType.Attacker, model.baseId, x, y);
                        armyVo.amount--;
                        break;
                    }
                }
            }
            else
            {
                GameTipsManager.Instance.ShowGameTips(EnumTipsID.Fight_10301);
                IsoMap.Instance.ShowGuardAreaMap(true);
            }
        }
        EventDispather.DispatherEvent(GameEvents.BATTLE_SPAWN, entityId);
    }

    /// <summary>
    /// 获取战斗结果
    /// </summary>
    /// <returns></returns>
    public BattleResultVO GetBattleResult()
    {
        if (result == null)
        {
            result = new BattleResultVO();
            result.star = BattleStar;
            result.percentage = Mathf.RoundToInt(DestroyBuildingPercent * 100);
            result.useDonatedArmy = useDonatedArmy;
            result.stolenResources.AddRange(stolenResources);
            result.rewardCrown = GetBattleRewardCrown(BattleStar > 0, BattleStar);
            RankModel current, next;
            DataCenter.Instance.FindRankModel(RankType.Crown, DataCenter.Instance.Attacker.player.crown,
                out current, out next);
            result.rewardGoldByCrownLevel = current.rewardGold;
            result.rewardOilByCrownLevel = current.rewardOil;
            result.timestamp = DateTimeUtil.DateTimeToUnixTimestampMS(BattleStartTime);
            result.peerId = DataCenter.Instance.Defender.player.userId;

            result.killedDefenderDonatedArmies.AddRange(killedDefenderDonatedArmies);
            result.brokenTraps.AddRange(brokenTraps);
            foreach (var armyVO in attackerData.armies)
            {
                foreach (var leftArmyVO in DataCenter.Instance.Attacker.armies)
                {
                    if (leftArmyVO.cid == armyVO.cid && leftArmyVO.amount != armyVO.amount)
                    {
                        result.usedArmies.Add(new ArmyVO() { cid = armyVO.cid, amount = armyVO.amount - leftArmyVO.amount });
                        break;
                    }
                }
            }
            foreach (var skillVO in attackerData.skills)
            {
                foreach (var leftSkillVO in DataCenter.Instance.Attacker.skills)
                {
                    if (leftSkillVO.cid == skillVO.cid && leftSkillVO.amount != skillVO.amount)
                    {
                        result.usedSkills.Add(new SkillVO() { cid = skillVO.cid, amount = skillVO.amount - leftSkillVO.amount });
                        break;
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 搜寻下一个
    /// </summary>
    public void FindNextBattle()
    {
        GameWorld.Instance.ChangeLoading(WorldType.Battle);
    }
    /// <summary>
    /// 回营
    /// </summary>
    public void BackToMyCamp()
    {
        GameWorld.Instance.ChangeLoading(WorldType.Home);
    }

    /// <summary>
    /// 结束战斗
    /// </summary>
    public void ForceBattleEnd()
    {
        DelayManager.Instance.AddDelayCall(() => { OnBattleEnd(); });   //  REMARK：把 OnBattleEnd 的调用延迟到 GameRecord.Update(dt) 的后面调用（：
    }

    private void OnBattleEnd()
    {
        if (this.IsBattleEnded)
            return;

        this.IsBattleEnded = true;
        GameRecord.OnBattleEnd();

        if (GameWorld.Instance.worldType != WorldType.Replay)
        {
            var replayData = GameRecord.GetRecordData();
            replayData.attacker = attackerData;
            replayData.defender = defenderData;
            new BattleResultCommand(GetBattleResult(), replayData).ExecuteAndSend();
        }
        EventDispather.DispatherEvent(GameEvents.BATTLE_END);

        var attackerEntities = IsoMap.Instance.GetAllEntitiesByOwner(OwnerType.Attacker);
        foreach (var attackerEntity in attackerEntities)
        {
            attackerEntity.Destroy();
        }
    }

    public void Init()
    {
        useDonatedArmy = false;
        result = null;
        IsBattleStarted = false;
        IsBattleEnded = false;
        hasGotHalfDestroyStar = false;
        stolenResources.Clear();
        stealableResources.Clear();
        killedDefenderDonatedArmies.Clear();
        brokenTraps.Clear();

        totalDefenderBuildingCount = 0;
        BattleStar = 0;
        defenderBuildingCount = 0;
        attackerArmyCount = 0;
    }

    /// <summary>
    /// 战斗未开始
    /// </summary>
    public void PreStartBattle()
    {
        //计算可以被偷的资源量
        var resourceBuildings = IsoMap.Instance.GetComponents<BaseResourceBuildingComponent>(OwnerType.Defender);
        foreach (var resourceBuilding in resourceBuildings)
        {
            AddStealableResource(new ResourceVO() { resourceType = resourceBuilding.ResourceType, resourceCount = resourceBuilding.StealableCount });
        }
    }
    /// <summary>
    /// 战斗开始
    /// </summary>
    public void StartBattle()
    {
        if (IsBattleStarted)
            return;
        IsBattleStarted = true;
        IsoMap.Instance.HideGuardAreaMap();

        //计算每个建筑的百分比
        var allEntities = IsoMap.Instance.GetAllEntitiesByOwner(OwnerType.Defender);
        foreach (var tileEntity in allEntities)
        {
            if (EntityTypeUtil.IsAnyNonWallBuilding(tileEntity.entityType))
            {
                ++defenderBuildingCount;
            }
        }
        totalDefenderBuildingCount = defenderBuildingCount;

        //计算士兵数量
        foreach (var armyVo in DataCenter.Instance.Attacker.armies)
        {
            attackerArmyCount += armyVo.amount;

            var model = DataCenter.Instance.FindEntityModelById(armyVo.cid);
            if (EntityTypeUtil.IsCurer(model))
            {
                attackerCurerCount += armyVo.amount;
            }
        }
        //加上援军的士兵数量
        foreach (var donatedArmy in DataCenter.Instance.Attacker.donatedArmies)
        {
            attackerArmyCount += donatedArmy.amount;

            var model = DataCenter.Instance.FindEntityModelById(donatedArmy.cid);
            if (EntityTypeUtil.IsCurer(model))
            {
                attackerCurerCount += donatedArmy.amount;
            }
        }

        //备份进攻方和防守方的数据,提交服务器时使用
        attackerData = ProtoBuf.Serializer.DeepClone(DataCenter.Instance.Attacker);
        defenderData = ProtoBuf.Serializer.DeepClone(DataCenter.Instance.Defender);
    }
    /// <summary>
    /// 增加可以被偷的资源量
    /// </summary>
    /// <param name="addResource"></param>
    private void AddStealableResource(ResourceVO addResource)
    {
        bool found = false;
        foreach (var resourceVo in stealableResources)
        {
            if (resourceVo.resourceType == addResource.resourceType)
            {
                found = true;
                resourceVo.resourceCount += addResource.resourceCount;
                if (resourceVo.resourceCount < 0)
                    resourceVo.resourceCount = 0;
                break;
            }
        }
        if (!found)
            stealableResources.Add(addResource);
    }

    /// <summary>
    /// 偷资源
    /// </summary>
    /// <param name="resource"></param>
    public void StolenResource(ResourceVO resource)
    {
        UnityEngine.Debug.Log("偷取:" + resource.resourceType + " " + resource.resourceCount);
        var found = false;
        foreach (var stolenResource in stolenResources)
        {
            if (stolenResource.resourceType == resource.resourceType)
            {
                stolenResource.resourceCount += resource.resourceCount;
                found = true;
                break;
            }
        }
        if (!found)
            stolenResources.Add(resource);
        //剩余被偷的资源量
        AddStealableResource(new ResourceVO() { resourceType = resource.resourceType, resourceCount = -resource.resourceCount });
        DataCenter.Instance.AddResource(resource, OwnerType.Attacker);
        EventDispather.DispatherEvent(GameEvents.STOLEN_RESOURCE, resource);
    }

    /// <summary>
    /// 有单位死亡了
    /// </summary>
    /// <param name="entity"></param>
    public void OnEntityDestroy(TileEntity entity)
    {
        //如果死亡的是建筑
        if (EntityTypeUtil.IsAnyNonWallBuilding(entity.entityType))
        {
            defenderBuildingCount--;
            DestroyBuildingPercent = (float)(totalDefenderBuildingCount - defenderBuildingCount) / totalDefenderBuildingCount;
            //摧毁50% 获得一颗星
            if (!hasGotHalfDestroyStar && DestroyBuildingPercent > 0.5f)
            {
                hasGotHalfDestroyStar = true;
                BattleStar++;
            }
            Debug.Log("摧毁:" + DestroyBuildingPercent + "%");

            //摧毁基地 获得一颗星
            if (EntityTypeUtil.IsCenterBuilding(entity.model))
            {
                BattleStar++;
            }
            //摧毁100% 获得一颗星
            if (defenderBuildingCount == 0)
            {
                BattleStar++;
            }

            EventDispather.DispatherEvent(GameEvents.BATTLE_PROGRESS_CHANGE);
        }
        else if (entity.GetOwner() == OwnerType.Attacker && EntityTypeUtil.IsAnyActor(entity.entityType))
        {
            attackerArmyCount--;
            if (EntityTypeUtil.IsCurer(entity.model))
            {
                attackerCurerCount--;
            }
        }
        else if (entity.GetOwner() == OwnerType.Defender)
        {
            if (EntityTypeUtil.IsAnyActor(entity.entityType))
            {
                if (entity.Friendly)
                {
                    var found = false;
                    foreach (var killedDefenderDonatedArmy in killedDefenderDonatedArmies)
                    {
                        if (killedDefenderDonatedArmy.cid == entity.model.baseId)
                        {
                            killedDefenderDonatedArmy.amount++;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        killedDefenderDonatedArmies.Add(new ArmyVO() { amount = 1, cid = entity.model.baseId });
                }
            }
            else if (EntityTypeUtil.IsAnyTrap(entity.model))
            {
                brokenTraps.Add(entity.buildingVO.sid);
            }

        }
        CheckBattleEnd();
    }

    private void CheckBattleEnd()
    {
        if (defenderBuildingCount == 0)
        {
            Debug.Log("战斗结束,进攻胜利");
            OnBattleEnd();
            return;
        }
        if (attackerArmyCount == 0)
        {
            //判断是否还剩下进攻技能
            Debug.Log("战斗结束,进攻失败");
            OnBattleEnd();
            return;
        }
        //如果只剩下医疗部队也结束
        if (attackerArmyCount == attackerCurerCount)
        {
            OnBattleEnd();
            return;
        }
    }

    /// <summary>
    /// 设置下一个准备释放的士兵或者技能
    /// </summary>
    /// <param name="baseId"></param>
    public void SetSpawnId(int baseId)
    {
        ((IsoWorldModeAttack)GameWorld.Instance.CurrentWorldMode).SetSpawnId(baseId);
    }

    /// <summary>
    /// 战争奖励系数
    /// </summary>
    /// <returns></returns>
    public float GetBattleRewardRatio()
    {
        var attackerLevel = DataCenter.Instance.GetCenterBuildingModel(OwnerType.Attacker).level;
        var defenderLevel = DataCenter.Instance.GetCenterBuildingModel(OwnerType.Defender).level;
        return GameDataAlgorithm.GetBattleRewardRatio(attackerLevel, defenderLevel);
    }
    /// <summary>
    /// 获取战争奖励金牌数
    /// </summary>
    /// <param name="star"></param>
    /// <returns></returns>
    public int GetBattleRewardMedal(int star)
    {
        var attackerLevel = DataCenter.Instance.GetCenterBuildingModel(OwnerType.Attacker).level;
        var defenderLevel = DataCenter.Instance.GetCenterBuildingModel(OwnerType.Defender).level;
        return GameDataAlgorithm.GetBattleRewardMetal(attackerLevel, defenderLevel, star);
    }

    /// <summary>
    /// 获取战斗积分奖励
    /// </summary>
    /// <param name="win">是否胜利</param>
    /// <param name="star"></param>
    /// <returns></returns>
    public int GetBattleRewardCrown(bool win, int star)
    {
        int attackerLevel = 0;
        int defenderLevel = 0;
        attackerLevel = DataCenter.Instance.GetCenterBuildingModel(OwnerType.Attacker).level;
        defenderLevel = DataCenter.Instance.GetCenterBuildingModel(OwnerType.Defender).level;
        if (win)
        {
            return GameDataAlgorithm.GetBattleRewardScore(attackerLevel, defenderLevel, star);
        }
        else
        {
            return GameDataAlgorithm.GetBattleRewardScore(defenderLevel, attackerLevel, 3);//进攻失败 == 被对方3星干掉
        }
    }
    public bool UseDonatedArmy
    {
        get
        {
            return this.useDonatedArmy;
        }
    }
}
