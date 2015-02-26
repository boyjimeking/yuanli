using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class UIMananger : Singleton<UIMananger>
{
    //UI的层（深度）
    public const int UI_FIXED = 0;
    public const int UI_SCREENMASK = 50;
    public const int UI_PANEL = 100;
    public const int UI_TIP = 150;
    //UI各个层
    public enum UILayer
    {
        UI_FIXED_LAYER = 0,
        UI_SCREENMASK_LAYER = 1,
        UI_NORMAL_LAYER = 2,
        UI_ZHUANCHANG_LAYER = 3,
        UI_TIPS_LAYER = 4,
    }
    private Dictionary<UILayer, GameObject> dicUILayers = new Dictionary<UILayer, GameObject>();
    //ui层容器
    public GameObject uiLayer;
    //存储ui面板的实例
    private Hashtable hashWin = new Hashtable();
    public void InitUILayers(UILayer layerName, GameObject layer)
    {
        if (!dicUILayers.ContainsKey(layerName))
            dicUILayers.Add(layerName, layer);
    }
    public GameObject ShowWin(string path, string winName)
    {
        GameObject childWin = (GameObject)hashWin[winName];
        if (null == childWin)
        {
            childWin = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/" + path + "/" + winName);
        }
        childWin.name = winName;
        childWin.SetActive(true);
        UIBaseWnd baseWin = childWin.GetComponent<UIBaseWnd>();
        childWin.transform.parent = GetLayerByLayer(baseWin.Layer);
        childWin.transform.localScale = Vector3.one;
        if (!hashWin.ContainsKey(winName))
            hashWin.Add(winName, childWin);
        CloseExculisionWin();
        return childWin;
    }
    public Transform GetLayerByLayer(UILayer layer)
    {
        if (dicUILayers.ContainsKey(layer))
            return dicUILayers[layer].transform;
        return null;
    }
    /// <summary>
    /// 确保面板只显示一个
    /// </summary>
    private void CloseExculisionWin()
    {

    }
    //关闭面板
    public void CloseWin(string name)
    {
        GameObject closeWin = (GameObject)hashWin[name];
        if (null == closeWin) return;
        //面板是否彻底移除
        bool isDestroy = closeWin.GetComponent<UIBaseWnd>().IsDestroy;
        if (isDestroy)
        {
            GameObject.Destroy(closeWin);
        }
        else
        {
            closeWin.SetActive(false);
        }
        hashWin.Remove(name);
    }
    public GameObject GetWinByName(string name)
    {
        if (!hashWin.ContainsKey(name))
            return null;
        GameObject win = (GameObject)hashWin[name];
        if (win.activeInHierarchy)
            return win;
        return null;
    }
    public void CloseWinByType(int type)
    {
        UIBaseWnd baseWin;
        Hashtable hasWinClone = (Hashtable)hashWin.Clone();
        foreach (DictionaryEntry tempDE in hasWinClone)
        {
            baseWin = (tempDE.Value as GameObject).GetComponent<UIBaseWnd>();
            if (null != baseWin && baseWin.CheckCloseOrHideType(type))
            {
                CloseWin((string)tempDE.Key);
            }
        }
    }
    public void CloseAllWin()
    {
        Hashtable hasWinClone = (Hashtable)hashWin.Clone();
        foreach (DictionaryEntry tempDE in hasWinClone)
        {
            if (tempDE.Key != "UITipTopCenterPanel")
                CloseWin((string)tempDE.Key);
        }
    }
}
