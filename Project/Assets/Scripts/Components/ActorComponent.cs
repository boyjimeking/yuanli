#define REALTIME_AI

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rubystyle;

/// <summary>
/// component 'ActorLogic'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/ActorLogic")]
public class ActorComponent : AiCompponent
{
    //  移动中对监视范围的目标已经计算过Dijkstra的目标进行缓存
    private Dictionary<TileEntity, bool> _movingDijkstraCache = new Dictionary<TileEntity, bool>();
    private float _moveTimePassed = 0.0f;

    /// <summary>
    /// 指定列表中是否存在优先目标（目标类型和自身攻击类型相同则为优先目标）
    /// </summary>
    /// <param name="targeters"></param>
    /// <returns></returns>
    private bool IsExistedImportantTargeter(List<TileEntity> targeters)
    {
        return targeters.RubyAny(targeter => { return (EntityAiType)targeter.model.entityType == Attacker.aiType; });
    }

    /// <summary>
    /// 刷新重要目标（目标变更返回true，否则返回false。）
    /// </summary>
    /// <returns></returns>
    private bool RefreshImportantTargetersInMonitorRange()
    {
        //  获取监视范围内目标
        Vector2 p = Attacker.GetCurrentPositionCenter();
        List<TileEntity> targets = IsoMap.Instance.GetEntitiesByRange(Attacker, Entity.GetTargetOwner(), p.x, p.y, Attacker.monitorRange, 0);
        if (targets.Count == 0)
            return false;

        //  没有优先目标
        if (!IsExistedImportantTargeter(targets))
            return false;

        //  是否是新出现在监视范围内的目标（缓存有任意一个没命中则为新出现目标）
        if (!targets.RubyAny(tar => { return !_movingDijkstraCache.ContainsKey(tar); }))
            return false;

        //  ※ 对新目标求最优（最小时间 or 最近距离）
        TilePoint? tempTargetPos;
        LinkedList<IMoveGrid> tempMoveRoute;
        List<TileEntity> tempTargeters = SelectTargeters(targets, out tempTargetPos, out tempMoveRoute);

        //  缓存
        foreach (var tar in targets)
        {
            if (!_movingDijkstraCache.ContainsKey(tar))
                _movingDijkstraCache.Add(tar, true);
        }

        //  比较新目标和现在目标是否相同
        foreach (var tar1 in tempTargeters)
        {
            foreach (var tar2 in m_tempTargeters)
            {
                //  发现更优目标
                if (tar1 != tar2)
                {
                    m_tempTargeters = tempTargeters;
                    m_tempTargetPos = tempTargetPos;
                    m_tempMoveRoute = tempMoveRoute;
                    return true;
                }
            }
        }
        return false;
    }

    protected override void UpdateMoving(float dt)
    {
        if (Attacker.aiType == EntityAiType.PriorToResource || Attacker.aiType == EntityAiType.PriorToTower)
        {
            _moveTimePassed += dt;
            if (_moveTimePassed >= 1.5f)
            {
                _moveTimePassed = 0.0f;
                if (RefreshImportantTargetersInMonitorRange())
                {
                    StopMoveAndTryAction();
                    return;
                }
            }
        }
        base.UpdateMoving(dt);
    }

    /// <summary>
    /// 尝试锁定更重要目标（锁定优先目标的前提是已经有目标了）
    /// </summary>
    protected override void TryLockMoreImportantTargeters()
    {
        Assert.Should(m_tempTargeters != null);

        //  自身没有优先选择则直接返回了
        if (Attacker.aiType != EntityAiType.PriorToResource && Attacker.aiType != EntityAiType.PriorToTower)
            return;

        //  当前的目标已经是优先目标了也直接返回了
        if (IsExistedImportantTargeter(m_tempTargeters))
            return;

        //  当前的目标不是最优先的目标（尝试寻找优先级更高的目标）
        List<TileEntity> newTargets = IsoMap.Instance.GetEntitiesByTT(Attacker.GetTargetOwner(), Attacker.aiType, Attacker);
        if (newTargets.Count == 0)
            return;

        //  ※ 设置新的目标~ （最小时间 or 最近距离）
        m_tempTargeters = SelectTargeters(newTargets, out m_tempTargetPos, out m_tempMoveRoute);
    }

