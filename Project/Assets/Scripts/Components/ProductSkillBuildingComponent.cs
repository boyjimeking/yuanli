
using com.pureland.proto;

public class ProductSkillBuildingComponent : BaseProductBuildingComponent
{
    public override void Init()
    {
        base.Init();

        EventDispather.AddEventListener(GameEvents.SKILL_UP,OnSkillUpEvent);
    }

    public override void Destroy()
    {
        EventDispather.RemoveEventListener(GameEvents.SKILL_UP,OnSkillUpEvent);
        base.Destroy();
    }

    /// <summary>
    /// 技能升级了,刷新正在升级的数据
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="obj"></param>
    private void OnSkillUpEvent(string eventtype, object obj)
    {
        var baseId = (int) obj;
        foreach (var productionItemVo in ProductionItems)
        {
            if (productionItemVo.cid == baseId)
            {
                var upgradeId = DataCenter.Instance.FindEntityModelById(baseId).upgradeId;
                productionItemVo.cid = upgradeId;
                break;
            }
        }
    }

    protected override int GetProductionTime(EntityModel model)
    {
        return model.trainTime;
    }

    protected override int GetProductionUseQueueSize(EntityModel model)
    {
        return 1;
    }

    protected override bool CanCompleteProduction()
    {
        return true;//REMARK 入队时有限制,肯定不会超出队列
    }

    protected override void OnComplete(ProductionItemVO productionItem)
    {
        DataCenter.Instance.AddSkill(new SkillVO() { cid = productionItem.cid, amount = productionItem.count });
        new ProductionRequestCommand(ProductionReq.ProductionRequestType.Complete, Entity.buildingVO, productionItem).ExecuteAndSend();
    }

    public override bool CanCompleteProductionImmediately()
    {
        return true;//REMARK 入队时有限制,肯定不会超出队列
    }

    public override void OnCompleteProductionImmediately()
    {
        foreach (var productionItem in ProductionItems)
        {
            DataCenter.Instance.AddSkill(new SkillVO() { cid = productionItem.cid, amount = productionItem.count });
        }
        ProductionItems.Clear();
        int diamond = GameDataAlgorithm.TimeToGem(TotalTimeLeft);
        EndTime = System.DateTime.MinValue;
        new ProductionRequestCommand(ProductionReq.ProductionRequestType.CompleteImmediately, Entity.buildingVO, null, diamond).ExecuteAndSend();
    }
}
