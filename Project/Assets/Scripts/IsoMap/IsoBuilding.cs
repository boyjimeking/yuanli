using UnityEngine;
using System.Collections;
using System;

public class IsoBuilding
{
    public GameObject go;
	public EntityType entityType;
	public int width;
	public int height;
    public int x;
    public int y;

    public override string ToString()
    {
        return string.Format("IsoBuilding[x:{0},y:{1},width:{2},height:{3}]", x, y, width, height);
    }
}
