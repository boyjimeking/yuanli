using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 建筑工人组件
/// </summary>
public class WorkmanComponent : EntityComponent
{
    /// <summary>
    /// 工人状态 o(╯□╰)o
    /// </summary>
    public enum WorkerState
    {
        Free,       //  休息中
        Working,    //  上班途中or工作中
        FinishWork  //  下班回家
    }

    private TileEntity _workerHouse = null;

    private WorkerState _state = WorkerState.Free;
    private TileEntity _buildTargeter = null;
    private TilePoint _buildindLastTilePos;
    private List<TilePoint> _buildArea = null;
    private float m_timePassed = 0.0f;

    public WorkerState State { get { return _state; } }

    public override void Init()
    {
        this.enabled = false;
    }

    /// <summary>
    /// 被添加到建筑小屋中
    /// </summary>
    /// <param name="workerHouse"></param>
    public void OnAddToWorkerHouse(TileEntity workerHouse)
    {
        _workerHouse = workerHouse;
    }

    /// <summary>
    /// 开始去建造
    /// </summary>
    /// <param name="targeter"></param>
    public void BuildStart(TileEntity targeter)
    {
        Assert.Should(_state == WorkerState.Free || _state == WorkerState.FinishWork);
        Assert.Should(targeter != null);

        this.enabled = true;

        _buildTargeter = targeter;
        _buildindLastTilePos = new TilePoint(9999, 9999);
        if (_buildArea == null) 
            _buildArea = new List<TilePoint>();

        //  当前休息中则从工人小屋出来 回家中则停止移动
        if (_state == WorkerState.Free)
        {
            Entity.tileOffset = new Vector2(0.0f, 0.0f);
            Entity.SetTilePosition(GetDoorOfTheWorkerHouse());
            Entity.ShowEntity();
        }
        else
        {
            ActorMoveComponent move = Entity.GetComponent<ActorMoveComponent>();
            Assert.Should(move != null);
            move.OnMoveCompleteEvent -= OnMoveCompleteEvent;
            move.CancelMove();
        }
        //  设置上班状态 && 开始移动（3倍速移动）
        RefreshBuildGridArea();
        _state = WorkerState.Working;
        DoActionMove(RandomMoveGrid(IsInBuildArea()), 3.0f);
    }

    /// <summary>
    /// 建造结束
    /// </summary>
    public void BuildFinish()
    {
        _buildTargeter = null;

        if (_state == WorkerState.Working)
        {
            //  设置下班状态
            _state = WorkerState.FinishWork;

            //  移动回工人小屋
            DoActionMove(GetDoorOfTheWorkerHouse());
        }
    }

    public override void Update(float dt)
    {
        switch (_state)
        {
            case WorkerState.Working:
                UpdateWorking(dt);
                break;
            case WorkerState.FinishWork:
                break;
            default:
                break;
        }
    }

    private void UpdateWorking(float dt)
    {
        //  刷新建造区域
        RefreshBuildGridArea();

        //  更新工人建造中动画（REMARK：这里用Attacking作为建造中状态）
        if (Entity.State == EntityStateType.Attacking)
        {
            //  建造中直接返回
            if (m_timePassed > 0)
            {
                m_timePassed -= dt;
                if (m_timePassed > 0)
                    return;
            }

            //  一次建造行为结束后随机移动
            DoActionMove(RandomMoveGrid(IsInBuildArea()));
        }
    }

    private void DoActionBuild()
    {
        Entity.State = EntityStateType.Attacking;
        var dir = Entity.GetDir8FromTargeter(_buildTargeter);
        //工人只有攻击右上动画
        if(dir == EntityDirection.Right)
            dir = EntityDirection.TopRight;
        else if(dir == EntityDirection.Left)
            dir = EntityDirection.TopLeft;
        Entity.PlayAnimationAtDir8(AnimationNames.Attack,dir);
        m_timePassed = BattleRandom.Range(1.0f, 3.5f);
    }

    private bool DoActionMove(TilePoint targetPos, float moveSpeedFactor = 1.0f)
    {
        ActorMoveComponent move = Entity.GetComponent<ActorMoveComponent>();
        if (move != null)
        {
            if (move.StartMove(targetPos, null, moveSpeedFactor))
            {
                move.OnMoveCompleteEvent += OnMoveCompleteEvent;
                return true;
            }
        }
        return false;
    }

