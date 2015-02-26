
using System;
using UnityEngine;

[Serializable]
public struct TilePoint
{
    public TilePoint(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int x;
    public int y;
    public bool Equals(TilePoint other)
    {
        return ((this.x == other.x) && (this.y == other.y));
    }
    public TilePoint Left { get {return new TilePoint(x-1,y);}}
    public TilePoint Right { get {return new TilePoint(x+1,y);}}
    public TilePoint Top { get {return new TilePoint(x,y+1);}}
    public TilePoint Bottom { get {return new TilePoint(x,y-1);}}
    public override bool Equals(object obj)
    {
        if (object.ReferenceEquals(null, obj))
        {
            return false;
        }
        return ((obj is TilePoint) && this.Equals((TilePoint)obj));
    }

    public override int GetHashCode()
    {
        return ((this.x * 997) ^ this.y);
    }

    public override string ToString()
    {
        return string.Format("[{0},{1}]", x, y);
    }

    public float GetSqrMagnitude()
    {
        return (float)((this.x * this.x) + (this.y * this.y));
    }

    public static implicit operator Vector2(TilePoint t)
    {
        return new Vector2((float)t.x, (float)t.y);
    }
    public static TilePoint operator +(TilePoint t1, TilePoint t2)
    {
        return new TilePoint(t1.x + t2.x, t1.y + t2.y);
    }

    public static TilePoint operator -(TilePoint t1, TilePoint t2)
    {
        return new TilePoint(t1.x - t2.x, t1.y - t2.y);
    }

    public static bool operator ==(TilePoint t1, TilePoint t2)
    {
        return ((t1.x == t2.x) && (t1.y == t2.y));
    }

    public static bool operator !=(TilePoint t1, TilePoint t2)
    {
        return !(t1 == t2);
    }
}