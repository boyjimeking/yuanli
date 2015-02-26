using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBullet 
{
    public event Action<GameBullet> OnHitted;
    public float radius2 = 0.5f;            //  目标半径的平方（子弹进入这个范围属于命中）

    protected Vector2 m_firePosition;
    protected Vector2 m_currPosition;
    protected Vector2 m_targetPosition;     //  目标点（REMARK：对地点的子弹使用）
    protected TileEntity m_targetEntity;    //  跟踪目标（REMARK：只有对目标的才有，对地点的为null。）

    private EntityBulletType m_bulletType;
    protected float m_bulletSpeed;          //  子弹速度（每秒）
    private GameObject m_view;
    private bool m_hitted;

    public Vector2 targetPosition
    {
        get { return m_targetPosition; }
    }

    public TileEntity targetEntity
    {
        get { return m_targetEntity; }
    }

    public EntityBulletType bulletType
    {
        get { return m_bulletType; }
    }

    public bool IsDead
    {
        get { return m_hitted; }
    }

    /// <summary>
    /// 设置目标半径
    /// </summary>
    /// <param name="radius"></param>
    public void SetTargetRadius(float radius)
    {
        radius2 = radius * radius;
    }

    /// <summary>
    /// 设置发射位置
    /// </summary>
    /// <param name="v"></param>
    public void SetFirePosition(Vector2 v)
    {
        m_firePosition = v;
        m_currPosition = v;
    }

    public void SetBullet(EntityBulletType type, float speed)
    {
        m_bulletType = type;
        m_bulletSpeed = speed;
    }

    public void Init(string bulletName = "bullet1")
    {
        OnHitted = null;
        m_hitted = false;
        var prefab = (GameObject) ResourceManager.Instance.Load("Bullets/" + bulletName);
        if (prefab != null)
        {
            m_view = PoolManager.Instance.Spawn(prefab);
        }
    }

    public void Destroy()
    {
        if (m_view != null)
        {
            PoolManager.Instance.Recycle(m_view);
            m_view = null;
        }
    }

    public void Update(float dt)
    {
        UpdateMoving(dt);
    }

    public void ImmediatelyRefreshBulletView(Vector2? targetPos = null, float dt = 0.0f)
    {
        if (m_view != null)
        {
            if (targetPos == null)
            {
                targetPos = calcTargetPosition();
            }
            Vector2 vdir1 = targetPos.Value - m_currPosition;
            float height = CalcHeight(dt);
            Vector3 vdir2 = new Vector3(vdir1.x, CalcHeight(dt), vdir1.y).normalized;
            Vector3 forward = Camera.main.camera.transform.forward;
            m_view.transform.position = GetRenderPosition(height);
            m_view.transform.rotation = Quaternion.AngleAxis(90, forward) * Quaternion.LookRotation(forward, vdir2);
        }
    }

    private void UpdateMoving(float dt)
    {
        if (m_hitted)
            return;

        //  获取子弹目标点位置
        Vector2 targetPos = calcTargetPosition();

        //  计算从子弹当前位置到目标点位置的 向量
        Vector2 vdir = targetPos - m_currPosition;

        //  计算该帧向量增量
        Vector2 vadd = vdir.normalized * m_bulletSpeed * dt;

        //  子弹移动
        m_currPosition += vadd;

        //  刷新子弹显示
        ImmediatelyRefreshBulletView(targetPos, dt);
        //float height = CalcHeight(dt);
        //if (m_view != null)
        //{
        //    m_view.transform.position = GetRenderPosition(height);
        //    m_view.transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(vdir.x, height, vdir.y));
        //}
        
        //  子弹移动后的位置 在目标区域 范围内判断
        if (IsHitted(targetPos.x, targetPos.y))
        {
            m_hitted = true;
        }
        else
        {
            //  计算该帧移动之前 离 目的地的距离
            float len1 = vdir.sqrMagnitude;
            //  计算子弹移动的距离
            float len2 = vadd.sqrMagnitude;
            //  REMARK：子弹移动的距离大于了到目的地的距离（但仍未命中目标（则说明子弹速度过快越过了目的地 ※ 此时也属于命中
            if (len2 >= len1)
            {
                m_hitted = true;
            }
        }

        //  命中后回调函数
        if (m_hitted && OnHitted != null)
        {
            OnHitted(this);
        }
    }

    private Vector3 GetRenderPosition(float height)
    {
        return new Vector3(m_currPosition.x, height, m_currPosition.y);
    }

    private bool IsHitted(float targetX, float targetY)
    {
        //  计算目标和子弹的距离（如果小于目标的半径则为命中）
        float dx = (targetX - m_currPosition.x);
        float dy = (targetY - m_currPosition.y);
        return dx * dx + dy * dy <= radius2;
    }

    public abstract void SetTarget(TileEntity target);

    protected abstract Vector2 calcTargetPosition();

    protected abstract float CalcHeight(float dt);
}

