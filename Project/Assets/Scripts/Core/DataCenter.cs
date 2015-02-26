using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Schema;
using com.pureland.proto;
using UnityEngine;

public class DataCenter : Singleton<DataCenter>
{
    public long myUserId;                //我的id

    public long nextItemSid;             //下一个建造的物品的服务器id
    private int requestSequenceId;       //下一个请求的序列id,本次session递增

    public string authToken;            //验证串
    private CampVO attacker = new CampVO() { player = new PlayerVO() };
    private CampVO defender = new CampVO() { player = new PlayerVO() };
    public CampVO originDefenderData;//TODO for test

    public List<EntityModel> entityModels = new List<EntityModel>();                                        //静态数据表
    private readonly Dictionary<int, EntityModel> entityModelCache = new Dictionary<int, EntityModel>();      //根据id缓存静态数据

    public List<ShopModel> shopModels = new List<ShopModel>();                                               //商店数据

    public List<BuildingLimitModel> buildingLimitModels = new List<BuildingLimitModel>();                   //建筑数量限制数据
    private List<RankModel> rankModels = new List<RankModel>();                                             //经验等级等数据
    private List<BattleRewardModel> battleRewardModels = new List<BattleRewardModel>();                     //战斗奖励表
    private List<TipModel> tipModels = new List<TipModel>();                                                //提示信息表

    private List<BattleResultVO> attackHistories;
    private List<BattleResultVO> defenseHistories;

    public PlayerLocalDataVO playerLocalDataVO;

    private int totalSpace;              //总人口
    private int spaceUsed;               //已经使用的人口

    /// <summary>
    /// 当前玩家自己的数据
    /// </summary>
    public CampVO Attacker
    {
        get { return attacker; }
        set { attacker = value; }
    }
    /// <summary>
    /// 当前玩家自己的家园数据
    /// </summary>
    public CampVO Defender
    {
        get { return defender; }
        set { defender = value; }
    }

    public int TotalWorker
    {
        get { return defender.player.maxWorker; }
        set
        {
            defender.player.maxWorker = value;
            EventDispather.DispatherEvent(GameEvents.WORKER_CHANGE);
        }
    }

    public int FreeWorker
    {
        get { return defender.player.freeWorker; }
        set
        {
            defender.player.freeWorker = value;
            EventDispather.DispatherEvent(GameEvents.WORKER_CHANGE);
        }
    }

    public int TotalSpace
    {
        get { return totalSpace; }
        set
        {
            totalSpace = value;
            EventDispather.DispatherEvent(GameEvents.SPACE_CHANGE);
        }
    }

    public int SpaceUsed
    {
        get { return spaceUsed; }
        set
        {
            spaceUsed = value;
            EventDispather.DispatherEvent(GameEvents.SPACE_CHANGE);
        }
    }

    public void SetBattleHistories(List<BattleResultVO> attackHistories, List<BattleResultVO> defenseHistories)
    {
        this.attackHistories = attackHistories;
        this.defenseHistories = defenseHistories;
        EventDispather.DispatherEvent(GameEvents.BATTLE_HISTORY_LOADED);
    }
    public List<BattleResultVO> AttackHistories
    {
        get
        {
            return this.attackHistories;
        }
    }
    public List<BattleResultVO> DefenseHistories
    {
        get
        {
            return this.defenseHistories;
        }
    }
    /// <summary>
    /// 获取存储某类资源的最大值
    /// </summary>
    /// <param name="resourceType"></param>
    /// <returns></returns>
    public int GetMaxResourceStorage(ResourceType resourceType)
    {
        int count = 0;
        var resourceStorageBuildings = IsoMap.Instance.GetComponents<ResourceStorageBuildingComponent>(OwnerType.Defender);
        foreach (var resourceStorageBuildingComponent in resourceStorageBuildings)
        {
            if (resourceStorageBuildingComponent.ResourceType == resourceType)
            {
                count += resourceStorageBuildingComponent.maxResourceStorage;
            }
        }
        return count;
    }

