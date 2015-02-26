using UnityEngine;

class ScaleYPlugin : NTweenPlugin
{
    private float from;
    private float change;
    private Transform theTarget;
    public ScaleYPlugin()
    {
        propName = "scaleY";
    }
    protected override void onUpdate(float ratio)
    {
        var scale = theTarget.localScale;
        scale.y = from + change * ratio;
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
        from = theTarget.localScale.y;
        change = CastToFloat(value);
        if (!isRelative)
            change -= from;
        return true;
    }
}
