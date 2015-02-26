using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class HomeLandManager : Singleton<HomeLandManager>
{
    public HomeLandManager()
    {
        GameModule.AddModule(new ModuleHomeLand());
    }
    public void SetCurCamp(CampVO campVO)
    {
        ModuleHomeLand module = (ModuleHomeLand)GameModule.GetModule(GameModule.MODULE_HOMELAND);
        module.campVO = campVO;
    }
    public void addEventListener()
    {

    }
    public void ShowMyHomeLandUI()
    {
        UIMananger.Instance.CloseWinByType(UICloseOrHideType.CLOSE_WORLD_TYPE_HOME);
        int panelOffsetX = 0, panelOffsetY = 0;
        UISprite sprite;
        //头部信息
        GameObject playerHead = UIMananger.Instance.ShowWin("PLG_MainUI", "UIPersonHeadPanel");
        sprite = playerHead.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0, 0, 1, 1), new Vector4(0, sprite.width, -sprite.height, 0));
        playerHead.GetComponent<UIPersonHeadWnd>().UpdatePersonInfo(null, null);
        //显示的资源信息
        GameObject moneyWin = UIMananger.Instance.ShowWin("PLG_MainUI", "UIPersonMoneyPanel");
        sprite = moneyWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(1, 1, 1, 1), new Vector4(-sprite.width, 0, -sprite.height, 0));
        moneyWin.GetComponent<UIPersonMoneyWnd>().SetPlayerMoney(OwnerType.Defender);
        //左下角窗体
        panelOffsetX = 5; panelOffsetY = 15;
        GameObject bottomLeftWin = UIMananger.Instance.ShowWin("PLG_MainUI", "UIBottomLeftPanel");
        sprite = bottomLeftWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0, 0, 0, 0), new Vector4(panelOffsetX, panelOffsetX + sprite.width, panelOffsetY, panelOffsetY + sprite.height));
        //右下角窗体
        panelOffsetX = 5; panelOffsetY = 15;
        GameObject bottomRightWin = UIMananger.Instance.ShowWin("PLG_MainUI", "UIBottomRightPanel");
        sprite = bottomRightWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(1, 1, 0, 0), new Vector4(-(panelOffsetX + sprite.width), -panelOffsetX, panelOffsetY, panelOffsetY + sprite.height));
        //左边的窗体
        panelOffsetX = 5; panelOffsetY = 15;
        GameObject leftWin = UIMananger.Instance.ShowWin("PLG_MainUI", "UILeftPanel");
        sprite = leftWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(0, 0, 0.5f, 0.5f), new Vector4(panelOffsetX, panelOffsetX + sprite.width, -panelOffsetY, -panelOffsetY + sprite.height));
        //右边窗体
        panelOffsetX = 0; panelOffsetY = 0;
        GameObject rightWin = UIMananger.Instance.ShowWin("PLG_MainUI", "UIRightPanel");
        sprite = rightWin.GetComponent<UISprite>();
        PanelUtil.SetPanelAnchors(sprite, UIMananger.Instance.uiLayer.transform, new Vector4(1, 1, 0.5f, 0.5f), new Vector4(panelOffsetX - sprite.width, panelOffsetX, -panelOffsetY - sprite.height * 0.5f, -panelOffsetY + sprite.height * 0.5f));
    }
    public void ShowBagWin()
    {
        GameObject bagWin = UIMananger.Instance.ShowWin("PLG_Bag", "UIBagPanel");
        bagWin.name = "UIBagPanel";
        bagWin.transform.parent = UIMananger.Instance.uiLayer.transform;
        //bagWin.transform.localPosition = new Vector3(0, 0);
        bagWin.transform.localScale = new Vector3(1, 1, 1);
        bagWin.GetComponent<UIPanel>().depth = UIMananger.UI_PANEL;
        bagWin.SetActive(true);
    }
}
