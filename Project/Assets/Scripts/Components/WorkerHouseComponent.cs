using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 建筑工人小屋组件
/// </summary>
public class WorkerHouseComponent : EntityComponent
{
    //  工人信息
    private Dictionary<TileEntity, WorkmanComponent> _workerHash = null;
    
    /// <summary>
    /// 添加一个工人
    /// </summary>
    /// <param name="workman"></param>
    /// <returns></returns>
    public bool AddAWorkman(TileEntity workman)
    {
        var comp = workman.GetComponent<WorkmanComponent>();
        //  没有工作能力的工人 o(╯□╰)o
        if (comp == null)
            return false;
        Assert.Should(_workerHash != null);
        _workerHash.Add(workman, comp);
        comp.OnAddToWorkerHouse(Entity);
        return true;
    }

    /// <summary>
    /// 请求一个工人出工o.o
    /// </summary>
    /// <param name="targeter"></param>
    public TileEntity AskAWorkman(TileEntity targeter)
    {
        //  获取工人：优先获取空闲状态的工人、没有空闲的则获取下班回家途中的工人（好惨
        TileEntity workman = GetWorkmanByState(WorkmanComponent.WorkerState.Free);
        if (workman == null)
        {
            workman = GetWorkmanByState(WorkmanComponent.WorkerState.FinishWork);
            if (workman == null)
                return null;
        }
        //  给工人分配工作
        _workerHash[workman].BuildStart(targeter);
        return workman;
    }

    /// <summary>
    /// 释放一个工人（建造完毕归还）
    /// </summary>
    public void GiveBackAWorkman(TileEntity workman)
    {
        if (workman == null)
            return;
        var comp = _workerHash[workman];
        //  哪里来的野工人，没在这里登记呢。o(╯□╰)o
        if (comp == null)
            return;
        //  收工
        comp.BuildFinish();
    }

    public override void Init()
    {
        base.Init();
        this.enabled = false;
        _workerHash = new Dictionary<TileEntity, WorkmanComponent>();
    }

    public override void Destroy()
    {
        _workerHash.Clear();
        base.Destroy();
    }

    /// <summary>
    /// 获取任意一个处于指定状态的工人
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private TileEntity GetWorkmanByState(WorkmanComponent.WorkerState state)
    {
        foreach (var item in _workerHash)
        {
            if (item.Value.State == state)
                return item.Key;
        }
        return null;
    }
}
