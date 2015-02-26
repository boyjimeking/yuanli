
using System.Collections;
using UnityEngine;

public class NTweenParam
{
    private Hashtable param = new Hashtable();

    public Hashtable getParam()
    {
        return param;
    }
    public NTweenParam prop(string propertyName, object value)
    {
        param.Add(propertyName,value);
        return this;
    }
    public NTweenParam gameObject(GameObject go)
    {
        param.Add("gameObject", go);
        return this;
    }
    public NTweenParam ease(NTweenEaseFunction easeFunction)
    {
        param.Add("ease", easeFunction);
        return this;
    }
    public NTweenParam delay(float delay)
    {
        param.Add("delay", delay);
        return this;
    }
    public NTweenParam loop(int loopCount = -1)
    {
        param["loop"] = loopCount;
        return this;
    }
    public NTweenParam loopDelay(float delay)
    {
        param.Add("loopDelay",delay);
        return this;
    }
    public NTweenParam yoyo()
    {
        param.Add("yoyo", true);
        return this;
    }
    public NTweenParam yoyoReverse()
    {
        param.Add("yoyoReverse", true);
        return this;
    }

    public NTweenParam rewind()
    {
        param.Add("yoyoReverse", true);
        if(!param.ContainsKey("loop"))
            param.Add("loop",1);
        return this;
    }
    public NTweenParam useFrames()
    {
        param.Add("useFrames",true);
        return this;
    }
    public NTweenParam skipPlugin()
    {
        param.Add("skipPlugin",true);
        return this;
    }
    public NTweenParam overwrite()
    {
        param.Add("overwrite",1);
        return this;
    }
    public NTweenParam renderNow()
    {
        param.Add("renderNow",true);
        return this;
    }
    public NTweenParam onStart(NTweenCallback callback, object payload=null)
    {
        param.Add("onStart", callback);
        if (payload != null)
            param.Add("onStartParams", payload);
        return this;
    }
    public NTweenParam onUpdate(NTweenCallback callback, object payload=null)
    {
        param.Add("onUpdate",callback);
        if(payload != null)
            param.Add("onUpdateParams",payload);
        return this;
    }
    public NTweenParam onComplete(NTweenCallback callback, object payload=null)
    {
        param.Add("onComplete",callback);
        if(payload != null)
            param.Add("onCompleteParams",payload);
        return this;
    }

    public NTweenParam onLoopComplete(NTweenCallback callback, object payload=null)
    {
        param.Add("onLoopComplete",callback);
        if(payload != null)
            param.Add("onLoopCompleteParams",payload);
        return this;
    }
}