/// <summary>
/// 对地点的子弹（抛物线轨迹）
/// </summary>
public class GameBulletPoint : GameBullet
{
    public override void SetTarget(TileEntity target)
    {
        m_targetEntity = null;
        m_targetPosition = target.GetCurrentPosition();
        SetTargetRadius(0.5f);  //  REMARK：打目标点的按照半格计算
    }

    protected override Vector2 calcTargetPosition()
    {
        return m_targetPosition;
    }
    
    protected override float CalcHeight(float dt)
    {
        //  单位化的抛物线公式：y = -1.5x^2 + 1.5x 点：A(0,0) B(1,0) C(2/3,1/3) 
        float mx = Mathf.Abs(m_targetPosition.x - m_firePosition.x);
        float my = Mathf.Abs(m_targetPosition.y - m_firePosition.y);
        float mm = Mathf.Max(mx, my);
        float x;
        if (mx > my)
        {
            x = Mathf.Abs(m_currPosition.x - m_targetPosition.x);
        }
        else
        {
            x = Mathf.Abs(m_currPosition.y - m_targetPosition.y);
        }
        x /= mm;     //  单位化
        return (-1.5f * x * x + 1.5f * x) * mm;
    }
}

/// <summary>
/// 对目标的子弹（会跟踪）
/// </summary>
public class GameBulletTarget : GameBullet
{
    public override void SetTarget(TileEntity target)
    {
        m_targetEntity = target;
        float radius = Mathf.Max(0.5f, target.blockingRange / 2.0f);    //  不可通行范围的一半   REMARK：士兵的情况没有不可通行范围（目前按照半格计算）
        SetTargetRadius(radius);
    }

    protected override Vector2 calcTargetPosition()
    {
        return m_targetEntity.GetCurrentPositionCenter();
    }
    
    protected override float CalcHeight(float dt)
    {
        return 0.0f;
    }
}

/// <summary>
/// 子弹管理器
/// </summary>
public class GameBulletManager : Singleton<GameBulletManager>
{
    private List<GameBullet> m_bullets = null;
    private List<GameBullet> m_delayRemoved = null;

    /// <summary>
    /// 添加自动（手动指定子弹类型）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="attacker"></param>
    /// <param name="targeter"></param>
    /// <param name="firepos"></param>
    /// <param name="callback"></param>
    public void AddBullet<T>(TileEntity attacker, TileEntity targeter, Vector2 firepos, Action<GameBullet> callback = null) where T : GameBullet
    {
        if (m_bullets == null)
            return;
        T b = Activator.CreateInstance<T>();
        b.Init(attacker.model.bulletName);
        b.SetBullet(attacker.model.bulletType, attacker.model.bulletSpeed);
        b.SetTarget(targeter);
        b.SetFirePosition(firepos);
        b.OnHitted += callback;
        b.ImmediatelyRefreshBulletView();
        m_bullets.Add(b);
    }

    public void Init()
    {
        if (m_bullets == null)
        {
            m_bullets = new List<GameBullet>();
            m_delayRemoved = new List<GameBullet>();
        }
        else
        {
            foreach (var b in m_bullets)
                b.Destroy();
            m_bullets.Clear();
            m_delayRemoved.Clear();
        }
    }

    public void Update(float dt)
    {
        if (m_bullets == null)
            return;

        foreach (GameBullet b in m_bullets)
        {
            b.Update(dt);
            if (b.IsDead)
            {
                m_delayRemoved.Add(b);
            }
        }
        foreach (GameBullet dead in m_delayRemoved)
        {
            dead.Destroy();
            m_bullets.Remove(dead);
        }
        m_delayRemoved.Clear();
    }
}