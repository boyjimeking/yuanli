
using System;
using System.Collections.Generic;
using com.pureland.proto;

public abstract class BaseProductBuildingComponent : EntityComponent
{
    private ProductionItemVO currentProduction;
    private List<ProductionItemVO> productionItems;
    private DateTime endTime;
    private int totalTime;
    private int currentQueueSize;
    private int maxQueueSize;                       //最大生产的队列长度

    public event Action<ProductionItemVO> EventComplete;//生产完一件

    /// <summary>
    /// 当前正在生产的单位
    /// </summary>
    public ProductionItemVO CurrentProduction
    {
        get { return currentProduction; }
        set
        {
            currentProduction = value;
            if (currentProduction != null)
            {
                CurrentProductionModel = DataCenter.Instance.FindEntityModelById(currentProduction.cid);
            }
            else
            {
                CurrentProductionModel = null;
            }

            if (currentProduction == null)
            {
                enabled = false;
            }
            else
            {
                enabled = true;
            }
        }
    }

    /// <summary>
    /// 当前正在生产的单位模型
    /// </summary>
    public EntityModel CurrentProductionModel { get; private set; }

    /// <summary>
    /// 当前队列长度
    /// </summary>
    public int CurrentQueueSize
    {
        get { return currentQueueSize; }
        private set { currentQueueSize = value; }
    }

    /// <summary>
    /// 队列最大长度
    /// </summary>
    public int MaxQueueSize
    {
        get { return maxQueueSize; }
        private set { maxQueueSize = value; }
    }

    /// <summary>
    /// 总剩余时间
    /// </summary>
    public int TotalTimeLeft
    {
        get { return (int)(totalTime - (GetProductionTime(CurrentProductionModel) - TimeLeft)); }
    }

    /// <summary>
    /// 当前训练单位剩余时间
    /// </summary>
    public int TimeLeft
    {
        get { return (int)(EndTime - ServerTime.Instance.Now()).TotalSeconds; }
    }

    public List<ProductionItemVO> ProductionItems
    {
        get { return productionItems; }
    }

    public DateTime EndTime
    {
        get { return endTime; }
        set
        {
            endTime = value;
            if (endTime == DateTime.MinValue)
            {
                Entity.buildingVO.productionBuildingVO.endTime = 0;
            }
            else
            {
                Entity.buildingVO.productionBuildingVO.endTime = DateTimeUtil.DateTimeToUnixTimestampMS(endTime);
            }
        }
    }

    public override void Init()
    {
        base.Init();
        if (Entity.buildingVO.productionBuildingVO == null)
        {
            Entity.buildingVO.productionBuildingVO = new ProductionBuildingVO();
        }
        productionItems = Entity.buildingVO.productionBuildingVO.productionItems;
        MaxQueueSize = Entity.model.queueSize;
        CalcCurrentQueueSizeAndTotalTime(out currentQueueSize, out totalTime);
        if (ProductionItems.Count > 0)
        {
            CurrentProduction = ProductionItems[0];
            EndTime = DateTimeUtil.UnixTimestampMSToDateTime(Entity.buildingVO.productionBuildingVO.endTime);
        }
        else
        {
            CurrentProduction = null;
        }
    }

    protected abstract int GetProductionTime(EntityModel model);

    protected abstract int GetProductionUseQueueSize(EntityModel model);

    protected abstract bool CanCompleteProduction();

    protected abstract void OnComplete(ProductionItemVO productionItem);

    /// <summary>
    /// 添加生产单位
    /// </summary>
    /// <param name="productionItem"></param>
    public void AddProduction(ProductionItemVO productionItem)
    {
        Assert.Should(CanAddToQueue(productionItem.cid));
        AddToQueue(productionItem);
        if (currentProduction == null)
        {
            PrepareNextProduction();
        }
        new ProductionRequestCommand(ProductionReq.ProductionRequestType.Add, Entity.buildingVO, productionItem).ExecuteAndSend();
    }

    /// <summary>
    /// 移除生产单位
    /// </summary>
    /// <param name="productionItem"></param>
    public void RemoveProduction(ProductionItemVO productionItem)
    {
        bool removeCurrentProduction = productionItem.cid == currentProduction.cid && productionItem.count == currentProduction.count;
        RemoveFromQueue(productionItem);
        if (removeCurrentProduction)
        {
            PrepareNextProduction();
        }
        new ProductionRequestCommand(ProductionReq.ProductionRequestType.Remove, Entity.buildingVO, productionItem).ExecuteAndSend();
    }

