using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorMoveComponent : EntityComponent 
{	
    private Vector2 velocity;
    private IMoveGrid currentNode = null;
    private LinkedList<IMoveGrid> path = null;
    //private float m_timePassed = 0.0f;
    private float m_moveSpeedFactor = 1.0f;

    public event Action<ActorMoveComponent> OnMoveCompleteEvent = null;

    public override void Init()
    {
        this.enabled = false;
    }

    public bool StartMove(TilePoint targetPos, List<TileEntity> whilteList = null, float moveSpeedFactor = 1.0f)
    {
        m_moveSpeedFactor = moveSpeedFactor;
        if (IsoMap.Instance.InRouteMap(targetPos.x, targetPos.y))
        {
            var p = IsoMap.Instance.SearchRoutes(Entity.GetTilePos().x, Entity.GetTilePos().y, targetPos.x, targetPos.y, Entity, whilteList);
            return Move(p);
        }
        return false;
    }

    public bool StartMove(LinkedList<IMoveGrid> moveRoute)
    {
        m_moveSpeedFactor = 1.0f;
        return Move(moveRoute);
    }

    /// <summary>
    /// 取消移动（手动取消不会触发移动完成事件）
    /// </summary>
    public void CancelMove()
    {
        //  TODO:
        path.Clear();
        path = null;
        currentNode = null;
        enabled = false;
    }

    /// <summary>
    /// 从拆墙状态恢复移动
    /// </summary>
    public void ResumeMove()
    {
        Entity.State = EntityStateType.Moving;
        Entity.PlayAnimationFaceToTile(AnimationNames.Run, new TilePoint(currentNode.X, currentNode.Y));
        this.enabled = true;
    }

    private bool Move(LinkedList<IMoveGrid> path)
    {
        if (path == null)
            return false; //  TODO:未找到路径
		if (path.Count > 0) {
            this.path = path;
            Entity.State = EntityStateType.Moving;
			path.RemoveFirst ();//remove starter grid
            enabled = true;
		}
        return true;
	}

    public override void Update(float dt)
    {
        UpdateMove(dt);
    }

    private void UpdateMove(float dt)
    {
        if (path == null)
            return;

        //  TODO：这个时候结束会导致少移动半格
        if (currentNode == null)
        {
            if (path.Count == 0)
            {
                Entity.State = EntityStateType.Idle;
                enabled = false;
                if (OnMoveCompleteEvent != null)
                {
                    OnMoveCompleteEvent(this);
                }
                return;
            }
            currentNode = path.First.Value;
            path.RemoveFirst();

            //  当前位置到目标位置的向量的单位化*移动速度
            CalcNewVelocity();

            //  战斗模式下拆墙（否则直接穿墙
            if (GameWorld.Instance.worldType == WorldType.Battle || GameWorld.Instance.worldType == WorldType.Replay)
            {
                //  不可穿越墙的情况下：转到拆墙
                if (!Entity.CanOverTheWall() && IsoMap.Instance.IsWallorLinker(currentNode.X, currentNode.Y))
                {
                    //  禁用AI和移动组件并唤醒拆墙AI组件
                    Entity.GetComponent<ActorComponent>().enabled = false;
                    this.enabled = false;
                    Entity.GetComponent<ActorDestroyWallComponent>().WakeUp(currentNode.X, currentNode.Y);
                    return;
                }
            }
             
            //  动画方向调整
            Entity.PlayAnimationFaceToTile(AnimationNames.Run, new TilePoint(currentNode.X, currentNode.Y));
        }

        //  [速度提升] 移动速度倍率
        float speedRate = 1.0f;
        GameBufferComponent buffMgr = Entity.GetComponent<GameBufferComponent>();
        if (buffMgr != null)
        {
            var buffer = buffMgr.GetBuffer(Constants.BUFF_TYPE_SPPEDUP);
            if (buffer != null)
            {
                speedRate *= buffer.buffDamage;
            }
        }

        //  移动
        Entity.tileOffset += velocity * dt * speedRate;

#if UNITY_EDITOR
        if (GameWorld.Instance.worldType == WorldType.Battle || GameWorld.Instance.worldType == WorldType.Replay)
        {
            BattleManager.logMoveList.Add(GameRecord.Frame.ToString() + ":" + Entity.tileOffset.ToString());
        }
#endif  //  UNITY_EDITOR

        //  坐标判断（REMARK：这里如果移动速度极快会出BUG 暂时忽略o.o）
        if (Mathf.Abs(Entity.tileOffset.x) > 0.6f || Mathf.Abs(Entity.tileOffset.y) > 0.6f)
        {
            var dx = 0;
            var dy = 0;
            if (Entity.tileOffset.x > 0.5f)
            {
                dx = 1;
            }
            else if (Entity.tileOffset.x < -0.5f)
            {
                dx = -1;
            }
            if (Entity.tileOffset.y > 0.5f)
            {
                dy = 1;
            }
            else if (Entity.tileOffset.y < -0.5f)
            {
                dy = -1;
            }
            var delta = new TilePoint(dx, dy);
            Entity.tileOffset -= delta;
            var newpt = Entity.GetTilePos() + delta;
            Entity.SetTilePosition(newpt);
            if (newpt.x == currentNode.X && newpt.y == currentNode.Y)
            {
                currentNode = null;
            }
            else
            {
                //  当前位置到目标位置的向量的单位化*移动速度
                CalcNewVelocity();
            }
        }
    }

    private void CalcNewVelocity()
    {
        //float x = BattleRandom.Range(currentNode.X - 0.2f, currentNode.X + 0.2f);
        //float y = BattleRandom.Range(currentNode.Y - 0.2f, currentNode.Y + 0.2f);
        //velocity = (new Vector2(x, y) - Entity.GetCurrentPosition()).normalized * Entity.model.speed * m_moveSpeedFactor;
        velocity = (new Vector2(currentNode.X, currentNode.Y) - Entity.GetCurrentPosition()).normalized * Entity.model.speed * m_moveSpeedFactor;
    }
}
