using UnityEngine;

class RotationPlugin : NTweenPlugin
{
    private Vector3 from;
    private Vector3 change;
    private Transform theTarget;
    public RotationPlugin()
    {
        propName = "rotation";
    }
    protected override void  onUpdate(float ratio)
    {
        theTarget.localRotation = Quaternion.Euler(from + change * ratio);
    }
    public override bool onInit(object target, object value, bool isRelative, NTween tween)
    {
        if (target is Transform)
        {
            theTarget = (Transform)target;
        }
        else
        {
            var go = (GameObject)GetPropValue(target, "gameObject");
            if (go.transform)
            {
                theTarget = go.transform;
            }
            else
            {
                return false;
            }
        }
        from = theTarget.transform.localRotation.eulerAngles;
        var to = (Vector3)value;
        if(isRelative)
        {
            to = from + to;
        }
        else
        {
            to = new Vector3(shortDistValue(from.x, to.x), shortDistValue(from.y, to.y), shortDistValue(from.z, to.z));
        }
        change = to - from;
        return true;
    }
    private float shortDistValue(float a, float b)
    {
        while (a > 360)
            a -= 360;
        while (a < 0)
            a += 360;
        while (b > 360)
            b -= 360;
        while (b < 0)
            b += 360;
        if (b - a < -180)
        {
            return 360 + b;
        }
        if (b - a > 180)
        {
            return b - 360;
        }
        return b;
    }
}
