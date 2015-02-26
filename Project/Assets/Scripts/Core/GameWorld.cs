
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.pureland.proto;
using UnityEngine;
using System.IO;

public class GameWorld : Singleton<GameWorld>
{
    //当前搜索模式
    public FightSearchReq.SearchType CurSearchType { get; private set; }
    /// <summary>
    /// 游戏世界初始化、更新、释放
    /// </summary>
    public void Init()
    {
        DataCenter.Instance.Init();
        if (Constants.ISCLIENT)
        {
            ChangeLoading(WorldType.Home);
        }
        else
        {
            if (GameManager.Instance.IsNewDevice())
            {
                LoginManager.Instance.ShowLoginMainFrame();
            }
            else
            {
                LoginManager.Instance.professionId = 2;
                LoginManager.Instance.playerName = "test" + Random.Range(1,10000);
                new NoAuthLoginCommand(2, LoginManager.Instance.playerName).ExecuteAndSend();
            }
        }
    }

    public void Update(float dt)
    {
        //  *** 在逻辑更新之前这里会处理各种touch事件~  这里面处理的逻辑有：生成士兵 和 ForceBattleEnd(这个延迟到DelayManager进行调用）。***
        currentWorldMode.Update(dt);            //  REMARK：这个移动到录像之前，确保所有的士兵产生逻辑都在录像之前。方便帧数统一。
        
        GameRecord.Update(dt);                  //  REMARK：这个应该位于其他逻辑的最上层、但位于产生士兵的下面。
        GameTime.Update(dt);
        IsoMap.Instance.Update(dt);             //  REMARK：这里面可能导致战斗结束
        GameBulletManager.Instance.Update(dt);
        GameEffectManager.Instance.Update(dt);
        GameSkillManager.Instance.Update(dt);
        UpdateManager.Instance.Update(dt);
        DelayManager.Instance.Update(dt);       //  REMARK：这里面也可能导致战斗结束（因为从上面的touch事件延迟到这里面了）这样 战斗结束事件都在 Record 的后面方便帧数统一。
    }
    public void Destroy()
    {
        //  TODO：
    }

    public bool Loading
    {
        get { return _loading; }
    }

    /// <summary>
    /// 转到加载：家园数据、搜索敌人数据、加载回放数据(需要参数)、加载本地关卡数据(需要参数)（REMARK：开始游戏时加载关卡不走该接口）
    /// </summary>
    public void ChangeLoading(WorldType worldType, LoadingCompleted callback = null, object args = null)
    {
        //  先停止先前的loading
        StopLoading();

        //  设置加载中标记
        _loading = true;

        //  显示云动画&&延迟1秒开始加载资源
        LoadingManager.Instance.ShowLoading();
        DelayManager.Instance.AddDelayCall(() =>
        {
            switch (worldType)
            {
                case WorldType.Home:
                    ChangeLoadingHome(callback, args);
                    break;
                case WorldType.Battle:
                    ChangeLoadingBattle(callback, args);
                    break;
                case WorldType.Replay:
                    ChangeLoadingReplay(callback, args);
                    break;
                case WorldType.Visit:
                    ChangeLoadingVisit(callback, args);
                    break;
                default:
                    break;
            }
        }, 1.0f);
    }
    private void StopLoading()
    {
        if (_loading)
        {
            //  TODO:
            _loading = false;
        }
    }
    public delegate void LoadingCompleted(bool result);
    private bool _loading = false;

    /// <summary>
    /// 加载家园
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="args"></param>
    private void ChangeLoadingHome(LoadingCompleted callback = null, object args = null)
    {
        if (Constants.ISCLIENT)
        {
            Create(WorldType.Home, LoadMockPlayerData(1));
        }
        else
        {
            GameManager.Instance.RequestHomeLandData(DataCenter.Instance.myUserId, WorldType.Home);
        }
        LoadingManager.Instance.CloseLoading();
        DelayManager.Instance.AddDelayCall(() => { _loading = false; if (callback != null) callback(true); }, 1.0f);
    }

