
using System;
using com.pureland.proto;
using UnityEngine;

/// <summary>
/// 处理建造或者升级相关
/// </summary>
public class ConstructBuildingComponent : EntityComponent
{
    private GameObject constructAnimationOverlay;
    private CountdownUI countdownUI;
    private TileEntity workman = null;
    private DateTime endTime;
    public override void Init()
    {
        base.Init();

        if (Entity.buildingVO.buildingStatus == BuildingVO.BuildingStatus.Construct || Entity.buildingVO.buildingStatus == BuildingVO.BuildingStatus.Upgrade)
        {
            //TODO add construct view
            constructAnimationOverlay = (GameObject)ResourceManager.Instance.LoadAndCreate("Homeland/BuilderUI/BuildFenceView");
            constructAnimationOverlay.GetComponent<BuildFenceView>().Init(Entity.model.tileSize);
            Entity.view.AddSubView(constructAnimationOverlay, new Vector3(-Entity.model.tileSize + 1.3f, 0, -Entity.model.tileSize + 1.3f));
            endTime = DateTimeUtil.UnixTimestampMSToDateTime(Entity.buildingVO.endTime);
            if (GameWorld.Instance.worldType == WorldType.Home)
            {
                countdownUI =
                ((GameObject)ResourceManager.Instance.LoadAndCreate("Homeland/BuilderUI/CountdownUI"))
                    .GetComponent<CountdownUI>();
                Entity.view.AddSubView(countdownUI.gameObject, new Vector3(0, 3, 0));
                countdownUI.TotalTime =
                    DataCenter.Instance.FindEntityModelById(Entity.model.baseId).buildTime;
                countdownUI.LeftTime = (Entity.buildingVO.endTime - ServerTime.Instance.GetTimestamp()) / 1000;
                countdownUI.OnCompleteEvent += OnCountdownComplete;

                //  请求一个工人进行建造
                //workman = IsoMap.Instance.GetWorkerHouseComponent().AskAWorkman(Entity);

                DelayManager.Instance.AddDelayCall(delegate()
                {
                    //  REMARK：这里是异步处理（可能处理的时候已经完成了 则不用请求工人了
                    if (Entity.buildingVO.buildingStatus == BuildingVO.BuildingStatus.Construct || Entity.buildingVO.buildingStatus == BuildingVO.BuildingStatus.Upgrade)
                    {
                        workman = IsoMap.Instance.GetWorkerHouseComponent().AskAWorkman(Entity);
                    }
                });
            }
        }
    }
    /// <summary>
    /// 升级建筑剩余时间
    /// </summary>
    public int TimeLeft
    {
        get
        {
            return (int) (endTime - ServerTime.Instance.Now()).TotalSeconds;
        }
    }
    public void OnCountdownComplete(bool obj)
    {
        OnComplete(false);
    }

    /// <summary>
    /// 立即完成
    /// </summary>
    public void CompleteImmediately()
    {
        OnComplete(true);
    }

    private void OnComplete(bool immediately)
    {
        IsoMap.Instance.GetWorkerHouseComponent().GiveBackAWorkman(workman);

        GameManager.Instance.RequestBuildingComplete(Entity, immediately);

        CleanUp();
    }

    private void CleanUp()
    {
        workman = null;
        if (countdownUI)
        {
            countdownUI.OnCompleteEvent -= OnCountdownComplete;
            GameObject.Destroy(countdownUI.gameObject);
        }
        if (constructAnimationOverlay)
            GameObject.Destroy(constructAnimationOverlay);
    }
    public override void Destroy()
    {
        CleanUp();

        base.Destroy();
    }
}

