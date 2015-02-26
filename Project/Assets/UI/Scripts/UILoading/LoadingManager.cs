using UnityEngine;
using System.Collections;

public class LoadingManager : Singleton<LoadingManager>
{
    private GameObject zhuanChang;
    public void ShowLoading()
    {
        if (null == zhuanChang)
        {
            zhuanChang = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_ZhuanChang/zhuanchang");
        }
        zhuanChang.name = "zhuanchang";
        zhuanChang.SetActive(true);
        zhuanChang.transform.parent = UIMananger.Instance.GetLayerByLayer(zhuanChang.GetComponent<UIBaseWnd>().Layer);
        zhuanChang.transform.localScale = new Vector3(360, 360, 360);
        zhuanChang.GetComponent<UIZhuanChangWnd>().Close();
    }
    public void CloseLoading()
    {
        if (null == zhuanChang)
            return;
        zhuanChang.GetComponent<UIZhuanChangWnd>().Open();
    }
}
