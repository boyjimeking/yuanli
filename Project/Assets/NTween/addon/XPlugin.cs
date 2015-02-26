using UnityEngine;

class XPlugin : NTweenPlugin
{
    private float from;
    private float change;
    private Transform theTarget;
    public XPlugin()
    {
        propName = "x";
    }
    protected override void onUpdate(float ratio)
    {
        var nowPosition = theTarget.transform.localPosition;
        nowPosition.x = from + change * ratio;
        SetPropValue(theTarget, "localPosition", nowPosition);
    }
    public override bool onInit(object target, object value, bool isRelative, NTween tween)
    {
        var r = GetPropValue(target, "localPosition");
        if (r != null)
        {
            from = ((Vector3)r).x;
            theTarget = (Transform)target;
        }
        else
        {
            var go = (GameObject)GetPropValue(target, "gameObject");
            if (go.transform)
            {
                from = go.transform.localPosition.x;
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
