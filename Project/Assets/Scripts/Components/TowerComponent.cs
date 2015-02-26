using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// component 'BaseTower'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
public class TowerComponent : AiCompponent
{
    protected override List<TileEntity> TryLockTargeters(out TilePoint? _targetPos, out LinkedList<IMoveGrid> _targetRoute)
    {
        _targetPos = null;  //  不移动
        _targetRoute = null;

        List<TileEntity> targets = IsoMap.Instance.GetEntitiesByRange(this.Entity, Entity.GetTargetOwner());
        if (targets.Count == 0)
            return null;

        //  从备选列表中筛选最近的目标
        List<TileEntity> nearest_targets = FindTargetsNearestLinear(Entity.GetCurrentPositionCenter(), targets, Entity.model.numTarget);
        Assert.Should(nearest_targets != null);
        return nearest_targets;
    }

    protected override void DoActionIdle()
    {
        //m_tempTargeters = null;
        //Entity.State = EntityStateType.Idle;
        //m_timePassed = BattleRandom.Range(3f, 10f);

        AuxRecycleAllBulletLoopEffect();

        m_tempTargeters = null;
        Entity.State = EntityStateType.Idle;
        m_timePassed = BattleRandom.Range(0.5f, 1.5f);

#if UNITY_EDITOR
        BattleManager.logWaitTime.Add(GameRecord.Frame + ":timePassed:" + m_timePassed);
#endif  //  UNITY_EDITOR

        if (Entity.AimTarget)
        {
            //  背向地图中央 
            var center = new Vector2(Constants.WIDTH / 2, Constants.HEIGHT / 2);
            var dir = Entity.GetCurrentPositionCenter() - center;
            var angle = MathUtil2D.GetAngleFromVector(IsoHelper.TileDirectionToScreenDirection(dir));

            Entity.PlayAnimationAtScreenAngle(AnimationNames.Stand, BattleRandom.Range(angle - 45, angle + 45));
        }
        else
        {
            Entity.PlayAnimation(AnimationNames.Stand);
        }
    }
}
