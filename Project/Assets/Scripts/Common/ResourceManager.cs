
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private readonly Dictionary<string, Object> resourceCache = new Dictionary<string, Object>(); 
    public Object Load(string path)
    {
        if (resourceCache.ContainsKey(path))
        {
            return resourceCache[path];
        }
        var obj = Resources.Load(path);
        if (obj == null)
        {
            Debug.LogError("ResourceManager load return null of path:" + path);
            return null;
        }
        resourceCache.Add(path,obj);
        return obj;
    }

    public Object LoadAndCreate(string path)
    {
        var obj = Load(path);
        if (obj == null)
            return null;
        return Object.Instantiate(obj);
    }

    public void ClearCache()
    {
        resourceCache.Clear();
        Resources.UnloadUnusedAssets();
    }
}
