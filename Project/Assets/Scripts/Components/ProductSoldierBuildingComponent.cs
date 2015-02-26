
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;
using UnityEngine;

public class ProductSoldierBuildingComponent : BaseProductBuildingComponent
{
    protected override int GetProductionTime(EntityModel model)
    {
        return model.trainTime;
    }

    protected override int GetProductionUseQueueSize(EntityModel model)
    {
        return model.spaceUse;
    }

    protected override bool CanCompleteProduction()
    {
        return DataCenter.Instance.SpaceUsed + CurrentProductionModel.spaceUse <= DataCenter.Instance.TotalSpace;
    }

    protected override void OnComplete(ProductionItemVO productionItem)
    {
        DataCenter.Instance.SpaceUsed += DataCenter.Instance.FindEntityModelById(productionItem.cid).spaceUse;
        DataCenter.Instance.AddArmy(new ArmyVO() { amount = productionItem.count, cid = productionItem.cid });
        new ProductionRequestCommand(ProductionReq.ProductionRequestType.Complete, Entity.buildingVO, productionItem).ExecuteAndSend();
        //创建可视化对象
        CreateViewActor(productionItem.cid);
    }
    private void CreateViewActor(int cid)
    {
        var model = DataCenter.Instance.FindEntityModelById(cid);
        var entity = TileEntity.Create(OwnerType.Defender, model);
        entity.SetTilePosition(Entity.GetTilePos());
        IsoMap.Instance.DelayAddEntity(entity);
    }
    public override bool CanCompleteProductionImmediately()
    {
        return DataCenter.Instance.SpaceUsed + CurrentQueueSize <= DataCenter.Instance.TotalSpace;
    }

    public override void OnCompleteProductionImmediately()
    {
        DataCenter.Instance.SpaceUsed += CurrentQueueSize;
        var newViewActors = new List<ArmyVO>();
        foreach (var productionItem in ProductionItems)
        {
            var army = new ArmyVO() {amount = productionItem.count, cid = productionItem.cid};
            DataCenter.Instance.AddArmy(army);
            newViewActors.Add(army);
        }
        ProductionItems.Clear();
        int diamond = GameDataAlgorithm.TimeToGem(TotalTimeLeft);
        EndTime = System.DateTime.MinValue;
        CoroutineHelper.Run(CreateViewActors(newViewActors));
        new ProductionRequestCommand(ProductionReq.ProductionRequestType.CompleteImmediately, Entity.buildingVO, null, diamond).ExecuteAndSend();
    }

    IEnumerator CreateViewActors(IEnumerable<ArmyVO> armies)
    {
        foreach (var armyVo in armies)
        {
            for (int i = 0; i < armyVo.amount; i++)
            {
                CreateViewActor(armyVo.cid);
                yield return new WaitForSeconds(Constants.SPAWN_INTERVAL_TIME);
            }
        }
    }
}
