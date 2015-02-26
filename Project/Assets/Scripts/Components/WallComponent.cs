using System.Collections;
using UnityEngine;

public class WallComponent : EntityComponent
{
    private IsoWall.WallDirection m_dir;

    public override void Init()
    {
        this.enabled = false;
        m_dir = IsoWall.WallDirection.Center;
    }

    /// <summary>
    /// 在建造墙或墙销毁的时候刷新墙的方向
    /// </summary>
    public void RefreshWallDirection()
    {
        Vector2 c = Entity.GetCurrentPositionCenter();
        int x = (int)c.x;
        int y = (int)c.y;
        int w = Entity.width;
        if (IsoMap.Instance.IsWallStrict(x, y + w))
        {
            if (IsoMap.Instance.IsWallStrict(x + w, y))
            {
                SetWallDirection(IsoWall.WallDirection.TopAndRight);
            }
            else
            {
                SetWallDirection(IsoWall.WallDirection.Top);
            }
        }
        else
        {
            if (!IsoMap.Instance.IsWallStrict(x + w, y))
            {
                SetWallDirection(IsoWall.WallDirection.Center);
            }
            else
            {
                SetWallDirection(IsoWall.WallDirection.Right);
            }
        }
    }

    /// <summary>
    /// 设置墙朝向
    /// </summary>
    /// <param name="direction"></param>
    private void SetWallDirection(IsoWall.WallDirection direction)
    {
        if (m_dir != direction)
        {
            m_dir = direction;
            Entity.view.SetSprite(Entity.animationNamePrefix + (int)direction);
        }
    }
}
