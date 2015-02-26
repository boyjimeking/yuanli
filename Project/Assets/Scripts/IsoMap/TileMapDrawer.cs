using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// component 'TileMapDrawer'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/TileMapDrawer")]
[ExecuteInEditMode]
public class TileMapDrawer : MonoBehaviour
{
    public GameObject tilePrefab;
    public bool redraw;
//    void Draw()
//    {
//        for (int x = 0; x < Constants.WIDTH; x++)
//        {
//            for (int y = 0; y < Constants.HEIGHT; y++)
//            {
//                GameObject tile = (GameObject)GameObject.Instantiate(tilePrefab, IsoHelper.GridToPosition(x, y), Quaternion.identity);
//                //tile.transform.position = tile.transform.position + new Vector3(0, 0, 10);//move behind
//                tile.GetComponent<TileEntity>().Init();
//                tile.GetComponent<TileEntity>().SetTilePosition(new TilePoint(x,y));
//                tile.transform.parent = transform;
//
//                IsoHelper.FaceToWorldCamera(transform);
//                IsoHelper.PushAwayFromCamera(tile.transform,10,Camera.main);
//            }
//        }
//    }
//    void Update()
//    {
//        if (redraw)
//        {
//            redraw = false;
//            Draw();
//        }
//    }

    void OnDrawGizmos()
    {
        //  原点
//        Gizmos.color = Color.red;
//        Gizmos.DrawSphere(Vector3.zero,0.2f);

        //  描绘格子和建筑区域
        for (int i = 0; i < Constants.WIDTH; i++)
        {
            for (int j = 0; j < Constants.HEIGHT; j++)
            {
                Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
                var pos = new Vector3(i, 0, j);
                if (IsoMap.Instance.IsBuilding(i, j))
                {
                    Gizmos.DrawCube(pos, new Vector3(1.0f, 0, 1.0f));
                }
                else
                {
                    Gizmos.DrawWireCube(pos, new Vector3(1.0f, 0, 1.0f));
                }
                //if (IsoMap.Instance.IsWall(i, j))
                //{
                //    Gizmos.color = Color.magenta;
                //    Gizmos.DrawSphere(pos, 0.1f);
                //}
            }
        }

        //  描绘不可通行区域
        Gizmos.color = Color.blue;
        for (int i = 0; i < Constants.EDGE_WIDTH; i++)
        {
            for (int j = 0; j < Constants.EDGE_HEIGHT; j++)
            {
                var pos = new Vector3(i - 0.5f, 0, j - 0.5f);
                if (!IsoMap.Instance.IsPassable(i, j))
                {
                    Gizmos.DrawSphere(pos, 0.1f);
                }
            }
        }

        //  描绘上次寻路路径
        LinkedList<IMoveGrid> path = IsoMap.Instance.m_dbgLastRoute;
        Gizmos.color = Color.yellow;
        if (path != null)
        {
            foreach (var grid in path)
            {
                var pos = new Vector3(grid.X - 0.5f, 0, grid.Y - 0.5f);
                Gizmos.DrawSphere(pos, 0.1f);
            }
        }

        //draw wall maps
        
//        for (int i = 0; i < RouteMap.WIDTH; i++)
//        {
//            for (int j = 0; j < RouteMap.HEIGHT; j++)
//            {
//                var pos = new Vector3(-0.5f + i * 0.5f, 0, -0.5f + j * 0.5f);
//                if (WallManager.Instance.wallMap[i, j] > 0)
//                {
//                    Gizmos.color = Color.red;
//                    Gizmos.DrawCube(pos, new Vector3(0.5f, 0, 0.5f));
//                }
//                else
//                {
//                    var v = 1 - Mathf.Max(1,RouteMap.Instance.routeMapShort[i, j] * 40f / (float) (RouteMap.WIDTH * RouteMap.HEIGHT));
//                    Gizmos.color = new Color(v,v,v);
//                    Gizmos.DrawCube(pos, new Vector3(0.5f, 0, 0.5f));
//                }
//            }
//        }
    }
}
