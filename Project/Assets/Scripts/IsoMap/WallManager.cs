//using System.Xml;
//using UnityEngine;
//using System.Collections;

///// <summary>
///// component 'WallManager'
///// ADD COMPONENT DESCRIPTION HERE
///// </summary>
//[AddComponentMenu("Scripts/WallManager")]
//public class WallManager : Singleton<WallManager>
//{
//    public sbyte[,] wallMap;

//    private WallManager()
//    {
//        Reset();
//    }

//    public void Reset()
//    {
//        wallMap = new sbyte[Constants.EDGE_WIDTH, Constants.EDGE_HEIGHT];
//        for (int i = 0; i < Constants.EDGE_WIDTH; ++i)
//        {
//            for (int j = 0; j < Constants.EDGE_HEIGHT; ++j)
//            {
//                wallMap[i, j] = 0;
//            }
//        }
//    }
//    /// <summary>
//    /// x,y有新建或者移除墙体时,修复自身和附近墙体的方向
//    /// </summary>
//    /// <param name="x"></param>
//    /// <param name="y"></param>
//    public void FixWallWhenChangeAt(TilePoint tilePos)
//    {
//        if(IsWall(tilePos.x,tilePos.y))
//            FixWallAt(tilePos);
//        var left = tilePos.Left;
//        if (IsWall(left.x,left.y))
//            FixWallAt(left);
//        var bottom = tilePos.Bottom;
//        if(IsWall(bottom.x,bottom.y))
//            FixWallAt(bottom);
//    }
//    /// <summary>
//    /// 修复x,y处墙体的方向
//    /// </summary>
//    /// <param name="x"></param>
//    /// <param name="y"></param>
//    private void FixWallAt(TilePoint tilePos)
//    {
//        int mask = 0;
//        if (IsWall(tilePos.Right.x,tilePos.Right.y))
//        {
//            mask |= 1 << 0;
//        }
//        if (IsWall(tilePos.Top.x,tilePos.Top.y))
//        {
//            mask |= 1 << 1;
//        }
//        IsoWall wall = new IsoWall() {x = tilePos.x, y = tilePos.y};
//        if (mask > 0)
//        {
//            GameObject newWallPrefab = null;
//            if (mask == 1 << 0)
//            {
//                newWallPrefab = Resources.Load("Walls/Wall_Right") as GameObject;
//                wall.direction = IsoWall.WallDirection.Right;
//            }
//            else if (mask == 1 << 1)
//            {
//                newWallPrefab = Resources.Load("Walls/Wall_Top") as GameObject; ;
//                wall.direction = IsoWall.WallDirection.Top;
//            }
//            else if (mask == (1 << 0 | 1 << 1))
//            {
//                newWallPrefab = Resources.Load("Walls/Wall_TR") as GameObject; ;
//                wall.direction = IsoWall.WallDirection.TopAndRight;
//            }

//            Assert.Should(newWallPrefab != null);
//            //TODO:
//            //IsoMap.Instance.GetGridAt(tilePos).GetWallEntity().Destroy();
//            //TODO create new wall
////            var go =
////                (GameObject) GameObject.Instantiate(newWallPrefab, Vector3.zero,
////                    Quaternion.identity);
////            go.GetComponent<TileEntity>().Init();
////            go.GetComponent<TileEntity>().SetTilePosition(tilePos);
////            IsoMap.Instance.AddEntity(go.GetComponent<TileEntity>());
//            //TODO:
//            //IsoMap.Instance.GetGridAt(tilePos).GetWallEntity().DestroyNow();
//            //var go =
//            //    (GameObject) GameObject.Instantiate(newWallPrefab, Vector3.zero,
//            //        Quaternion.identity);
//            //go.GetComponent<TileEntity>().Init();
//            //go.GetComponent<TileEntity>().SetTilePosition(tilePos);
//            //IsoMap.Instance.AddEntity(go.GetComponent<TileEntity>());
//        }
//        else
//        {
//            wall.direction = IsoWall.WallDirection.Center;
//        }
//        WriteWall(wall);
//    }

//    private bool IsWall(int x, int y)
//    {
//        return IsoMap.Instance.IsWall(x, y);
//    }

//    private void WriteWall(IsoWall wall)
//    {
//        int x = wall.x * 2 + 1;
//        int y = wall.y * 2 + 1;
//        wallMap[x + 1, y] = 0;//reset
//        wallMap[x, y + 1] = 0;//reset
//        wallMap[x, y] = 1;
//        if (wall.direction == IsoWall.WallDirection.Top)
//        {
//            wallMap[x, y + 1] = 1;
//        }
//        else if (wall.direction == IsoWall.WallDirection.Right)
//        {
//            wallMap[x + 1, y] = 1;
//        }
//        else if (wall.direction == IsoWall.WallDirection.TopAndRight)
//        {
//            wallMap[x + 1, y] = 1;
//            wallMap[x, y + 1] = 1;
//        }
//    }

//    private void RemoveWall(IsoWall wall)
//    {
//        int x = wall.x * 2 + 1;
//        int y = wall.y * 2 + 1;
//        wallMap[x + 1, y] = 0;//reset
//        wallMap[x, y + 1] = 0;//reset
//        wallMap[x, y] = 0;//reset
//    }
//}
