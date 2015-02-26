//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using System.Collections;

//public class RouteMap : Singleton<RouteMap>
//{
//    private RouteMap()
//    {
//        Reset();

////        IsoBuilding test1 = new IsoBuilding() { width = 3, height = 3, x = 3, y = 3 };
////        IsoBuilding test2 = new IsoBuilding() { width = 3, height = 3, x = 0, y = 2 };
////        WriteBuilding(test1);
////        WriteBuilding(test2);
////        RebuildRouteMap();
////        Dump(routeMapLong);
////        Dump(routeMapShort);
//    }
//    private static short[,] IsoBuildingData1x1 =
//    {
//        {V, O, V},
//        {O, H, O},
//        {V, O, V}

////        {O, O, O},
////        {O, H, O},
////        {O, O, O}
//    };

//    private static short[,] IsoBuildingData2x2 = {
//        {V,V,O,V,V},
//        {V,O,H,O,V},
//        {O,H,H,H,O},
//        {V,O,H,O,V},
//        {V,V,O,V,V}
            
////        {O,O,O,O,O},
////        {O,H,H,H,O},
////        {O,H,H,H,O},
////        {O,H,H,H,O},
////        {O,O,O,O,O}
//    };

//    private static short[,] IsoBuildingData3x3 = {
//        {V,V,O,O,O,V,V},
//        {V,O,H,H,H,O,V},
//        {O,H,H,H,H,H,O},
//        {O,H,H,H,H,H,O},
//        {O,H,H,H,H,H,O},
//        {V,O,H,H,H,O,V},
//        {V,V,O,O,O,V,V},

////        {O,O,O,O,O,O,O},
////        {O,H,H,H,H,H,O},
////        {O,H,H,H,H,H,O},
////        {O,H,H,H,H,H,O},
////        {O,H,H,H,H,H,O},
////        {O,H,H,H,H,H,O},
////        {O,O,O,O,O,O,O},
//    };

//    private static short[,] IsoBuildingData4x4 = {
//        {V,V,O,O,O,O,O,V,V},
//        {V,O,H,H,H,H,H,O,V},
//        {O,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,O},
//        {V,O,H,H,H,H,H,O,V},
//        {V,V,O,O,O,O,O,V,V},

////        {O,O,O,O,O,O,O,O,O},
////        {O,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,O},
////        {O,O,O,O,O,O,O,O,O},
//    };
//    private static short[,] IsoBuildingData5x5 = {
//        {V,V,O,O,O,O,O,O,O,V,V},
//        {V,O,H,H,H,H,H,H,H,O,V},
//        {O,H,H,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,H,H,O},
//        {O,H,H,H,H,H,H,H,H,H,O},
//        {V,O,H,H,H,H,H,H,H,O,V},
//        {V,V,O,O,O,O,O,O,O,V,V},

////        {O,O,O,O,O,O,O,O,O,O,O},
////        {O,H,H,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,H,H,O},
////        {O,H,H,H,H,H,H,H,H,H,O},
////        {O,O,O,O,O,O,O,O,O,O,O},
//    };

//    private short[,] GetBuildingData(int width)
//    {
//        switch (width)
//        {
//        case 1:
//            return IsoBuildingData1x1;
//        case 2:
//            return IsoBuildingData2x2;
//        case 3:
//            return IsoBuildingData3x3;
//        case 4:
//            return IsoBuildingData4x4;
//        case 5:
//            return IsoBuildingData5x5;
//        default:
//            Debug.LogError("Known building width:" + width);
//            break;
//        }
//        return null;
//    }

//    private const short H = BUILDING_VALUE;//for gridData map
//    private const short V = NOT_INIT_VALUE;//for gridData map
//    private const short O = STANDING_VALUE;//for gridData map

//    public const short BUILDING_VALUE = short.MinValue;
//    public const short NOT_INIT_VALUE = short.MaxValue;
//    public const short STANDING_VALUE = 0;
//    private const short WALL_COST = 20;

//    public const int WIDTH = Constants.WIDTH * 2 + 1;
//    public const int HEIGHT = Constants.WIDTH * 2 + 1;

