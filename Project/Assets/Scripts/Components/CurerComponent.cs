using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurerComponent : AiCompponent
{
    protected override List<TileEntity> TryLockTargeters(out TilePoint? _targetPos, out LinkedList<IMoveGrid> _targetRoute)
    {
        _targetPos = null;
        _targetRoute = null;

        //  查找医疗兵【监视范围】内目标（没目标则不进行任何行为了）
        var selfPos = Attacker.GetCurrentPositionCenter();
        List<TileEntity> targetersInWatchRange = IsoMap.Instance.GetEntitiesByRange(Attacker, Entity.GetTargetOwner(), selfPos.x, selfPos.y, Entity.model.cureRange, 0.0f, MyTargeterFilter);
        if (targetersInWatchRange.Count == 0)
            return null;

        //  查找【治疗范围】内目标
        List<TileEntity> targetersInCureRange = new List<TileEntity>();
        foreach (var tar in targetersInWatchRange)
        {
            if (tar.IsInAttackRange(selfPos.x, selfPos.y, Attacker.model.range, 0.0f))
                targetersInCureRange.Add(tar);
        }

        if (targetersInCureRange.Count == 0)
        {
            //  移动到监视范围内最近的目标 REMARK：对于医疗兵搜索的目标这里都设置为1。
            var targeters = FindTargetsNearestLinear(selfPos, targetersInWatchRange, 1);
            var moveTargeter = targeters[0];

            //  REMARK：以下计算假设治疗兵都是飞行的（否则不正确）

            //  计算目标位置（从目标位置到自身位置 减去 治疗范围）
            var tarPos = moveTargeter.GetCurrentPositionCenter();
            var vdir = (selfPos - tarPos).normalized;
            var newPos = tarPos + vdir * Mathf.Max(Entity.model.range - 1.5f, 0);   //  REMARK：减去单位1.5f为了移动后位置在治疗范围内（避免在vector转grid的时候出错，grid的斜边为1.414）

            //  移动：设置移动目标
            _targetPos = new TilePoint((int)newPos.x, (int)newPos.y);
            return targeters;
        }
        else
        {
            //  不移动：从备选列表中筛选最近的目标 REMARK：对于医疗兵搜索的目标这里都设置为1。
            return FindTargetsNearestLinear(selfPos, targetersInCureRange, 1);
        }
    }
    
    private bool MyTargeterFilter(TileEntity tar)
    {
        //  过滤掉治疗兵
        return EntityTypeUtil.IsCurer(tar.model);
    }

    protected override bool IsCancelMove()
    {
        //  所有目标都进入攻击范围了则停止移动
        return (base.IsCancelMove() || AuxIsAllTargeterInAttackRange(Attacker));
    }
}
