using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rubystyle;

/// <summary>
/// 军队空闲时组件（在家园建造模式下使用）
/// </summary>
public class ArmyComponent : EntityComponent
{
    private TileEntity m_camp = null;
    private TilePoint m_campTilePos;
    private Rect m_campRect;
    private List<TilePoint> m_passableGrids = null;
    private float m_timePassed = 0.0f;

    public override void Update(float dt)
    {
        switch (Entity.State)
        {
            case EntityStateType.Idle: UpdateIdle(dt); break;
            case EntityStateType.Moving: UpdateMoving(dt); break;
            default:
                break;
        }
    }

    private void UpdateIdle(float dt)
    {
        //  发呆中
        if (m_timePassed > 0)
        {
            m_timePassed -= dt;
            if (m_timePassed > 0)
                return;
        }

        //  尝试移动
        TryMove();
    }

    private void TryMove()
    {
        //  尝试移动之前先初始化军营（如果尚未初始化）
        if (!InitCamp())
        {
            m_timePassed = 3.0f;   //  REMARK：没军营？老实呆着吧o.o
            return;
        }

        //  尝试移动
        if (m_passableGrids != null && m_passableGrids.Count > 0)
        {
            TilePoint grid = RandomGrid();
            if (grid == Entity.GetTilePos())
            {
                DoActionIdle();
                return;
            }
            if (!DoActionMove(grid))
            {
                DoActionIdle();
                return;
            }
        }
        else
        {
            DoActionIdle();
        }
    }

    private void UpdateMoving(float dt)
    {
        //  什么也不做
    }

    private void OnMoveCompleteEvent(ActorMoveComponent moveComp)
    {
        moveComp.OnMoveCompleteEvent -= OnMoveCompleteEvent;

        if (IsInCamp())
        {
            //  移动结束后在军营内（则待机一会）
            //DoActionIdle(1.0f, 5.0f);
            DoActionIdle(3.0f, 15.0f);  //  TODO：调整为根据总士兵数相关更好，总兵数越多每个的等待时间就可以越长。
        }
        else
        {
            //  移动结束后仍然不在军营内（移动过程中军营搬家了o(╯□╰)o）则继续移动
            TryMove();
        }
    }

    private TilePoint RandomGrid()
    {
        float mindiff = Mathf.Max(Entity.model.speed * 2.0f, 1.5f);
        float mindiff2 = mindiff * mindiff;
        List<TilePoint> recommendedGrids = new List<TilePoint>();
        foreach (var grid in m_passableGrids)
        {
            float diff2 = Entity.DistanceSquareTo(grid.x, grid.y);
            if (diff2 >= mindiff2)
            {
                recommendedGrids.Add(grid);
            }
        }

        if (recommendedGrids.Count > 0)
        {
            return recommendedGrids[BattleRandom.Range(0, recommendedGrids.Count)];
        }
        else
        {
            return m_passableGrids[BattleRandom.Range(0, m_passableGrids.Count)];
        }
    }

    private bool DoActionMove(TilePoint targetPos)
    {
        ActorMoveComponent move = Entity.GetComponent<ActorMoveComponent>();
        if (move != null)
        {
            if (move.StartMove(targetPos))
            {
                move.OnMoveCompleteEvent += OnMoveCompleteEvent;
                return true;
            }
        }
        return false;
    }

    private void DoActionIdle(float min = 0.5f, float max = 1.5f)
    {
        Entity.State = EntityStateType.Idle;
        m_timePassed = BattleRandom.Range(min, max);
        Entity.PlayAnimation(AnimationNames.Stand);
    }

    /// <summary>
    /// 士兵是否在军营内
    /// </summary>
    /// <returns></returns>
    private bool IsInCamp()
    {
        Assert.Should(m_camp != null);
        return m_campRect.Contains(Entity.GetCurrentPositionCenter());
    }

    /// <summary>
    /// 初始化军营
    /// </summary>
    private bool InitCamp()
    {
        if (m_camp == null)
        {
            m_camp = FindCampEntity();
            if (m_camp == null)
                return false;

            //  军营区域
            RefreshCampGridData(m_camp.GetTilePos());
            return true;
        }
        else
        {
            //  位置变更了（拖拽等）
            TilePoint campPos = m_camp.GetTilePos();
            if (m_campTilePos != campPos)
            {
                RefreshCampGridData(campPos);
            }
            return true;
        }
    }

    /// <summary>
    /// 刷新军营格子数据等
    /// </summary>
    /// <param name="campPos"></param>
    private void RefreshCampGridData(TilePoint campPos)
    {
        //  记录军营当前位置
        m_campTilePos = campPos;

        int x = m_campTilePos.x;
        int y = m_campTilePos.y;
        int w = m_camp.width;
        int h = m_camp.height;
        m_campRect = new Rect(x, y, w, h);

        //  不可通行区域
        int blkBgnX, blkEndX, blkBgnY, blkEndY;
        int blocking = m_camp.blockingRange;
        if (blocking > 0)
        {
            int offset_x = (int)((w + 1 - blocking) / 2);
            int offset_y = (int)((h + 1 - blocking) / 2);
            blkBgnX = x + offset_x;
            blkEndX = blkBgnX + blocking - 1;
            blkBgnY = y + offset_y;
            blkEndY = blkBgnY + blocking - 1;
        }
        else
        {
            blkBgnX = blkEndX = blkBgnY = blkEndY = -1;
        }

        //  获取可通行区域
        if (m_passableGrids != null)
        {
            m_passableGrids.Clear();
        }
        else
        {
            m_passableGrids = new List<TilePoint>();
        }
        for (int i = x; i <= x + w; i++)
        {
            for (int j = y; j <= y + h; j++)
            {
                //  不在不可通行区域
                if (i < blkBgnX || i > blkEndX || j < blkBgnY || j > blkEndY)
                {
                    m_passableGrids.Add(new TilePoint(i, j));
                }
            }
        }
    }

    /// <summary>
    /// 查找一个军营
    /// </summary>
    /// <returns></returns>
    private TileEntity FindCampEntity()
    {
        var allEntities = IsoMap.Instance.GetAllEntitiesByOwner(Entity.GetOwner());
        //  这里随便寻找一个军营  TODO：后期根据士兵分配选择一个合适的军营
        return allEntities.RubyFind(entity => EntityTypeUtil.IsBarracks(entity.model));
    }
}
