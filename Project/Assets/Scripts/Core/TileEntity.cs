using System;
using System.Collections.Generic;
using com.pureland.proto;
using UnityEngine;
using System.Collections;
using Rubystyle;

public class TileEntity
{
    private OwnerType owner;
    public EntityType entityType;
    public EntityAiType aiType;
    public string animationNamePrefix = "";
    public EntityViewComponent view;
    private static RangeView rangeView;//全局只有一个射程
    protected TileEntity currentTarget;

    protected TilePoint tilePos = new TilePoint(int.MaxValue,int.MaxValue);
    public int width;
    public int height;

    public int blockingRange;           //  建筑物的不可通行范围（等于 建筑所占格子的边数 - 2) eg: 小屋 大格子2x2 小格子4x4 边5x5 不可通行范围 3     ※ 兵营除外
    public float monitorRange = 0.0f;   //  士兵的监视范围（在该访问内详细索敌、范围外粗略判断。） REMARK：仅对士兵有效

    protected Vector2 renderTileSizeOffset;
    public Vector2 tileOffset;
    public float verticalHeight = 0;

    public float maxHp;
    public float currentHp;

    public EntityModel model;
    public BuildingVO buildingVO = new BuildingVO();

    private bool inited;

    private List<EntityComponent> components = new List<EntityComponent>();

    public delegate void EntityMessageHandler(TileEntity entity, EntityMessageType msg, object data);

    public event EntityMessageHandler OnEntityMessage;

    private int halfDestroyEffectId;

    /// <summary>
    /// 是否是友军/援军
    /// </summary>
    public bool Friendly { get; private set; }

    /// <summary>
    /// 是否对准目标
    /// </summary>
    public bool AimTarget
    {
        get { return this.model.aimTarget && this.model.numTarget <= 1; }
    }

    /// <summary>
    /// 特性：是否可穿越墙（飞行兵种和特殊技能和援军）
    /// </summary>
    /// <returns></returns>
    public bool CanOverTheWall()
    {
        return (EntityTypeUtil.IsFlyable(model.entityType) || EntityTypeUtil.IsTraitOverTheWall(model) || this.Friendly);
    }

    /// <summary>
    /// 是否可飞行
    /// </summary>
    /// <returns></returns>
    public bool CanFlying()
    {
        return EntityTypeUtil.IsFlyable(model.entityType);
    }

    ///// <summary>
    ///// 建筑物的中心点坐标（AI会根据建筑物的中心寻找最近距离）
    ///// </summary>
    //public TilePoint CenterPoint
    //{
    //    get { return new TilePoint(tilePos.x + (int)(width / 2), tilePos.y + (int)(height / 2)); }
    //}

    ///// <summary>
    ///// 获取对象的攻击范围
    ///// </summary>
    //public float AttackRange
    //{
    //    get
    //    {
    //        float range = 0.0f;
    //        if (model.entityType == EntityType.Tower)
    //        {
    //            range = model.range;
    //        }
    //        else if (model.entityType == EntityType.Actor)
    //        {
    //            //  REMARK：这几个常量数值可以调整
    //            switch (model.attackRangeType)
    //            {
    //                case AttackRangeType.Short: range = 2.0f; break;
    //                case AttackRangeType.Long: range = 3.0f; break;
    //                case AttackRangeType.LongLong: range = 5.0f; break;
    //                default:
    //                    break;
    //            }
    //        }
    //        return range;
    //    }
    //}

    private EntityStateType _state;
    public EntityStateType State
    {
        get { return _state; }
        set
        {
            var oldState = _state;
            _state = value;
            foreach (var entityComponent in components)
            {
                entityComponent.HandleStateChange(oldState,_state);
            }
        }
    }

    #region 初始化相关