    protected override List<TileEntity> TryLockTargeters(out TilePoint? _targetPos, out LinkedList<IMoveGrid> _targetRoute)
    {
        _targetPos = null;
        _targetRoute = null;

        var allTargeters = IsoMap.Instance.GetAllEntitiesByOwner(Attacker.GetTargetOwner()).RubySelect(tar =>
        {
            if (tar.IsDead())
                return false;

            //  不是限定类型的目标则过滤掉
            if (EntityTypeUtil.IsAnyActor(tar.entityType) && Attacker.model.onlyAttackTargetType != EntityType.None && Attacker.model.onlyAttackTargetType != tar.entityType)
                return false;

            return true;
        });
        if (allTargeters.Count == 0)
            return null;

        List<TileEntity> targets = null;

        //  1、有优先目标的士兵先筛选优先目标
        if (Entity.aiType != EntityAiType.Other) targets = allTargeters.RubySelect(tar => { return tar.entityType == (EntityType)Entity.aiType; });

        //  2、没有优先目标 or 优先目标为空 则优先筛选援军
        if (targets == null || targets.Count == 0) targets = allTargeters.RubySelect(tar => { return tar.Friendly; });

        //  3、没有援军则筛选默认的非墙建筑
        if (targets.Count == 0) targets = allTargeters.RubySelect(tar => { return EntityTypeUtil.IsAnyNonWallBuilding(tar.entityType); });

        //  4、未找到目标直接返回
        if (targets.Count == 0)
            return null;

#if REALTIME_AI
        //float t1 = UnityEngine.Time.realtimeSinceStartup;
        //for (int i = 0; i < 5000; i++)
        //{
        //    FindTargetsNearestTimeCost(Attacker, targets);
        //}
        //float t2 = UnityEngine.Time.realtimeSinceStartup;
        //Debug.Log(t2 - t1);
        //Debug.Break();

        //  筛选最近目标（最小时间 or 最近距离）
        return SelectTargeters(targets, out _targetPos, out _targetRoute);
#else
        //  从备选列表中筛选最近的目标
        IEnumerator<IsoGridTarget> iterator = FindTargetsNearestTimeCost(Attacker, targets);
        if (iterator == null)
            return null;

        //  延迟处理
        DelayManager.Instance.AddDelayAi<IsoGridTarget>(Attacker, OnDelayLockTargetersCompleted, iterator);
        Entity.State = EntityStateType.Thinking;
        return null;
#endif

    }

    /// <summary>
    /// 延迟计算回调函数
    /// </summary>
    /// <param name="delayAI"></param>
    private void OnDelayLockTargetersCompleted(DelayAiObject<IsoGridTarget> delayAI)
    {
        m_tempTargetPos = null;
        m_tempMoveRoute = null;
        //Entity.State = EntityStateType.Thinking;
        IsoGridTarget targetInfos = delayAI.result;
        Assert.Should(targetInfos != null && targetInfos.MoveRoute != null);
        if (targetInfos.MoveRoute.Count > 1)
        {
            m_tempMoveRoute = targetInfos.MoveRoute;
        }
        m_tempTargeters = AuxConvertToList(targetInfos.Targeter);

        //  执行行动
        DoAction();
    }

    protected override void DoActionMove()
    {
        base.DoActionMove();
        if (m_tempTargeters != null)
        {
            _movingDijkstraCache.Clear();
            _moveTimePassed = 0.0f;
            foreach (var tar in m_tempTargeters)
                _movingDijkstraCache.Add(tar, true);
        }
    }

    protected override bool IsCancelMove()
    {
        //  所有目标都进入攻击范围了则停止移动
        return (base.IsCancelMove() || AuxIsAllTargeterInAttackRange(Attacker));
    }

    /// <summary>
    /// 重新锁定目标
    /// </summary>
    /// <returns></returns>
    protected override bool IsRelockTargeters()
    {
        //  没有优先目标的士兵 && 当前目标里没有援军 && 援军出现（则立刻重新锁定目标 不用等待当前目标死亡）
        if (Attacker.aiType == EntityAiType.Other && !AuxIsAnyFriend())
        {
            var targets = IsoMap.Instance.GetEntitiesByTT(Attacker.GetTargetOwner(), EntityAiType.Other, Attacker, true);
            if (targets.Count > 0 && targets.RubyAny(tar => { return tar.Friendly; }))
                return true;
        }
        //  默认判断
        return base.IsRelockTargeters();
    }

    /// <summary>
    /// 从目标列表里根据距离时间等权重筛选最有目标
    /// </summary>
    /// <param name="targeters"></param>
    /// <param name="_targetPos"></param>
    /// <param name="_targetRoute"></param>
    /// <returns></returns>
    private List<TileEntity> SelectTargeters(List<TileEntity> targeters, out TilePoint? _targetPos, out LinkedList<IMoveGrid> _targetRoute)
    {
        _targetPos = null;
        _targetRoute = null;

        //  先在监视范围内寻找时间消耗最低的目标
        IsoGridTarget targetInfos = FindTargetsNearestTimeCost(Attacker, targeters);
        if (targetInfos != null)
        {
            Assert.Should(targetInfos.MoveRoute != null);
            if (targetInfos.MoveRoute.Count > 1)
            {
                _targetRoute = targetInfos.MoveRoute;
            }
            return AuxConvertToList(targetInfos.Targeter);
        }

        //  未找到则直接寻找直线最近的目标
        var nearest_targets = FindTargetsNearestLinear(Attacker.GetCurrentPositionCenter(), targeters, 1);
        var targeter = nearest_targets[0];
        Vector2 p = targeter.GetCurrentPositionCenter();
        //int randomOffset = Mathf.Min((int)(targeter.blockingRange / 2.0f + Attacker.model.range), targeter.width / 2);  //  TODO：如果该值过大导致移动结束后目标不在攻击范围内（则会导致重新锁定目标） 则需要酌情调整该值算法
        int randomOffset = Mathf.Min((int)Attacker.model.range, targeter.width / 2);
        int x = (int)p.x;
        int y = (int)p.y;
        if (randomOffset > 0)
        {
            x = BattleRandom.Range(x - randomOffset, x + randomOffset);
            y = BattleRandom.Range(y - randomOffset, y + randomOffset);
        }
        _targetPos = new TilePoint(x, y);
        return nearest_targets;
    }
}
