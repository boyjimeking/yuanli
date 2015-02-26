
using System;

public class EntityAnimationDirection
{
    public EntityDirection direction;
    public bool flipX;

    public override string ToString()
    {
        return direction.ToString() + " flipX:" + flipX;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        var other = obj as EntityAnimationDirection;
        return other.direction == direction && other.flipX == flipX;
    }

    public override int GetHashCode()
    {
        throw new Exception("NotIplements");
    }
}
