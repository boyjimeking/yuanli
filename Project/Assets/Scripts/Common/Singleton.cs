using System;
using UnityEngine;
using System.Collections;

public abstract class Singleton<T> where T : class
{
    private static T _instance;
    private static object _lock = new object();
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = Activator.CreateInstance(typeof(T), true) as T;
                }
                return _instance;
            }
        }
    }
}