    /// <summary>
    /// 加载战斗（本地或服务器）
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="args"></param>
    private void ChangeLoadingBattle(LoadingCompleted callback = null, object args = null)
    {
        if (args != null)
        {
            if (Constants.ISCLIENT)
            {
                Create(WorldType.Battle, LoadMockPlayerData(2), DataCenter.Instance.Defender);
            }
            else
            {
                int[] info = (int[])args;
                CurSearchType = (FightSearchReq.SearchType)info[0];
                GameManager.Instance.RequestFightSearch(info[0], info[1], resp =>
                {
                    if (resp == null)
                    {
                        //  超时 TODO：考虑添加返回按钮咯
                    }
                    else if (resp.errorType > 0)
                    {
                        GameTipsManager.Instance.ShowGameTips("搜索失败");//TODO：考虑添加返回按钮咯
                        return;
                    }
                    else
                    {
                        //  TODO:
                        Create(WorldType.Battle, resp.respWrapper.campResp.campVO, null);
                    }
                });
            }
        }
        //  加载本地关卡 TODO
        LoadingManager.Instance.CloseLoading();
        DelayManager.Instance.AddDelayCall(() => { _loading = false; if (callback != null) callback(true); }, 1.0f);
    }

    /// <summary>
    /// 加载回放
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="args"></param>
    private void ChangeLoadingReplay(LoadingCompleted callback = null, object args = null)
    {
        if (Constants.ISCLIENT)
        {
            var vo = ExtensionMethods.DeSerialize<BattleReplayVO>(Application.streamingAssetsPath + "/record.pbd");
            LoadingManager.Instance.CloseLoading();
            DelayManager.Instance.AddDelayCall(() => 
            { 
                _loading = false; 
                if (callback != null) 
                    callback(true);
                Create(WorldType.Replay, vo.defender, vo.attacker, vo);
            }, 1.0f);
        }
        else
        {
            GameManager.Instance.RequestPlayReplay((long)args);
            LoadingManager.Instance.CloseLoading();
            DelayManager.Instance.AddDelayCall(() => { _loading = false; if (callback != null) callback(true); }, 1.0f);
        }
    }

    /// <summary>
    /// 加载好友家园
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="args"></param>
    private void ChangeLoadingVisit(LoadingCompleted callback = null, object args = null)
    {
        Assert.Should(args != null);
        //  加载好友
        Assert.Fail("unsupport now");
    }

    /// <summary>
    /// 加载玩家模拟数据
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public CampVO LoadMockPlayerData(long userId = 1)
    {
        var asset = Resources.Load<TextAsset>("Binary/palyer" + userId);
        var stream = new MemoryStream(asset.bytes);
        var campVO = ProtoBuf.Serializer.Deserialize<CampVO>(stream);
        Resources.UnloadAsset(asset);
        return campVO;
    }

