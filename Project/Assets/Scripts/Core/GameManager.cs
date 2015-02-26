
using System.Collections.Generic;
using com.pureland.proto;
using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public string MachineId
    {
        get
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
    }
    /// <summary>
    /// 购买建筑
    /// </summary>
    /// <param name="cid"></param>
    public void BuyBuilding(int cid)
    {
        var builder = GameWorld.Instance.CurrentWorldMode as IsoWorldModeBuilder;
        Assert.Should(builder != null, "BuyBuilding only work in Homeland");
        if (builder != null)
        {
            var building = DataCenter.Instance.FindEntityModelById(cid);
            Assert.Should(building != null);
            var tileEntity = TileEntity.Create(OwnerType.Defender, building);
            tileEntity.view.Init();
            builder.SetBuildingObject(tileEntity);
        }
    }

    public void RequestBuyBuilding(TileEntity entity)
    {
        new BuildingConsCommand(entity).ExecuteAndSend();
    }
    /// <summary>
    /// 升级
    /// </summary>
    /// <param name="entity"></param>
    public void RequestUpgradeBuilding(TileEntity entity, ResourceType resourceType, bool isCancel)
    {
        new BuildingUpgradeCommand(entity, resourceType, isCancel).ExecuteAndSend();
    }

    /// <summary>
    /// 取消升级
    /// </summary>
    /// <param name="entity"></param>
    public void RequestCancelUpgradeBuilding(TileEntity entity)
    {
        new BuildingUpgradeCommand(entity, ResourceType.None, true).ExecuteAndSend();
    }

    /// <summary>
    /// 播放录像
    /// </summary>
    /// <param name="replayId"></param>
    public void RequestPlayReplay(long replayId)
    {
        new BattleReplayCommand(replayId).ExecuteAndSend();
    }

    /// <summary>
    /// 是否是新设备
    /// </summary>
    /// <returns></returns>
    public bool IsNewDevice()
    {
        return DataCenter.Instance.playerLocalDataVO == null;
    }

    /// <summary>
    /// 搜索玩家
    /// </summary>
    /// <param name="searchType">搜索类型</param>
    /// <param name="level">关卡</param>
    /// <param name="completeCallback"></param>
    public void RequestFightSearch(int searchType, int level, Action<BaseResp> completeCallback)
    {
        var cmd = new FightSearchCommand((FightSearchReq.SearchType)searchType);
        cmd.CompleteCallback = (resp) => { completeCallback(resp); };
        cmd.TimeoutCallback = () => { cmd.Cancel(); completeCallback(null); };
        cmd.ExecuteAndSend();
    }
    /// <summary>
    /// 请求玩家家园数据
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="worldType"></param>
    public void RequestHomeLandData(long userID, WorldType worldType)
    {
        new LoadHomelandCommand(userID, worldType).ExecuteAndSend();
    }
    /// <summary>
    /// 请求玩家历史战斗数据
    /// </summary>
    public void RequestBattleHistory()
    {
        new BattleHistoryRequestCommand().ExecuteAndSend();
    }

    public void RequestMoveBuilding(TileEntity entity)
    {
        var list = new List<BuildingVO>();
        list.Add(entity.buildingVO);
        new MoveBuildingCommand(list).ExecuteAndSend();
    }

    public void RequestBuildingComplete(TileEntity entity, bool immediately)
    {
        new BuildingCompleteCommand(entity, immediately).ExecuteAndSend();
    }
    public void RequestRefillAllTrap()
    {
        new TrapRefillCommand(TrapRefillReq.RefillType.All, null).ExecuteAndSend();
    }
    public void RequestBuyResource(ResourceVO resVO)
    {
        new BuyResourceCommand(resVO).ExecuteAndSend();
    }
    public void RequestChargeDiamond(int diamond)
    {
        new ChargeDiamondCommand(diamond).ExecuteAndSend();
    }
}
