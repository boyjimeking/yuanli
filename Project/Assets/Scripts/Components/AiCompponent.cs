#define REALTIME_AI

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rubystyle;

#if REALTIME_AI
using RetvGridTargetInfo = IsoGridTarget;
#else
using RetvGridTargetInfo = System.Collections.Generic.IEnumerator<IsoGridTarget>;
#endif

public abstract class AiCompponent : EntityComponent
{
    protected float m_timePassed = 0.0f;
    protected List<TileEntity> m_tempTargeters = null;
    protected TilePoint? m_tempTargetPos = null;
    protected LinkedList<IMoveGrid> m_tempMoveRoute = null;
    protected List<int> m_bulletLoopEffectIdList = null;

    public override void Init()
    {
        base.Init();
        if (Attacker.view != null)
        {
            Attacker.view.OnFireBullet += view_OnFireBullet;
        }
    }

    public override void Destroy()
    {
        AuxRecycleAllBulletLoopEffect();

        if (Attacker.view != null)
        {
            Attacker.view.OnFireBullet -= view_OnFireBullet;
        }
        base.Destroy();
    }

    #region 各状态更新函数

    public override void Update(float dt)
    {
        switch (Entity.State)
        {
            case EntityStateType.Idle: UpdateIdle(dt); break;
            case EntityStateType.Thinking: break;   //  REMARK：什么也不做
            case EntityStateType.Moving: UpdateMoving(dt); break;
            case EntityStateType.Attacking: UpdateAttacking(dt); break;
            case EntityStateType.Rotating: break;   //  REMARK：暂时什么也不做
            case EntityStateType.Dead: break;       //  REMARK：什么也不做了
            default:
                break;
        }
    }

    protected virtual void UpdateIdle(float dt)
    {
        //  发呆中
        if (m_timePassed > 0)
        {
            m_timePassed -= dt;
            if (m_timePassed > 0)
                return;
        }

        //  麻痹状态下直接返回
        if (AuxStateMabi())
            return;

        //  尝试行动
        Assert.Should(m_tempTargeters == null || m_tempTargeters.Count == 0);
        TryAction();
    }

    protected virtual void UpdateMoving(float dt)
    {
        //  取消移动判断 && 重新行动
        if (IsCancelMove())
        {
            StopMoveAndTryAction();
        }
    }

    protected virtual void UpdateAttacking(float dt)
    {
        //  停止攻击
        if (IsCancelAttack())
        {
            DoActionIdle();
            return;
        }

        //  发射频率（攻击间隔
        m_timePassed += dt;
        if (m_timePassed >= Entity.model.rate)
        {
            m_timePassed -= Entity.model.rate;
            ProcessReadyAttack();
        }
    }

    #endregion

    #region 各种辅助函数

    /// <summary>
    /// 回收所有循环子弹效果
    /// </summary>
    protected void AuxRecycleAllBulletLoopEffect()
    {
        if (m_bulletLoopEffectIdList != null)
        {
            foreach (var effId in m_bulletLoopEffectIdList)
            {
                GameEffectManager.Instance.RemoveEffect(effId);
            }
            m_bulletLoopEffectIdList = null;
        }
    }

    protected List<TileEntity> AuxConvertToList(TileEntity entity)
    {
        List<TileEntity> ret = new List<TileEntity>();
        ret.Add(entity);
        return ret;
    }

    /// <summary>
    /// 是否所有目标都死亡了
    /// </summary>
    /// <returns></returns>
    protected bool AuxIsAllDead()
    {
        if (m_tempTargeters == null)
            return true;

        foreach (var targeter in m_tempTargeters)
        {
            if (!targeter.IsDead())
                return false;
        }

        m_tempTargeters = null;
        return true;
    }

