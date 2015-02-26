using System;
using System.Collections.Generic;
using System.Collections;

public class IsoGridTarget
{
    public TileEntity Targeter { get; set; }

    public int Distance { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    /// <summary>
    /// 到目标的移动路线
    /// </summary>
    public LinkedList<IMoveGrid> MoveRoute { get; set; }
}

public class IsoPathGrid : SettlersEngine.IPathNode<AStarUserContext>, IMoveGrid
{
    public int X { get; set; }

    public int Y { get; set; }

    public int EntityID { get; set; }

    public bool IsWalkable(AStarUserContext inContext)
    {
        Assert.Should(inContext != null && inContext.map != null && inContext.entity != null);

        ///<    可飞行
        if (inContext.entity.CanFlying())
            return true;

        ///<    动态评估格式是否可通行
        return inContext.map.IsPassable(X, Y, true, inContext.whiteList);
    }

    public Double GetWeight(AStarUserContext inContext)
    {
        Assert.Should(inContext != null && inContext.map != null);
        return inContext.map.CalcAuxWeight(X, Y, inContext);
    }

    public override int GetHashCode()
    {
        return ((this.X * 997) ^ this.Y);
    }
}