    public static TileEntity Create(OwnerType owner, EntityModel model, bool isFriendly = false)
    {
        var entityObj = LoadAndCreate(model);
        var tileEntity = new TileEntity();
        tileEntity.owner = owner;
        tileEntity.model = model;
        tileEntity.State = EntityStateType.Idle;
        tileEntity.aiType = (EntityAiType)model.targetType;
        tileEntity.entityType = model.entityType;
        tileEntity.Friendly = isFriendly;
        if (model.tileSize > 0)
        {
            tileEntity.width = tileEntity.height = model.tileSize * 2;      //  REMARK:建筑实际占用格子扩大2倍
        }
        else
        {
            tileEntity.width = tileEntity.height = 1;
        }
        tileEntity.currentHp = tileEntity.maxHp = Mathf.Max(model.hp, 1);   //  REMARK：这里设置最大hp的最小值为1，不然对于工人等直接就是dead状态。
        tileEntity.animationNamePrefix = model.nameForResource + "_";

        //  REMARK：建筑按照格子建造、士兵按照边行走（所以士兵位置比建筑偏移(0.5,0.5)）
        if (EntityTypeUtil.IsAnyActor(model.entityType))
        {
            tileEntity.renderTileSizeOffset = new Vector2((tileEntity.width * 0.5f) - 1.0f, (tileEntity.height * 0.5f) - 1.0f);
        }
        else
        {
            tileEntity.renderTileSizeOffset = new Vector2((tileEntity.width * 0.5f) - 0.5f, (tileEntity.height * 0.5f) - 0.5f);
        }
        tileEntity.view = entityObj.GetComponent<EntityViewComponent>();
        if (tileEntity.view == null)
        {
            if (EntityTypeUtil.IsAnyActor(tileEntity.entityType))
            {
                tileEntity.view = entityObj.AddComponent<ActorView>();
            }
            else if (EntityTypeUtil.IsAnyTower(tileEntity.entityType))
            {
                tileEntity.view = entityObj.AddComponent<TowerView>();
            }else
            {
                tileEntity.view = entityObj.AddComponent<EntityViewComponent>();
            }
        }

        if (EntityTypeUtil.IsAnyBuilding(tileEntity.entityType))//[建筑类]
        {
            //1格以上建筑有地皮
            if (model.tileSize > 2)
            {
                var floorBase = (GameObject)ResourceManager.Instance.LoadAndCreate(model.raceType + "/Entities/Misc/Floor4");//地皮
                tileEntity.view.AddSubView(floorBase, Vector3.zero);
                IsoHelper.FaceToWorldCamera(floorBase.transform);
                IsoHelper.MoveAlongCamera(floorBase.transform, Constants.FLOOR_Z_ORDER);
                floorBase.transform.localScale = Vector3.one * model.tileSize / 4;//REMARK 现有素材占4x4个tilesize
            }
            
            if (EntityTypeUtil.IsBarracks(model))
            {
                tileEntity.blockingRange = 3;
            }
            else
            {
                tileEntity.blockingRange = model.tileSize * 2 - 1;
            }
            tileEntity.AddComponent<ConstructBuildingComponent>();
            if (model.entityType == EntityType.Tower)
            {
                tileEntity.AddComponent<TowerComponent>();
            }
            else if(model.entityType == EntityType.Wall)
            {
                tileEntity.AddComponent<WallComponent>();
            }
            else if (EntityTypeUtil.IsGatherResourceBuilding(model))
            {
                tileEntity.AddComponent<GatherResourceBuildingComponent>();
            }
            else if (EntityTypeUtil.IsStorageResoruceBuilding(model))
            {
                tileEntity.AddComponent<ResourceStorageBuildingComponent>();
            }
            else if (EntityTypeUtil.IsArmyShop(model))
            {
                tileEntity.AddComponent<ProductSoldierBuildingComponent>();
            }
            else if (EntityTypeUtil.IsSkillShop(model))
            {
                tileEntity.AddComponent<ProductSkillBuildingComponent>();
            }
            else if (EntityTypeUtil.IsWorkerHouse(model))
            {
                tileEntity.AddComponent<WorkerHouseComponent>();
            }
            else if (EntityTypeUtil.IsFederal(model))
            {
                tileEntity.AddComponent<FederalComponent>();
            }
            else if (EntityTypeUtil.IsResearch(model))
            {
                tileEntity.AddComponent<ResearchBuildingComponent>();
            }
        }
        else if(EntityTypeUtil.IsAnyActor(model.entityType))   //[角色类]
        {
            //  战斗模式 家园模式 通用组件
            tileEntity.AddComponent<ActorMoveComponent>();
            if (GameWorld.Instance.worldType == WorldType.Home || GameWorld.Instance.worldType == WorldType.Visit) //家园模式 或者 访问模式
            {
                //  家园模式
                //  工人（添加工人组件&添加到工人小屋中）
                if (EntityTypeUtil.IsWorker(model))
                {
                    tileEntity.AddComponent<WorkmanComponent>();
                    var houseComp = IsoMap.Instance.GetWorkerHouseComponent();
                    Assert.Should(houseComp != null, "worker house is not exist...");
                    houseComp.AddAWorkman(tileEntity);
                }
                else
                {
                    tileEntity.AddComponent<ArmyComponent>();
                }
            }
            else                                                                                                    //战斗模式
            {
                tileEntity.AddComponent<GameBufferComponent>();
                //  设置士兵的监视范围（寻找目标用）    REMARK：设定为4.0秒可以移动的范围
                tileEntity.monitorRange = Mathf.Clamp(model.speed * 4.0f, 5.0f, 20.0f);
                if (EntityTypeUtil.IsBombMan(model))
                {
                    tileEntity.monitorRange = 0.0f;     //  暂时不限制炸弹人
                    tileEntity.AddComponent<BombManComponent>();
                }
                else if (EntityTypeUtil.IsCurer(model))
                {
                    tileEntity.AddComponent<CurerComponent>();
                }
                else if (isFriendly)                    //  友军/援军
                {
                    tileEntity.AddComponent<FriendComponent>();
                }
                else
                {
                    tileEntity.AddComponent<ActorComponent>();
                }
                //  除【友军/援军】以外添加拆墙组件（REMARK：友军/援军不可拆墙）
                if (!isFriendly)
                {
                    tileEntity.AddComponent<ActorDestroyWallComponent>();
                }
            }
        }
        else if (EntityTypeUtil.IsAnyTrap(model))   //  [陷阱类]
        {
            tileEntity.AddComponent<TrapComponent>();
        }
        Assert.Should((tileEntity.blockingRange == 0 || (tileEntity.blockingRange % 2) == 1), "invalid blockingRange...");
        Assert.Should(tileEntity.blockingRange <= tileEntity.width - 1 && tileEntity.blockingRange <= tileEntity.height - 1, "invalid blockingRange...");
        return tileEntity;
    }

