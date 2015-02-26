using System;
using System.Collections.Generic;
using UnityEngine;

public class GameBuffer
{
    public readonly EntityModel buffModel;      //  buff数据
    public readonly string buffEffectName;      //  buff效果,资源用
    public readonly string buffType;            //  buff类型
    public readonly float buffDamage;           //  buff伤害
    public readonly int buffActiveTimes;        //  buff作用次数
    public readonly float buffIntervalTime;     //  buff作用间隔

    private float m_delta;
    private int m_activeTime;

    private TileEntity m_owner = null;
    private GameObject m_view = null;

    public GameBuffer(TileEntity owner, EntityModel buffModel, string buffType)
    {
        m_owner = owner;
        this.buffModel = buffModel;
        this.buffEffectName = buffModel.buffEffectName;
        this.buffType = buffType;
        this.buffDamage = buffModel.buffDamage;
        this.buffActiveTimes = buffModel.buffActiveTimes;
        this.buffIntervalTime = buffModel.buffIntervalTime;

        m_delta = 0.0f;
        m_activeTime = 0;
        if (buffEffectName != null && buffEffectName != Constants.EMPTY)
        {
            var prefab = (GameObject)ResourceManager.Instance.Load("Effects/" + buffEffectName);
            if (prefab != null)
            {
                m_view = PoolManager.Instance.Spawn(prefab);
            }
            if (m_view != null)
            {
                m_owner.view.AddSubView(m_view, Vector3.zero);
            }
        }
    }

    public bool IsFinish
    {
        get { return m_activeTime >= buffActiveTimes; }
    }

    public void Destroy()
    {
        if (m_view != null)
        {
            m_view.transform.parent = null;
            PoolManager.Instance.Recycle(m_view);
        }
    }

    /// <summary>
    /// 更新Buffer作用时间，达到一个效果周期则返回true，否则返回false。
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public bool UpdateBufferDelta(float dt)
    {
        m_delta += dt;
        if (m_delta >= buffIntervalTime)
        {
            m_delta -= buffIntervalTime;
            ++m_activeTime;
            return true;
        }
        return false;
    }
}

/// <summary>
/// Buffer状态管理器
/// </summary>
public class GameBufferComponent : EntityComponent
{
    private List<GameBuffer> m_buffers = null;
    private Dictionary<string, GameBuffer> m_buffTypes = null;

    private List<GameBuffer> m_delayRemoved = null;

    public bool AddBuffer(EntityModel attackerModel)
    {
        //  不附件状态
        string buffType = attackerModel.buffType;
        if (buffType == null || buffType == Constants.EMPTY)
            return false;

        //  初始化
        if (m_buffers == null)
        {
            m_buffers = new List<GameBuffer>();
            m_buffTypes = new Dictionary<string, GameBuffer>();
            m_delayRemoved = new List<GameBuffer>();
        }

        //  相同状态新的状态替换旧的状态
        if (m_buffTypes.ContainsKey(buffType))
        {
            var buf = m_buffTypes[buffType];
            buf.Destroy();
            m_buffTypes.Remove(buffType);
            m_buffers.Remove(buf);
        }

        //  添加到列表并标记
        var buff = new GameBuffer(Entity, attackerModel, buffType);
        m_buffers.Add(buff);
        m_buffTypes.Add(buffType, buff);
        return true;
    }

    public GameBuffer GetBuffer(string buffType)
    {
        if (m_buffTypes != null && m_buffTypes.ContainsKey(buffType))
            return m_buffTypes[buffType];
        return null;
    }

    public bool AddBuffer(TileEntity attacker)
    {
        return AddBuffer(attacker.model);
    }

    public override void Update(float dt)
    {
        if (m_buffers == null)
            return;

        foreach (var b in m_buffers)
        {
            if (b.UpdateBufferDelta(dt))
            {
                //  REMARK：除去提升攻击、提升速度和麻痹状态的 buff 以外全部为伤害型 buff ※ 考虑用配置表处理o.o
                if (b.buffType != Constants.BUFF_TYPE_ATTACKUP && b.buffType != Constants.BUFF_TYPE_SPPEDUP && b.buffType != Constants.BUFF_TYPE_MABI)
                {
                    Entity.MakeDamage(b.buffDamage);
                    if (Entity.IsDead())
                        return;
                }
            }
            if (b.IsFinish)
                m_delayRemoved.Add(b);
        }

        foreach (var fin in m_delayRemoved)
        {
            fin.Destroy();
            m_buffTypes.Remove(fin.buffType);
            m_buffers.Remove(fin);
        }
        m_delayRemoved.Clear();
    }

    public override void Destroy()
    {
        if (m_buffers == null)
            return;

        foreach (var b in m_buffers)
            b.Destroy();

        m_buffers.Clear();
        m_buffTypes.Clear();
        m_delayRemoved.Clear();
    }
}