using UnityEngine;
using System.Collections;
using System;

public class UIShopItemDetail : MonoBehaviour
{
    public UILabel txtItemName;
    public UILabel txtCount;
    public UILabel txtTime;
    public UILabel txtConsume;
    public GameObject icon;
    private ShopModel itemData;
    public UILabel txtTip;
    public GameObject timeIcon;
    public UISprite moneyIcon;
    private bool isMax = false;
    private int openLevel = -1;
    public GameObject btnDes;
    public event Action<bool> OnClickButtonDes;
    private bool isGrey;
    void OnEnable()
    {
        UIEventListener.Get(btnDes).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (null != OnClickButtonDes)
        {
            OnClickButtonDes(true);
        }
    }
    void OnDestroy()
    {
        UIEventListener.Get(btnDes).onClick -= OnClickButton;
        OnClickButtonDes = null;
    }
    private void SetGreyByType()
    {
        UISprite[] sprites = this.transform.GetComponentsInChildren<UISprite>();
        foreach (UISprite sprite in sprites)
        {
            sprite.color = Color.black;
        }
    }
    public ShopModel ItemData
    {
        set
        {
            this.itemData = value;
            EntityModel model = DataCenter.Instance.FindEntityModelById(itemData.baseId);
            if (null == model)
            {
                Debug.Log("当前没有BaseID为" + itemData.baseId + "的数据");
                return;
            }
            txtItemName.text = model.nameForView;
            //判断大本营等级
            if (model.buildNeedLevel > DataCenter.Instance.GetCenterBuildingModel().level)
            {
                openLevel = model.buildNeedLevel;
                txtTip.text = "要求大本营达到" + openLevel + "级";
                txtTip.gameObject.SetActive(true);
                timeIcon.SetActive(false);
                txtTime.gameObject.SetActive(false);
                txtCount.gameObject.SetActive(false);
                SetGreyByType();
                isGrey = true;
            }
            else
            {
                openLevel = -1;
                timeIcon.SetActive(true);
                txtTime.gameObject.SetActive(true);
                txtCount.gameObject.SetActive(true);
            }
            if (!isGrey)
            {
                //建造数量
                int maxCount = DataCenter.Instance.FindBuildingLimitById(itemData.baseId);
                if (maxCount > 0)
                {
                    int curCount = DataCenter.Instance.CountBuilding(model.subType);
                    txtCount.text = "已建\n" + curCount + "/" + maxCount;
                    isMax = curCount >= maxCount;
                    if (isMax)
                        SetGreyByType();
                }
            }
            if (model.buildTime <= 0)
            {
                txtTime.text = "无";
            }
            else
            {
                txtTime.text = DateTimeUtil.PrettyFormatTimeSeconds(model.buildTime, 2);
            }
            //资源花费及花费类型
            if (model.costResourceCount > DataCenter.Instance.GetResource(model.costResourceType))
                txtConsume.text = "[FF0000]" + model.costResourceCount + "[-]";
            else
                txtConsume.text = "[FFFFFF]" + model.costResourceCount + "[-]";
            moneyIcon.spriteName = model.costResourceType.ToString();
            int width = moneyIcon.width + txtConsume.width + 4;
            txtConsume.transform.localPosition = new Vector3(-width / 2f + txtConsume.width / 2f, txtConsume.transform.localPosition.y);
            moneyIcon.transform.localPosition = new Vector3(txtConsume.transform.localPosition.x + txtConsume.width / 2f + 4 + moneyIcon.width / 2f, moneyIcon.transform.localPosition.y);

            //设置显示图标 使用游戏中的对象当作图标,使用的另外的相机,需要设置layer为UILayer2,缩放10倍
            if (null != icon.GetComponent<UISprite>().atlas.GetSprite(model.nameForResource))
            {
                icon.GetComponent<UISprite>().spriteName = model.nameForResource;
            }
            else
            {
                var iconObject = (GameObject)TileEntity.LoadAndCreate(model);
                if (iconObject != null)
                {
                    var view = iconObject.AddMissingComponent<EntityViewComponent>();
                    view.Init(false);
                    icon.GetComponent<UISprite>().spriteName = "";
                    iconObject.transform.parent = icon.transform;
                    iconObject.SetLayerRecursively(LayerMask.NameToLayer("UILayerMiddle"));
                    iconObject.transform.localScale = new Vector3(20, 20, 1);
                    iconObject.transform.localPosition = new Vector3(-10, -30, 0);
                }
            }
        }
        get
        {
            return this.itemData;
        }
    }
    public bool IsMax
    {
        get
        {
            return this.isMax;
        }
    }
    public int OpenLevel
    {
        get
        {
            return this.openLevel;
        }
    }
}