    /// <summary>
    /// 替换Entity
    /// </summary>
    /// <param name="model"></param>
    /// <returns>替换后的对象</returns>
    public TileEntity ReplaceWith(EntityModel model,BuildingVO buildingVO)
    {
        var newEntity = Create(owner, model);
        newEntity.buildingVO = buildingVO;
        newEntity.SetTilePosition(GetTilePos());
        newEntity.tileOffset = tileOffset;
        newEntity.Init();
        if (IsoMap.Instance.Contains(this))
        {
            IsoMap.Instance.ForceRemoveEntity(this);
            IsoMap.Instance.ForceAddEntity(newEntity);
        }
        Destroy();
        return newEntity;
    }

    public static GameObject LoadAndCreate(EntityModel model)
    {
        //TODO remove this
        var last = model.nameForResource.Substring(model.nameForResource.Length - 2);
        int lastInt;
        if (last != "01" && int.TryParse(last, out lastInt))
        {
            Debug.LogError("由于没有其他等级的模型,暂时使用1级的模型");
            model.nameForResource = model.nameForResource.Replace(last, "01");//因为没有其他资源,暂时都使用1级的资源
        }
        string name = String.Empty;
        if (EntityTypeUtil.IsAnyActor(model.entityType))
        {
            name = "Actors/" + model.nameForResource;
        }else if (model.entityType == EntityType.Wall)
        {
            name = "Walls/" + model.nameForResource;
        }
        else if (EntityTypeUtil.IsAnyTrap(model))
        {
            name = "Trap/" + model.nameForResource;
        }
        else
        {
            name = "Buildings/" + model.nameForResource;
        }
        return (GameObject)ResourceManager.Instance.LoadAndCreate(model.raceType + "/Entities/" + name);
    }
    virtual public void Init()
    {
        if (!inited)
        {
            inited = true;

            view.Init();
            view.transform.position = GetRenderPosition();

            if (EntityTypeUtil.IsFlyable(model.entityType))
            {
                view.body.Translate(0, Constants.FLY_HEIGHT,0,Space.Self);
                IsoHelper.MoveAlongCamera(view.shadow, -2 * Constants.SHADOW_Z_ORDER);//空军的阴影显示在建筑上面
            }

            foreach (var tileEntityComponent in components)
            {
                tileEntityComponent.Init();
            }
        }
    }

