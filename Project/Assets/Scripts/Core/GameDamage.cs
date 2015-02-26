using System.Collections.Generic;
using UnityEngine;

class GameDamageManager
{
    /// <summary>
    /// 计算目标的伤害值（伤害公式）
    /// </summary>
    /// <param name="targeterModel"></param>
    /// <param name="attackerModel"></param>
    /// <param name="attacker">技能道具等情况下 attacker 为 null </param>
    /// <param name="factor">伤害修正系数</param>
    /// <returns></returns>
    private static float CalcDamageValue(EntityModel targeterModel, EntityModel attackerModel, TileEntity attacker = null, float factor = 1.0f)
    {
        float damage = 0.0f;

        if (EntityTypeUtil.IsCurer(attackerModel))
        {
            //  回血为负
            damage = -factor * attackerModel.cure;
        }
        else
        {
            //  REMARK：伤害公式可以调整
            damage = attackerModel.damage - targeterModel.defense;
            if (attackerModel.additionDamageSubType != Constants.EMPTY && attackerModel.additionDamageSubType == targeterModel.subType)
                damage *= attackerModel.additionDamageRatio;

            //  技能伤害的时候 attacker 不存在
            if (attacker != null)
            {
                //  [攻击提升] buffer 的情况下乘以伤害倍率
                GameBufferComponent attackerBufferMgr = attacker.GetComponent<GameBufferComponent>();
                if (attackerBufferMgr != null)
                {
                    var buffer = attackerBufferMgr.GetBuffer(Constants.BUFF_TYPE_ATTACKUP);
                    if (buffer != null)
                    {
                        damage *= buffer.buffDamage;
                    }
                }
            }

            damage = Mathf.Max(damage * factor, 0);
        }

        //  处理伤害
        return damage;
    }

    /// <summary>
    /// 对单个目标进行伤害处理
    /// </summary>
    /// <param name="targeter"></param>
    /// <param name="attackerModel"></param>
    /// <param name="attacker">技能道具等情况下 attacker 为 null </param>
    /// <param name="factor"></param>
    public static void ProcessDamageOneTargeter(TileEntity targeter, EntityModel attackerModel, TileEntity attacker = null, float factor = 1.0f)
    {
        if (!targeter.IsDead())
        {
            //  计算伤害
            float damage = CalcDamageValue(targeter.model, attackerModel, attacker, factor);
            
            //  处理伤害
            targeter.MakeDamage(damage);

            //  没死亡时附加buffer效果
            if (!targeter.IsDead())
            {
                GameBufferComponent bufferMgr = targeter.GetComponent<GameBufferComponent>();
                if (bufferMgr != null)
                {
                    bufferMgr.AddBuffer(attackerModel);
                }
            }
            else
            {
                //  [特殊技能] 死亡后大回复
                if (EntityTypeUtil.IsTraitBlessing(targeter.model))
                {
                    //  TODO：
                }
            }

            //  [特殊技能] 吸血   REMARK：考虑是否需要回血光效？
            if (attacker != null && damage > 0 && EntityTypeUtil.IsTraitSuckBlood(attackerModel))
            {
                attacker.MakeDamage(-damage * Constants.SUCK_BLOOD_RATIO);
            }
        }
    }

    /// <summary>
    /// 对多个目标进行伤害处理
    /// </summary>
    /// <param name="targeters"></param>
    /// <param name="attackerModel"></param>
    /// <param name="attacker">技能道具等情况下 attacker 为 null </param>
    /// <param name="factor"></param>
    public static void ProcessDamageMultiTargeters(List<TileEntity> targeters, EntityModel attackerModel, TileEntity attacker = null, float factor = 1.0f)
    {
        foreach (var targeter in targeters)
        {
            ProcessDamageOneTargeter(targeter, attackerModel, attacker, factor);
        }
    }
}