    /// <summary>
    /// 是否中了麻痹状态
    /// </summary>
    /// <returns></returns>
    protected bool AuxStateMabi()
    {
        //  [麻痹状态]
        var buffMgr = Attacker.GetComponent<GameBufferComponent>();
        if (buffMgr != null)
        {
            if (buffMgr.GetBuffer(Constants.BUFF_TYPE_MABI) != null)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 是否有任意目标死亡
    /// </summary>
    /// <returns></returns>
    protected bool AuxIsAnyDead()
    {
        Assert.Should(m_tempTargeters != null);
        return m_tempTargeters.RubyAny(targeter => targeter.IsDead());
    }

    /// <summary>
    /// 当前目标列表中是否有任意目标位【援军】
    /// </summary>
    /// <returns></returns>
    protected bool AuxIsAnyFriend()
    {
        if (m_tempTargeters == null)
            return false;
        return m_tempTargeters.RubyAny(targeter => targeter.Friendly);
    }

    /// <summary>
    /// 是否有任意目标移出了攻击范围
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns></returns>
    protected bool AuxIsAnyMoveoutAttackRange(TileEntity attacker)
    {
        Assert.Should(m_tempTargeters != null);
        Vector2 p = attacker.GetCurrentPositionCenter();
        float blindRange2 = attacker.model.blindRange * attacker.model.blindRange;
        return m_tempTargeters.RubyAny(targeter => !targeter.IsInAttackRange(p.x, p.y, attacker.model.range, blindRange2));
    }

    /// <summary>
    /// 是否所有目标都在攻击范围内
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns></returns>
    protected bool AuxIsAllTargeterInAttackRange(TileEntity attacker)
    {
        Vector2 p = attacker.GetCurrentPositionCenter();
        float blindRange2 = attacker.model.blindRange * attacker.model.blindRange;
        return m_tempTargeters.RubyAll(targeter => targeter.IsInAttackRange(p.x, p.y, attacker.model.range, blindRange2));
    }

    #endregion

    #region 执行行动相关

    protected virtual void TryAction()
    {
        //  没有目标 或 重新锁定目标的情况下
        if (m_tempTargeters == null || IsRelockTargeters())
        {
            m_tempTargeters = TryLockTargeters(out m_tempTargetPos, out m_tempMoveRoute);
#if UNITY_EDITOR
            if (m_tempTargeters != null)
                BattleManager.logTryLockTargeter.Add(GameRecord.Frame + ":" + Entity.model.nameForView + " try lock targeter: ok~ ");
            else
                BattleManager.logTryLockTargeter.Add(GameRecord.Frame + ":" + Entity.model.nameForView + " try lock targeter: failed~ ");
#endif  //  UNITY_EDITOR
            //  REMARK：延迟计算中（直接返回）
            if (Entity.State == EntityStateType.Thinking)
                return;
        }
        //  有目标的情况下尝试锁定更优先的目标
        else
        {
            TryLockMoreImportantTargeters();
        }

        //  锁定目标完毕执行行动
        DoAction();
    }

    protected virtual void DoAction()
    {
        //  锁定目标则执行行为（移动或攻击等）
        if (m_tempTargeters != null)
        {
            if (m_tempTargetPos != null || m_tempMoveRoute != null)
                DoActionMove();
            else
                DoActionAttack();
        }
        //  REMARK：没目标 则随机发呆一段时间 & 播放待机动画
        else
        {
            DoActionIdle();
        }
    }

    /// <summary>
    /// 执行行动-发呆
    /// </summary>
    protected virtual void DoActionIdle()
    {
        AuxRecycleAllBulletLoopEffect();

        m_tempTargeters = null;
        Entity.State = EntityStateType.Idle;
        m_timePassed = BattleRandom.Range(0.5f, 1.5f);
        Entity.PlayAnimation(AnimationNames.Stand);
    }

    /// <summary>
    /// 执行行动-攻击
    /// </summary>
    protected virtual void DoActionAttack()
    {
        if (Entity.State != EntityStateType.Attacking)
        {
            Entity.State = EntityStateType.Attacking;
            //ProcessReadyAttack(true);
            ProcessReadyAttack();
        }
    }

    /// <summary>
    /// 执行行动-移动
    /// </summary>
    protected virtual void DoActionMove()
    {
        Assert.Should(m_tempTargetPos != null || m_tempMoveRoute != null);

        if (m_tempTargetPos != null)
        {
            if (!TryMove(m_tempTargetPos.Value))
                DoActionIdle();
            m_tempTargetPos = null;
        }
        else
        {
            if (!TryMove(m_tempMoveRoute))
                DoActionIdle();
            m_tempMoveRoute = null;
        }
    }

    /// <summary>
    /// 是否重新锁定目标判断
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsRelockTargeters()
    {
        //  有任意目标死亡了 or 有任意目标移除攻击范围了
        return (AuxIsAnyDead() || AuxIsAnyMoveoutAttackRange(Attacker));
    }


    /// <summary>
    /// 尝试锁定目标
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    protected abstract List<TileEntity> TryLockTargeters(out TilePoint? targetPos, out LinkedList<IMoveGrid> targetRoute);

    /// <summary>
    /// 尝试锁定更重要（优先）的目标
    /// </summary>
    protected virtual void TryLockMoreImportantTargeters()
    {
        //  默认什么也不做
    }

    protected virtual bool TryMove(TilePoint targetPos)
    {
        ActorMoveComponent move = Entity.GetComponent<ActorMoveComponent>();
        if (move != null)
        {
            if (move.StartMove(targetPos, m_tempTargeters))
            {
                move.OnMoveCompleteEvent += OnMoveCompleteEvent;
                return true;
            }
        }
        return false;
    }

    protected virtual bool TryMove(LinkedList<IMoveGrid> moveRoute)
    {
        ActorMoveComponent move = Entity.GetComponent<ActorMoveComponent>();
        if (move != null)
        {
            if (move.StartMove(moveRoute))
            {
                move.OnMoveCompleteEvent += OnMoveCompleteEvent;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 停止移动重新行动
    /// </summary>
    protected virtual void StopMoveAndTryAction()
    {
        ActorMoveComponent move = Entity.GetComponent<ActorMoveComponent>();
        Assert.Should(move != null);
        move.OnMoveCompleteEvent -= OnMoveCompleteEvent;
        move.CancelMove();

        TryAction();
    }

    /// <summary>
    /// 是否考虑停止攻击
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsCancelAttack()
    {
        //  REMARK：麻痹状态了则停止攻击
        return AuxStateMabi();
    }

    /// <summary>
    /// 是否考虑取消移动（停止移动）
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsCancelMove()
    {
        //  REMARK：默认所有目标死亡了则取消移动
        return AuxIsAllDead();
    }

    private void OnMoveCompleteEvent(ActorMoveComponent moveComp)
    {
        moveComp.OnMoveCompleteEvent -= OnMoveCompleteEvent;

        //  移动完毕后重新锁定目标
        TryAction();
    }

    /// <summary>
    /// 准备攻击1（旋转动画对准目标）
    /// </summary>
    /// <param name="onlyProcessAnimation"></param>
    protected void ProcessReadyAttack(bool onlyProcessAnimation = false)
    {
        //  [速度提升] 动画频率加快
        float animatorSpeedRate = 0.0f;
        GameBufferComponent buffMgr = Entity.GetComponent<GameBufferComponent>();
        if (buffMgr != null)
        {
            var buffer = buffMgr.GetBuffer(Constants.BUFF_TYPE_SPPEDUP);
            if (buffer != null)
            {
                animatorSpeedRate = buffer.buffDamage;
            }
        }
        //animatorSpeedRate = 2.0f;   //  DEBUG

        //  REMARK：对准目标动画 仅针对炮塔并且目标数量为1时才有效
        if (EntityTypeUtil.IsAnyTower(Entity.entityType) && Entity.AimTarget)
        {
            if (AuxIsAllDead())
            {
                //  重新行动
                TryAction();
            }
            else
            {
                Entity.State = EntityStateType.Rotating;

                //  旋转对准目标 REMARK：这里Stand就是旋转动画
                Entity.PlayAnimationTowardsTarget(AnimationNames.Stand, m_tempTargeters[0], animatorSpeedRate, animationName =>
                {
                    if (Entity.State != EntityStateType.Rotating)
                        return;
                    ProcessReadyAttack2(onlyProcessAnimation, animatorSpeedRate);
                });
            }
        }
        else
        {
            ProcessReadyAttack2(onlyProcessAnimation, animatorSpeedRate);
        }
    }

    /// <summary>
    /// 准备攻击2（攻击动画）
    /// </summary>
    /// <param name="onlyProcessAnimation"></param>
    /// <param name="animatorSpeedRate"></param>
    private void ProcessReadyAttack2(bool onlyProcessAnimation, float animatorSpeedRate)
    {
        if (!AuxIsAllDead())
        {
            Entity.State = EntityStateType.Attacking;

            //  播放攻击动画
            if (Entity.AimTarget)
            {
                //  播放攻击动画（朝向目标）
                Entity.PlayAnimationTowardsTarget(AnimationNames.Attack, m_tempTargeters[0], animatorSpeedRate);
            }
            else
            {
                //  播放攻击动画（脉冲等范围性的不朝向目标）
                Entity.PlayAnimation(AnimationNames.Attack, animatorSpeedRate);
            }

            //  处理攻击动画时附带的效果动画
            ProcessBullet_LoopEffect();

            //  如果仅仅只显示攻击动画 则直接返回了（不进行下面的伤害处理）
            if (onlyProcessAnimation)
                return;

            //  播放攻击动画后根据子弹类型处理（发射子弹还是直接造成伤害）
            //switch (Entity.model.bulletType)
            //{
            //    case EntityBulletType.Direct:
            //    case EntityBulletType.Line:
            //        ProcessDirectDamage();
            //        break;
            //    case EntityBulletType.Point:
            //    case EntityBulletType.Target:
            //        ProcessFireBullet();
            //        break;
            //    default:
            //        Assert.Should(false, "invalid bullet type...");
            //        break;
            //}
        }
        //  已经处理伤害 or 子弹已经发射 则开始重新锁定目标
        TryAction();
    }

    /// <summary>
    /// 设置所有子弹效果的相关属性
    /// </summary>
    /// <param name="t"></param>
    /// <param name="effectPoint">所有效果的发射点（挂载点）</param>
    /// <param name="effectIds">所有效果的ID</param>
    /// <param name="targetPosList">所有目标所在位置</param>
    private void InitBulletEffectAttribute(EntityBulletType t, List<Vector3> effectPoint, List<int> effectIds, List<Vector2> targetPosList)
    {
        switch (Entity.model.bulletType)
        {
            //  对准目标：设置位置、旋转        效果数量：等于挂载点数量
            case EntityBulletType.LoopEffectLock:
                {
                    Assert.Should(effectIds.Count == effectPoint.Count);
                    //  对准目标则对准所有目标的重心点
                    var total_x = targetPosList.RubyInject(0.0f, (r, p) => p.x + r);
                    var total_y = targetPosList.RubyInject(0.0f, (r, p) => p.y + r);
                    var final_pos = new Vector2(total_x / targetPosList.Count, total_y / targetPosList.Count);
                    //  所有效果都朝向重心点
                    for (int i = 0; i < effectPoint.Count; i++)
                    {
                        GameEffectManager.Instance.GetEffect(effectIds[i]).SetPositionAndRotation(effectPoint[i], final_pos);
                    }
                }
                break;
            //  连接目标：设置位置、旋转、缩放     效果数量：等于挂载点数量 * 目标数量
            case EntityBulletType.LoopEffectLink:
            case EntityBulletType.UnLoopEffectLink:
                {
                    Assert.Should(effectIds.Count == effectPoint.Count * targetPosList.Count);
                    var basedis = Attacker.model.range;
                    Assert.Should(basedis > 0.0f, "invalid range ...");
                    //  挂载点和目标点分别连接
                    for (int i = 0; i < effectPoint.Count; i++)
                    {
                        for (int j = 0; j < targetPosList.Count; j++)
                        {
                            int effIdx = i * j;
                            var attackPoint = effectPoint[i];
                            var diff = Vector2.Distance(new Vector2(attackPoint.x, attackPoint.z), targetPosList[j]);
                            GameEffectManager.Instance.GetEffect(effectIds[effIdx]).SetPositionAndRotationAndScale(effectPoint[i], targetPosList[j], diff / basedis);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 处理子弹 - 循环效果类的子弹
    /// </summary>
    private void ProcessBullet_LoopEffect()
    {
        //  不是循环效果类型子弹直接返回
        if (!BulletTypeUtil.IsLoopEffect(Entity.model.bulletType))
            return;

        //  没对应的挂载点则返回
        var effAttackPoint = Entity.effAttachPoint();
        if (effAttackPoint == null)
            return;

        Assert.Should(Entity.model.bulletName != null && Entity.model.bulletName != Constants.EMPTY);

        //  添加循环子弹效果
        var effectName = Entity.model.bulletName;

        //  计算需要的效果数量
        var needEffectCount = 0;
        switch (Entity.model.bulletType)
        {
            case EntityBulletType.LoopEffectLock:
                needEffectCount = effAttackPoint.Count;                         //  朝阳目标的（类似火焰等）需要的数量和挂载点数一致）
                break;
            case EntityBulletType.LoopEffectLink:
                needEffectCount = effAttackPoint.Count * m_tempTargeters.Count; //  REMARK：连接目标的（需要的熟练是挂在点数和目标数的乘机）
                break;
            default:
                break;
        }

        //  初始化N个效果
        if (m_bulletLoopEffectIdList == null)
        {
            m_bulletLoopEffectIdList = new List<int>();
            for (int i = 0; i < needEffectCount; i++)
            {
                m_bulletLoopEffectIdList.Add(GameEffectManager.Instance.AddEffect(effectName, Vector3.zero, true));
            }
        }
        //  释放多余的效果
        else if (m_bulletLoopEffectIdList.Count > needEffectCount)
        {
            do
            {
                int effId = m_bulletLoopEffectIdList[0];
                m_bulletLoopEffectIdList.RemoveAt(0);
                GameEffectManager.Instance.RemoveEffect(effId);
            } while (m_bulletLoopEffectIdList.Count > needEffectCount);
        }
        //  补齐不足的数量
        else
        {
            int num = needEffectCount - m_bulletLoopEffectIdList.Count;
            for (int i = 0; i < num; i++)
            {
                m_bulletLoopEffectIdList.Add(GameEffectManager.Instance.AddEffect(effectName, Vector3.zero, true));
            }
        }

        //  初始化效果相关属性
        InitBulletEffectAttribute(Entity.model.bulletType, effAttackPoint, m_bulletLoopEffectIdList, m_tempTargeters.RubyMap(tar => tar.GetCurrentPositionCenter()));
    }

    /// <summary>
    /// 根据AttackPoint处理发射字段或直接伤害
    /// </summary>
    /// <param name="obj"></param>
    private void view_OnFireBullet(List<Vector3> obj)
    {
        //  全跪了则不处理了o.o
        if (AuxIsAllDead())
            return;

        var bt = Entity.model.bulletType;

        //  在AttackPoint点造成直接伤害
        if (BulletTypeUtil.IsDirectDamage(bt))
        {
            //  处理非循环子弹效果
            if (BulletTypeUtil.IsUnLoopEffect(bt))
            {
                Assert.Should(Entity.model.bulletName != null && Entity.model.bulletName != Constants.EMPTY);
                //  连接类型效果数量是 挂载点数 * 目标数
                var effIdList = new List<int>();
                for (int i = 0; i < obj.Count * m_tempTargeters.Count; i++)
                {
                    effIdList.Add(GameEffectManager.Instance.AddEffect(Entity.model.bulletName, Vector3.zero, false));
                }
                //  初始化效果相关属性
                InitBulletEffectAttribute(bt, obj, effIdList, m_tempTargeters.RubyMap(tar => tar.GetCurrentPositionCenter()));
            }
            ProcessDirectDamage();
        }
        //  在AttackPoint处发射子弹
        else if (BulletTypeUtil.IsFireBullet(bt))
        {
            foreach (var firepos in obj)
            {
                ProcessFireBullet(new Vector2(firepos.x, firepos.z));
            }
        }
        else
        {
            Assert.Should(false, "invalid bullet type...: " + bt);
        }
    }

    /// <summary>
    /// 处理发射子弹
    /// </summary>
    /// <param name="firepos"></param>
    private void ProcessFireBullet(Vector2 firepos)
    {
        Assert.Should(m_tempTargeters != null);
        Assert.Should(Entity.model.bulletName != null && Entity.model.bulletName != Constants.EMPTY, "invalid bulletname: " + Entity.model.nameForView);
        switch (Entity.model.bulletType)
        {
            case EntityBulletType.Point:
                foreach (var tar in m_tempTargeters)
                {
                    GameBulletManager.Instance.AddBullet<GameBulletPoint>(this.Entity, tar, firepos, OnBulletHitted);
                }
                break;
            case EntityBulletType.Target:
                foreach (var tar in m_tempTargeters)
                {
                    GameBulletManager.Instance.AddBullet<GameBulletTarget>(this.Entity, tar, firepos, OnBulletHitted);
                }
                break;
            default:
                Assert.Should(false, "invalid bullet type...");
                break;
        }
    }

    /// <summary>
    /// [回调] 子弹命中目标 或 子弹命中目标点
    /// </summary>
    /// <param name="bullet"></param>
    private void OnBulletHitted(GameBullet bullet)
    {
        //  TODO：如果子弹命中回调时自身已经挂了的情况是否需要考虑？？
        //if (this.Entity.IsDead())
        //    return;

        Vector2 hitPos = Vector2.zero;
        float randomRadius = 0.0f;

        switch (bullet.bulletType)
        {
            case EntityBulletType.Point:    //  对地点伤害
                {
                    Assert.Should(bullet.targetEntity == null);
                    ProcessDamagePoint(bullet.targetPosition);
                    hitPos = bullet.targetPosition;
                    randomRadius = 0.3f;    //  REMARK：打目标点的按照半格计算
                }
                break;
            case EntityBulletType.Target:   //  对目标伤害
                {
                    Assert.Should(bullet.targetEntity != null);
                    if (Entity.model.splashRange > 0)
                        ProcessDamagePoint(bullet.targetEntity.GetCurrentPositionCenter());
                    else
                        GameDamageManager.ProcessDamageOneTargeter(bullet.targetEntity, Entity.model, Entity);
                    hitPos = bullet.targetEntity.GetCurrentPositionCenter();
                    randomRadius = Mathf.Max(0.3f, bullet.targetEntity.blockingRange / 6.0f);
                }
                break;
            default:
                Assert.Should(false, "invalid bullet type...");
                break;
        }

        //  命中效果（范围内随机设置个命中点）
        if (randomRadius > 0)
        {
            hitPos.x = BattleRandom.Range(hitPos.x - randomRadius, hitPos.x + randomRadius);
            hitPos.y = BattleRandom.Range(hitPos.y - randomRadius, hitPos.y + randomRadius);
        }
        GameEffectManager.Instance.AddEffect(Entity.model.hitEffectName, new Vector3(hitPos.x, 0, hitPos.y));
    }

    /// <summary>
    /// 处理命中时的伤害变化
    /// </summary>
    private void ProcessDirectDamage()
    {
        Assert.Should(m_tempTargeters != null);

        //  直接伤害：直线
        if (BulletTypeUtil.IsLine(Entity.model.bulletType))
        {
            foreach (var tar in m_tempTargeters)
            {
                var pos = tar.GetCurrentPosition();
                var targeters = FindTargetersLine(pos.x, pos.y, 0.8f);  //  REMARK：这里直线宽度暂时设置为0.8个格子大小（可以适当调整）
                GameDamageManager.ProcessDamageMultiTargeters(targeters, Entity.model, Entity);
            }
        }
        //  直接伤害：扇形（包括360度环形）
        else if (Entity.model.attackAngle > 0)
        {
            foreach (var tar in m_tempTargeters)
            {
                var pos = tar.GetCurrentPosition();
                var targeters = FindTargetersInAngle(pos.x, pos.y, Entity.model.attackAngle);
                GameDamageManager.ProcessDamageMultiTargeters(targeters, Entity.model, Entity);
            }
        }
        //  直接伤害：其他形状
        else
        {
            GameDamageManager.ProcessDamageMultiTargeters(m_tempTargeters, Entity.model, Entity);
        }
    }

    /// <summary>
    /// 针对地点进行伤害处理
    /// </summary>
    /// <param name="pos"></param>
    protected void ProcessDamagePoint(Vector2 pos)
    {
        List<TileEntity> targeters = FindTargetersInSplash(pos.x, pos.y);
        if (targeters.Count == 0)
            return;
        int targeterCount = 0;
        foreach (TileEntity targeter in targeters)
        {
            //  REMARK：溅射伤害系数修正
            if (++targeterCount <= Constants.SPLASH_FULL_DAMAGE_MAX)
            {
                GameDamageManager.ProcessDamageOneTargeter(targeter, Entity.model, Entity);
            }
            else
            {
                GameDamageManager.ProcessDamageOneTargeter(targeter, Entity.model, Entity, Constants.SPLASH_PARTIAL_DAMAGE_RATIO);
            }
        }
    }

    #endregion

    #region 查找目标相关


    /// <summary>
    /// 从备选目标对象列表筛选最近的目标（时间消耗最低）
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="targets"></param>
    /// <returns></returns>
    protected RetvGridTargetInfo FindTargetsNearestTimeCost(TileEntity attacker, List<TileEntity> targets)
    {
        //  备选列表 低于 需求量则不用排序直接全部返回
        if (targets.Count == 0)
            return null;

        ///<    计算最近目标
        Dictionary<TilePoint, IsoGridTarget> gridTargets = new Dictionary<TilePoint, IsoGridTarget>();
        if (attacker.model.range < 3.0f)    //  REMARK：3.0f是建筑和士兵之间跨越墙的最短距离（低于3则直接根据目标点中心求最近即可、否则需要根据建筑周边被攻击范围覆盖的所有区域求最近。）
        {
            foreach (var tar in targets)
            {
                Vector2 p = tar.GetCurrentPositionCenter();
                TilePoint grid = new TilePoint((int)p.x, (int)p.y);
                IsoGridTarget tarInfos = new IsoGridTarget() { Targeter = tar, Distance = 0, X = grid.x, Y = grid.y };
                gridTargets.Add(grid, tarInfos);
            }
        }
        else
        {
            foreach (var tar in targets)
            {
                Vector2 p = tar.GetCurrentPositionCenter();
                int cx = (int)p.x;
                int cy = (int)p.y;
                int range = (int)(attacker.model.range + Mathf.Max(0.5f, tar.blockingRange / 2.0f));
                int bgnX = Math.Max(cx - range, 0);
                int endX = Math.Min(cx + range, Constants.EDGE_WIDTH - 1);
                int bgnY = Math.Max(cy - range, 0);
                int endY = Math.Min(cy + range, Constants.EDGE_HEIGHT - 1);
                int range2 = range * range;
                for (int x = bgnX; x <= endX; x++)
                {
                    for (int y = bgnY; y <= endY; y++)
                    {
                        int dx = cx - x;
                        int dy = cy - y;
                        int diff = dx * dx + dy * dy;
                        if (diff <= range2 && IsoMap.Instance.IsPassable(x, y))
                        {
                            TilePoint grid = new TilePoint(x, y);
                            if (gridTargets.ContainsKey(grid))
                            {
                                IsoGridTarget tarInfos = gridTargets[grid];
                                if (diff < tarInfos.Distance)
                                {
                                    tarInfos.Distance = diff;
                                    tarInfos.Targeter = tar;
                                }
                            }
                            else
                            {
                                IsoGridTarget tarInfos = new IsoGridTarget() { Targeter = tar, Distance = diff, X = grid.x, Y = grid.y };
                                gridTargets.Add(grid, tarInfos);
                            }
                        }
                    }
                }
            }
        }
        if (gridTargets.Count == 0)
            return null;
#if REALTIME_AI
        return IsoMap.Instance.SearchDijkstra(attacker, gridTargets);
#else
        return IsoMap.Instance.SearchDijkstra(attacker, gridTargets);
#endif
    }

    /// <summary>
    /// 从备选目标对象列表筛选最近的N个目标（直线最近）
    /// </summary>
    /// <param name="self"></param>
    /// <param name="targets"></param>
    /// <param name="numTarget"></param>
    /// <returns></returns>
    protected List<TileEntity> FindTargetsNearestLinear(Vector2 self, List<TileEntity> targets, int numTarget = 1)
    {
        //  备选列表 低于 需求量则不用排序直接全部返回
        if (targets.Count <= numTarget)
            return targets;

        //  只请求一个结果的情况下 直接取最近的即可（不用排序）
        if (numTarget <= 1)
        {
            TileEntity target = null;
            float mindiff = 9999999.0f;  //  REMARK：不应该出现超过这么大的距离的o.o

            foreach (var tar in targets)
            {
                float diff = tar.DistanceSquareTo(self.x, self.y);
                if (diff <= mindiff)
                {
                    mindiff = diff;
                    target = tar;
                }
            }

            return AuxConvertToList(target);
        }
        //  取多个结果的情况下排序列表（然后取最近的几个）
        else
        {
            //  计算所有对象到目标点的距离
            Dictionary<TileEntity, float> diff_hash = new Dictionary<TileEntity, float>();
            foreach (var tar in targets)
            {
                float diff = tar.DistanceSquareTo(self.x, self.y);
                diff_hash.Add(tar, diff);
            }

            //  按照到目标点的距离【升序】排列
            targets.Sort((a, b) => diff_hash[a].CompareTo(diff_hash[b]));

            //  取前面的numTarget个目标即为最近的
            return targets.GetRange(0, numTarget);
        }
    }

    /// <summary>
    /// 获取溅射范围内的对象列表
    /// </summary>
    /// <param name="x">溅射位置X</param>
    /// <param name="y">溅射位置Y</param>
    /// <returns></returns>
    protected List<TileEntity> FindTargetersInSplash(float x, float y)
    {
        Assert.Should(Entity.model.splashRange > 0);
        return IsoMap.Instance.GetEntitiesByRange(this.Entity, Entity.GetTargetOwner(), x, y, Entity.model.splashRange, 0.0f);
    }

    /// <summary>
    /// 获取扇形范围内对象列表
    /// </summary>
    /// <param name="center_x">扇形中心线X</param>
    /// <param name="center_y">扇形中心线Y</param>
    /// <param name="angle">扇形角度</param>
    /// <returns></returns>
    protected List<TileEntity> FindTargetersInAngle(float center_x, float center_y, int angle)
    {
        //  1、第一步获取攻击范围内的目标（不包括盲区内的、不包括死亡的、不包括自身）
        Vector2 self = Entity.GetCurrentPositionCenter();
        List<TileEntity> targeters = IsoMap.Instance.GetEntitiesByRange(this.Entity, Entity.GetTargetOwner(), self.x, self.y, Entity.model.range, Entity.model.blindRange);
        if (targeters.Count == 0)
            return targeters;

        //  圆环形状直接返回（不用筛选）
        if (angle >= 360)
            return targeters;

        //  2、在筛选扇形范围内的目标
        List<TileEntity> ret = new List<TileEntity>();
        float half_angle = angle / 2.0f;

        //  塔到释放点向量的单位距离
        Vector2 vdir = new Vector2(center_x - self.x, center_y - self.y);
        vdir.Normalize();

        foreach (TileEntity target in targeters)
        {
            Vector2 p = target.GetCurrentPosition();

            //  塔到目标点向量的单位距离
            Vector2 vtar = new Vector2(p.x - self.x, p.y - self.y);
            vtar.Normalize();

            //  点乘 A * B = |A||B|cos(angle) -> 单位化时候：A*B = cos(angle)
            float cosC = Vector2.Dot(vdir, vtar);
            float radian_c = Mathf.Acos(cosC);
            float angle_c = 180.0f * radian_c / Mathf.PI;

            //  扇形内
            if (angle_c < half_angle)
            {
                ret.Add(target);
            }
        }

        return ret;
    }

    /// <summary>
    /// 获取直线范围内对象列表
    /// </summary>
    /// <param name="center_x"></param>
    /// <param name="center_y"></param>
    /// <param name="line_width"></param>
    /// <returns></returns>
    protected List<TileEntity> FindTargetersLine(float center_x, float center_y, float line_width)
    {
        //  1、第一步获取攻击范围内的目标（不包括盲区内的、不包括死亡的、不包括自身）
        Vector2 self = Entity.GetCurrentPositionCenter();
        var targeters = IsoMap.Instance.GetEntitiesByRange(this.Entity, Entity.GetTargetOwner(), self.x, self.y, Entity.model.range, Entity.model.blindRange);
        if (targeters.Count == 0)
            return targeters;

        //  2、筛选被直线覆盖的目标
        List<TileEntity> ret = new List<TileEntity>();
        float half_width = line_width / 2.0f;

        //  塔到释放点向量
        Vector2 vdir = new Vector2(center_x - self.x, center_y - self.y);
        float vlen = vdir.magnitude;
        vdir.Normalize();

        foreach (var target in targeters)
        {
            var p = target.GetCurrentPosition();

            //  塔到目标点向量
            Vector2 vtar = new Vector2(p.x - self.x, p.y - self.y);
            vtar.Normalize();

            //  点乘 A * B = |A||B|cos(angle) -> 单位化时候：A*B = cos(angle)
            float cosC = Vector2.Dot(vdir, vtar);
            float radian_c = Mathf.Acos(cosC);
            if (radian_c <= Mathf.PI / 2.0f)
            {
                //  直线内
                float dis = Mathf.Tan(radian_c) * vlen;
                if (dis < half_width)
                {
                    ret.Add(target);
                }
            }
        }

        return ret;
    }

    #endregion
}
