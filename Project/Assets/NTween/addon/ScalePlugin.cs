using UnityEngine;

class ScalePlugin : NTweenPlugin
{
    private Vector3 from;
    private Vector3 change;
    private Transform theTarget;
    public ScalePlugin()
    {
        propName = "scale";
    }
    protected override void onUpdate(float ratio)
    {
        theTarget.localScale = from + change * ratio;
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
        from = theTarget.localScale;
        if(value is Vector3)
        {
            change = (Vector3) value;
        }
        else
        {
            var f = CastToFloat(value);
            change = new Vector3(f, f, f);
        }
        if(!isRelative)
        {
            change -= from;
        }
        return true;
    }
}
