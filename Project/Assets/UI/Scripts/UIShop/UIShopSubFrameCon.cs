using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIShopSubFrameCon : MonoBehaviour
{
    public GameObject shopSubTypeFrame;
    private List<GameObject> shopTypeList = new List<GameObject>();
    public UISprite subFrameScrollBack;
    public UISprite shopSubFrameBack;
    void OnEnable()
    {
        int frameWidth = Screen.width * Constants.UI_HEIGHT / Screen.height;
        subFrameScrollBack.gameObject.SetActive(true);
        subFrameScrollBack.width = frameWidth;
        subFrameScrollBack.GetComponent<BoxCollider>().size = new Vector3(frameWidth, subFrameScrollBack.height);
        shopSubFrameBack.width = frameWidth;
    }
    public void SetShopSubTypeFrameData(int shopOrder)
    {
        ModuleShop module = (ModuleShop)GameModule.GetModule(GameModule.MODULE_SHOP);
        int width = Screen.width * Constants.UI_HEIGHT / Screen.height;
        for (int i = 0, imax = module.shopData[shopOrder].Count; i < imax; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(shopSubTypeFrame, Vector3.zero, Quaternion.identity);
            obj.transform.parent = this.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localPosition = new Vector3(-(width / 2 - obj.GetComponent<UISprite>().width / 2 - 50) + (i / 2) * (obj.GetComponent<UISprite>().width + 10), 140 - Mathf.Floor(i % 2) * (obj.GetComponent<UISprite>().height + 30));
            obj.SetActive(true);
            obj.GetComponent<UIShopItem>().ItemData = (ShopModel)module.shopData[shopOrder][i];
            shopTypeList.Add(obj);
        }
    }
    void OnDisable()
    {
        foreach (GameObject obj in shopTypeList)
        {
            GameObject.Destroy(obj);
        }
        shopTypeList.Clear();
        subFrameScrollBack.gameObject.SetActive(false);
        UIScrollView scrollView = this.gameObject.GetComponent<UIScrollView>();
        scrollView.DisableSpring();
        UIScrollView.list.Remove(scrollView);
        this.transform.localPosition = new Vector3(0, -70, 0);
        this.gameObject.GetComponent<UIPanel>().clipOffset = new Vector2(0, 0);
    }
}