    private void OnMoveCompleteEvent(ActorMoveComponent moveComp)
    {
        moveComp.OnMoveCompleteEvent -= OnMoveCompleteEvent;
        if (_state == WorkerState.Working)
        {
            //  移动结束如果在建造区域内则开始建造（否则说明建筑移动过了？则继续移动）
            if (IsInBuildArea())
                DoActionBuild();
            else
                DoActionMove(RandomMoveGrid(false));
        }
        else if (_state == WorkerState.FinishWork)
        {
            TilePoint doorPos = GetDoorOfTheWorkerHouse();
            if (Entity.GetTilePos() != doorPos)
            {
                //  回家时（小屋搬家了（233
                DoActionMove(doorPos);
            }
            else
            {
                //  回到家（
                Entity.HideEntity();
                _state = WorkerState.Free;
                this.enabled = false;   
            }
        }
    }

    /// <summary>
    /// 从建筑区域随机获取一个移动格子（REMARK：如果当前目标已经在建筑区域内，则根据距离作为权重随机获取，否则直接随机获取。）
    /// </summary>
    /// <param name="inBuildArea"></param>
    /// <returns></returns>
    private TilePoint RandomMoveGrid(bool inBuildArea)
    {
        if (inBuildArea)
        {
            if (_buildArea.Count == 1)
            {
                return _buildArea[0];
            }
            else
            {
                //  权重算法：距离越大（权重越低、距离越近权重越高、距离0跳过）
                int maxDis = _buildArea.Count - 1;
                List<TilePoint> randomList = new List<TilePoint>();
                TilePoint selfPos = Entity.GetTilePos();
                foreach (var grid in _buildArea)
                {
                    int dis = Mathf.Abs(selfPos.x - grid.x) + Mathf.Abs(selfPos.y - grid.y);
                    if (dis > 0)
                    {
                        int weight = maxDis + 1 - dis;
                        for (int i = 0; i < weight; i++)
                            randomList.Add(grid);
                    }
                }
                return randomList[BattleRandom.Range(0, randomList.Count)];
            }
        }
        else
        {
            return _buildArea[BattleRandom.Range(0, _buildArea.Count)];
        }
    }

    private bool IsInBuildArea()
    {
        TilePoint selfPos = Entity.GetTilePos();
        foreach (var grid in _buildArea)
        {
            if (grid == selfPos)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 获取建造格子区域
    /// </summary>
    private void RefreshBuildGridArea()
    {
        TilePoint currPos = _buildTargeter.GetTilePos();
        if (currPos != _buildindLastTilePos)
        {
            _buildindLastTilePos = currPos;
            _buildArea.Clear();

            int x = currPos.x;
            int y = currPos.y;
            int w = _buildTargeter.width;
            int h = _buildTargeter.height;

            //  不可通行区域
            int blkBgnX, blkEndX, blkBgnY, blkEndY;
            int blocking = _buildTargeter.blockingRange;
            Assert.Should(blocking > 0, "invalid blockingRange...");

            int offset_x = (int)((w + 1 - blocking) / 2);
            int offset_y = (int)((h + 1 - blocking) / 2);
            blkBgnX = x + offset_x;
            blkEndX = blkBgnX + blocking - 1;
            blkBgnY = y + offset_y;
            blkEndY = blkBgnY + blocking - 1;

            //  获取建造区域（不可通行区域的左下右下两边）
            //  *           *
            //     *     *
            //        *
            for (int i = blkBgnX - 1; i <= blkEndX; i++)
            {
                _buildArea.Add(new TilePoint(i, blkBgnY - 1));
            }
            for (int j = blkBgnY; j <= blkEndY; j++)
            {
                _buildArea.Add(new TilePoint(blkBgnX - 1, j));
            }
        }
    }

    /// <summary>
    /// 获取建筑小屋门的位置  REMARK：这里门的位置假设定义在右下边的中心（邻近不可通行区域）
    /// </summary>
    /// <returns></returns>
    private TilePoint GetDoorOfTheWorkerHouse()
    {
        int blocking = _workerHouse.blockingRange;
        Assert.Should(blocking > 0, "invalid blockingRange...");

        int doorX = _workerHouse.GetTilePos().x + (int)(_workerHouse.width * 0.5f);
        int doorY = _workerHouse.GetTilePos().y + (int)((_workerHouse.height + 1 - blocking) / 2) - 1;

        return new TilePoint(doorX, doorY);
    }
}
