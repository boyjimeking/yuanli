using UnityEngine;

class ScaleXPlugin : NTweenPlugin
{
    private float from;
    private float change;
    private Transform theTarget;
    public ScaleXPlugin()
    {
        propName = "scaleX";
    }
    protected override void onUpdate(float ratio)
    {
        var scale = theTarget.localScale;
        scale.x = from + change * ratio;
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
        from = theTarget.localScale.x;
        change = CastToFloat(value);
        if (!isRelative)
            change -= from;
        return true;
    }
}
