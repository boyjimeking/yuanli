using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// A* 自定义上下文
/// </summary>
public class AStarUserContext
{
    public readonly IsoMap map;
    public readonly TileEntity entity;
    public readonly Dictionary<TileEntity, bool> whiteList;

    public AStarUserContext(IsoMap map, TileEntity entity, Dictionary<TileEntity, bool> whiteList = null)
    {
        this.map = map;
        this.entity = entity;
        this.whiteList = whiteList;
    }
}

public class PathSolver<TResultNode, TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TResultNode, TPathNode,
TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>, TResultNode
{
    /// <summary>
    /// 当前节点到目标节点的评估值
    /// </summary>
    /// <param name="inStart"></param>
    /// <param name="inEnd"></param>
    /// <param name="inContext"></param>
    /// <returns></returns>
    protected override Double Heuristic(PathNode inStart, PathNode inEnd, TUserContext inContext)
	{
        Double h = base.Heuristic(inStart, inEnd, inContext);
        h += inStart.UserNode.GetWeight(inContext);
        return h;
	}

    /// <summary>
    /// 父节点到当前节点的值
    /// </summary>
    /// <param name="inStart"></param>
    /// <param name="inEnd"></param>
    /// <param name="inContext"></param>
    /// <returns></returns>
    protected override Double NeighborDistance(PathNode inStart, PathNode inEnd, TUserContext inContext)
    {
        Double n = base.NeighborDistance(inStart, inEnd, inContext);
        n += inEnd.UserNode.GetWeight(inContext);
        return n;
    }
	
	public PathSolver(TPathNode[,] inGrid)
		: base(inGrid)
	{
	}
} 