    public void Init()
    {
        entityModels = XmlData.Deserialize<EntityModel>();
        foreach (var entityModel in entityModels)
        {
            entityModelCache[entityModel.baseId] = entityModel;
        }
        shopModels = XmlData.Deserialize<ShopModel>();
        buildingLimitModels = XmlData.Deserialize<BuildingLimitModel>();
        rankModels = XmlData.Deserialize<RankModel>();
        battleRewardModels = XmlData.Deserialize<BattleRewardModel>();
        tipModels = XmlData.Deserialize<TipModel>();
        try
        {
#if UNITY_EDITOR
            playerLocalDataVO = ExtensionMethods.DeSerialize<PlayerLocalDataVO>(Application.streamingAssetsPath + "/playerLocalDataModel.pbd");
#else
            playerLocalDataVO = ExtensionMethods.DeSerialize<PlayerLocalDataVO>(Application.persistentDataPath + "/playerLocalDataModel.pbd");
#endif
        }
        catch (Exception)
        {
            //pass new player
        }
    }

    public void SavePlayerLocalData()
    {
#if UNITY_EDITOR
        playerLocalDataVO.Serialize(Application.streamingAssetsPath + "/playerLocalDataModel.pbd");
#else
        playerLocalDataVO.Serialize(Application.persistentDataPath + "/playerLocalDataModel.pbd");
#endif
    }
    public void SetHomelandData(CampVO camp)
    {
        Defender = camp;

        if (camp.player.userId == myUserId)//保存的数据
        {
            Attacker = camp;
        }
    }
    /// <summary>
    /// 购买或生产下一个物品的服务器端id
    /// </summary>
    /// <returns></returns>
    public long CreateNextItemSid()
    {
        return Interlocked.Increment(ref nextItemSid);
    }

    public int CreateNextRequestSequenceId()
    {
        return Interlocked.Increment(ref requestSequenceId);
    }

    public int GetResource(ResourceType type, OwnerType owner = OwnerType.Defender)
    {
        CampVO camp = owner == OwnerType.Attacker ? Attacker : Defender;
        foreach (var resourceVo in camp.player.resources)
        {
            if (resourceVo.resourceType == type)
            {
                return resourceVo.resourceCount;
            }
        }
        return 0;
    }

    /// <summary>
    /// 获得或者使用资源
    /// </summary>
    /// <param name="resource"></param>
    public void AddResource(ResourceVO addResource, OwnerType owner = OwnerType.Defender)
    {
        AddResource(addResource.resourceType, addResource.resourceCount, owner);
    }

    public void AddResource(ResourceType resourceType, int resourceCount, OwnerType owner = OwnerType.Defender)
    {
        CampVO camp = owner == OwnerType.Attacker ? Attacker : Defender;
        foreach (var resourceVo in camp.player.resources)
        {
            if (resourceVo.resourceType == resourceType)
            {
                resourceVo.resourceCount += resourceCount;
                if (resourceVo.resourceCount < 0)
                    resourceVo.resourceCount = 0;
                if (owner == OwnerType.Defender)
                {
                    GameWorld.Instance.AverageResourceStorageComponents(resourceType);
                    EventDispather.DispatherEvent(GameEvents.RESOURCE_CHANGE, resourceVo);
                }
                return;
            }
        }
        Assert.Fail("should not reach here");
    }

    public void RemoveResource(ResourceType resourceType, int resourceCount, OwnerType owner = OwnerType.Defender)
    {
        AddResource(resourceType, -resourceCount, owner);
    }
    public void AddExp(int exp)
    {
        Defender.player.experience += exp;
        EventDispather.DispatherEvent(GameEvents.EXP_CHANGE);
        RankModel current, next;
        FindRankModel(RankType.Level, Defender.player.experience, out current, out next);
        var newlevel = int.Parse(current.name);
        if (newlevel > Defender.player.level)
        {
            Defender.player.level = newlevel;
            EventDispather.DispatherEvent(GameEvents.LEVEL_UP);
        }
    }

