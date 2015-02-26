
using System;
using UnityEngine;

class AlphaPlugin : NTweenPlugin
{
    public AlphaPlugin()
    {
        propName = "alpha";
    }
    protected override void onUpdate(float ratio)
    {
        NTweenData pt = this._firstPT;
        while (pt != null)
        {
            var co = ((Color)(pt.change));
            var r = co.a * ratio;
            SetPropValue(pt.target, pt.property, (Color)pt.start + new Color(0, 0, 0, r));
            pt = pt._next;
        }
        
    }
    public override bool onInit(object target, object value,bool isRelative, NTween tween)
    {
        var c = GetPropValue(target, "color");
        if(c != null)
        {
            Color theColor = (Color) c;
            object theTarget = target;
            var change = new Color(0, 0, 0, CastToFloat(value));
            if (!isRelative)
                change.a -= theColor.a;
            addSubTween(theTarget, "color", theColor, change);
            return true;
        }else
        {
            var go = (GameObject) GetPropValue(target, "gameObject");
            var a = CastToFloat(value);
            if(go)
            {
                add(go, a, isRelative);
                foreach (Transform child in go.transform)
                {
                    add(child.gameObject, a, isRelative);
                }
                return true;
            }
            return false;
        }
    }
    private void add(GameObject go,float value,bool isRelative)
    {
        if (go.renderer)
        {
            var theColor = go.renderer.material.color;
            var theTarget = go.renderer.material;
            var change = new Color(0, 0, 0, value);
            if (!isRelative)
                change.a -= theColor.a;
            addSubTween(theTarget, "color", theColor, change);
        }
    }
}
