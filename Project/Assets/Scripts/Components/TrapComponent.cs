using System;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;
using UnityEngine;

public class TrapComponent : AiCompponent
{
    private float m_timePassed = 0.0f;
    private Action<float> m_FlyTrapUpdateProc = null;
    private TileEntity m_currTraceTargeter = null;

    public override void Init()
    {
        base.Init();
        if (Entity.buildingVO.trapBuildingVO == null)
        {
            Entity.buildingVO.trapBuildingVO = new TrapBuildingVO(){broken = false};
        }
        if (Entity.buildingVO.trapBuildingVO.broken)
        {
            enabled = false;
            Entity.view.SwitchBody(2);//broken view
        }
        else
        {
            enabled = true;
            Entity.view.SwitchBody(1);
        }
        if (GameWorld.Instance.worldType == WorldType.Home || GameWorld.Instance.worldType == WorldType.Visit)
        {
            enabled = false;
            EventDispather.AddEventListener(GameEvents.TRAP_REFILL,OnTrapRefill);
        }
        else
        {
            Entity.HideEntity(); //  陷阱初始不可见
        }
    }

    public override void Destroy()
    {
        EventDispather.RemoveEventListener(GameEvents.TRAP_REFILL, OnTrapRefill);
        base.Destroy();
    }

    private void OnTrapRefill(string eventtype, object obj)
    {
        var buildingVo = (BuildingVO) obj;
        if (buildingVo == null)//refill all
        {
            Init();     //reinit
        }else if (buildingVo.sid == Entity.buildingVO.sid)
        {
            Init();
        }
    }

    /// <summary>
    /// 重置花费
    /// </summary>
    public ResourceVO RefillCost
    {
        get
        {
            var model = DataCenter.Instance.FindEntityModelById(Entity.buildingVO.cid);
            return new ResourceVO()
            {
                resourceType = ResourceType.Gold,
                resourceCount = model.refillCostResourceCount
            };
        }
    }

    /// <summary>
    /// 全部重置花费
    /// </summary>
    public ResourceVO RefillCostAll
    {
        get
        {
            var resource = new ResourceVO();
            foreach (var buildingVo in DataCenter.Instance.Defender.buildings)
            {
                if (buildingVo.trapBuildingVO != null && buildingVo.trapBuildingVO.broken)
                {
                    var model = DataCenter.Instance.FindEntityModelById(buildingVo.cid);
                    resource.resourceType = ResourceType.Gold;
                    resource.resourceCount += model.refillCostResourceCount;
                }
            }
            return resource;
        }
    }
    /// <summary>
    /// 请求重置
    /// </summary>
    /// <param name="refillAll"></param>
    public void Refill(bool refillAll)
    {
        if (refillAll)
        {
            new TrapRefillCommand(TrapRefillReq.RefillType.All, null).ExecuteAndSend();
        }
        else
        {
            new TrapRefillCommand(TrapRefillReq.RefillType.Single, Entity.buildingVO).ExecuteAndSend();
        }
    }

    public override void Update(float dt)
    {
        //  更新飞天陷阱
        if (m_FlyTrapUpdateProc != null)
        {
            m_FlyTrapUpdateProc(dt);
            return;
        }

        //  可见-已经触发
        if (Entity.IsVisible())
        {
            m_timePassed += dt;
            if (m_timePassed >= Entity.model.rate)
            {
                if (EntityTypeUtil.IsFlyTrap(Entity.model))
                {
                    m_FlyTrapUpdateProc = UpdateFlyTrace;
                }
                else
                {
                    ProcessExplode(false);
                }
            }
        }
        //  不可见-检测是否触发
        else if (IsTriggered())
        {
            Entity.ShowEntity();
        }
    }

    private void ProcessExplode(bool fly)
    {
        //  伤害处理（溅射范围为0则针对单个目标，否则根据溅射范围计算。）
        if (Entity.model.splashRange > 0)
        {
            //  飞天陷阱根据body计算位置、普通陷阱获取自身位置。
            Vector2 p;
            if (fly)
            {
                var currPosition = Entity.view.body.position;
                p = new Vector2(currPosition.x, currPosition.z);
            }
            else
            {
                p = Entity.GetCurrentPositionCenter();
            }
            GameDamageManager.ProcessDamageMultiTargeters(FindTargetersInSplash(p.x, p.y), Entity.model, Entity);
        }
        else
        {
            GameDamageManager.ProcessDamageOneTargeter(m_currTraceTargeter, Entity.model, Entity);
        }

        //  爆炸显示效果
        GameEffectManager.Instance.AddEffect(Entity.model.hitEffectName, fly ? Entity.view.body.position : Entity.view.transform.position);

        //  自身死亡
        Entity.Die();

        //  改变数据
        Entity.buildingVO.trapBuildingVO.broken = true;
    }

    private bool IsHitted(Vector2 targetPos, Vector2 currPos, Vector2 vdir, Vector2 vadd)
    {
        //  REMARK：命中计算参考子弹部分
        float dx = (targetPos.x - currPos.x);
        float dy = (targetPos.y - currPos.y);

        float radius = Mathf.Max(0.5f, m_currTraceTargeter.blockingRange / 2.0f);

        if (dx * dx + dy * dy <= radius * radius)
        {
            return true;
        }
        else
        {
            float len1 = vdir.sqrMagnitude;
            float len2 = vadd.sqrMagnitude;
            if (len2 >= len1)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 更新追踪阶段
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateFlyTrace(float dt)
    {
        Vector2 targetPos = m_currTraceTargeter.GetCurrentPositionCenter();

        var currPosition = Entity.view.body.position;
        var currPosition2 = new Vector2(currPosition.x, currPosition.z);

        float dis = Entity.model.bulletSpeed * dt;
        Vector2 vdir = targetPos - currPosition2;
        Vector2 vadd = vdir.normalized * dis;

        //  更新水平位置
        currPosition2 += vadd;

        //  更新 body 位置
        Entity.view.body.position = new Vector3(currPosition2.x, Mathf.Min(currPosition.y + dis, Constants.FLY_HEIGHT), currPosition2.y);

        //  命中
        if (IsHitted(targetPos, currPosition2, vdir, vadd))
        {
            ProcessExplode(true);
        }
    }

    private bool IsTriggered()
    {
        var selfPos = Entity.GetCurrentPositionCenter();
        var targetersInRange = IsoMap.Instance.GetEntitiesByRange(Entity, Entity.GetTargetOwner(), selfPos.x, selfPos.y, Entity.model.range, 0.0f);
        if (targetersInRange.Count > 0)
        {
            m_currTraceTargeter = targetersInRange[0];
            return true;
        }
        return false;
    }

    //private bool MyTargeterFilter(TileEntity tar)
    //{
    //    //  过滤掉 飞行兵（REMARK：陷阱对飞行兵种无效）
    //    return EntityTypeUtil.IsFlyable(tar.entityType);
    //}

    protected override List<TileEntity> TryLockTargeters(out TilePoint? _targetPos, out LinkedList<IMoveGrid> _targetRoute)
    {
        _targetPos = null;
        _targetRoute = null;

        //  REMARK：陷阱不锁定目标（故什么也不做）

        return null;
    }
}
