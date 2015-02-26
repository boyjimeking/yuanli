using System;
using UnityEngine;
using System.Collections;

public abstract class NTweenPlugin : NTweenCore
{
    public string propName;
    protected NTweenData _firstPT;//plugin may contains serveral tween datas.
    private float _ratio;

    public virtual float getRatio()
    {
        return _ratio ;
    }
    public virtual void setRatio(float value)
    {
        _ratio = value;
        onUpdate(_ratio);
    }

    protected NTweenPlugin()
    {
        this._firstPT = null;
    }

    public abstract bool onInit(object target, object value, bool isRelative, NTween tween);

    protected void addSubTween(object target, string propName, object start, object change)
    {
        if (change != null)
        {
            _firstPT = new NTweenData(target, propName, start, change, propName, false, _firstPT);
        }
    }
    protected virtual void onUpdate(float ratio)
    {
        var pt = this._firstPT;
        while (pt != null)
        {
            SetPropValue(pt.target, pt.property, Addition(pt.start, Multiply(pt.change, ratio)));
            pt = pt._next;
        }
    }
}
