using UnityEngine;
using System.Collections;

#region NTweenData
public class NTweenData
{
    public object target;
    public string property;
    public object start;
    public object change;
    public string name;
    public bool isPlugin;
    public NTweenData _next;
    public NTweenData _prev;

    public NTweenData(object target, string property, object start, object change, string name, bool isPlugin, NTweenData nextNode = null)
    {
        this.target = target;
        this.property = property;
        this.start = start;
        this.change = change;
        this.name = name;
        this.isPlugin = isPlugin;
        if (nextNode != null)
        {
            nextNode._prev = this;
            this._next = nextNode;
        }
    }
    public override string ToString()
    {
        return "NTweenData[target:" + target + ",property:" + property + "]";
    }
}
#endregion