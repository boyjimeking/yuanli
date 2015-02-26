using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameTipsManager : Singleton<GameTipsManager>
{
    public delegate void CallBack(bool isConfirm, ArrayList arrayList);
    public void ShowGameTips(string tipStr)
    {
        GameObject topCenter = UIMananger.Instance.ShowWin("PLG_GameTips", "UITipTopCenterPanel");
        topCenter.transform.localPosition = new Vector3(0, 240, 0);
        topCenter.GetComponent<UITipTopCenterWnd>().ShowInfo(tipStr);
    }
    public void ShowGameDevelopTips(string tipStr)
    {
        GameObject topCenter = UIMananger.Instance.ShowWin("PLG_GameTips", "UITipTopCenterPanel");
        topCenter.transform.localPosition = new Vector3(0, 240, 0);
        topCenter.GetComponent<UITipTopCenterWnd>().ShowInfo(tipStr);
    }
    public void ShowServerErrorMsg(string errorStr)
    {
        GameObject topCenter = UIMananger.Instance.ShowWin("PLG_GameTips", "UIGameDevelopErrorPanel");
        topCenter.GetComponent<UIGameDevelopErrorWnd>().SetErrorMessage(errorStr);
    }
    public void ShowGameTips(EnumTipsID tipId, string[] param = null, ArrayList addParam = null, CallBack callbackMethod = null, ArrayList callbackParam = null)
    {
        GameObject tipWnd = null;
        TipModel tipModel = DataCenter.Instance.FindTipModelById((int)tipId);
        if (null == tipModel)
        {
            return;
        }
        string content = ReplaceStr(tipModel.content, param);
        switch (tipModel.type)
        {
            case "Type_A":
                tipWnd = UIMananger.Instance.ShowWin("PLG_GameTips", "UITipTopCenterPanel");
                tipWnd.transform.localPosition = new Vector3(0, 240, 0);
                tipWnd.GetComponent<UITipTopCenterWnd>().ShowInfo(content);
                break;
            case "Type_B":
                tipWnd = UIMananger.Instance.ShowWin("PLG_GameTips", "UIGameDiamondTipPanel");
                tipWnd.GetComponent<UIGameDiamondTipWnd>().SetTipInfo((int)tipId, content, addParam, callbackMethod, callbackParam);
                break;
            case "Type_C":
                //暂时
                if (tipId == EnumTipsID.ShopTip_10105)
                    UIMananger.Instance.CloseWinByType(UICloseOrHideType.CLOSE_DIAMOND_TIP);
                tipWnd = UIMananger.Instance.ShowWin("PLG_GameTips", "UIGameAlertTipPanel");
                tipWnd.GetComponent<UIGameAlertTipWnd>().SetTipInfo((int)tipId, content, addParam, callbackMethod, callbackParam);
                break;

        }
    }
    private string ReplaceStr(string strText, string[] param)
    {
        if (null != param)
        {
            for (int i = 0, imax = param.Length; i < imax; i++)
            {
                strText = strText.Replace("{" + i + "}", param[i]);
            }
        }
        return strText;
    }
}