    #endregion

    #region 对象的组件相关

    public EntityComponent AddComponent(EntityComponent component)
    {
#if UNITY_EDITOR
        foreach (var entityComponent in components)
        {
            if (entityComponent.GetType() == component.GetType())
            {
                Assert.Fail("添加了相同的组建");
            }
        }
#endif
        components.Add(component);
        component.Entity = this;

        return component;
    }

    public EntityComponent AddComponent<T>() where T : EntityComponent
    {
        return AddComponent(Activator.CreateInstance<T>());
    }

    public T GetComponent<T>() where T : EntityComponent 
    {
        foreach (var entityComponent in components)
        {
            if (entityComponent is T)
                return (T)entityComponent;
        }
        return null;
    }

    public void HandleMessage(EntityMessageType msg, object data=null)
    {
        foreach (var tileEntityComponent in components)
        {
            tileEntityComponent.HandleMessage(msg, data);
        }
        if (this.OnEntityMessage != null)
        {
            this.OnEntityMessage(this, msg, data);
        }
    }

    #endregion

    #region 位置相关

    public Vector3 GetRenderPosition()
    {
        return GetRenderPosition(tilePos, verticalHeight);
    }
    public Vector3 GetRenderPosition(TilePoint pos, float verticalHeight)
    {
        return new Vector3((pos.x + tileOffset.x) + renderTileSizeOffset.x, verticalHeight, (pos.y + tileOffset.y) + renderTileSizeOffset.y);
    }
    public TilePoint GePositionFromRender(Vector3 vect)
    {
        return new TilePoint(Mathf.RoundToInt(vect.x - (tileOffset.x + renderTileSizeOffset.x)), Mathf.RoundToInt(vect.z - (tileOffset.y + renderTileSizeOffset.y)));
    }

    public TilePoint GetTilePos()
    {
        return tilePos;
    }

    public Vector2 GetCurrentPosition()
    {
        return tileOffset + tilePos;
    }

    public Vector2 GetCurrentPositionCenter()
    {
        //  REMARK：对于塔等不可移动建筑 tileOffset 为（0，0）
        return new Vector2(tilePos.x + tileOffset.x + width * 0.5f, tilePos.y + tileOffset.y + height * 0.5f);
    }
    public void SetTilePosition(TilePoint pPos)
    {
        IsoMap.Instance.AdjustEntityCount(this.tilePos, pPos);
        this.tilePos = pPos;
    }

    /// <summary>
    /// 计算自身到指定坐标点的直线距离的平方
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <returns></returns>
    public float DistanceSquareTo(float x1, float y1)
    {
        Vector2 p2 = GetCurrentPositionCenter();
        float dx = (x1 - p2.x);
        float dy = (y1 - p2.y);
        return dx * dx + dy * dy;
    }

    /// <summary>
    /// 判断自身是否处于攻击者的攻击范围内
    /// </summary>
    /// <param name="attacker_x"></param>
    /// <param name="attacker_y"></param>
    /// <param name="range"></param>
    /// <param name="blindrange2"></param>
    /// <returns></returns>
    public bool IsInAttackRange(float attacker_x, float attacker_y, float range, float blindrange2 = 0.0f)
    {
        Vector2 p2 = GetCurrentPositionCenter();
        float dx = (attacker_x - p2.x);
        float dy = (attacker_y - p2.y);
        float diff2 = dx * dx + dy * dy;
        if (diff2 < blindrange2)
            return false;
        range += Mathf.Max(0.5f, this.blockingRange / 2.0f);
        return diff2 <= range * range;
    }

