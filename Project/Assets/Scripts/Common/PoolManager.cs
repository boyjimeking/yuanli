using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<GameObject,Stack<GameObject> > instanceCache = new Dictionary<GameObject, Stack<GameObject>>();
    private readonly Dictionary<GameObject,Stack<GameObject>> instanceToPoolMap = new Dictionary<GameObject, Stack<GameObject>>();
    private GameObject CreateInstance(GameObject prefab,Vector3 position,Quaternion rotation)
    {
        var obj = (GameObject) Object.Instantiate(prefab,position,rotation);
        instanceToPoolMap.Add(obj, instanceCache[prefab]);
        return obj;
    }
    public void Cache(GameObject prefab, int count)
    {
        if (!instanceCache.ContainsKey(prefab))
        {
            instanceCache.Add(prefab,new Stack<GameObject>());
        }
        var stack = instanceCache[prefab];
        while (stack.Count < count)
        {
            stack.Push(CreateInstance(prefab,Vector3.zero,Quaternion.identity));
        }
    }

    public GameObject Spawn(GameObject prefab,Vector3 position,Quaternion rotation)
    {
        if (!instanceCache.ContainsKey(prefab))
            instanceCache.Add(prefab, new Stack<GameObject>());
        if (instanceCache[prefab].Count > 0)
        {
            var obj = instanceCache[prefab].Pop();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            obj.BroadcastMessage("Start",SendMessageOptions.DontRequireReceiver);
            return obj;
        }
        return CreateInstance(prefab, position, rotation);
    }

    public GameObject Spawn(GameObject prefab)
    {
        return Spawn(prefab, Vector3.zero, Quaternion.identity);
    }

    public void Recycle(GameObject inst)
    {
        if (!instanceToPoolMap.ContainsKey(inst))
        {
            Debug.LogWarning("Recycle object not create by PoolManager or PoolManager Cleared,using Destroy!");
            Object.Destroy(inst);
            return;
        }
        var pool = instanceToPoolMap[inst];
        inst.SetActive(false);
        pool.Push(inst);
    }

    public void Clear()
    {
        instanceToPoolMap.Clear();
        foreach (var keyValuePair in instanceCache)
        {
            foreach (var inst in keyValuePair.Value)
            {
                Object.Destroy(inst);
            }
        }
        instanceCache.Clear();
        Resources.UnloadUnusedAssets();
    }
}