    public void AddCrown(int crown, OwnerType owner = OwnerType.Defender)
    {
        CampVO camp = owner == OwnerType.Attacker ? Attacker : Defender;
        camp.player.crown += crown;
    }

    public void AddBuilding(BuildingVO building)
    {
        Defender.buildings.Add(building);
    }

    public void AddArmy(ArmyVO army)
    {
        foreach (var armyVo in Defender.armies)
        {
            if (armyVo.cid == army.cid)
            {
                armyVo.amount += army.amount;
                ChangeArmyExp(army);
                return;
            }
        }
        Defender.armies.Add(army);
        ChangeArmyExp(army);
    }

    private void ChangeArmyExp(ArmyVO army)
    {
        //判断当前的兵能不能加经验
        if (!ItemOperationManager.Instance.IsItemCanLevelUp(army.cid)) return;
        List<ArmyExpVO> list = Defender.player.armyShop;
        EntityModel nextModel = ModelUtil.GetNextLevelModel(army.cid);
        //升级到的Id
        int levelUpId = 0;
        foreach (ArmyExpVO expVo in list)
        {
            if (expVo.cid == army.cid)
            {
                expVo.exp += army.amount;
                //可能跳跃升级
                while (null != nextModel && expVo.exp >= nextModel.costResourceCount)
                {
                    expVo.cid = nextModel.baseId;
                    levelUpId = nextModel.baseId;
                    nextModel = ModelUtil.GetNextLevelModel(nextModel.baseId);
                }
                break;
            }
        }
        if (levelUpId != 0)
        {
            foreach (var armyVo in Defender.armies)
            {
                if (armyVo.cid == army.cid)
                {
                    armyVo.cid = levelUpId;
                    break;
                }
            }
            EventDispather.DispatherEvent(GameEvents.SOLDIER_UP, army.cid);
        }
        else
        {
            EventDispather.DispatherEvent(GameEvents.SOLDIER_COUNT_CHANGE, army.cid);
        }
    }

    public void AddSkill(SkillVO skill)
    {
        foreach (var skillVo in Defender.skills)
        {
            if (skillVo.cid == skill.cid)
            {
                skillVo.amount += skill.amount;
                return;
            }
        }
        Defender.skills.Add(skill);
    }
    /// <summary>
    /// 获取entitymodel
    /// </summary>
    /// <param name="cid">baseId</param>
    /// <returns></returns>
    public EntityModel FindEntityModelById(int cid)
    {
        if (entityModelCache.ContainsKey(cid))
        {
            return entityModelCache[cid];
        }
        foreach (var entityModel in entityModels)
        {
            if (entityModel.baseId != cid) continue;
            entityModelCache.Add(cid, entityModel);
            return entityModel;
        }
        return null;
    }

    public List<EntityModel> GetAllEntityModelOfRaceType(RaceType raceType)
    {
        List<EntityModel> ret = new List<EntityModel>();
        foreach (var entityModel in entityModels)
        {
            if (entityModel.raceType == raceType)
            {
                ret.Add(entityModel);
            }
        }
        return ret;
    }

    public EntityModel FindEntityModel(RaceType raceType, string subType, int level)
    {
        foreach (var entityModel in entityModels)
        {
            if (entityModel.raceType == raceType && entityModel.subType == subType && entityModel.level == level)
            {
                return entityModel;
            }
        }
        return null;
    }

    public EntityModel FindEntityModelByResourceName(string name)//TODO use only for debug usage
    {
        foreach (var entityModel in entityModels)
        {
            if (entityModel.nameForResource != name) continue;
            return entityModel;
        }
        return null;
    }

