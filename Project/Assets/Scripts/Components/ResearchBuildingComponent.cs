
using System;
using com.pureland.proto;

public class ResearchBuildingComponent : EntityComponent
{
    private int currentResearchId;
    private DateTime endTime;

    //完成事件
    public event Action EventComplete;

    //当前研究的技能,研究前的id,<=0时代表没有研究
    public int CurrentResearchId { get { return currentResearchId; } }

    /// <summary>
    /// 升级剩余时间
    /// </summary>
    public int TimeLeft
    {
        get { return (int)(endTime - ServerTime.Instance.Now()).TotalSeconds; }
    }

    public override void Init()
    {
        base.Init();

        if (GameWorld.Instance.worldType != WorldType.Home)
        {
            enabled = false;
            return;
        }
        if (Entity.buildingVO.researchBuildingVO == null)
        {
            Entity.buildingVO.researchBuildingVO = new ResearchBuildingVO();
        }
        currentResearchId = Entity.buildingVO.researchBuildingVO.cid;
        endTime = DateTimeUtil.UnixTimestampMSToDateTime(Entity.buildingVO.researchBuildingVO.endTime);

        if (CurrentResearchId > 0)
        {
            enabled = true;
        }
        else
        {
            enabled = false;
        }
    }

    /// <summary>
    /// 开始升级
    /// </summary>
    /// <param name="cid"></param>
    public void Research(int cid)
    {
        new ResearchRequestCommand(Entity.buildingVO.researchBuildingVO,ResearchReq.ResearchRequestType.Research, cid,0).ExecuteAndSend();

        Init();//init from vo
    }

    /// <summary>
    /// 立即完成
    /// </summary>
    public void CompleteResearchImmediately()
    {
        Assert.Should(CurrentResearchId > 0);
        OnComplete(true);
    }

    private void OnComplete(bool immediately)
    {
        if (immediately)
        {
            new ResearchRequestCommand(Entity.buildingVO.researchBuildingVO, ResearchReq.ResearchRequestType.CompleteImmediately, CurrentResearchId, GameDataAlgorithm.TimeToGem(TimeLeft)).ExecuteAndSend();
        }
        else
        {
            new ResearchRequestCommand(Entity.buildingVO.researchBuildingVO, ResearchReq.ResearchRequestType.Complete, CurrentResearchId, 0).ExecuteAndSend();
        }

        Init();//init from vo
    }

    public override void Update(float dt)
    {
        if (CurrentResearchId > 0 && ServerTime.Instance.Now() >= endTime)
        {
            OnComplete(false);
            if (EventComplete != null)
            {
                EventComplete();
            }
        }
    }
}