    private void PrepareNextProduction()
    {
        if (ProductionItems.Count > 0)
        {
            CurrentProduction = ProductionItems[0];
            EndTime = ServerTime.Instance.Now().AddSeconds(GetProductionTime(CurrentProductionModel));
        }
        else
        {
            CurrentProduction = null;
            EndTime = DateTime.MinValue;
            totalTime = 0;
        }
    }

    /// <summary>
    /// 是否能理解完成
    /// </summary>
    /// <returns></returns>
    public abstract bool CanCompleteProductionImmediately();

    public abstract void OnCompleteProductionImmediately();
    /// <summary>
    /// 立即完成
    /// </summary>
    public void CompleteProductionImmediately()
    {
        OnCompleteProductionImmediately();
        currentQueueSize = 0;
        CurrentProduction = null;
        totalTime = 0;
    }
    /// <summary>
    /// 完成生产单位
    /// </summary>
    private void CompleteProduction()
    {
        UnityEngine.Debug.Log("CompleteProduction");
        var item = new ProductionItemVO() { cid = CurrentProduction.cid, count = 1 };
        RemoveFromQueue(item);
        PrepareNextProduction();
        if (EventComplete != null)
        {
            EventComplete(item);
        }
        OnComplete(item);
    }
    /// <summary>
    /// 获取当前正在生产的队列长度,和总共需要的建造时间
    /// </summary>
    private void CalcCurrentQueueSizeAndTotalTime(out int size, out int totalTime)
    {
        size = 0;
        totalTime = 0;
        foreach (var productionItemVo in ProductionItems)
        {
            var model = DataCenter.Instance.FindEntityModelById(productionItemVo.cid);
            size += GetProductionUseQueueSize(model) * productionItemVo.count;
            totalTime += GetProductionTime(model) * productionItemVo.count;
        }
    }

    /// <summary>
    /// 添加入生产队列
    /// </summary>
    /// <param name="productionItem"></param>
    private void AddToQueue(ProductionItemVO productionItem)
    {
        var model = DataCenter.Instance.FindEntityModelById(productionItem.cid);
        CurrentQueueSize += GetProductionUseQueueSize(model) * productionItem.count;
        totalTime += GetProductionTime(model) * productionItem.count;
        for (int i = 0; i < ProductionItems.Count; i++)
        {
            if (ProductionItems[i].cid == productionItem.cid)
            {
                ProductionItems[i].count += productionItem.count;
                return;
            }
        }
        ProductionItems.Add(productionItem);
    }

    /// <summary>
    /// 从队列中移除
    /// </summary>
    /// <param name="productionItem"></param>
    private void RemoveFromQueue(ProductionItemVO productionItem)
    {
        var model = DataCenter.Instance.FindEntityModelById(productionItem.cid);
        CurrentQueueSize -= GetProductionUseQueueSize(model) * productionItem.count;
        totalTime -= GetProductionTime(model) * productionItem.count;

        for (int i = 0; i < ProductionItems.Count; i++)
        {
            if (ProductionItems[i].cid == productionItem.cid)
            {
                ProductionItems[i].count -= productionItem.count;
                if (ProductionItems[i].count <= 0)
                {
                    ProductionItems.RemoveAt(i);
                }
                return;
            }
        }
        Assert.Fail("Should not reach here");
    }

    /// <summary>
    /// 是否能加入队列
    /// </summary>
    /// <param name="cid"></param>
    /// <returns></returns>
    public bool CanAddToQueue(int cid)
    {
        return CurrentQueueSize + GetProductionUseQueueSize(DataCenter.Instance.FindEntityModelById(cid)) <= MaxQueueSize;
    }

    public override void Update(float dt)
    {
        if (CurrentProduction != null && ServerTime.Instance.Now() >= EndTime)
        {
            if (CanCompleteProduction())
            {
                CompleteProduction();
            }
            else
            {
                //UnityEngine.Debug.Log("不能完成生产,容器已满");
            }
        }
    }
}

