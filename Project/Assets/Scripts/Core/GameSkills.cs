using System.Collections.Generic;
using UnityEngine;

public enum ESkillState
{
    Init,
    TriggerDamage,
    DamageProcessed,
    Completed
}

public abstract class IGameSkill
{
    public EntityModel model;
    public int posX;
    public int posY;

    public ESkillState status = ESkillState.Init;

    public IGameSkill(EntityModel model)
    {
        this.model = model;
    }

    public virtual void SetPos(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public virtual void Destroy() { }
    public virtual void Update(float dt) { }
}

public class GameSkill : IGameSkill
{
    private GameObject _view;

    public GameSkill(EntityModel model) : base(model)
    {
        _view = (GameObject)ResourceManager.Instance.LoadAndCreate("Effects/" + model.nameForResource);
        if (_view != null)
        {
            var trigger = _view.GetComponent<EffectTrigger>();
            Assert.Should(trigger != null, "Miss EffectTrigger Component: " + model.nameForView);
            trigger.TriggeredEvent += trigger_TriggeredEvent;
            trigger.CompleteEvent += trigger_CompleteEvent;
        }
    }

    void trigger_CompleteEvent(EffectTrigger obj)
    {
        Debug.Log(this.model.nameForView + "skill: complete... ");
        this.status = ESkillState.Completed;
        obj.CompleteEvent -= trigger_CompleteEvent;
    }

    void trigger_TriggeredEvent(EffectTrigger obj)
    {
        Debug.Log(this.model.nameForView + "skill: trigger... ");
        status = ESkillState.TriggerDamage;
        obj.TriggeredEvent -= trigger_TriggeredEvent;
    }

    public override void SetPos(int x, int y)
    {
        base.SetPos(x, y);
        _view.transform.position = new Vector3(x, 0.0f, y);
    }

    public override void Destroy()
    {
        if (_view != null)
        {
            GameObject.Destroy(_view);
            _view = null;
        }
    }

    public override void Update(float dt)
    {
        if (status == ESkillState.TriggerDamage)
        {
            status = ESkillState.DamageProcessed;
            IsoMap.Instance.ProcessAoeDamage(this.model, this.posX, this.posY);
        }
    }
}

public class GameSkillManager : Singleton<GameSkillManager>
{
    private List<IGameSkill> _skills = new List<IGameSkill>();
    private List<IGameSkill> _delayRemoved = new List<IGameSkill>();

    public void AddSkill(EntityModel skillMode, int posX, int posY)
    {
        var s = new GameSkill(skillMode);
        s.SetPos(posX, posY);
        _skills.Add(s);
    }

    public void Init()
    {
        foreach (var s in _skills)
            s.Destroy();
        _skills.Clear();
        _delayRemoved.Clear();
    }

    public void Update(float dt)
    {
        //  更新技能效果
        foreach (var s in _skills)
        {
            s.Update(dt);
            if (s.status == ESkillState.Completed)
            {
                _delayRemoved.Add(s);
            }
        }

        //  处理消失的技能效果
        foreach (var s in _delayRemoved)
        {
            s.Destroy();
            _skills.Remove(s);
        }
        _delayRemoved.Clear();
    }
}