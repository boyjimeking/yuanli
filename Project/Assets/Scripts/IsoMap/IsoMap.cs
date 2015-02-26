#define REALTIME_AI

using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
#if REALTIME_AI
using RetvGridTargetInfo = IsoGridTarget;
#else
using RetvGridTargetInfo = System.Collections.Generic.IEnumerator<IsoGridTarget>;
#endif

public class IsoMap : Singleton<IsoMap>
{
    private List<TileEntity>[] m_entities = new List<TileEntity>[2];
    private List<TileEntity> m_entitiesToAdd = new List<TileEntity>();                                  ///<    延迟添加 ※ 在下次update的时候处理
    private List<TileEntity> m_entitiesToRemove = new List<TileEntity>();                               ///<    延迟删除 ※ 在下次update的时候处理

    private int[,] m_buildingMap = null;                                                                ///<    地表建筑数据图
    private IsoPathGrid[,] m_routeMap = new IsoPathGrid[Constants.EDGE_WIDTH, Constants.EDGE_HEIGHT];   ///<    地表通行数据图（因为建筑和通行数据不同，所以这里另外处理。）
    private int[,] m_entityCount = new int[Constants.EDGE_WIDTH, Constants.EDGE_HEIGHT];
    private Dictionary<int, TileEntity> m_entityHash = new Dictionary<int, TileEntity>();               ///<    key -> entity id value entity
    private PathSolver<IMoveGrid, IsoPathGrid, AStarUserContext> m_aStar;                               ///<    A*
    private Dijkstra m_dijkstra;                                                                        ///<    Dijkstra
    private object m_wallLinker;                                                                        ///<    墙和墙之间的连接对象
    private int m_wallLinkerId;                                                                         ///<    墙和墙之间的连接对象 id

    private WorkerHouseComponent _cacheWorkerHouse;                                                     ///<    缓存-建筑工人小屋        

    private GuardAreaValue[,] m_guardAreaMap = null;                                                    ///<    [仅战斗模式] 防御区域数据
    private GuardAreaView m_guardAreaView = null;

    //  DEBUG
    public LinkedList<IMoveGrid> m_dbgLastRoute;                                                        ///<    [调试] 上次搜索的移动路径

    /// <summary>
    /// 格子目标数量统计
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <returns></returns>
    public int GetEntityCount(int edge_x, int edge_y)
    {
        return m_entityCount[edge_x, edge_y];
    }
                                                                                                  
    public void AdjustEntityCount(TilePoint src, TilePoint dst)
    {
        if (src.x == int.MaxValue || src.y == int.MaxValue)
        {
            m_entityCount[dst.x, dst.y]++;
        }
        else
        {
            if (src != dst)
            {
                if (InRouteMap(src.x, src.y))
                    m_entityCount[src.x, src.y]--;
                if (InRouteMap(dst.x, dst.y))
                    m_entityCount[dst.x, dst.y]++;
            }
        }
    }

    public TileEntity GetBuildingAtScreenPoint(Vector3 screenPoint)
    {
        int mx, my;
        if (IsoHelper.ScreenPositionToGrid(screenPoint, out mx, out my))
        {
            return GetAnyBuildingEntity(mx, my);
        }
        return null;
    }

    /// <summary>
    /// 获取指定位置的建筑对象（如果指定位置没对象 或 不是建筑都范围null）
    /// </summary>
    /// <param name="grid_x"></param>
    /// <param name="grid_y"></param>
    /// <returns></returns>
    public TileEntity GetAnyBuildingEntity(int grid_x, int grid_y)
    {
        int build_id = m_buildingMap[grid_x, grid_y];
        if (build_id != 0)
        {
            TileEntity entity = m_entityHash[build_id];
            Assert.Should(entity != null);
            return entity;
        }
        return null;
    }

    /// <summary>
    /// 是否在地图可建造区域范围内
    /// </summary>
    /// <param name="dst_x">目标X位置</param>
    /// <param name="dst_y">目标Y位置</param>
    /// <param name="width">建筑宽度</param>
    /// <param name="height">建筑高度</param>
    /// <returns></returns>
    public bool InMapBuildableRange(int dst_x, int dst_y, int width, int height)
    {
        if (dst_x < Constants.SAFE_AREA_WIDTH ||
            dst_y < Constants.SAFE_AREA_WIDTH ||
            dst_x + width > Constants.WIDTH - Constants.SAFE_AREA_WIDTH ||
            dst_y + height > Constants.HEIGHT - Constants.SAFE_AREA_WIDTH)
            return false;
        return true;
    }

