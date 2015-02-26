using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class UILayer : MonoBehaviour
{
    //背部相机
    public GameObject cameraBack;
    //中层相机
    public GameObject cameraMiddle;
    //前景相机
    public GameObject cameraFront;
    //固定UI层
    public GameObject fixedUILayer;
    //ui遮罩层
    public GameObject uiMaskLayer;
    //普通的UI层
    public GameObject normalUILayer;
    //转场动画
    public GameObject zhuanchangLayer;
    //提示层
    public GameObject tipsUILayer;
    void Awake()
    {
        UIMananger.Instance.InitUILayers(UIMananger.UILayer.UI_FIXED_LAYER, fixedUILayer);
        UIMananger.Instance.InitUILayers(UIMananger.UILayer.UI_SCREENMASK_LAYER, uiMaskLayer);
        UIMananger.Instance.InitUILayers(UIMananger.UILayer.UI_NORMAL_LAYER, normalUILayer);
        UIMananger.Instance.InitUILayers(UIMananger.UILayer.UI_ZHUANCHANG_LAYER, zhuanchangLayer);
        UIMananger.Instance.InitUILayers(UIMananger.UILayer.UI_TIPS_LAYER, tipsUILayer);
        UIMananger.Instance.uiLayer = this.gameObject;
    }
}
