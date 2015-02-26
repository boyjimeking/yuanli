using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 友军/援军组件
/// </summary>
public class FriendComponent : AiCompponent
{
    protected override List<TileEntity> TryLockTargeters(out TilePoint? _targetPos, out LinkedList<IMoveGrid> _targetRoute)
    {
        _targetPos = null;
        _targetRoute = null;

        //  获取全部目标
        List<TileEntity> allTargeters = new List<TileEntity>();
        foreach (var entity in IsoMap.Instance.GetAllEntitiesByOwner(Entity.GetTargetOwner()))
        {
            if (entity.IsDead() || !EntityTypeUtil.IsAnyActor(entity.entityType))
                continue;

            //  不是限定类型的目标则过滤掉
            if (EntityTypeUtil.IsAnyActor(entity.entityType) && Attacker.model.onlyAttackTargetType != EntityType.None && Attacker.model.onlyAttackTargetType != entity.entityType)
                continue;

            allTargeters.Add(entity);
        }
        if (allTargeters.Count == 0)
            return null;

        //  筛选1个直线最近的目标
        var nearest_targets = FindTargetsNearestLinear(Attacker.GetCurrentPositionCenter(), allTargeters, 1);
        var targeter = nearest_targets[0];
        Vector2 p = targeter.GetCurrentPositionCenter();
        _targetPos = new TilePoint((int)p.x, (int)p.y);
        return nearest_targets;
    }
    
    protected override bool IsCancelMove()
    {
        //  所有目标都进入攻击范围了则停止移动
        return (base.IsCancelMove() || AuxIsAllTargeterInAttackRange(Attacker));
    }
}
