using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEffect
{
    public readonly int effectId;

    private GameObject m_view;
    private tk2dSpriteAnimator m_animator;
    
    public GameEffect(int effectId, string effectName, Vector3 pos, bool loop)
    {
        this.effectId = effectId;
        var prefab = (GameObject) ResourceManager.Instance.Load("Effects/" + effectName);
        if (prefab != null)
        {
            m_view = PoolManager.Instance.Spawn(prefab);
        }
        if (m_view != null)
        {
            m_view.transform.position = pos;
            IsoHelper.MoveAlongCamera(m_view.transform, Constants.EFFECT_Z_ORDER);
            IsoHelper.FaceToWorldCamera(m_view.transform);
            //  REMARK：循环效果应该手动释放
            if (!loop)
            {
                var trigger = m_view.GetComponent<EffectTrigger>();
                if (trigger != null)
                {
                    trigger.CompleteEvent += trigger_CompleteEvent;
                }
                else
                {
                    //TODO 临时方案
                    m_animator = m_view.GetComponentInChildren<tk2dSpriteAnimator>();
                    if (m_animator != null)
                    {
                        m_animator.AnimationCompleted = OnAnimationCompleted;
                        m_animator.DefaultClip.wrapMode = (loop ? tk2dSpriteAnimationClip.WrapMode.Loop : tk2dSpriteAnimationClip.WrapMode.Once);
                        if (!m_animator.playAutomatically)
                        {
                            m_animator.Play(m_animator.DefaultClip);
                        }
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("effect {0} miss animator...", effectName));
                    }
                }
            }
        }
    }

    public void SetPositionAndRotation(Vector3 newpos, Vector2 target)
    {
        if (m_view != null)
        {
            m_view.transform.position = newpos;
            Vector3 vdir2 = new Vector3(target.x - newpos.x, 0, target.y - newpos.z).normalized;
            Vector3 forward = Camera.main.camera.transform.forward;
            m_view.transform.rotation = Quaternion.AngleAxis(90, forward) * Quaternion.LookRotation(forward, vdir2);
        }
    }

    public void SetPositionAndRotationAndScale(Vector3 newpos, Vector2 target, float scale)
    {
        SetPositionAndRotation(newpos, target);
        if (m_view != null)
        {
            m_view.transform.localScale = new Vector3(scale, 1.0f, 1.0f);
        }
    }

    private void trigger_CompleteEvent(EffectTrigger obj)
    {
        obj.CompleteEvent -= trigger_CompleteEvent;
        GameEffectManager.Instance.RemoveEffect(this);
    }

    public void Destroy()
    {
        if (m_view != null)
        {
            PoolManager.Instance.Recycle(m_view);
            m_view = null;
        }
    }

    private void OnAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
    {
        GameEffectManager.Instance.RemoveEffect(this);
    }
}


/// <summary>
/// 场景动画效果管理器
/// </summary>
public class GameEffectManager : Singleton<GameEffectManager>
{
    private List<GameEffect> m_effects = null;
    private Dictionary<int, GameEffect> m_effectsHash = null;
    private List<GameEffect> m_delayRemoved = null;
    private int m_effectUniqueId = 0;

    /// <summary>
    /// 添加一个动画效果，返回效果的唯一ID。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pos"></param>
    /// <param name="loop"></param>
    /// <returns></returns>
    public int AddEffect(string name, Vector3 pos, bool loop = false)
    {
        if (m_effects == null)
            return 0;

        if (name == null || name == Constants.EMPTY)
            return 0;

        int effectId = ++m_effectUniqueId;
        GameEffect eff = new GameEffect(effectId, name, pos, loop);
        m_effects.Add(eff);
        m_effectsHash.Add(effectId, eff);
        return effectId;
    }

    /// <summary>
    /// 根据效果ID获取动画效果对象
    /// </summary>
    /// <param name="effectId"></param>
    /// <returns></returns>
    public GameEffect GetEffect(int effectId)
    {
        if (m_effectsHash.ContainsKey(effectId))
        {
            return m_effectsHash[effectId];
        }
        return null;
    }

    /// <summary>
    /// 根据效果ID移除动画效果（一般对于循环的动画使用，非循环都自动移除。）
    /// </summary>
    /// <param name="effectId"></param>
    public void RemoveEffect(int effectId)
    {
        if (m_effectsHash.ContainsKey(effectId))
        {
            RemoveEffect(m_effectsHash[effectId]);
        }
    }

    /// <summary>
    /// 直接移除某个效果
    /// </summary>
    /// <param name="effect"></param>
    public void RemoveEffect(GameEffect effect)
    {
        m_delayRemoved.Add(effect);
    }
    
    public void Init()
    {
        if (m_effects == null)
        {
            m_effects = new List<GameEffect>();
            m_effectsHash = new Dictionary<int, GameEffect>();
            m_delayRemoved = new List<GameEffect>();
        }
        else
        {
            foreach (var eff in m_effects)
                eff.Destroy();
            m_effects.Clear();
            m_effectsHash.Clear();
            m_delayRemoved.Clear();
        }
        m_effectUniqueId = 0;
    }

    public void Update(float dt)
    {
        if (m_effects == null)
            return;

        foreach (var eff in m_delayRemoved)
        {
            eff.Destroy();
            m_effectsHash.Remove(eff.effectId);
            m_effects.Remove(eff);
        }
        m_delayRemoved.Clear();
    }
}