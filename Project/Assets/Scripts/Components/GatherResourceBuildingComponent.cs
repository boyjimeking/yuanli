
using System;
using com.pureland.proto;
using UnityEngine;

public class GatherResourceBuildingComponent : BaseResourceBuildingComponent
{
    private DateTime lastGatherTime;
    private DateTime nextShowIconTime;
    private bool showingGatherIcon;
    private GameObject gatherIcon;
    public int storage;//剩余量
    public override void Init()
    {
        if (GameWorld.Instance.worldType != WorldType.Home || Entity.buildingVO.buildingStatus != BuildingVO.BuildingStatus.On)
        {
            enabled = false;
        }
        else
        {
            EventDispather.AddEventListener(GameEvents.RESOURCE_CHANGE, OnResourceChange);
        }
        UpdateState(DateTimeUtil.UnixTimestampMSToDateTime(Entity.buildingVO.resourceBuildingVO.lastGatherTime),storage);
        showingGatherIcon = false;
        base.Init();
    }

    private void OnResourceChange(string eventtype, object obj)
    {
        if (!showingGatherIcon)
        {
            return;
        }
        //装满了
        if (DataCenter.Instance.GetResource(Entity.model.resourceType) >=
            DataCenter.Instance.GetMaxResourceStorage(Entity.model.resourceType))
        {
            gatherIcon.GetComponentInChildren<UISprite>().color = Color.black;
        }
        else
        {
            gatherIcon.GetComponentInChildren<UISprite>().color = Color.white;
        }
    }

    public void UpdateState(DateTime lastGatherTime,int storage)
    {
        this.lastGatherTime = lastGatherTime;
        nextShowIconTime = lastGatherTime.AddSeconds(60);
        this.storage = storage;
        UpdateStorageView();
    }

    override public bool HandleOnTap()
    {
        if (CanGather())
        {
            GatherResource();
            return true;
        }
        return false;
    }

    protected override int CalcStealableResourceCount()
    {
        var total = CalculateResourceFromLastGather(BattleManager.Instance.BattleStartTime);
        return Mathf.FloorToInt(total * Constants.STEALABLE_RATIO_RESOURCE * BattleManager.Instance.GetBattleRewardRatio());
    }

    protected override float CalcStoragePercent()
    {
        return (float)CalculateResourceFromLastGather(ServerTime.Instance.Now()) / Entity.model.maxResourceStorage;
    }

    /// <summary>
    /// 计算可以收取的量
    /// </summary>
    /// <param name="gatherTime"></param>
    /// <returns></returns>
    public int CalculateResourceFromLastGather(DateTime gatherTime)
    {
        long milliseconds = (long) (gatherTime - lastGatherTime).TotalMilliseconds;
        int resource = Mathf.FloorToInt(Entity.model.resourcePerSecond * milliseconds / 1000) + storage;
        return Mathf.Min(resource,Entity.model.maxResourceStorage);//TODO 玩家加成
    }

    private bool CanGather()
    {
        return Entity.buildingVO.buildingStatus == BuildingVO.BuildingStatus.On && showingGatherIcon;
    }

    public void GatherResource()
    {
        showingGatherIcon = false;
        gatherIcon.SetActive(false);
        if (DataCenter.Instance.GetResource(Entity.model.resourceType) >= DataCenter.Instance.GetMaxResourceStorage(Entity.model.resourceType))
        {
            GameTipsManager.Instance.ShowGameTips("仓库已经满啦!");//TODO
            nextShowIconTime = ServerTime.Instance.Now().AddSeconds(60);
        }
        else
        {
            string name;
            int i;
            if (ResourceType == ResourceType.NewOil)
            {
                name = "Energy";
            }else if (ResourceType == ResourceType.Gold)
            {
                name = "Money";
            }
            else
            {
                name = "Sun";
            }
            if (viewIndex <= 1)
            {
                i = 1;
            }else if (viewIndex <= 4)
            {
                i = 2;
            }
            else
            {
                i = 3;
            }
            GameEffectManager.Instance.AddEffect("Shouji_" + name + "_" +i, Entity.GetRenderPosition());
            var gatherResourceCommand = new GatherResourceCommand(this);
            gatherResourceCommand.ExecuteAndSend();
        }
    }

    public override void Update(float dt)
    {
        if (!showingGatherIcon && nextShowIconTime < ServerTime.Instance.Now())
        {
            showingGatherIcon = true;
            //显示icon
            if (!gatherIcon)
            {
                gatherIcon = (GameObject)ResourceManager.Instance.LoadAndCreate("Homeland/BuilderUI/CollectResIcon");
                IsoHelper.FaceToWorldCamera(gatherIcon.transform);
                CollectResIconUI view = gatherIcon.GetComponent<CollectResIconUI>();
                view.OnClickEvent += GatherResource;
                view.SpriteName = Entity.model.resourceType.ToString();
                Entity.view.AddSubView(gatherIcon, new Vector3(0, Constants.COLLECTABLE_ICON_VERTICAL_HEIGHT, 0));

                //装满了
                if (DataCenter.Instance.GetResource(Entity.model.resourceType) >=
                    DataCenter.Instance.GetMaxResourceStorage(Entity.model.resourceType))
                {
                    gatherIcon.GetComponentInChildren<UISprite>().color = Color.black;
                }
            }
            gatherIcon.SetActive(true);
        }
    }

    public override void Destroy()
    {
        if (gatherIcon != null)
        {
            gatherIcon.GetComponent<CollectResIconUI>().OnClickEvent -= GatherResource;
            GameObject.Destroy(gatherIcon);
            gatherIcon = null;
        }
        EventDispather.RemoveEventListener(GameEvents.RESOURCE_CHANGE, OnResourceChange);
        base.Destroy();
    }
}
