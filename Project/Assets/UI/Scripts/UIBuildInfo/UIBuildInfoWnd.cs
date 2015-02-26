using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBuildInfoWnd : UIBaseWnd
{
    //建筑信息条容器
    public Transform propertyCon;
    //建筑Icon
    public Transform buildIconArea;
    private GameObject buildModel;
    //建筑名字
    public UILabel txtPanelName;
    //数据信息
    public TileEntity tileEntity;
    public GameObject buildInfoArea;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.isLockScreen = true;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        for (int i = 0; i < propertyCon.transform.childCount; i++)
        {
            GameObject.Destroy(propertyCon.transform.GetChild(i).gameObject);
        }
        GameObject.Destroy(buildModel);
        BuildOptManager.Instance.ShowBuildingOptWin(this.tileEntity);
    }
    public TileEntity CurTileEntity
    {
        set
        {
            this.tileEntity = value;
            UpdatePanelByData();
        }
    }
    private void UpdatePanelByData()
    {
        txtPanelName.text = tileEntity.model.nameForView + "(等级" + tileEntity.model.level + ")";
        buildModel = (GameObject)TileEntity.LoadAndCreate(tileEntity.model);
        if (buildModel != null)
        {
            var view = buildModel.AddMissingComponent<EntityViewComponent>();
            view.Init(false);
            buildModel.transform.parent = buildIconArea;
            buildModel.SetLayerRecursively(LayerMask.NameToLayer("UILayerMiddle"));
            buildModel.transform.localScale = new Vector3(20, 20, 1);
            buildModel.transform.localPosition = new Vector3(0, -20, 0);
        }
        UpdateBuildInfo();
    }
    private void UpdateBuildInfo()
    {
        //属性信息
        List<string> list = BuildPropertyUtil.GetPropertyList(tileEntity);
        UILogicInfoProperty buildProperty;
        for (int i = 0, imax = list.Count; i < imax; i++)
        {
            GameObject obj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/Property");
            obj.transform.parent = propertyCon.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(0, 80 - i * (obj.GetComponent<UISprite>().height + 2));
            obj.SetActive(true);
            buildProperty = obj.AddMissingComponent<UILogicInfoProperty>();
            string desStr = BuildOptManager.Instance.GetBuildingPropertyDes(list[i]);
            buildProperty.SetPropertyIcon(GetPropertyIcon(list[i]));
            if (list[i] == BuildPropertyUtil.SPACEPROVIDE)
            {
                buildProperty.SetBingYingInfo(desStr);
            }
            else if (list[i] == BuildPropertyUtil.QUEUESIZE)
            {
                buildProperty.SetArmyFactoryInfo(desStr, tileEntity);
            }
            else if (list[i] == BuildPropertyUtil.RESOURCEPERSECONDFORVIEW)
            {
                buildProperty.SetPropertyInfo(GetPropertyValue(tileEntity.model, list[i]), -1, desStr);
            }
            else if (list[i] == BuildPropertyUtil.MAXRESOURCESTORAGE)
            {
                buildProperty.SetResourceCollectInfo(desStr, tileEntity);
            }
            else
            {
                EntityModel maxEntityModel = BuildOptManager.Instance.GetMaxLevelEntityModel(tileEntity.model.baseId);
                buildProperty.SetPropertyInfo(GetPropertyValue(tileEntity.model, list[i]), GetPropertyValue(maxEntityModel, list[i]), desStr);
            }
        }
        buildInfoArea.GetComponent<UIBuildInfoArea>().SetBuildingInfo(tileEntity);
    }
    private int GetPropertyValue(EntityModel model, string property)
    {
        return BuildOptManager.Instance.GetPropertyValue(model, property);
    }
    private string GetPropertyIcon(string property)
    {
        return BuildOptManager.Instance.GetBuildingPropertyIcon(property);
    }
}
