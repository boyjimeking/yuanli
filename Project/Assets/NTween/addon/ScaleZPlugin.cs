using UnityEngine;

class ScaleZPlugin : NTweenPlugin
{
    private float from;
    private float change;
    private Transform theTarget;
    public ScaleZPlugin()
    {
        propName = "scaleZ";
    }
    protected override void onUpdate(float ratio)
    {
        var scale = theTarget.localScale;
        scale.z = from + change * ratio;
        theTarget.localScale = scale;
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
        from = theTarget.localScale.z;
        change = CastToFloat(value);
        if (!isRelative)
            change -= from;
        return true;
    }
}
