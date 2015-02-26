using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManComponent : AiCompponent
{
    protected override List<TileEntity> TryLockTargeters(out TilePoint? _targetPos, out LinkedList<IMoveGrid> _targetRoute)
    {
        _targetPos = null;
        _targetRoute = null;

        ///<    有目标了直接返回
        if (m_tempTargeters != null)
            return m_tempTargeters;

        ///<    方案1：计算全地图目标时间最近（并且有墙的路线）REMARK：有墙基本就可以理解为封闭区域
        Dictionary<TilePoint, IsoGridTarget> gridTargets = new Dictionary<TilePoint, IsoGridTarget>();
        var targets = IsoMap.Instance.GetEntitiesByTT(Attacker.GetTargetOwner(), EntityAiType.Other, Attacker);
        foreach (var tar in targets)
        {
            Vector2 p = tar.GetCurrentPositionCenter();
            TilePoint grid = new TilePoint((int)p.x, (int)p.y);
            IsoGridTarget tarInfos = new IsoGridTarget() { Targeter = tar, Distance = 0, X = grid.x, Y = grid.y };
            gridTargets.Add(grid, tarInfos);
        }
        LinkedList<IMoveGrid> route = IsoMap.Instance.SearchDijkstraNearestWall(Attacker, gridTargets);
        if (route != null)
        {
            Assert.Should(route.Count >= 2);
            //  墙的位置
            IMoveGrid wallGrid = route.Last.Value;
            route.RemoveLast();
            //  墙的前一格
            IMoveGrid lastGrid = route.Last.Value;
            //  获取墙
            TileEntity wallTargeter = IsoMap.Instance.GetWallTargeter(lastGrid.X, lastGrid.Y, wallGrid.X, wallGrid.Y);
            Assert.Should(wallTargeter != null);
            //  返回
            _targetRoute = route;
            return AuxConvertToList(wallTargeter);
        }

        ///<    方案2：求最近的墙o.o
        TileEntity targeter = GetWallEntityNearest(Attacker);
        if (targeter == null)
            return null;

        //  查找移动目标点（墙四周离自身最近的点）
        int self_x = Entity.GetTilePos().x;
        int self_y = Entity.GetTilePos().y;

        Vector2 c = targeter.GetCurrentPositionCenter();
        int wall_x = (int)c.x;
        int wall_y = (int)c.y;
        int wall_w = 1; //   REMARK：连接器到墙中心的距离

        int mindiff = 999999;
        int goal_x = -1;
        int goal_y = -1;

        //  依次为 左上、右上、左下、右下
        DetectNearestGrid(ref mindiff, ref goal_x, ref goal_y, self_x, self_y, wall_x, wall_y + wall_w);
        DetectNearestGrid(ref mindiff, ref goal_x, ref goal_y, self_x, self_y, wall_x + wall_w, wall_y);
        DetectNearestGrid(ref mindiff, ref goal_x, ref goal_y, self_x, self_y, wall_x - wall_w, wall_y);
        DetectNearestGrid(ref mindiff, ref goal_x, ref goal_y, self_x, self_y, wall_x, wall_y - wall_w);

        //  找到目标点 并且 目标点位置不是自身位置
        if (goal_x >= 0 && goal_y >= 0 && ((goal_x != self_x || goal_y != self_y)))
        {
            _targetPos = new TilePoint(goal_x, goal_y);
        }

        return AuxConvertToList(targeter);
    }

    protected override void DoActionAttack()
    {
        m_timePassed = 0.0f;
        Entity.State = EntityStateType.Attacking;
    }

    /// <summary>
    /// [覆盖] 处理攻击过程（不沿用父类的流程）
    /// </summary>
    /// <param name="dt"></param>
    protected override void UpdateAttacking(float dt)
    {
        m_timePassed += dt;
        if (m_timePassed >= Entity.model.rate)
        {
            //  伤害处理
            ProcessDamagePoint(Entity.GetCurrentPositionCenter());

            //  爆炸显示效果
            GameEffectManager.Instance.AddEffect(Entity.model.hitEffectName, Entity.view.transform.position);

            //  自身死亡
            Entity.Die();
        }
    }

    private void DetectNearestGrid(ref int minDiff, ref int goalX, ref int goalY, int selfX, int selfY, int testX, int testY)
    {
        if (IsoMap.Instance.IsPassableStrict(testX, testY))
        {
            int diff = Math.Abs(testX - selfX) + Math.Abs(testY - selfY);
            if (diff <= minDiff)
            {
                minDiff = diff;
                goalX = testX;
                goalY = testY;
            }
        }
    }

    /// <summary>
    /// 获取离自身最近的墙对象（炸弹人用）
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public TileEntity GetWallEntityNearest(TileEntity self)
    {
        var allEntities = IsoMap.Instance.GetAllEntitiesByOwner(self.GetTargetOwner());

        int selfx = self.GetTilePos().x;
        int selfy = self.GetTilePos().y;
        int mindiff = 999999;
        TileEntity target = null;
        foreach (var entity in allEntities)
        {
            if (entity.IsDead())
                continue;

            if (entity.entityType == EntityType.Wall)
            {
                Vector2 c = entity.GetCurrentPositionCenter();
                int x = (int)c.x;
                int y = (int)c.y;
                int diff = Math.Abs(selfx - x) + Math.Abs(selfy - y);
                if (diff <= mindiff)
                {
                    mindiff = diff;
                    target = entity;
                }
            }
        }

        return target;
    }
}
