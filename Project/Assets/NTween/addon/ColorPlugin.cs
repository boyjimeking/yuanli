using System;
using UnityEngine;

class ColorPlugin : NTweenPlugin
{
    public ColorPlugin()
    {
        propName = "color";
    }
    protected override void onUpdate(float ratio)
    {
        NTweenData pt = this._firstPT;
        while (pt != null)
        {
            var co = ((Color)(pt.change));
            var change = co * ratio;
            SetPropValue(pt.target, pt.property, (Color)pt.start + change);
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
            var change = (Color)value;
            if (!isRelative)
                change -= theColor;
            addSubTween(theTarget, "color", theColor, change);
            return true;
        }else
        {
            var go = (GameObject) GetPropValue(target, "gameObject");
            if(go)
            {
                add(go, (Color)value, isRelative);
                foreach (Transform child in go.transform)
                {
                    add(child.gameObject, (Color)value, isRelative);
                }
                return true;
            }
            return false;
        }
    }
    private void add(GameObject go,Color value,bool isRelative)
    {
        if (go.renderer)
        {
            var theColor = go.renderer.material.color;
            var theTarget = go.renderer.material;
            var change = value;
            if (!isRelative)
                change -= theColor;
            addSubTween(theTarget, "color", theColor, change);
        }
    }
}
