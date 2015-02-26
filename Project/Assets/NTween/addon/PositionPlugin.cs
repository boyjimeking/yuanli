using UnityEngine;

class PositionPlugin : NTweenPlugin
{
    private Vector3 from;
    private Vector3 change;
    private Transform theTarget;
    public PositionPlugin()
    {
        propName = "position";
    }
    protected override void onUpdate(float ratio)
    {
        theTarget.localPosition = from + change*ratio;
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
        from = theTarget.localPosition;
        change = (Vector3) value;
        if (!isRelative)
        {
            change -= from;
        }
        return true;
    }
}
