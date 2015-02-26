using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System;

public class UIProductingItem : MonoBehaviour
{
    //兵种icon
    public UISprite itemIcon;
    //减少生产按钮
    public GameObject btnReduce;
    //兵的数量
    public UILabel txtProductCount;
    //时间轴容器
    public GameObject progressCon;
    //时间提示
    public GameObject labTip;
    //时间条
    public UISlider progressTime;
    //时间或者是提示文本框
    public UILabel txtTime;
    //生产信息
    private ProductionItemVO itemVO;
    //训练时间
    private int trainTotalTime = 1;
    //是否已经有计时器了
    private bool isCounting;
    public void RemoveOneItem()
    {
        EventDispather.DispatherEvent(ItemOperationManager.REMOVE_ONE_ITEM, this.gameObject);
    }
    public ProductionItemVO ItemVO
    {
        set
        {
            this.itemVO = value;
            if (itemVO.count <= 0)
                txtProductCount.text = "";
            else
                txtProductCount.text = itemVO.count.ToString() + "X";
            EntityModel model = DataCenter.Instance.FindEntityModelById(itemVO.cid);
            trainTotalTime = model.trainTime;
            itemIcon.spriteName = ResourceUtil.GetItemIconByModel(model);
        }
        get
        {
            return this.itemVO;
        }
    }
    public bool IsCurrentProduct
    {
        set
        {
            if (!isCounting)
            {
                OnTimer();
                this.InvokeRepeating("OnTimer", 0.1f, 1);
                isCounting = true;
            }
        }
    }
    private void OnTimer()
    {
        ModuleOperateItem module = (ModuleOperateItem)GameModule.GetModule(GameModule.MODULE_PRODUCTSOLDIER);
        if (module.currentProductFactory.TimeLeft <= 0 && DataCenter.Instance.SpaceUsed >= DataCenter.Instance.TotalSpace)
        {
            progressCon.SetActive(false);
            labTip.SetActive(true);
            return;
        }
        progressCon.SetActive(true);
        TimeSpan tt = new TimeSpan(0, 0, module.currentProductFactory.TimeLeft);
        string textstr = "";
        if (tt.Days > 0)
            textstr += tt.Days + "天 " + tt.Hours + "时 ";
        else if (tt.Hours > 0)
            textstr += tt.Hours + "时 " + tt.Minutes + "分 ";
        else if (tt.Minutes > 0)
            textstr += tt.Minutes + "分 " + tt.Seconds + "秒 ";
        else if (tt.Seconds > 0)
            textstr += tt.Seconds + "秒 ";
        txtTime.text = textstr;
        progressTime.value = module.currentProductFactory.TimeLeft * 1.0f / trainTotalTime;
    }
}