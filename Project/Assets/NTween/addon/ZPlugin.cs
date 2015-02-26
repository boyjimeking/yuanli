using UnityEngine;

class ZPlugin : NTweenPlugin
{
    private float from;
    private float change;
    private Transform theTarget;
    public ZPlugin()
    {
        propName = "z";
    }
    protected override void onUpdate(float ratio)
    {
        var nowPosition = theTarget.localPosition;
        nowPosition.z = from + change * ratio;
        SetPropValue(theTarget, "localPosition", nowPosition);
    }
    public override bool onInit(object target, object value, bool isRelative, NTween tween)
    {
        var r = GetPropValue(target, "localPosition");
        if (r != null)
        {
            from = ((Vector3)r).z;
            theTarget = (Transform)target;
        }
        else
        {
            var go = (GameObject)GetPropValue(target, "gameObject");
            if (go.transform)
            {
                from = go.transform.localPosition.z;
                theTarget = go.transform;
            }
            else
            {
                return false;
            }
        }
        change = CastToFloat(value);
        if(!isRelative)
            change -= from;
        return true;
    }
}