    /// <summary>
    /// 创建模拟玩家数据
    /// </summary>
    /// <param name="userId"></param>
    private void CreateMockPlayerData(long userId = 1)
    {
        /// 1、构造建筑数据
        var buildings = new List<BuildingVO>();

        var mockProductionBuildingVO = new ProductionBuildingVO();
        //	    mockProductionBuildingVO.endTime = ServerTime.Instance.GetTimestamp(10);
        //        mockProductionBuildingVO.productionItems.Add(new ProductionItemVO(){cid=12601,count=10});

        var mockResoruceBuildingVO = new ResourceBuildingVO();
        mockResoruceBuildingVO.lastGatherTime = ServerTime.Instance.GetTimestamp(-60);
        buildings.Add(new BuildingVO()
        {
            buildingStatus = BuildingVO.BuildingStatus.On,
            cid = 20101,//大本营
            sid = 5,
            endTime = ServerTime.Instance.GetTimestamp(10),
            x = 30,
            y = 30
        });
        buildings.Add(new BuildingVO()
        {
            buildingStatus = BuildingVO.BuildingStatus.On,
            cid = 26601,//建造工人小屋
            sid = 3,
            endTime = ServerTime.Instance.GetTimestamp(10),
            x = 30,
            y = 40
        });
        buildings.Add(new BuildingVO()
        {
            buildingStatus = BuildingVO.BuildingStatus.On,
            cid = 23801,//军营
            sid = 4,
            endTime = ServerTime.Instance.GetTimestamp(10),
            x = 40,
            y = 30
        });
        buildings.Add(new BuildingVO()
        {
            buildingStatus = BuildingVO.BuildingStatus.On,
            cid = 21201,
            productionBuildingVO = mockProductionBuildingVO,
            sid = 1,
            endTime = ServerTime.Instance.GetTimestamp(10),
            x = 40,
            y = 40
        });
        buildings.Add(new BuildingVO()
        {
            buildingStatus = BuildingVO.BuildingStatus.On,
            cid = 20201,
            resourceBuildingVO = mockResoruceBuildingVO,
            sid = 2,
            endTime = 0,
            x = 50,
            y = 40
        });
        buildings.Add(new BuildingVO()
        {
            buildingStatus = BuildingVO.BuildingStatus.On,
            cid = 20601,
            resourceBuildingVO = mockResoruceBuildingVO,
            sid = 2,
            endTime = 0,
            x = 50,
            y = 50
        });
        buildings.Add(new BuildingVO()
        {
            buildingStatus = BuildingVO.BuildingStatus.On,
            cid = 23901,
            sid = 7,
            endTime = 0,
            x = 40,
            y = 50
        });
        var allTowers = new int[] { 21801, 21901, 22001, 22101, 22201, 22301, 22401, 22501 };
        //所有炮塔
        for (int i = 0; i < allTowers.Length; ++i)
        {
            buildings.Add(new BuildingVO()
            {
                buildingStatus = BuildingVO.BuildingStatus.On,
                cid = allTowers[i],
                sid = 8 + i,
                endTime = 0,
                x = (i * 10) / 80,
                y = (i * 10) % 80
            });
        }

        var allTraps = new int[] { 21501, 21701, 26001, 26101, 26201 };
        for (int i = 0; i < allTraps.Length; i++)
        {
            buildings.Add(new BuildingVO()
            {
                buildingStatus = BuildingVO.BuildingStatus.On,
                trapBuildingVO = new TrapBuildingVO() { broken = false },
                cid = allTraps[i],
                sid = 20 + i,
                endTime = 0,
                x = 60 + (i * 10) / 80,
                y = (i * 10) % 80
            });
        }

        /// 2、构造其他数据
        var campVO = new CampVO() { player = new PlayerVO() };
        //  player
        campVO.player.freeWorker = 4;
        campVO.player.maxWorker = 4;
        campVO.player.baseId = 20101;
        campVO.player.userId = userId;
        campVO.player.raceType = (int)RaceType.Predator;
        campVO.player.armyShop.AddRange(new[] {
            new ArmyExpVO(){cid=22601,exp=0},
            new ArmyExpVO(){cid=22701,exp=0},
            new ArmyExpVO(){cid=22901,exp=0},
            new ArmyExpVO(){cid=23001,exp=0},
            new ArmyExpVO(){cid=23201,exp=0},
            new ArmyExpVO(){cid=22801,exp=0},
            new ArmyExpVO(){cid=23101,exp=0},
            new ArmyExpVO(){cid=23301,exp=0}});
        campVO.player.skillShop.AddRange(new[] { 23401, 23601, 26301, 26401, 26501, 23501 });
        var myResoruce = new List<ResourceVO>();
        myResoruce.Add(new ResourceVO() { resourceType = ResourceType.Gold, resourceCount = 1000 });
        myResoruce.Add(new ResourceVO() { resourceType = ResourceType.Oil, resourceCount = 1000 });
        myResoruce.Add(new ResourceVO() { resourceType = ResourceType.Medal, resourceCount = 1000 });
        myResoruce.Add(new ResourceVO() { resourceType = ResourceType.Diamond, resourceCount = 1000 });
        campVO.player.resources.AddRange(myResoruce);
        //  building
        campVO.buildings.AddRange(buildings);
        //  armies&donatedArmies
        campVO.armies.Add(new ArmyVO() { amount = 20, cid = DataCenter.Instance.FindEntityModelByResourceName("PsoldierA01").baseId });
        campVO.armies.Add(new ArmyVO() { amount = 20, cid = DataCenter.Instance.FindEntityModelByResourceName("PsoldierB01").baseId });
        campVO.armies.Add(new ArmyVO() { amount = 20, cid = DataCenter.Instance.FindEntityModelByResourceName("PsoldierC01").baseId });
        campVO.armies.Add(new ArmyVO() { amount = 20, cid = DataCenter.Instance.FindEntityModelByResourceName("PsoldierD01").baseId });
        campVO.armies.Add(new ArmyVO() { amount = 20, cid = DataCenter.Instance.FindEntityModelByResourceName("PsoldierE01").baseId });
        campVO.armies.Add(new ArmyVO() { amount = 20, cid = DataCenter.Instance.FindEntityModelByResourceName("PsoldierF01").baseId });
        campVO.armies.Add(new ArmyVO() { amount = 20, cid = DataCenter.Instance.FindEntityModelByResourceName("PsoldierG01").baseId });
        campVO.armies.Add(new ArmyVO() { amount = 20, cid = DataCenter.Instance.FindEntityModelByResourceName("PsoldierH01").baseId });
        campVO.donatedArmies.AddRange(campVO.armies);

        campVO.skills.Add(new SkillVO() { amount = 10, cid = 26301 });
        campVO.skills.Add(new SkillVO() { amount = 10, cid = 26401 });
        campVO.skills.Add(new SkillVO() { amount = 10, cid = 26501 });
        campVO.skills.Add(new SkillVO() { amount = 10, cid = 23401 });
        campVO.skills.Add(new SkillVO() { amount = 10, cid = 23501 });
        campVO.skills.Add(new SkillVO() { amount = 10, cid = 23601 });

        /// 3、写入文件
        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        ProtoBuf.Serializer.Serialize<ProtoBuf.IExtensible>(stream, campVO);
        System.Byte[] bytes = stream.ToArray();
        using (FileStream fs = new FileStream(Application.streamingAssetsPath + "/palyer" + userId + ".bytes", FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(bytes);
            }
        }
        Debug.Log("Saved");
    }

