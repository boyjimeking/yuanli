using UnityEngine;
using System.Collections;

public class UIStartWnd : UIBaseWnd
{
    //登陆
    public GameObject loginCon;
    //选择种族
    public GameObject loginMainFrame;
    //选择头像
    public GameObject createFaceCon;
    //种族展示
    public GameObject professionDes;
    //当前显示的对象
    private GameObject curDisplay;
    protected override void Awake()
    {
        base.Awake();
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_PANEL);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    /// <summary>
    /// 根据类型显示对应窗体
    /// 1、登陆2、创建种族3、创建头像
    /// </summary>
    /// <param name="type"></param>
    public void ShowWinByType(int type)
    {
        if (curDisplay)
        {
            curDisplay.SetActive(false);
        }
        if (1 == type)
        {
            loginCon.SetActive(true);
            curDisplay = loginCon;
        }
        else if (2 == type)
        {
            loginMainFrame.SetActive(true);
            loginMainFrame.GetComponent<UILoginMainFrame>().UpdatePlayerList();
            curDisplay = loginMainFrame;
        }
        else if (3 == type)
        {
            createFaceCon.SetActive(true);
            curDisplay = createFaceCon;
        }
    }
    public void ShowProfessionDes(bool isShow)
    {
        professionDes.SetActive(isShow);
    }
}
