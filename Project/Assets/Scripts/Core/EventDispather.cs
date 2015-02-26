using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventDispather
{
    //  委托
    public delegate void CallBack(string eventType, object obj);

    //  存储事件
    private static Dictionary<string, Dictionary<CallBack, bool>> hashEventType = new Dictionary<string,Dictionary<CallBack,bool>>();

    /**
     * 添加事件监听
     * @param type 事件类型
     * @param method 监听函数
     * */
    public static void AddEventListener(string type, CallBack method)
    {
        Dictionary<CallBack, bool> hashMethod;
        if (hashEventType.ContainsKey(type))
        {
            hashMethod = hashEventType[type];
        }
        else
        {
            hashMethod = new Dictionary<CallBack, bool>();
            hashEventType.Add(type, hashMethod);
        }
        if (!hashMethod.ContainsKey(method))
            hashMethod.Add(method, true);
    }
    /**
     * 移除事件监听
     * @param type 事件类型
     * @param method 监听函数
     * */
    public static void RemoveEventListener(string type, CallBack method)
    {
        if (!hashEventType.ContainsKey(type))
            return;
        var hashMethod = hashEventType[type];
        hashMethod.Remove(method);
        if (0 == hashMethod.Keys.Count)
            hashEventType.Remove(type);
    }
    /**
     * 是否含有某个事件
     * @param type 事件类型
     * */
    public static bool HasEventListener(string type)
    {
        return hashEventType.ContainsKey(type);
    }
    /**
     * 事件执行
     * @param type 事件类型
     * @param method 监听函数
     * */
    public static void DispatherEvent(string type, object obj = null)
    {
        if (!hashEventType.ContainsKey(type))
            return;
        var hashMethod = hashEventType[type];
        Dictionary<CallBack, bool> hashClone = new Dictionary<CallBack, bool>();
        //  防止快照不同步，做个拷贝
        foreach (var de in hashMethod)
        {
            hashClone.Add(de.Key, de.Value);
        }
        //  事件派发
        foreach (var de in hashClone)
        {
            de.Key.Invoke(type, obj);
        }
    }
}
