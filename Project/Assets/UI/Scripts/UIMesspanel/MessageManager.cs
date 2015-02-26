using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class MessageManager : Singleton<MessageManager>
{
    public void CreateMockBattleResultVO()
    {
        BattleResultVO attVO = new BattleResultVO();
        attVO.percentage = 10;
        attVO.star = 3;
        attVO.useDonatedArmy = true;
        List<ArmyVO> vos = new List<ArmyVO>();
        vos.Add(new ArmyVO() { cid = 12601, amount = 1 });
        vos.Add(new ArmyVO() { cid = 12701, amount = 3 });
        vos.Add(new ArmyVO() { cid = 13001, amount = 5 });
        vos.Add(new ArmyVO() { cid = 12901, amount = 9 });
        attVO.usedArmies.AddRange(vos);
        var myResoruce = new List<ResourceVO>();
        myResoruce.Add(new ResourceVO() { resourceType = ResourceType.Gold, resourceCount = 1000 });
        myResoruce.Add(new ResourceVO() { resourceType = ResourceType.Oil, resourceCount = 1000 });
        myResoruce.Add(new ResourceVO() { resourceType = ResourceType.Medal, resourceCount = 1000 });
        myResoruce.Add(new ResourceVO() { resourceType = ResourceType.Diamond, resourceCount = 1000 });
        attVO.stolenResources.AddRange(myResoruce);
        attVO.rewardCrown = 10;
        attVO.timestamp = 1022222;
        attVO.peerName = "天下第一";
        attVO.peerClanName = "天下第一";
        attVO.peerCrown = 2000;
        List<BattleResultVO> battleVOS = new List<BattleResultVO>();
        for (int i = 0; i < 10; i++)
        {
            attVO.star = Random.Range(0, 3);
            battleVOS.Add(attVO);
        }
        DataCenter.Instance.SetBattleHistories(battleVOS, battleVOS);
    }
}
