#define REALTIME_AI

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SettlersEngine;
using System;
#if REALTIME_AI
using RetvGridTargetInfo = IsoGridTarget;
#else
using RetvGridTargetInfo = System.Collections.Generic.IEnumerator<IsoGridTarget>;
#endif

public interface IMoveGrid
{
    int X { get; }

    int Y { get; }
}

public class DijkstraVertex : IComparer<DijkstraVertex>, IIndexedObject, IMoveGrid
{
    public static readonly DijkstraVertex Comparer = new DijkstraVertex(0, 0);

    public int Index { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    /// <summary>
    /// 起始点到当前节点的总权重
    /// </summary>
    public float Weight { get; set; }

    /// <summary>
    /// 当前节点自身的附加权重（比如墙会附加一定权重等） ※ 负数则说明未知（尚不明确权重值）
    /// </summary>
    public float AuxWeight { get; set; }

    public DijkstraVertex Parent { get; set; }

    public bool Visited { get; set; }

    public bool InNotVisitedList { get; set; }

    public void Reset()
    {
        this.Weight = float.MaxValue;
        this.AuxWeight = -1;
        this.Parent = null;
        this.Visited = false;
        this.InNotVisitedList = false;
    }

    public DijkstraVertex(int x, int y)
    {
        this.X = x;
        this.Y = y;
        this.Reset();
    }

    public int Compare(DijkstraVertex x, DijkstraVertex y)
    {
        if (x.Weight < y.Weight)
            return -1;
        else if (x.Weight > y.Weight)
            return 1;

        return 0;
    }
}

public class Dijkstra
{
    private DijkstraVertex[,] m_SearchSpace = null;
    private DijkstraVertex m_StartVertex = null;

    private PriorityQueue<DijkstraVertex> m_NotVisitedList = null;
    private DijkstraVertex[] m_NeighborNodes = null;

    public Dijkstra()
    {
        m_SearchSpace = new DijkstraVertex[Constants.EDGE_WIDTH, Constants.EDGE_HEIGHT];
        for (int x = 0; x < Constants.EDGE_WIDTH; x++)
        {
            for (int y = 0; y < Constants.EDGE_HEIGHT; y++)
            {
                m_SearchSpace[x, y] = new DijkstraVertex(x, y);
            }
        }

        m_NeighborNodes = new DijkstraVertex[8];
        m_NotVisitedList = new PriorityQueue<DijkstraVertex>(DijkstraVertex.Comparer);
    }

    public RetvGridTargetInfo Search(int startX, int startY, Dictionary<TilePoint, IsoGridTarget> gridTargets, AStarUserContext inContext)
    {
        //  执行搜索
#if REALTIME_AI
        SearchCore(startX, startY, inContext);
#else
        foreach (var _break in SearchCore(startX, startY, inContext))
        {
            if (_break)
                break;
            yield return null;
        }
#endif

        //  筛选所有权重最低的格子
        float miniWeight = float.MaxValue;
        IsoGridTarget targeterInfos = null;
        DijkstraVertex vertex = null;
        foreach (var item in gridTargets)
        {
            DijkstraVertex v = m_SearchSpace[item.Key.x, item.Key.y];
            if (v.Visited && v.Weight < miniWeight)
            {
                miniWeight = v.Weight;
                targeterInfos = item.Value;
                vertex = v;
            }
        }

        //  未找到直接范围
        if (targeterInfos == null)
            return null;

        //  生成移动路线（从起点到目标点）
        LinkedList<IMoveGrid> route = new LinkedList<IMoveGrid>();
        for (var v = vertex; v != null; v = v.Parent)
            route.AddFirst(v);
        targeterInfos.MoveRoute = route;

#if REALTIME_AI
        return targeterInfos;
#else
        yield return targeterInfos;
#endif
    }

