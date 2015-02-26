using UnityEngine;
using SettlersEngine;

public class IsoHelper {
    //public const int GRID_WIDTH = Constants.WIDTH;
    //public const int GRID_HEIGHT = Constants.HEIGHT;
    //public const int ROUTE_GRID_WIDTH = GRID_WIDTH * 2 + 1;
    //public const int ROUTE_GRID_HEIGHT = GRID_HEIGHT * 2 + 1;

    private static Plane floorPlane = new Plane(Vector3.up,Vector3.zero);

    /// <summary>
    /// 获取建造格子坐标系坐标
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
	private static bool PositionToGrid(Vector3 pos,out int x,out int y)
	{
	    x = Mathf.RoundToInt(pos.x);
	    y = Mathf.RoundToInt(pos.z);
        return IsoMap.Instance.InMapRange(x, y);
	}

    /// <summary>
    /// 获取移动的边坐标系坐标
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static bool PositionToEdge(Vector3 pos, out int x, out int y)
    {
        x = Mathf.RoundToInt(pos.x + 0.5f);
        y = Mathf.RoundToInt(pos.z + 0.5f);
        return IsoMap.Instance.InRouteMap(x, y);
    }

    public static Vector3 GridToPosition(TilePoint point)
    {
        return GridToPosition(point.x, point.y);
    }
    public static void MoveAlongCamera(Transform trans, float dist)
    {
        trans.position = MoveAlongCamera(trans.position, dist);
    }

    public static Vector3 MoveAlongCamera(Vector3 pos, float dist)
    {
        var vec = Camera.main.transform.forward * dist;
        return pos + vec;
    }
	public static Vector3 GridToPosition(int x,int y)
	{
        return new Vector3(x,0,y);
	}

    //public static TilePoint RoutePointToGrid(int x, int y)
    //{
    //    int gridX = x / 2;
    //    int gridY = y / 2;
    //    return new TilePoint(gridX,gridY);
    //}

    //public static TilePoint GridToRoutePoint(int x, int y)
    //{
    //    int routeX = x * 2;
    //    int routeY = y * 2;
    //    return new TilePoint(routeX,routeY);
    //}

    //public static bool PositionToRoutePoint(Vector3 pos,out int x,out int y)
    //{
    //    x = (int) (pos.x);
    //    y = (int) (pos.z);
    //    return x >= 0 && x < ROUTE_GRID_WIDTH && y >= 0 && y < ROUTE_GRID_HEIGHT;
    //}
    //public static Vector3 RoutePointToPosition(int x,int y)
    //{
    //    return new Vector3(x, 0, y);
    //}

    private static Vector3 GetWorldGroundPositionFromScreenPoint(Vector3 screenPoint)
    {
        float num;
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        floorPlane.Raycast(ray, out num);
        Debug.DrawRay(ray.origin,ray.direction);
        return ray.GetPoint(num);
    }

    public static bool ScreenPositionToGrid(Vector3 screenPoint, out int x, out int y)
    {
        return PositionToGrid(GetWorldGroundPositionFromScreenPoint(screenPoint), out x, out y);
    }

    public static bool ScreenPositionToEdge(Vector3 screenPoint, out int x, out int y)
    {
        return PositionToEdge(GetWorldGroundPositionFromScreenPoint(screenPoint), out x, out y);
    }

    public static void FaceToWorldCamera(Transform transform)
    {
        Camera camera = Camera.main;
        transform.rotation = Quaternion.LookRotation(camera.transform.forward, Vector3.up);
    }

    public static Vector2 WorldDirectionToScreenDirection(Vector3 direction)
    {
        return new Vector2(direction.x - direction.z, direction.x + direction.z);
    }

    public static Vector2 TileDirectionToScreenDirection(Vector2 direction)
    {
        return new Vector2(direction.x - direction.y, direction.x + direction.y);
    }
    public static Vector3 ScreenDirectionToWorldDirection(Vector2 direction)
    {
        return new Vector3((direction.x + direction.y) * 0.5f,0,(direction.y - direction.x) * 0.5f);
    }
}
