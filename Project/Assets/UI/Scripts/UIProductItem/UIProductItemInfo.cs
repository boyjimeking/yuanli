using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System;

public class UIProductItemInfo : MonoBehaviour
{
    //兵种描述按钮
    public GameObject btnDes;
    //兵种Icon背景
    public UISprite backIcon;
    //兵种Icon
    public UISprite itemIcon;
    //士兵信息
    private ProductionItemVO itemVO;
    //生产数量
    public UILabel txtCount;
    //提示信息
    public UILabel txtTip;
    //等级文本
    public UILabel txtLevel;
    //生产花费
    public UILabel txtConsume;
    //背景
    public UISprite backGround;
    //等级
    public GameObject backLevel;
    public UISprite iconMoney;
    //开放等级
    private int openLevel = -1;
    void OnEnable()
    {
        UIEventListener.Get(btnDes).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        EventDispather.DispatherEvent(ItemOperationManager.DISPLAY_ITEM_DES, this.gameObject);
    }
    void OnDisable()
    {
        UIEventListener.Get(btnDes).onClick -= OnClickButton;
    }
    public void ProductOneSoldier()
    {
        EventDispather.DispatherEvent(ItemOperationManager.PRODUCT_ONE_ITEM, this.gameObject);
    }
    /// <summary>
    /// 信息数据
    /// </summary>
    public ProductionItemVO ItemVO
    {
        set
        {
            this.itemVO = value;
            if (itemVO.count <= 0)
            {
                txtCount.text = "";
            }
            else
            {
                txtCount.text = itemVO.count.ToString() + "X";
            }
            EntityModel model = DataCenter.Instance.FindEntityModelById(itemVO.cid);
            if (null == model) return;
            txtLevel.text = model.level.ToString();
            txtConsume.text = model.trainCostResourceCount.ToString();
            itemIcon.spriteName = ResourceUtil.GetItemIconByModel(model);
            iconMoney.spriteName = model.trainCostResourceType.ToString();
        }
        get
        {
            return this.itemVO;
        }
    }
    /// <summary>
    /// 不符合条件时置灰
    /// </summary>
    public bool IsGrey
    {
        set
        {
            Color color = value ? PanelUtil.greyColor : Color.white;
            btnDes.GetComponent<UIButton>().isEnabled = !value;
            PanelUtil.SetUIRectColor(null, color, false, this.gameObject);
        }
    }
    /// <summary>
    /// 开放等级
    /// </summary>
    public int OpenLevel
    {
        set
        {
            this.openLevel = value;
            bool isVisible = value <= 0 ? true : false;
            txtTip.transform.gameObject.SetActive(!isVisible);
            iconMoney.transform.gameObject.SetActive(isVisible);
            txtConsume.transform.gameObject.SetActive(isVisible);
            backLevel.SetActive(isVisible);
            btnDes.SetActive(isVisible);
            if (value > 0)
            {
                EntityModel model = DataCenter.Instance.FindEntityModelById(itemVO.cid);
                var name = DataCenter.Instance.NeedToLocalName(model.buildNeedType, model.buildNeedLevel);
                txtTip.text = "[FF0000]需要" + value + "级" + name + "[-]";
            }
            //this.GetComponent<UIRepeatedButton>().enabled = isVisible;
        }
        get
        {
            return this.openLevel;
        }
    }
}
