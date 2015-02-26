
using UnityEngine;

public abstract class EntityComponent
{
    private TileEntity _entity;
    public bool enabled = true;

    public TileEntity Entity
    {
        get { return _entity; }
        set
        {
            _entity = value;
            AddedToEntity();
        }
    }

    public TileEntity Attacker
    {
        get { return _entity; }
    }

    public virtual void Init()
    {
    }

    public virtual void AddedToEntity()
    {

    }

    public virtual void HandleMessage(EntityMessageType msg, object data=null)
    {
    }

    public virtual void HandleStateChange(EntityStateType oldState, EntityStateType nowState)
    {   
    }

    public virtual void Update(float dt)
    {
    }

    public virtual void Destroy()
    {
    }
}

