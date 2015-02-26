using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System;

public class UIBuildSoldier : MonoBehaviour
{
    public UISprite iconSoldier;
    public UILabel txtSoldierCount;
    public GameObject btnReduce;
    private ArmyVO soldierVO;
    public event Action ClickReduceButton;
    void OnEnable()
    {
        UIEventListener.Get(btnReduce).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (soldierVO.amount > 0)
            soldierVO.amount--;
        txtSoldierCount.text = soldierVO.amount.ToString();
        if (null != ClickReduceButton)
        {
            ClickReduceButton();
        }
    }
    void OnDisable()
    {
        UIEventListener.Get(btnReduce).onClick -= OnClickButton;
    }
    public ArmyVO SoldierVO
    {
        set
        {
            this.soldierVO = value;
            EntityModel soldierModel = ModelUtil.GetEntityModel(soldierVO.amount);
            txtSoldierCount.text = soldierVO.amount.ToString();
            iconSoldier.spriteName = ResourceUtil.GetItemIconByModel(soldierModel);
        }
        get
        {
            return this.soldierVO;
        }
    }
}
