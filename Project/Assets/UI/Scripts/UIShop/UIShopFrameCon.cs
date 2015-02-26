using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIShopFrameCon : MonoBehaviour
{
    public GameObject shopTypeFrame;
    private List<GameObject> shopTypeList = new List<GameObject>();
    public event Action<int> ClickShopType;
    public void SetShopTypeFrameData()
    {
        if (shopTypeList.Count > 0)
            return;
        ModuleShop module = (ModuleShop)GameModule.GetModule(GameModule.MODULE_SHOP);
        for (int i = 0, imax = module.shopData.Count; i < imax; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(shopTypeFrame, Vector3.zero, Quaternion.identity);
            obj.transform.parent = this.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(-363 + (i / 2) * (obj.GetComponent<UISprite>().width + 46), 149 - Mathf.Floor(i % 2) * (obj.GetComponent<UISprite>().height + 103));
            obj.SetActive(true);
            ShopModel model = (ShopModel)module.shopData[i][0];
            obj.GetComponent<UIShopTypeFrame>().SetShopInfo(i, model.shopIcon, model.shopName);
            shopTypeList.Add(obj);
            UIEventListener.Get(obj).onClick += OnClickShopTypeFrame;
        }
    }

    private void OnClickShopTypeFrame(GameObject go)
    {
        if (null != ClickShopType)
        {
            ClickShopType(go.GetComponent<UIShopTypeFrame>().shopOrder);
        }
    }
    public void ClearShopFrameCon()
    {
        foreach (GameObject obj in shopTypeList)
        {
            UIEventListener.Get(obj).onClick -= OnClickShopTypeFrame;
            GameObject.Destroy(obj);
        }
        ClickShopType = null;
        shopTypeList.Clear();
    }
}