    public LinkedList<IMoveGrid> SearchNearestWall(int startX, int startY, Dictionary<TilePoint, IsoGridTarget> gridTargets, AStarUserContext inContext)
    {
        //  执行搜索
#if REALTIME_AI
        SearchCore(startX, startY, inContext);
#else
        foreach (var _break in SearchCore(startX, startY, inContext))
        {
            if (_break)
                break;
            yield return null;
        }
#endif

        //  筛选所有权重最低并且有墙的路径的目标
        float miniWeight = float.MaxValue;
        DijkstraVertex vertex = null;
        foreach (var item in gridTargets)
        {
            var v = m_SearchSpace[item.Key.x, item.Key.y];
            if (v.Visited && v.Weight < miniWeight && HasWall(v))
            {
                miniWeight = v.Weight;
                vertex = v;
            }
        }
        if (vertex == null)
            return null;

        //  生成临时路线（从起点到目标点） REMARK：这条路线上肯定有墙
        LinkedList<DijkstraVertex> route = new LinkedList<DijkstraVertex>();
        for (var v = vertex; v != null; v = v.Parent)
            route.AddFirst(v);

        //  生成最终路线（从起点到第一处墙的格子）
        LinkedList<IMoveGrid> finalRoute = new LinkedList<IMoveGrid>();
        foreach (var v in route)
        {
            finalRoute.AddLast(v);
            if (v.AuxWeight > 0)
                break;
        }
        return finalRoute;
    }

