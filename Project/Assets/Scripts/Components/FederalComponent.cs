using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 公会建筑组件
/// </summary>
public class FederalComponent : EntityComponent
{
    private float m_timePassed = 0.0f;
    private LinkedList<int> m_donatedArmiesList = null;

    public override void Init()
    {
        if (GameWorld.Instance.worldType != WorldType.Battle && GameWorld.Instance.worldType != WorldType.Replay)
        {
            this.enabled = false;
            return;
        }
        //  默认禁止
        this.enabled = false;
        //  循环判断如果有援军则启用
        var donatedArmies = DataCenter.Instance.Defender.donatedArmies;
        if (donatedArmies != null)
        {
            foreach (var vo in donatedArmies)
            {
                if (vo.amount > 0)
                {
                    this.enabled = true;
                    break;
                }
            }
        }
        //  拷贝一份出兵列表
        if (this.enabled)
        {
            m_donatedArmiesList = new LinkedList<int>();
            foreach (var vo in donatedArmies)
            {
                for (int i = 0; i < vo.amount; i++)
                {
                    m_donatedArmiesList.AddLast(vo.cid);
                }
            }
        }
    }

    public override void Update(float dt)
    {
        m_timePassed -= dt;
        if (m_timePassed <= 0)
        {
            UpdateCheckEnemy(dt);
            m_timePassed = Constants.FEDERAL_DISPATCH_TROOPS_SPACE;
        }
    }

    private void UpdateCheckEnemy(float dt)
    {
        //  援军全部出动
        if (m_donatedArmiesList == null || m_donatedArmiesList.Count <= 0)
            return;

        var selfPos = Entity.GetCurrentPositionCenter();
        var targetersInRange = IsoMap.Instance.GetEntitiesByRange(Entity, Entity.GetTargetOwner(), selfPos.x, selfPos.y, Entity.model.range, 0.0f);
        //  有对方士兵进入自身的警戒范围，则派出士兵。
        if (targetersInRange.Count > 0)
        {
            int cid = m_donatedArmiesList.First.Value;
            m_donatedArmiesList.RemoveFirst();
            var model = DataCenter.Instance.FindEntityModelById(cid);
            if (model != null)
            {
                //  REMARK：出兵位置暂定建筑的正下方角落
                Spawn(model, Entity.GetTilePos().x, Entity.GetTilePos().y);
            }
        }
    }

    public void Spawn(EntityModel model, int x, int y)
    {
        //  创建对象（设置友军/援军标记）
        var tileEntity = TileEntity.Create(Entity.GetOwner(), model, true);
        tileEntity.SetTilePosition(new TilePoint(x, y));

        //  延迟添加到地图上（避免迭代器损坏）
        IsoMap.Instance.DelayAddEntity(tileEntity);
    }
}