    #endregion

    /// <summary>
    /// 建筑模式下被点击
    /// </summary>
    /// <returns>是否响应了点击事件</returns>
    public bool HandleOnTap()
    {
        var resourceBuildingComponent = GetComponent<BaseResourceBuildingComponent>();
        if(resourceBuildingComponent != null)
            return resourceBuildingComponent.HandleOnTap();
        return false;
    }

    /// <summary>
    /// 获取自身的阵营类型
    /// </summary>
    /// <returns></returns>
    public OwnerType GetOwner()
    {
        return owner;
    }

    /// <summary>
    /// 获取对方的阵营类型
    /// </summary>
    /// <returns></returns>
    public OwnerType GetPeerOwner()
    {
        if(owner == OwnerType.Attacker)
            return OwnerType.Defender;
        return OwnerType.Attacker;
    }

    /// <summary>
    /// 获取自身的战斗目标的阵营
    /// </summary>
    /// <returns></returns>
    public OwnerType GetTargetOwner()
    {
        //  治疗者（获取自身
        if (EntityTypeUtil.IsCurer(model))
        {
            return GetOwner();
        }
        //  其他（获取对方
        else
        {
            return GetPeerOwner();
        }
    }

    public void AddedToWorld()
    {
        Init();
    }

    public void Update(float dt)
    {
        if (view != null)
            view.UpdateView(dt);
        UpdateEntityComponents(dt);
        UpdateDestroy(dt);
    }

    private void UpdateEntityComponents(float dt)
    {
        if (!IsDead())
        {
            foreach (var entityComponent in components)
            {
                if (entityComponent.enabled)
                {
                    entityComponent.Update(dt);
                    if (IsDead())
                        return;
                }
            }
            view.transform.position = GetRenderPosition();
        }
    }

    private void UpdateDestroy(float dt)
    {
        //  死亡了 但尚未置于死亡状态（则释放）
        if (currentHp <= 0 && this.State != EntityStateType.Dead)
        {
            this.State = EntityStateType.Dead;

            if (GameWorld.Instance.worldType == WorldType.Battle || GameWorld.Instance.worldType == WorldType.Replay)
            {
                BattleManager.Instance.OnEntityDestroy(this);
            }
            
            //创建战斗时被销毁的残渣
            if (EntityTypeUtil.IsAnyBuilding(entityType))
            {
                GameObject.Destroy(view.body.gameObject);
                GameObject.Destroy(view.hpBar.gameObject);
                if (view.shadow != null)
                {
                    GameObject.Destroy(view.shadow.gameObject);
                }
                GameEffectManager.Instance.AddEffect("Phouse_die", GetRenderPosition());//TODO
                var destroyView = (GameObject) ResourceManager.Instance.LoadAndCreate("Misc/DestroyedBuildingView");
                view.AddSubView(destroyView, Vector3.zero);
                IsoHelper.FaceToWorldCamera(destroyView.transform);
                IsoHelper.MoveAlongCamera(destroyView.transform, 19);
                Destroy(false);
                IsoMap.Instance.RemoveBuildingMapData(this);//移除占地数据,对象还是在IsoMap中
            }
            else if (EntityTypeUtil.IsAnyActor(entityType))
            {
                GameEffectManager.Instance.AddEffect("Psoldier_die", GetRenderPosition());
                IsoMap.Instance.DelayRemoveEntity(this);
                Destroy();
            }
            else{
                IsoMap.Instance.DelayRemoveEntity(this);
                Destroy();
            }
        }
    }

    public void Destroy(bool destroyView=true)
    {
        if (halfDestroyEffectId > 0)
        {
            GameEffectManager.Instance.RemoveEffect(halfDestroyEffectId);
        }
        this.State = EntityStateType.Dead;
        foreach (var entityComponent in components)
        {
            entityComponent.Destroy();
        }
        if (view != null && destroyView)
        {
            GameObject.Destroy(view.gameObject);
            view = null;
        }
    }
 