    private IsoWorldMode currentWorldMode = new IsoWorldModeVisit();//防止update空指针

    public IsoWorldMode CurrentWorldMode
    {
        get { return currentWorldMode; }
        private set
        {
            if (currentWorldMode != null)
            {
                currentWorldMode.ExitMode();
            }
            currentWorldMode = value;
            if (currentWorldMode != null)
            {
                currentWorldMode.EnterMode();
            }
        }
    }

    public WorldType worldType { get; private set; }

    public void Create(WorldType worldType, CampVO defender = null, CampVO attacker = null, BattleReplayVO replayVO = null)
    {
        this.worldType = worldType;

        IsoMap.Instance.Clear();
        GameBulletManager.Instance.Init();
        GameEffectManager.Instance.Init();
        GameSkillManager.Instance.Init();
        UpdateManager.Instance.Clear();
        DelayManager.Instance.Init();
        BattleManager.Instance.Init();
        PoolManager.Instance.Clear();
        ResourceManager.Instance.ClearCache();

        //  初始化数据
        if (defender != null)
        {
            DataCenter.Instance.Defender = defender;
        }
        if (attacker != null)
        {
            DataCenter.Instance.Attacker = attacker;
        }

        //  REMARK：录像模式或回放模式从这里开始处理（之前的模块释放完毕，开始放置建筑之前。）
        if (worldType == WorldType.Battle)
        {
            GameRecord.StartRecord();
        }
        else if (worldType == WorldType.Replay)
        {
            Assert.Should(replayVO != null);
            DataCenter.Instance.Attacker = replayVO.attacker;
            DataCenter.Instance.Defender = replayVO.defender;
            GameRecord.StartReplay(replayVO);
        }

        //  添加建筑
        for (int i = 0; i < DataCenter.Instance.Defender.buildings.Count; i++)
        {
            var buildingVO = DataCenter.Instance.Defender.buildings[i];
            var entityModel = DataCenter.Instance.FindEntityModelById(buildingVO.cid);
            Assert.Should(entityModel != null);
            var tileEntity = TileEntity.Create(OwnerType.Defender, entityModel);
            tileEntity.buildingVO = buildingVO;
            tileEntity.SetTilePosition(new TilePoint(buildingVO.x, buildingVO.y));
            IsoMap.Instance.ForceAddEntity(tileEntity);
        }

        //  添加工人,士兵（仅在建造模式下）
        if (worldType == WorldType.Home)
        {
            foreach (var armyVo in DataCenter.Instance.Defender.armies)
            {
                for (int i = 0; i < armyVo.amount; i++)
                {
                    var actor = DataCenter.Instance.FindEntityModelById(armyVo.cid);
                    var tileEntity = TileEntity.Create(OwnerType.Defender, actor);
                    tileEntity.SetTilePosition(new TilePoint(0, 0));
                    IsoMap.Instance.ForceAddEntity(tileEntity);
                }
            }
            //  TODO：临时
            for (int i = 0; i < DataCenter.Instance.Defender.player.maxWorker; i++)
            {
                var actor = DataCenter.Instance.FindEntityModelByResourceName("Pworker");
                var tileEntity = TileEntity.Create(OwnerType.Defender, actor);
                tileEntity.SetTilePosition(new TilePoint(5, 5));
                IsoMap.Instance.ForceAddEntity(tileEntity);
                tileEntity.HideEntity();
            }

            //计算最大人口
            DataCenter.Instance.TotalSpace = 0;
            foreach (var buildingVo in DataCenter.Instance.Defender.buildings)
            {
                var model = DataCenter.Instance.FindEntityModelById(buildingVo.cid);
                if (EntityTypeUtil.IsBarracks(model))
                {
                    DataCenter.Instance.TotalSpace += model.spaceProvide;
                }
            }
            //计算已使用人口
            DataCenter.Instance.SpaceUsed = 0;
            foreach (var armyVo in DataCenter.Instance.Defender.armies)
            {
                var model = DataCenter.Instance.FindEntityModelById(armyVo.cid);
                DataCenter.Instance.SpaceUsed += model.spaceUse * armyVo.amount;
            }
        }

        //把资源平均分配到资源库
        AverageResourceStorageComponents(ResourceType.Gold);
        AverageResourceStorageComponents(ResourceType.Oil);

        switch (worldType)
        {
            case WorldType.Battle:
                IsoMap.Instance.InitGuardAreaMap();
                IsoMap.Instance.ShowGuardAreaMap(false);
                CurrentWorldMode = new IsoWorldModeAttack();
                BattleManager.Instance.PreStartBattle();
                BattleUIManager.Instance.ShowBattleUI();
                break;
            case WorldType.Replay:
                CurrentWorldMode = new IsoWorldModeReplay();
                BattleManager.Instance.PreStartBattle();
                BattleManager.Instance.StartBattle();   //  TODO：暂时先放这里
                BattleUIManager.Instance.ShowReplayUI();
                break;
            case WorldType.Visit:
                CurrentWorldMode = new IsoWorldModeVisit();
                break;
            case WorldType.Home:
                CurrentWorldMode = new IsoWorldModeBuilder();
                HomeLandManager.Instance.ShowMyHomeLandUI();
                break;
        }
    }