    /// <summary>
    /// 是否在地图范围内
    /// </summary>
    /// <param name="grid_x"></param>
    /// <param name="grid_y"></param>
    /// <returns></returns>
    public bool InMapRange(int grid_x, int grid_y)
    {
        return grid_x >= 0 && grid_x < Constants.WIDTH && grid_y >= 0 && grid_y < Constants.HEIGHT;
    }

    /// <summary>
    /// 是否在行走坐标系内
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <returns></returns>
    public bool InRouteMap(int edge_x, int edge_y)
    {
        return (edge_x >= 0 && edge_y >= 0 && edge_x < Constants.EDGE_WIDTH && edge_y < Constants.EDGE_HEIGHT);
    }

    /// <summary>
    /// 能否在指定位置建造建筑物
    /// </summary>
    /// <param name="dst_x"></param>
    /// <param name="dst_y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public bool CanPlaceBuilding(int dst_x, int dst_y, int width, int height)
    {
        //  不在建造区域内直接返回
        if (!InMapBuildableRange(dst_x, dst_y, width, height))
            return false;

        for (int x = dst_x; x < dst_x + width; ++x)
        {
            for (int y = dst_y; y < dst_y + height; ++y)
            {
                if (IsBuilding(x, y))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 是否可以下兵
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <returns></returns>
    public bool CanPlaceSoldier(int edge_x, int edge_y)
    {
        if (GameWorld.Instance.worldType == WorldType.Replay)
        {
            return true;
        }
        Assert.Should(m_guardAreaMap != null);
        return (InRouteMap(edge_x, edge_y) && (m_guardAreaMap[edge_x, edge_y] == GuardAreaValue.NIL));
    }
    /// <summary>
    /// 是否可以使用技能
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <returns></returns>
    public bool CanPlaceSkill(int edge_x, int edge_y)
    {
        if (GameWorld.Instance.worldType == WorldType.Replay)
        {
            return true;
        }
        Assert.Should(m_guardAreaMap != null);
        return (InRouteMap(edge_x, edge_y));
    }

    /// <summary>
    /// 是否有建筑
    /// </summary>
    /// <param name="grid_x"></param>
    /// <param name="grid_y"></param>
    /// <returns></returns>
    public bool IsBuilding(int grid_x, int grid_y)
    {
        return m_buildingMap[grid_x, grid_y] != 0;
    }

    /// <summary>
    /// 是否是墙判断
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <param name="linkerIsWall">连接器是否作为墙考虑</param>
    /// <returns></returns>
    public bool IsWall(int edge_x, int edge_y, bool linkerIsWall = false)
    {
        int entity_id = m_routeMap[edge_x, edge_y].EntityID;
        if (entity_id != 0)
        {
            if (entity_id == m_wallLinkerId)
                return linkerIsWall;

            TileEntity entity = m_entityHash[entity_id];
            Assert.Should(entity != null);
            return entity.entityType == EntityType.Wall;
        }
        return false;
    }

    /// <summary>
    /// 是否是墙或连接器
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <returns></returns>
    public bool IsWallorLinker(int edge_x, int edge_y)
    {
        return IsWall(edge_x, edge_y, true);
    }

    /// <summary>
    /// 是否是墙判断（严格模式：检测是否越界）
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <returns></returns>
    public bool IsWallStrict(int edge_x, int edge_y)
    {
        return (InRouteMap(edge_x, edge_y) && IsWall(edge_x, edge_y));
    }

    /// <summary>
    /// 获取指定边位置上的墙对象（失败返回null）
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <returns></returns>
    private TileEntity GetWallStrict(int edge_x, int edge_y)
    {
        if (!InRouteMap(edge_x, edge_y))
            return null;

        int entity_id = m_routeMap[edge_x, edge_y].EntityID;
        if (entity_id != 0 && entity_id != m_wallLinkerId)
        {
            TileEntity entity = m_entityHash[entity_id];
            Assert.Should(entity != null);
            if (entity.entityType == EntityType.Wall)
                return entity;
        }

        return null;
    }

    /// <summary>
    /// 是否可通行判断
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <param name="destroy_wall">墙默认是不可通行的，如果可摧毁则认为墙可通行。 </param>
    /// <param name="whiteList">白名单列表，该列表中的对象不阻挡通行。</param>
    /// <returns></returns>
    public bool IsPassable(int edge_x, int edge_y, bool destroy_wall = false, Dictionary<TileEntity, bool> whiteList = null)
    {
        int entity_id = m_routeMap[edge_x, edge_y].EntityID;
        if (entity_id != 0)
        {
            //  连接器的情况下（是否通行根据墙标记确定）
            if (entity_id == m_wallLinkerId)
                return destroy_wall;

            TileEntity entity = m_entityHash[entity_id];
            Assert.Should(entity != null);

            //  白名单中的对象可通行
            if (whiteList != null && whiteList.ContainsKey(entity))
                return true;

            if (destroy_wall)
            {
                if (entity.entityType != EntityType.Wall && EntityTypeUtil.IsAnyBlockage(entity.entityType))
                {
                    return false;
                }
            }
            else
            {
                if (EntityTypeUtil.IsAnyBlockage(entity.entityType))
                {
                    return false;
                }
            }

        }
        return true;
    }

    /// <summary>
    /// 是否可通行（严格模式：检测是否越界）
    /// </summary>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <param name="destroy_wall">是否可摧毁</param>
    /// <returns></returns>
    public bool IsPassableStrict(int edge_x, int edge_y, bool destroy_wall = false)
    {
        return (InRouteMap(edge_x, edge_y) && IsPassable(edge_x, edge_y, destroy_wall));
    }

    /// <summary>
    /// 计算目标点的附加权重值（给A星以及Dijkstra用）
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    ///// <param name="inContext"></param>
    /// <returns></returns>
    public float CalcAuxWeight(int x, int y, AStarUserContext inContext)
    {
        Assert.Should(inContext != null && inContext.entity != null);
        //  墙 && 不可穿越
        if (IsWallorLinker(x, y) && !inContext.entity.CanOverTheWall())
        {
            return 10.0f;
            ////  不同兵种拆墙时间估算（REMARK：这里简单处理不考虑墙的HP等情况了）
            //if (EntityTypeUtil.IsBombMan(inContext.entity.model))
            //{
            //    return inContext.entity.model.rate + 1.0f;
            //}
            //else
            //{
            //    return 10.0f;
            //}
        }
        //  其他情况不附加权重
        return 0.0f;
    }

    public RetvGridTargetInfo SearchDijkstra(TileEntity attacker, Dictionary<TilePoint, IsoGridTarget> gridTargets)
    {
        return m_dijkstra.Search(attacker.GetTilePos().x, attacker.GetTilePos().y, gridTargets, new AStarUserContext(this, attacker));
    }

    public LinkedList<IMoveGrid> SearchDijkstraNearestWall(TileEntity attacker, Dictionary<TilePoint, IsoGridTarget> gridTargets)
    {
        LinkedList<IMoveGrid> path = m_dijkstra.SearchNearestWall(attacker.GetTilePos().x, attacker.GetTilePos().y, gridTargets, new AStarUserContext(this, attacker));

        //  DEBUG
        if (path != null)
        {
            m_dbgLastRoute = new LinkedList<IMoveGrid>();
            foreach (var v in path)
            {
                m_dbgLastRoute.AddFirst(v);
            }
        }
        else
        {
            m_dbgLastRoute = null;
        }

        return path;
    }

    /// <summary>
    /// 搜索行走路线，找到则返回列表，未找到返回null。
    /// </summary>
    /// <param name="iBgnX"></param>
    /// <param name="iBgnY"></param>
    /// <param name="iEndX"></param>
    /// <param name="iEndY"></param>
    /// <param name="entity">本次寻路相关联的对象</param>
    /// <param name="whilteList">白名单列表，不阻挡通行。</param>
    /// <returns></returns>
    public LinkedList<IMoveGrid> SearchRoutes(int iBgnX, int iBgnY, int iEndX, int iEndY, TileEntity entity = null, List<TileEntity> whilteList = null)
    {
        Dictionary<TileEntity, bool> whiteHash = null;
        if (whilteList != null)
        {
            whiteHash = new Dictionary<TileEntity, bool>();
            foreach (var tar in whilteList)
                whiteHash.Add(tar, true);
        }
        LinkedList<IMoveGrid> path = m_aStar.Search(new TilePoint(iBgnX, iBgnY), new TilePoint(iEndX, iEndY), new AStarUserContext(this, entity, whiteHash));

        //  DEBUG
        if (path != null)
        {
            m_dbgLastRoute = new LinkedList<IMoveGrid>();
            foreach (var v in path)
            {
                m_dbgLastRoute.AddFirst(v);
            }
        }
        else
        {
            m_dbgLastRoute = null;
        }

        return path;
    }

    private IsoMap()
    {
        Init();
    }

    public void Clear()
    {
        if (m_guardAreaView != null)
        {
            UnityEngine.Object.Destroy(m_guardAreaView.gameObject);
            m_guardAreaView = null;
        }
        UpdateDelayObject();//flush delay objects
        for (int i = 0; i < (int)OwnerType.Count; i++)
        {
            foreach (var tileEntity in m_entities[i])
            {
                tileEntity.Destroy();
            }
        }
        Init();
    }

    public void Init()
    {
        for (int x = 0; x < Constants.EDGE_WIDTH; x++)
        {
            for (int y = 0; y < Constants.EDGE_HEIGHT; y++)
            {
                m_routeMap[x, y] = new IsoPathGrid
                {
                    EntityID = 0,
                    X = x,
                    Y = y,
                };

                m_entityCount[x, y] = 0;
            }
        }

        m_buildingMap = new int[Constants.WIDTH, Constants.HEIGHT];

        for (int i = 0; i < (int)OwnerType.Count; i++)
        {
            m_entities[i] = new List<TileEntity>();
        }

        m_entityHash.Clear();
        m_entitiesToAdd.Clear();
        m_entitiesToRemove.Clear();

        m_aStar = new PathSolver<IMoveGrid, IsoPathGrid, AStarUserContext>(m_routeMap);
        m_dijkstra = new Dijkstra();

        m_wallLinker = new object();
        m_wallLinkerId = m_wallLinker.GetHashCode();

        _cacheWorkerHouse = null;

        m_guardAreaMap = null;
    }

    /// <summary>
    /// 初始化战斗中的防御区域
    /// </summary>
    public void InitGuardAreaMap()
    {
        //  1、初始化空
        m_guardAreaMap = new GuardAreaValue[Constants.EDGE_WIDTH, Constants.EDGE_HEIGHT];
        for (int x = 0; x < Constants.EDGE_WIDTH; x++)
        {
            for (int y = 0; y < Constants.EDGE_HEIGHT; y++)
            {
                m_guardAreaMap[x, y] = GuardAreaValue.NIL;
            }
        }

        //  2、根据建筑初始化防御区域，这里获取Defender方建筑。

        //  REMARK：扩展范围宽度 这个值可以调整
        int extendWidth = 2;

        foreach (var entity in GetAllEntitiesByOwner(OwnerType.Defender))
        {
            if (!EntityTypeUtil.IsAnyBuilding(entity.entityType))
                continue;

            int bgnX = Mathf.Max(entity.GetTilePos().x - extendWidth, 0);
            int endX = Mathf.Min(entity.GetTilePos().x + entity.width + extendWidth, Constants.EDGE_WIDTH - 1);

            int bgnY = Mathf.Max(entity.GetTilePos().y - extendWidth, 0);
            int endY = Mathf.Min(entity.GetTilePos().y + entity.height + extendWidth, Constants.EDGE_HEIGHT - 1);

            for (int x = bgnX; x <= endX; x++)
            {
                for (int y = bgnY; y <= endY; y++)
                {
                    m_guardAreaMap[x, y] = GuardAreaValue.Uninitialized;
                }
            }
        }

        //  3、计算每个防御区域格子贴图方向
        for (int x = 0; x < Constants.EDGE_WIDTH; x++)
        {
            for (int y = 0; y < Constants.EDGE_HEIGHT; y++)
            {
                if (m_guardAreaMap[x, y] == GuardAreaValue.NIL || m_guardAreaMap[x, y] != GuardAreaValue.Uninitialized)
                    continue;

                //  计算防御区域方向（默认为zero无方向）
                GuardAreaValue dir = GuardAreaValue.Zero;
                {
                    //  上
                    if (y + 1 < Constants.EDGE_HEIGHT && m_guardAreaMap[x, y + 1] == GuardAreaValue.NIL)
                        dir |= GuardAreaValue.Top;

                    //  下
                    if (y - 1 >= 0 && m_guardAreaMap[x, y - 1] == GuardAreaValue.NIL)
                        dir |= GuardAreaValue.Bottom;

                    //  左
                    if (x - 1 >= 0 && m_guardAreaMap[x - 1, y] == GuardAreaValue.NIL)
                        dir |= GuardAreaValue.Left;

                    //  右
                    if (x + 1 < Constants.EDGE_WIDTH && m_guardAreaMap[x + 1, y] == GuardAreaValue.NIL)
                        dir |= GuardAreaValue.Right;
                }

                //  设置新的方向
                m_guardAreaMap[x, y] = dir;
            }
        }

        if (m_guardAreaView == null)
        {
            var go = new GameObject();
            go.name = "GuardAreaView";
            m_guardAreaView = go.AddComponent<GuardAreaView>();
            go.SetActive(false);
        }
        m_guardAreaView.Init(m_guardAreaMap);
    }

    public void ShowGuardAreaMap(bool delayHide)
    {
        if (m_guardAreaView != null)
        {
            m_guardAreaView.Show(delayHide);
        }
    }

    public void HideGuardAreaMap()
    {
        if (m_guardAreaView != null)
        {
            m_guardAreaView.Hide();
        }
    }
    private void UpdateDelayObject()
    {
        ///<    处理延迟删除
        if (m_entitiesToRemove.Count > 0)
        {
            foreach (var tileEntity in m_entitiesToRemove)
            {
                RealRemoveEntity(tileEntity);
            }
            m_entitiesToRemove.Clear();
        }
        ///<    处理延迟添加
        if (m_entitiesToAdd.Count > 0)
        {
            foreach (var tileEntity in m_entitiesToAdd)
            {
                RealAddEntity(tileEntity);
            }
            m_entitiesToAdd.Clear();
        }
    }

    public void Update(float dt)
    {
        ///<    更新延迟对象
        UpdateDelayObject();

        ///<    更新各种entity
        foreach (List<TileEntity> eachSideEntity in m_entities)
        {
            foreach (var tileEntity in eachSideEntity)
            {
                tileEntity.Update(dt);
            }
        }
    }

    public void DelayAddEntity(TileEntity entity)
    {
        m_entitiesToAdd.Add(entity);
    }

    public void DelayRemoveEntity(TileEntity entity)
    {
        m_entitiesToRemove.Add(entity);
    }

    public void ForceAddEntity(TileEntity entity)
    {
        RealAddEntity(entity);
    }

    public void ForceRemoveEntity(TileEntity entity)
    {
        RealRemoveEntity(entity);
    }
    /// <summary>
    /// 编辑模式下,暂时移除当前建筑的占地数据
    /// </summary>
    /// <param name="entity"></param>
    public void BeginMoveEntity(TileEntity entity)
    {
        Assert.Should(Contains(entity));
        FillBuildingMapData(entity, 0);
    }

    public void EndMoveEntity(TileEntity entity)
    {
        int entity_id = entity.GetHashCode();
        FillBuildingMapData(entity, entity_id);
    }

    public void RemoveBuildingMapData(TileEntity entity)
    {
        FillBuildingMapData(entity, 0);
    }
    /// <summary>
    /// 填充地表建筑数据（0为清除）
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    private void FillBuildingMapData(TileEntity entity, int entity_id)
    {
        if (!EntityTypeUtil.IsCostMapGrid(entity.entityType))
        {
            return;
        }
        int x = entity.GetTilePos().x;
        int y = entity.GetTilePos().y;
        //  填充建筑全部区域
        int w = entity.width;
        int h = entity.height;
        for (int i = x; i < x + w; ++i)
        {
            for (int j = y; j < y + h; ++j)
            {
                m_buildingMap[i, j] = entity_id;
            }
        }

        //  填充建筑不可通行区域（blockingRange最大值为边数减1，所以不会导致建筑边上的entity_id重合。）
        int blocking = entity.blockingRange;
        if (blocking > 0)
        {
            int offset_x = (int)((w + 1 - blocking) / 2);
            int offset_y = (int)((h + 1 - blocking) / 2);
            Assert.Should(offset_x >= 1 && offset_y >= 1, "Entity的blockingRange不正确...");
            for (int i = x + offset_x; i < x + offset_x + blocking; ++i)
            {
                for (int j = y + offset_y; j < y + offset_y + blocking; ++j)
                {
                    m_routeMap[i, j].EntityID = entity_id;
                }
            }
        }

        //  更新墙之间的连接器 并 刷新墙的方向（显示用）
        if (entity.entityType == EntityType.Wall)
        {
            RefreshWallLinkerAndDirection(entity, entity_id);
        }
    }

    private void RefreshWallLinkerAndDirection(TileEntity entity, int entity_id)
    {
        Vector2 c = entity.GetCurrentPositionCenter();
        int x = (int)c.x;
        int y = (int)c.y;
        int w = entity.width;
        //  entity_id为0则取消连接，不为0则设置连接。
        int wallLinkerId = (entity_id != 0 ? m_wallLinkerId : 0);

        //  REMARK:目前这里只连接了一个格子（如果墙的尺寸有变 这里需要相应的调整 否则会出BUG）

        //  +Y
        //       w   w
        //         w  
        //       w   w
        //  -X-Y     +X
        TileEntity wall;

        //  左上
        if ((wall = GetWallStrict(x, y + w)) != null)
        {
            m_routeMap[x, y + 1].EntityID = wallLinkerId;
            wall.GetComponent<WallComponent>().RefreshWallDirection();
        }
        //  右上
        if ((wall = GetWallStrict(x + w, y)) != null)
        {
            m_routeMap[x + 1, y].EntityID = wallLinkerId;
            wall.GetComponent<WallComponent>().RefreshWallDirection();
        }
        //  左下
        if ((wall = GetWallStrict(x - w, y)) != null)
        {
            m_routeMap[x - 1, y].EntityID = wallLinkerId;
            wall.GetComponent<WallComponent>().RefreshWallDirection();
        }
        //  右下
        if ((wall = GetWallStrict(x, y - w)) != null)
        {
            m_routeMap[x, y - 1].EntityID = wallLinkerId;
            wall.GetComponent<WallComponent>().RefreshWallDirection();
        }

        //  刷新新建的墙自身的显示
        if (entity_id != 0)
        {
            entity.GetComponent<WallComponent>().RefreshWallDirection();
        }
    }

    private void RealAddEntity(TileEntity entity)
    {
        //  先初始化（否则在后面添加墙数据时没有精灵会导致空引用）
        entity.AddedToWorld();

        int entity_id = entity.GetHashCode();
        Assert.Should(!m_entityHash.ContainsKey(entity_id));

        //  添加到列表和Hash表
        m_entities[(int)entity.GetOwner()].Add(entity);
        m_entityHash.Add(entity_id, entity);
        FillBuildingMapData(entity, entity_id);
    }

    private void RealRemoveEntity(TileEntity entity)
    {
        int entity_id = entity.GetHashCode();
        Assert.Should(m_entityHash.ContainsKey(entity_id));

        m_entities[(int)entity.GetOwner()].Remove(entity);
        m_entityHash.Remove(entity_id);
        FillBuildingMapData(entity, 0);
    }

    public bool Contains(TileEntity entity)
    {
        return m_entities[(int) entity.GetOwner()].Contains(entity);
    }

    private void DetectNearestWall(ref int minDiff, ref int wallX, ref int wallY, int selfX, int selfY, int testX, int testY)
    {
        if (IsWallStrict(testX, testY))
        {
            int diff = Math.Abs(testX - selfX) + Math.Abs(testY - selfY);
            if (diff <= minDiff)
            {
                minDiff = diff;
                wallX = testX;
                wallY = testY;
            }
        }
    }

    /// <summary>
    /// 根据指定坐标获取目标墙
    /// </summary>
    /// <param name="self_x"></param>
    /// <param name="self_y"></param>
    /// <param name="edge_x"></param>
    /// <param name="edge_y"></param>
    /// <returns></returns>
    public TileEntity GetWallTargeter(int self_x, int self_y, int edge_x, int edge_y)
    {
        int entity_id = m_routeMap[edge_x, edge_y].EntityID;
        if (entity_id != 0)
        {
            if (entity_id == m_wallLinkerId)
            {
                //  是连接器则取最靠近自身的墙
                int w = 1; //   REMARK：连接器到墙中心的距离
                int mindiff = 999999;
                int goal_x = -1;
                int goal_y = -1;
                //  依次为 左上、右上、左下、右下
                DetectNearestWall(ref mindiff, ref goal_x, ref goal_y, self_x, self_y, edge_x, edge_y + w);
                DetectNearestWall(ref mindiff, ref goal_x, ref goal_y, self_x, self_y, edge_x + w, edge_y);
                DetectNearestWall(ref mindiff, ref goal_x, ref goal_y, self_x, self_y, edge_x - w, edge_y);
                DetectNearestWall(ref mindiff, ref goal_x, ref goal_y, self_x, self_y, edge_x, edge_y - w);
                //  至少存在一个墙的节点
                Assert.Should(goal_x >= 0 && goal_y >= 0);
                //  获取墙的id
                entity_id = m_routeMap[goal_x, goal_y].EntityID;
                Assert.Should(entity_id != 0);
            }
            //  获取墙对象
            TileEntity entity = m_entityHash[entity_id];
            Assert.Should(entity != null && entity.entityType == EntityType.Wall);
            return entity;
        }
        return null;
    }

    /// <summary>
    /// 类型获取对象列表
    /// </summary>
    /// <param name="ownerType"></param>
    /// <param name="aiType"></param>
    /// <param name="attacker"></param>
    /// <param name="includeFriend">是否包含友军（默认只获取建筑型目标）</param>
    /// <returns></returns>
    public List<TileEntity> GetEntitiesByTT(OwnerType ownerType, EntityAiType aiType, TileEntity attacker, bool includeFriend = false)
    {
        List<TileEntity> ret = new List<TileEntity>();
        foreach (TileEntity entity in m_entities[(int)ownerType])
        {
            if (entity == attacker)
                continue;

            if (entity.IsDead())
                continue;

            //  不是限定类型的目标则过滤掉
            if (EntityTypeUtil.IsAnyActor(entity.entityType) && attacker.model.onlyAttackTargetType != EntityType.None && attacker.model.onlyAttackTargetType != entity.entityType)
                continue;

            switch (aiType)
            {
                case EntityAiType.PriorToWall:
                    if (entity.entityType == EntityType.Wall)
                    {
                        ret.Add(entity);
                    }
                    break;
                case EntityAiType.PriorToTower:
                    if (entity.entityType == EntityType.Tower)
                    {
                        ret.Add(entity);
                    }
                    break;
                case EntityAiType.PriorToResource:
                    if (entity.entityType == EntityType.Resource)
                    {
                        ret.Add(entity);
                    }
                    break;
                default:
                    {
                        //  建筑目标
                        if (EntityTypeUtil.IsAnyNonWallBuilding(entity.entityType))
                        {
                            ret.Add(entity);
                        }
                        //  援军
                        else if (includeFriend && entity.Friendly)
                        {
                            ret.Add(entity);
                        }
                    }
                    break;
            }
        }
        return ret;
    }

    /// <summary>
    /// 根据阵营已经攻击者对象获取攻击范围内的对象列表
    /// </summary>
    /// <param name="self"></param>
    /// <param name="ownerType"></param>
    /// <returns></returns>
    public List<TileEntity> GetEntitiesByRange(TileEntity attacker, OwnerType ownerType)
    {
        Vector2 p = attacker.GetCurrentPositionCenter();
        return GetEntitiesByRange(attacker, ownerType, p.x, p.y, attacker.model.range, attacker.model.blindRange);
    }

    /// <summary>
    /// 获取指定范围内的对象列表（攻击范围、溅射范围、治疗范围等）
    /// </summary>
    /// <param name="attacker">过滤掉的对象</param>
    /// <param name="ownerType">阵营</param>
    /// <param name="x">范围的中心点X</param>
    /// <param name="y">范围的中心点Y</param>
    /// <param name="range">范围</param>
    /// <param name="blindrange">盲区范围</param>
    /// <param name="filter">过滤器，如果过滤器返回true，则指定的对象被过滤掉。</param>
    /// <returns></returns>
    public List<TileEntity> GetEntitiesByRange(TileEntity attacker, OwnerType ownerType, float x, float y, float range, float blindrange, Func<TileEntity, bool> filter = null)
    {
        List<TileEntity> ret = new List<TileEntity>();
        float blindrange2 = blindrange * blindrange;
        foreach (TileEntity targeter in m_entities[(int)ownerType])
        {
            if (attacker == targeter)
                continue;

            if (targeter.IsDead())
                continue;

            if (!EntityTypeUtil.IsAnyTargeter(targeter.entityType))
                continue;

            //  不是限定类型的目标则过滤掉
            if (attacker != null && EntityTypeUtil.IsAnyActor(targeter.entityType) && attacker.model.onlyAttackTargetType != EntityType.None && attacker.model.onlyAttackTargetType != targeter.entityType)
                continue;

            if (filter != null && filter(targeter))
                continue;

            if (targeter.IsInAttackRange(x, y, range, blindrange2))
                ret.Add(targeter);
        }
        return ret;
    }

    /// <summary>
    /// 获取指定阵营的全部对象列表
    /// </summary>
    /// <param name="ownerType"></param>
    /// <returns></returns>
    public List<TileEntity> GetAllEntitiesByOwner(OwnerType ownerType)
    {
        return m_entities[(int)ownerType];
    }

    /// <summary>
    /// 获取所有组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="owner"></param>
    /// <returns></returns>
    public List<T> GetComponents<T>(OwnerType owner) where T : EntityComponent
    {
        List<T> components = new List<T>();
        foreach (var entity in GetAllEntitiesByOwner(owner))
        {
            var comp = entity.GetComponent<T>();
            if (comp != null)
            {
                components.Add(comp);
            }
        }
        return components;
    }

    /// <summary>
    /// 获取建筑工人小屋组件
    /// </summary>
    /// <returns></returns>
    public WorkerHouseComponent GetWorkerHouseComponent()
    {
        if (_cacheWorkerHouse == null)
        {
            //  REMARK：建筑工人小屋就一个设施、并且属于防御方。
            foreach (var entity in GetAllEntitiesByOwner(OwnerType.Defender))
            {
                if (EntityTypeUtil.IsWorkerHouse(entity.model))
                {
                    _cacheWorkerHouse = entity.GetComponent<WorkerHouseComponent>();
                    break;
                }
            }
        }
        Assert.Should(_cacheWorkerHouse != null);
        return _cacheWorkerHouse;
    }

    /// <summary>
    /// 处理技能范围伤害
    /// </summary>
    /// <param name="skillModel"></param>
    /// <param name="hitX"></param>
    /// <param name="hitY"></param>
    public void ProcessAoeDamage(EntityModel skillModel, float hitX, float hitY)
    {
        var targeters = GetEntitiesByRange(null, OwnerType.Defender, hitX, hitY, skillModel.splashRange, 0.0f);
        if (targeters.Count == 0)
            return;
        GameDamageManager.ProcessDamageMultiTargeters(targeters, skillModel);
    }

    public TileEntity CreateEntityAt(OwnerType owner, int entityId, int x, int y)
    {
        var model = DataCenter.Instance.FindEntityModelById(entityId);
        //  创建对象
        var tileEntity = TileEntity.Create(owner, model);
        tileEntity.SetTilePosition(new TilePoint(x, y));

        //  添加到地图上
        ForceAddEntity(tileEntity);
        return tileEntity;
    }
}
