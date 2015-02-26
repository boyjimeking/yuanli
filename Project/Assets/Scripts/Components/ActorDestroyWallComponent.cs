using System.Collections;
using System.Collections.Generic;

public class ActorDestroyWallComponent : AiCompponent
{
    private int edge_x, edge_y;

    public override void Init()
    {
        base.Init();
        this.enabled = false;
    }

    public void WakeUp(int edge_x, int edge_y)
    {
        this.edge_x = edge_x;
        this.edge_y = edge_y;

        //  启用
        m_tempTargeters = null;
        m_timePassed = 0.0f;
        Entity.State = EntityStateType.Idle;
        this.enabled = true;
    }

    private void RestoreToMoving()
    {
        this.enabled = false;
        Entity.GetComponent<ActorComponent>().enabled = true;
        Entity.GetComponent<ActorMoveComponent>().ResumeMove();
    }

    protected override void UpdateAttacking(float dt)
    {
        if (AuxIsAllDead())
        {
            //  墙死亡了（恢复到移动状态）
            RestoreToMoving();
        }
        else
        {
            //  继续攻击
            base.UpdateAttacking(dt);
        }
    }

    protected override List<TileEntity> TryLockTargeters(out TilePoint? _targetPos, out LinkedList<IMoveGrid> _targetRoute)
    {
        _targetPos = null;  //  不移动
        _targetRoute = null;

        if (m_tempTargeters != null)
            return m_tempTargeters;

        TilePoint p = Attacker.GetTilePos();
        TileEntity targeter = IsoMap.Instance.GetWallTargeter(p.x, p.y, edge_x, edge_y);
        if (targeter == null)
        {
            RestoreToMoving();
            return null;
        }

        return AuxConvertToList(targeter);
    }

}
