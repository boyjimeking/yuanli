using UnityEngine;
using System.Collections;

public class UILogicInfoProperty : MonoBehaviour
{
    //组件脚本
    private UIControlProperty controlProperty;
    private string preDes = "";
    private TileEntity tileEntity;
    private int updateType = -1;
    void OnEnable()
    {
        controlProperty = this.gameObject.GetComponent<UIControlProperty>();
    }
    public void SetPropertyIcon(string iconName)
    {
        controlProperty.iconProperty.spriteName = iconName;
    }
    /// <summary>
    /// 普通的建筑属性信息
    /// </summary>
    /// <param name="curProperty"></param>
    /// <param name="maxProperty"></param>
    /// <param name="propertyDes"></param>
    public void SetPropertyInfo(int curProperty, int maxProperty, string propertyDes)
    {
        preDes = propertyDes;
        if (maxProperty > 0)
        {
            controlProperty.txtProperty.text = preDes + maxProperty + "/" + maxProperty;
        }
        else
        {
            controlProperty.txtProperty.text = preDes + curProperty;
        }
        controlProperty.curSlider.value = 1;
    }
    /// <summary>
    /// 兵营
    /// </summary>
    /// <param name="propertyDes"></param>
    public void SetBingYingInfo(string propertyDes)
    {
        preDes = propertyDes;
        EventDispather.AddEventListener(GameEvents.SPACE_CHANGE, OnSpaceChange);
        OnSpaceChange(null, null);
    }
    /// <summary>
    /// 兵营空间
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="obj"></param>
    private void OnSpaceChange(string eventType, object obj)
    {
        controlProperty.txtProperty.text = preDes + DataCenter.Instance.SpaceUsed + "/" + DataCenter.Instance.TotalSpace;
        controlProperty.curSlider.value = DataCenter.Instance.SpaceUsed * 1.0f / DataCenter.Instance.TotalSpace;
    }
    public void SetArmyFactoryInfo(string propertyDes, TileEntity entity)
    {
        this.tileEntity = entity;
        preDes = propertyDes;
        entity.GetComponent<BaseProductBuildingComponent>().EventComplete += UpdateProductSize;
        UpdateProductSize(null);
    }

    private void UpdateProductSize(com.pureland.proto.ProductionItemVO obj)
    {
        BaseProductBuildingComponent component = this.tileEntity.GetComponent<BaseProductBuildingComponent>();
        controlProperty.txtProperty.text = preDes + component.CurrentQueueSize + "/" + component.MaxQueueSize;
        controlProperty.curSlider.value = component.CurrentQueueSize * 1.0f / component.MaxQueueSize;
    }
    public void SetResourceCollectInfo(string propertyDes, TileEntity entity)
    {
        this.tileEntity = entity;
        preDes = propertyDes;
        updateType = 1;
        Update();
    }
    void OnDisable()
    {
        EventDispather.RemoveEventListener(GameEvents.SPACE_CHANGE, OnSpaceChange);
        if (null != this.tileEntity)
        {
            tileEntity.GetComponent<BaseProductBuildingComponent>().EventComplete -= UpdateProductSize;
        }
    }
    void Update()
    {
        if (updateType == -1) return;
        if (updateType == 1)
        {
            //收集资源类的
            GatherResourceBuildingComponent component = this.tileEntity.GetComponent<GatherResourceBuildingComponent>();
            controlProperty.txtProperty.text = preDes + component.CalculateResourceFromLastGather(ServerTime.Instance.Now()) + "/" + tileEntity.model.maxResourceStorage;
            controlProperty.curSlider.value = component.CalculateResourceFromLastGather(ServerTime.Instance.Now()) * 1.0f / tileEntity.model.maxResourceStorage;
        }

    }
}