    /// <summary>
    /// 判断在起点到顶点v的最短路径上是否有墙（REMARK：这里假设附加权重大于0则为有墙）
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    private bool HasWall(DijkstraVertex v)
    {
        for (var t = v; t != null; t = t.Parent)
        {
            if (t.AuxWeight > 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 执行Dijkstra搜索核心
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="inContext"></param>
    private 
#if REALTIME_AI
        void
#else
        IEnumerable<bool> 
#endif
        SearchCore(int startX, int startY, AStarUserContext inContext)
    {
        Assert.Should(startX >= 0 && startX < Constants.EDGE_WIDTH);
        Assert.Should(startY >= 0 && startY < Constants.EDGE_HEIGHT);
        Assert.Should(inContext != null && inContext.map != null && inContext.entity != null);

        //  初始化
        for (int x = 0; x < Constants.EDGE_WIDTH; x++)
        {
            for (int y = 0; y < Constants.EDGE_HEIGHT; y++)
            {
                m_SearchSpace[x, y].Reset();
            }
        }

#if !REALTIME_AI
        yield return false;
#endif

        //  初始化未访问列表（把起始节点添加到该列表）
        m_StartVertex = m_SearchSpace[startX, startY];
        m_StartVertex.Weight = 0.0f;
        m_StartVertex.Parent = null;

        m_NotVisitedList.Clear();
        m_NotVisitedList.Push(m_StartVertex);

        DijkstraVertex v1 = null;

        //  循环处理
        int iVisitedCount = 0;
        while (m_NotVisitedList.Count > 0)
        {
            //  从未访问列表中获取最近的节点作为当前节点（并添加访问过标记）
            v1 = m_NotVisitedList.Pop();
            v1.Visited = true;

            //  遍历当前节点v1的所有邻近节点
            foreach (DijkstraVertex v2 in NeighborNodes(v1, inContext))
            {
                //  尚未添加到 未访问 列表则添加
                if (!v2.InNotVisitedList)
                {
                    v2.InNotVisitedList = true;
                    m_NotVisitedList.Push(v2);
                }
                //  计算路径权重（startToV1 + V1ToV2）
                float startToV2 = v1.Weight + CalcWeight(v1, v2, inContext);
                //  更优的情况则更新节点信息 和 优先队列信息
                if (startToV2 < v2.Weight)
                {
                    v2.Weight = startToV2;
                    v2.Parent = v1;
                    m_NotVisitedList.Update(v2);
                }
            }

            //  迭代
            if (++iVisitedCount >= 1000)
            {
                iVisitedCount = 0;
#if !REALTIME_AI
                //yield return false;
#endif
            }
        }

        ////  检测（是否全部节点都访问过了）
        //Debug.Log("Start Check...");
        //bool pass = true;
        //for (int x = 0; x < Constants.EDGE_WIDTH; x++)
        //{
        //    for (int y = 0; y < Constants.EDGE_HEIGHT; y++)
        //    {
        //        if (!m_SearchSpace[x, y].Visited)
        //        {
        //            //int diffX = Math.Abs(m_StartVertex.X - m_SearchSpace[x, y].X);
        //            //int diffY = Math.Abs(m_StartVertex.Y - m_SearchSpace[x, y].Y);
        //            //if (diffX + diffY >= 15)
        //            //    continue;

        //            Debug.Log("not Visited");
        //            pass = false;
        //        }
        //    }
        //}
        //Assert.Should(pass, "check failed...");

#if !REALTIME_AI
        yield return true;
#endif
    }

    private static readonly float SQRT_2 = Mathf.Sqrt(2);

    /// <summary>
    /// 计算两节点间的权重信息
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="inContext">上下文信息</param>
    /// <returns></returns>
    private float CalcWeight(DijkstraVertex v1, DijkstraVertex v2, AStarUserContext inContext)
    {
        //  REMARK：该方法计算的权重值应该和A星里的NeighborDistance的值相同。

        //  距离权重
        int diffX = Math.Abs(v1.X - v2.X);
        int diffY = Math.Abs(v1.Y - v2.Y);
        float dis = 0.0f;
        switch (diffX + diffY)
        {
            case 1: dis = 1; break;
            case 2: dis = SQRT_2; break;
            case 0: dis = 0; break;
            default:
                Assert.Should(false);
                break;
        }

        //  计算目标节点v2自身的附加权重
        if (v2.AuxWeight < 0)
        {
            v2.AuxWeight = inContext.map.CalcAuxWeight(v2.X, v2.Y, inContext);
        }

        //  返回总权重值（TODO：附加权重是考虑的时间，时间和距离仅有1:1的情况下才可以相加，否则不是太准确。※ 距离1需要1s则ok，否则需要把距离转换为时间。t=s/v）
        return dis + v2.AuxWeight;
    }

    /// <summary>
    /// 邻近节点迭代器
    /// </summary>
    /// <param name="v1">当前节点</param>
    /// <param name="inContext"></param>
    /// <returns></returns>
    private IEnumerable NeighborNodes(DijkstraVertex v1, AStarUserContext inContext)
    {
        //  获取所有邻近节点
        int x = v1.X;
        int y = v1.Y;

        if ((x > 0) && (y > 0))
            m_NeighborNodes[0] = m_SearchSpace[x - 1, y - 1];
        else
            m_NeighborNodes[0] = null;

        if (y > 0)
            m_NeighborNodes[1] = m_SearchSpace[x, y - 1];
        else
            m_NeighborNodes[1] = null;

        if ((x < Constants.EDGE_WIDTH - 1) && (y > 0))
            m_NeighborNodes[2] = m_SearchSpace[x + 1, y - 1];
        else
            m_NeighborNodes[2] = null;

        if (x > 0)
            m_NeighborNodes[3] = m_SearchSpace[x - 1, y];
        else
            m_NeighborNodes[3] = null;

        if (x < Constants.EDGE_WIDTH - 1)
            m_NeighborNodes[4] = m_SearchSpace[x + 1, y];
        else
            m_NeighborNodes[4] = null;

        if ((x > 0) && (y < Constants.EDGE_HEIGHT - 1))
            m_NeighborNodes[5] = m_SearchSpace[x - 1, y + 1];
        else
            m_NeighborNodes[5] = null;

        if (y < Constants.EDGE_HEIGHT - 1)
            m_NeighborNodes[6] = m_SearchSpace[x, y + 1];
        else
            m_NeighborNodes[6] = null;

        if ((x < Constants.EDGE_WIDTH - 1) && (y < Constants.EDGE_HEIGHT - 1))
            m_NeighborNodes[7] = m_SearchSpace[x + 1, y + 1];
        else
            m_NeighborNodes[7] = null;

        //  返回所有节点
        for (int i = 0; i < m_NeighborNodes.Length; i++)
        {
            DijkstraVertex v2 = m_NeighborNodes[i];
            if (v2 == null || v2.Visited)
                continue;

            //  略过监视范围外节点
            if (inContext.entity.monitorRange > 0.0f)
            {
                float dx = m_StartVertex.X - v2.X;
                float dy = m_StartVertex.Y - v2.Y;
                if (dx * dx + dy * dy > inContext.entity.monitorRange * inContext.entity.monitorRange)
                    continue;
            }

            //  略过不可通行的节点
            if (!IsWalkable(v2.X, v2.Y, inContext))
                continue;

            yield return v2;
        }
    }

    private bool IsWalkable(int X, int Y, AStarUserContext inContext)
    {
        ///<    可飞行
        if (inContext.entity.CanFlying())
            return true;

        ///<    动态评估格式是否可通行
        return inContext.map.IsPassable(X, Y, true);
    }
}
