using System;
using System.Collections;
using System.Collections.Generic;

public interface IDelayAiObject
{
    bool Run();

    int UniqueId { get; }
}

public class DelayAiObject<TResult> : IDelayAiObject
{
    public readonly TileEntity entity;
    public readonly Action<DelayAiObject<TResult>> callback;
    public readonly IEnumerator<TResult> iterator;

    private int _uniqueId;
    private TResult _result;

    public TResult result
    {
        get { return _result; }
    }

    public int UniqueId
    {
        get { return _uniqueId; }
    }

    public DelayAiObject(int id, TileEntity entity, Action<DelayAiObject<TResult>> callback, IEnumerator<TResult> iterator)
    {
        this.entity = entity;
        this.callback = callback;
        this.iterator = iterator;
        _uniqueId = id;
        _result = default(TResult);
    }

    public bool Run()
    {
        if (entity.IsDead())
            return true;

        if (iterator.MoveNext())
        {
            _result = iterator.Current;
            return false;
        }
        else
        {
            if (callback != null)
            {
                callback(this);
            }
            return true;
        }
    }
}

public class DelayManager : Singleton<DelayManager>
{
    private class DelayTime
    {
        public float time;
        public Action func;
    }

    //  延迟调用列表
    private List<Action> delayCallList1 = new List<Action>();
    private List<DelayTime> delayCallList2 = new List<DelayTime>();

    //  延迟计算
    private IDelayAiObject _current = null;
    private List<IDelayAiObject> _aiList = new List<IDelayAiObject>();
    private Dictionary<int, IDelayAiObject> _aiHash = new Dictionary<int, IDelayAiObject>();
    private int _aiUniqueId = 0;

    /// <summary>
    /// 添加延迟调用对象（会在下次逻辑帧更新的时候调用 or 延迟时间为0时调用）
    /// </summary>
    /// <param name="delayCall"></param>
    /// <param name="time"></param>
    public void AddDelayCall(Action delayCall, float time = 0.0f)
    {
        if (time > 0)
        {
            delayCallList2.Add(new DelayTime() { time = time, func = delayCall });
        }
        else
        {
            delayCallList1.Add(delayCall);
        }
    }

    /// <summary>
    /// 添加延迟AI对象，返回唯一ID号。
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="entity"></param>
    /// <param name="callback"></param>
    /// <param name="iterator"></param>
    /// <returns></returns>
    public int AddDelayAi<TResult>(TileEntity entity, Action<DelayAiObject<TResult>> callback, IEnumerator<TResult> iterator)
    {
        int uniqueId = ++_aiUniqueId;
        var ai = new DelayAiObject<TResult>(uniqueId, entity, callback, iterator);
        _aiList.Add(ai);
        _aiHash.Add(uniqueId, ai);
        return uniqueId;
    }

    public int AddDelayAi<TResult>(TileEntity entity, Action<DelayAiObject<TResult>> callback, IEnumerable<TResult> iterator)
    {
        return AddDelayAi<TResult>(entity, callback, iterator.GetEnumerator());
    }

    /// <summary>
    /// 根据唯一ID号移除延迟AI
    /// </summary>
    /// <param name="uniqueId"></param>
    public void RemoveDelayAi(int uniqueId)
    {
        if (_aiHash.ContainsKey(uniqueId))
        {
            _aiList.Remove(_aiHash[uniqueId]);
            _aiHash.Remove(uniqueId);
        }
        else if (_current != null && _current.UniqueId == uniqueId)
        {
            //  REMARK：这里可以进行stop处理
            _current = null;
        }
    }

    public void Init()
    {
        delayCallList1.Clear();
        delayCallList2.Clear();

        _aiList.Clear();
        _aiHash.Clear();
        _current = null;
        _aiUniqueId = 0;
    }

    public void Update(float dt)
    {
        UpdateDelayCall(dt);
        UpdateDelayAi(dt);
    }

    private void UpdateDelayCall(float dt)
    {
        UpdateDelayCall1(dt);
        UpdateDelayCall2(dt);
    }

    private void UpdateDelayCall1(float dt)
    {
        if (delayCallList1.Count == 0)
            return;
        foreach (var call in delayCallList1.Clone())
            call();
        delayCallList1.Clear();
    }

    private void UpdateDelayCall2(float dt)
    {
        if (delayCallList2.Count == 0)
            return;
        foreach (var call in delayCallList2.Clone())
        {
            call.time -= dt;
            if (call.time <= 0)
            {
                call.func();
                delayCallList2.Remove(call);
            }
        }
    }

    private void UpdateDelayAi(float dt)
    {
        if (_current == null)
        {
            if (_aiList.Count <= 0)
                return;
            _current = _aiList[0];
            _aiList.RemoveAt(0);
            _aiHash.Remove(_current.UniqueId);
        }
        if (_current.Run())
        {
            _current = null;
        }
    }
}