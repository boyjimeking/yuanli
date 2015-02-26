using System.Collections.Generic;
using com.pureland.proto;
using UnityEngine;
using System.Collections;

public class SpawnDonatedArmyHelper : IUpdate
{
    private float nextSpawnTime;
    private List<ArmyVO> armies;
    private int x;
    private int y;

    public SpawnDonatedArmyHelper(List<ArmyVO> armies,int x,int y)
    {
        this.armies = armies;
        this.x = x;
        this.y = y;
        nextSpawnTime = Constants.SPAWN_INTERVAL_TIME;
    }
    
    public void Update(float dt)
    {
        nextSpawnTime -= dt;
        if (nextSpawnTime <= 0)
        {
            nextSpawnTime += Constants.SPAWN_INTERVAL_TIME;
            foreach (var armyVo in armies)
            {
                if (armyVo.amount > 0)
                {
                    armyVo.amount --;
                    IsoMap.Instance.CreateEntityAt(OwnerType.Attacker,armyVo.cid,x,y);
                    return;
                }
            }
            UpdateManager.Instance.RemoveUpdate(this);
        }
    }
}