    public TileEntity CurrentTarget 
    {
        get
        {
            return currentTarget;
        }
        set
        {
            currentTarget = value; 
        }
    }

    public bool IsDead()
    {
        return (currentHp <= 0 || State == EntityStateType.Dead);
    }

    /// <summary>
    /// 即死
    /// </summary>
    public void Die()
    {
        MakeDamage(currentHp);
    }

    virtual public void MakeDamage(float damage)
    {
#if UNITY_EDITOR
        BattleManager.logDamageList.Add(GameRecord.Frame + ":" + this.model.nameForView + ":" + damage + ":" + currentHp);
#endif  //  UNITY_EDITOR

        //  REMARK：damage如果为负则回血
        currentHp -= damage;
        HandleMessage(EntityMessageType.MakeDamage,damage);
        //  限制不能超过最大maxHp（在回血的情况下）
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        float percent = currentHp / (float)maxHp;
        view.SetHpBarPercent(percent);

        if (halfDestroyEffectId==0 && percent < 0.5f && EntityTypeUtil.IsAnyBuilding(entityType))
        {
            halfDestroyEffectId = GameEffectManager.Instance.AddEffect("Phouse_huo", GetRenderPosition(), true);
        }
    }

    #region PlayAnimation

    public void PlayAnimationFaceToTile(string animationTypeName, TilePoint target, float fps = 0.0f)
    {
        view.PlayAnimation(animationNamePrefix + animationTypeName, GetDir5AndMirrorFlagFromDir8(GetDir8FromScreenAngle(MathUtil2D.GetAngleFromVector(IsoHelper.TileDirectionToScreenDirection(new Vector2(target.x - tilePos.x, target.y - tilePos.y))))), fps);
    }

    public void PlayAnimation(string animationName, float fps = 0.0f)
    {
        view.PlayAnimation(animationNamePrefix + animationName, null, fps);//没有朝向
    }

    public void PlayAnimationAtScreenAngle(string animationName, float screenAngle, float fps = 0.0f)
    {
        PlayAnimationAtDir8(animationName, GetDir8FromScreenAngle(screenAngle), fps);
    }

    public void PlayAnimationAtDir8(string animationName, EntityDirection direction, float fps = 0.0f, Action<string> callback=null)
    {
        view.PlayAnimation(animationNamePrefix + animationName, GetDir5AndMirrorFlagFromDir8(direction), fps, callback);
    }

    public void PlayAnimationTowardsTarget(string animation, TileEntity targeter = null, float fps = 0.0f, Action<string> callback = null)
    {
        if (targeter == null) 
            targeter = CurrentTarget;
        PlayAnimationAtDir8(animation, GetDir8FromTargeter(targeter), fps, callback);
    }

    /// <summary>
    /// 根据8方向获取5方向以及镜像信息
    /// </summary>
    /// <param name="dir8"></param>
    /// <returns></returns>
    protected EntityAnimationDirection GetDir5AndMirrorFlagFromDir8(EntityDirection dir8)
    {
        EntityDirection dir5 = dir8;
        bool mirror = false;
        if (dir5 == EntityDirection.TopLeft)
        {
            dir5 = EntityDirection.TopRight;
            mirror = true;
        }
        else if (dir5 == EntityDirection.Left)
        {
            dir5 = EntityDirection.Right;
            mirror = true;
        }
        else if (dir5 == EntityDirection.BottomLeft)
        {
            dir5 = EntityDirection.BottomRight;
            mirror = true;
        }
        return new EntityAnimationDirection() { direction = dir5, flipX = mirror };
    }

    /// <summary>
    /// 根据自身和目标之间的位置获取8方向之一。
    /// </summary>
    /// <param name="targeter"></param>
    /// <returns></returns>
    public EntityDirection GetDir8FromTargeter(TileEntity targeter)
    {
        var direction = (targeter.GetCurrentPositionCenter() - GetCurrentPositionCenter()).normalized;
        var logicAngle = MathUtil2D.GetAngleFromVector(direction);
        return GetDir8FromLogicAngle(logicAngle);
    }

