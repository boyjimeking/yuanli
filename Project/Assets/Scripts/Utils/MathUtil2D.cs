
using UnityEngine;

public class MathUtil2D
{
    public const float PI_OVER_180 = Mathf.PI / 180f;
    public const float _180_OVER_PI = 180f / Mathf.PI;

    public static float GetAngleFromVector(Vector2 vec)
    {
        var angle = Mathf.Atan2(vec.y, vec.x) * _180_OVER_PI;
        if (angle < 0)
            angle += 360;
        return angle;
    }
}