    /// <summary>
    /// 平均分配资源到资源存储器
    /// </summary>
    /// <param name="resourceType"></param>
    public void AverageResourceStorageComponents(ResourceType resourceType)
    {
        List<ResourceStorageBuildingComponent> allResourceStorageBuildingComponents =
            IsoMap.Instance.GetComponents<ResourceStorageBuildingComponent>(OwnerType.Defender)
            .OrderBy(o => o.maxResourceStorage).ToList();
        if (allResourceStorageBuildingComponents.Count == 0)
            return;
        List<ResourceStorageBuildingComponent> resourceStorageBuildingComponents = new List<ResourceStorageBuildingComponent>();
        foreach (var resourceStorageComponent in allResourceStorageBuildingComponents)
        {
            if (resourceStorageComponent.Entity.model.resourceType == resourceType &&
                resourceStorageComponent.Entity.buildingVO.buildingStatus != BuildingVO.BuildingStatus.Construct)//建筑中的建筑不参加
            {
                resourceStorageBuildingComponents.Add(resourceStorageComponent);
            }
        }
        if (resourceStorageBuildingComponents.Count == 0)
            return;
        //总资源量
        var total = DataCenter.Instance.GetResource(resourceType);
        while (total > 0)
        {
            if (resourceStorageBuildingComponents.Count == 0)
            {
                Debug.LogError("钱超过容量上限了");
                return;
            }
            //每一个容器的平均资源量
            var average = total / resourceStorageBuildingComponents.Count;
            //如果最小容器装不下平均值,装满最小容器后,其他容器进行平均
            if (resourceStorageBuildingComponents[0].maxResourceStorage < average)
            {
                resourceStorageBuildingComponents[0].resourceStorage =
                    resourceStorageBuildingComponents[0].maxResourceStorage;
                resourceStorageBuildingComponents[0].Init();//数据变化了 重新初始化
                total -= resourceStorageBuildingComponents[0].resourceStorage;
                resourceStorageBuildingComponents.RemoveAt(0);
                continue;
            }
            else
            {
                foreach (var resourceStorageBuildingComponent in resourceStorageBuildingComponents)
                {
                    resourceStorageBuildingComponent.resourceStorage = average;
                    resourceStorageBuildingComponent.Init();//数据变化了 重新初始化
                    total -= average;
                }
                //还有一点没装下
                if (total > 0)
                {
                    var last = resourceStorageBuildingComponents[resourceStorageBuildingComponents.Count - 1];
                    last.resourceStorage += total;
                    last.Init();//数据变化了 重新初始化
                }
                total = 0;
            }

        }
    }
}