    /// <summary>
    /// 根据屏幕坐标系角度获取8方向之一。
    /// </summary>
    /// <param name="screenAngle"></param>
    /// <returns></returns>
    protected static EntityDirection GetDir8FromScreenAngle(float screenAngle)
    {
        //  REMARK：45为360的8等分
        return (EntityDirection)(((int)((screenAngle + 22.5f) / 45.0f)) % 8);
    }

    /// <summary>
    /// 根据游戏逻辑坐标系角度获取8方向之一。（※ 游戏角度加45度即屏幕角度）
    /// </summary>
    /// <param name="logicAngle"></param>
    /// <returns></returns>
    protected static EntityDirection GetDir8FromLogicAngle(float logicAngle)
    {
        //  REMARK：如果视角不是45度则这里不太准确。
        return GetDir8FromScreenAngle(logicAngle + 45.0f);
    }

    #endregion PlayAnimation

    /// <summary>
    /// 获取发射位置（REMARK：这里仅处理一个发射位置，如果后期需要有多个，则对应调整。）
    /// </summary>
    /// <returns></returns>
    //public Vector2 GetFirePosition()
    //{
    //    if (view == null)
    //    {
    //        return GetCurrentPositionCenter();
    //    }
    //    List<Transform> trans = view.GetFirePositions();
    //    if (trans == null || trans.Count == 0)
    //    {
    //        return GetCurrentPositionCenter();
    //    }
    //    Vector3 p = trans[0].position;
    //    return new Vector2(p.x, p.z);
    //}

    /// <summary>
    /// 获取效果挂点位置 or null
    /// </summary>
    /// <returns></returns>
    public List<Vector3> effAttachPoint()
    {
        if (view == null)
            return null;
        var ap = view.GetEffAttachPoint();
        if (ap == null)
            return null;
        return ap.RubyMap(t => t.position);
    }

    public void OnSelected()
    {
        NTween.by(view.body.transform, 0.15f, new NTweenParam().gameObject(view.gameObject).prop("y", 1.2f).rewind());
        NTween.to(view.body.transform, 0.15f, new NTweenParam().gameObject(view.gameObject).prop("scale", 1.1f).rewind());
        //闪烁颜色
        foreach (var childSprite in view.body.GetComponentsInChildren<tk2dSprite>())
        {
            NTween.to(childSprite, 1f, new NTweenParam().gameObject(view.gameObject).prop("color", new Color(0.5f, 0.5f, 0.5f)).rewind().loop());
        }

        BuildOptManager.Instance.ShowBuildingOptWin(this);

        
        if (model.range > 0)
        {
            if (!rangeView)
            {
                rangeView =
                    ((GameObject) ResourceManager.Instance.LoadAndCreate("Misc/RangeView")).GetComponent<RangeView>();
            }
            rangeView.SetEntity(this);
            rangeView.transform.localScale = Vector3.zero;
            NTween.to(rangeView, 0.15f, new NTweenParam().gameObject(rangeView.gameObject).prop("scale", 1));
        }
    }

    public void OnUnselected()
    {
        NTween.killTweensByHost(view.gameObject, true);
        foreach (var childSprite in view.body.GetComponentsInChildren<tk2dSprite>())
        {
            childSprite.color = Color.white;
        }
        view.body.transform.localPosition = Vector3.zero;
        view.body.transform.localScale = Vector3.one;

        BuildOptManager.Instance.CloseBuildOptWin();

        if (model.range > 0)
        {
            NTween.to(rangeView, 0.15f, new NTweenParam().gameObject(rangeView.gameObject).prop("scale", 0));
        }
    }

    /// <summary>
    /// 显示or隐藏对象
    /// </summary>
    public void ShowEntity()
    {
        Assert.Should(view != null);
        view.gameObject.SetActive(true);
    }

    public void HideEntity()
    {
        Assert.Should(view != null);
        view.gameObject.SetActive(false);
    }

    public bool IsVisible()
    {
        Assert.Should(view != null);
        return view.gameObject.activeSelf;
    }
}