//    private short[,] buildingMap;
//    public short[,] routeMapShort;
//    private short[,] routeMapLong;

//    private sbyte[,] wallMap;

//    public const int LONG_RANGE_VALUE = 6;
//    private void Dump(short[,] tiles)
//    {
//        string output = "";
//        for(int j=tiles.GetLowerBound(1);j<=tiles.GetUpperBound(1);++j)//row
//        {
//            for(int i=tiles.GetLowerBound(0);i<=tiles.GetUpperBound(0);++i)//column
//            {
//                output += tiles[i,j] + "\t";
//            }
//            output += "\n";
//        }
//        Debug.Log(output);
//    }

//    private void Dump(sbyte[,] tiles)
//    {
//        string output = "";
//        for (int j = tiles.GetLowerBound(1); j <= tiles.GetUpperBound(1); ++j)//row
//        {
//            for (int i = tiles.GetLowerBound(0); i <= tiles.GetUpperBound(0); ++i)//column
//            {
//                output += tiles[i, j] + "\t";
//            }
//            output += "\n";
//        }
//        Debug.Log(output);
//    }

//    private void BuildRoadMap(short[,] map)
//    {
//        bool dirty = true;
//        while (dirty)
//        {
//            dirty = false;
//            for (int i = 0; i < WIDTH; i++)
//            {
//                for (int j = 0; j < HEIGHT; j++)
//                {
//                    if (map[i, j] == BUILDING_VALUE)
//                        continue;
//                    int minX;
//                    int minY;
//                    if (GetMinNeighbor(map, i, j, out minX, out minY,false,false))
//                    {
//                        if (map[i, j] > map[minX, minY] + 1)//if much bigger
//                        {
//                            if (wallMap[i, j] == 0 || map[minX, minY] < -1)//if not wall, or long attack within range do not consider wall
//                            {
//                                map[i, j] = (sbyte)(map[minX, minY] + 1);
//                                dirty = true;
//                            }
//                            else
//                            {
//                                if (map[i, j] > map[minX, minY] + WALL_COST + 1)
//                                {
//                                    map[i, j] = (sbyte)(map[minX, minY] + WALL_COST + 1);
//                                    dirty = true;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// 写入建筑占地数据
//    /// </summary>
//    /// <param name="building"></param>
//    /// <param name="type"></param>
//    public void WriteBuilding(TileEntity building)
//    {
//        int width = building.width * 2;
//        int height = building.height * 2;
//        int startX = building.GetTilePos().x * 2;
//        int startY = building.GetTilePos().y * 2;
//        short[,] data = GetBuildingData(building.width);
	    
//        for(int i=0;i <= width;i++)
//            for (int j = 0; j <= height; j++)
//            {

//                buildingMap[startX + i, startY + j] = data[i, j];
//            }
//    }

//    public void RemoveBuilding(TileEntity building)
//    {
//        Assert.Should(building != null);
//        int width = building.width * 2;
//        int height = building.height * 2;
//        int startX = building.GetTilePos().x * 2;
//        int startY = building.GetTilePos().y * 2;

//        for (int i = 0; i <= width; i++)
//            for (int j = 0; j <= height; j++)
//            {
//                buildingMap[startX + i, startY + j] = NOT_INIT_VALUE;
//            }
//    }

//    public void RebuildRouteMap(sbyte[,] wallMap)
//    {
//        this.wallMap = wallMap;
//        var startBuildTime = Time.realtimeSinceStartup;
        