    /// <summary>
    /// 获取每级最大建筑数量
    /// </summary>
    /// <param name="centerId">大本营id</param>
    /// <returns></returns>
    public List<BuildingLimitModel> FindBuildingLimitAll(int centerId)
    {
        List<BuildingLimitModel> limits = new List<BuildingLimitModel>();
        foreach (var buildingLimitModel in buildingLimitModels)
        {
            if (buildingLimitModel.baseId == centerId)
            {
                limits.Add(buildingLimitModel);
            }
        }
        return limits;
    }

    /// <summary>
    /// 获取某类建筑的最大数量
    /// </summary>
    /// <param name="cid"></param>
    /// <returns></returns>
    public int FindBuildingLimitById(int cid)
    {
        foreach (var buildingLimitModel in buildingLimitModels)
        {
            if (buildingLimitModel.baseId == GetCenterBuildingModel().baseId && buildingLimitModel.buildingBaseId == cid)
            {
                return buildingLimitModel.buildingCount;
            }
        }
        return 0;
    }

    /// <summary>
    /// 获取指定建造类型数量
    /// </summary>
    /// <param name="subType"></param>
    /// <returns></returns>
    public int CountBuilding(string subType)
    {
        int i = 0;
        foreach (var buildingVo in Defender.buildings)
        {
            if (FindEntityModelById(buildingVo.cid).subType == subType)
            {
                ++i;
            }
        }
        return i;
    }

    /// <summary>
    /// 获取等级信息
    /// </summary>
    /// <param name="type">等级类型</param>
    /// <param name="exp">等级总经验</param>
    /// <param name="currentRankModel">当前等级模型</param>
    /// <param name="nextRankModel">下一等级的模型,顶级时为空</param>
    public void FindRankModel(RankType type, int exp, out RankModel currentRankModel, out RankModel nextRankModel)
    {
        currentRankModel = null;
        nextRankModel = null;
        foreach (var rankModel in rankModels)
        {
            if (rankModel.type == type && rankModel.exp <= exp)
            {
                currentRankModel = rankModel;
            }
            if (currentRankModel != null && (rankModel.type == type && rankModel.exp > exp || rankModel.type != type))
            {
                if (rankModel.type == type)
                {
                    nextRankModel = rankModel;
                }
                break;
            }
        }
    }

    public BattleRewardModel FindBattleMedalRewardModel(int attackerLevel, int defenderLevel)
    {
        var deltaLevel = attackerLevel - defenderLevel;
        deltaLevel = Mathf.Clamp(deltaLevel, -3, 4);
        foreach (var battleRewardModel in battleRewardModels)
        {
            if (battleRewardModel.level == deltaLevel)
                return battleRewardModel;
        }
        Assert.Fail("should find");
        return null;
    }
    public TipModel FindTipModelById(int tipId)
    {
        foreach (TipModel tipModel in tipModels)
        {
            if (tipModel.id != tipId) continue;
            return tipModel;
        }
        return null;
    }
    /// <summary>
    /// 得到大本营的Model
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    public EntityModel GetCenterBuildingModel(OwnerType owner = OwnerType.Defender)
    {
        CampVO camp = owner == OwnerType.Attacker ? Attacker : Defender;
        return FindEntityModelById(camp.player.baseId);
    }
    /// <summary>
    /// 改变技能shop
    /// </summary>
    /// <param name="skillId">当前升级的技能id</param>
    public void ChangeSkillShop(int skillId)
    {
        EntityModel skillModel = FindEntityModelById(skillId);
        int index = Defender.player.skillShop.IndexOf(skillId);
        if (index >= 0)
        {
            Defender.player.skillShop[index] = skillModel.upgradeId;
            foreach (SkillVO skillVO in Defender.skills)
            {
                if (skillVO.cid == skillId)
                {
                    skillVO.cid = skillModel.upgradeId;
                }
            }
            EventDispather.DispatherEvent(GameEvents.SKILL_UP, skillId);
        }
    }

    public string NeedToLocalName(RankType needType, int needLevel)
    {
        var model = FindEntityModel((RaceType)Defender.player.raceType, needType.ToString(), needLevel);
        return model.nameForView;
    }
}