//        InitRouteMap(routeMapShort,AttackRangeType.Short);
//        BuildRoadMap(routeMapShort);
//        //Trick method use short map to creat long map
//        for (int i = 0; i < WIDTH; i++)
//        {
//            for (int j = 0; j < HEIGHT; j++)
//            {
//                if (routeMapShort[i, j] != BUILDING_VALUE)
//                {
//                    routeMapLong[i, j] = (short)(routeMapShort[i, j] - LONG_RANGE_VALUE);
//                }
//                else
//                {
//                    routeMapLong[i, j] = BUILDING_VALUE;
//                }
//            }
//        }
//        /*
//        InitRouteMap(routeMapLong,AttackRangeType.Long);
//        BuildRoadMap(routeMapLong);
//        */
//        Debug.Log("RebuildRouteMap Time:" + (Time.realtimeSinceStartup - startBuildTime));
//        //TODO
////        Dump(buildingMap);
////        Dump(wallMap);
////        Dump(routeMapShort);
////        Dump(routeMapLong);
//    }
//    private void InitRouteMap(short[,] routeMap,AttackRangeType type)
//    {
//        sbyte initValue;
//        switch (type)
//        {
//        case AttackRangeType.Short:
//            initValue = 0;
//            break;
//        case AttackRangeType.Long:
//            initValue = -LONG_RANGE_VALUE;
//            break;
//        default:
//            Debug.LogError("Unknown AttackRangeType");
//            return;
//        }
//        for (int i = 0; i < WIDTH; i++)
//        {
//            for (int j = 0; j < HEIGHT; j++)
//            {
//                if (buildingMap[i, j] == STANDING_VALUE)
//                {
//                    routeMap[i, j] = initValue;
//                }
//                else
//                {
//                    routeMap[i, j] = buildingMap[i, j];
//                }
//            }
//        }
//    }
//    public LinkedList<TilePoint> GetPath(int x, int y,AttackRangeType type,out int targetRouteX,out int targetRouteY)
//    {
//        short[,] map = null;
//        switch (type)
//        {
//            case AttackRangeType.Short:
//            map = routeMapShort;
//            break;
//            case AttackRangeType.Long:
//            map = routeMapLong;
//            break;
//        default:
//            Debug.LogError("Known type");
//            break;
//        }
//        LinkedList<TilePoint> path = new LinkedList<TilePoint>();
//        targetRouteX = 0;
//        targetRouteY = 0;
//        while (true)
//        {
//            var value = map[x, y];
//            TilePoint tilePoint = new TilePoint {x = x,y = y};
//            if (value >= 0) //We have reach the distination. or can't move
//            {
//                path.AddLast(tilePoint);
//            }else if (value == BUILDING_VALUE)
//            {
//                targetRouteX = x;
//                targetRouteY = y;
//                break;
//            }
//            int minX, minY;
//            bool found = GetMinNeighbor(map, x, y, out minX, out minY,true,true);
//            Assert.Should(found);
//            if (!found)
//            {
//                break;
//            }

//            x = minX;
//            y = minY;
//        }

//        return path;
//    }

//    private bool GetMinNeighbor(short[,] map, int x, int y, out int minX, out int minY, bool includeDiagonal,bool includeBuildingValue)
//    {
//        short minValue = Int16.MaxValue;
//        bool found = false;
//        minX = 0;
//        minY = 0;
//        for (int i = - 1; i <= 1; i++)
//        {
//            for (int j = - 1; j <= 1; j++)
//            {
//                //不包括斜角
//                if (!includeDiagonal && i != 0 && j != 0)
//                {
//                    continue;
//                }
//                var currX = x + i;
//                var currY = y + j;
//                if (currX >= 0 && currX < WIDTH && currY >= 0 && currY < HEIGHT)
//                {
//                    if (map[currX, currY] < minValue && (includeBuildingValue || map[currX, currY] > BUILDING_VALUE))
//                    {
//                        found = true;
//                        minValue = map[currX, currY];
//                        minX = currX;
//                        minY = currY;
//                    }
//                }
//            }
//        }
//        return found;
//    }

//    public void Reset()
//    {
//        buildingMap = new short[WIDTH, HEIGHT];
//        routeMapShort = new short[WIDTH, HEIGHT];
//        routeMapLong = new short[WIDTH, HEIGHT];
//        for (int i = 0; i < WIDTH; ++i)
//        {
//            for (int j = 0; j < HEIGHT; ++j)
//            {
//                buildingMap[i, j] = NOT_INIT_VALUE;
//            }
//        }
//    }
//}
