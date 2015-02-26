using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class UILoginMainFrame : MonoBehaviour
{
    public GameObject profession0;
    public GameObject profession1;
    public GameObject profession2;
    public GameObject profession3;
    public GameObject profession4;
    public UILabel txtPlayerName;
    public GameObject btnConfirm;
    public GameObject btnAccount;
    private GameObject curSelectProfession;
    void OnEnable()
    {
        UIEventListener.Get(profession0).onClick += OnClickProfession;
        UIEventListener.Get(profession1).onClick += OnClickProfession;
        UIEventListener.Get(profession2).onClick += OnClickProfession;
        //UIEventListener.Get(profession3).onClick += OnClickProfession;
        //UIEventListener.Get(profession4).onClick += OnClickProfession;
        UIEventListener.Get(btnConfirm).onClick += OnClickButton;
        UIEventListener.Get(btnAccount).onClick += OnClickButton;
        profession3.transform.Find("icon").GetComponent<UISprite>().color = Color.black;
        profession4.transform.Find("icon").GetComponent<UISprite>().color = Color.black;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnConfirm))
        {
            if (null == curSelectProfession)
                return;
            if (null == curSelectProfession.GetComponent<UISelectPlayer>().PlayerInfo)
            {
                if (txtPlayerName.text == "" || txtPlayerName.text == "请输入昵称")
                {
                    GameTipsManager.Instance.ShowGameTips("输入昵称");
                    return;
                }
                if (txtPlayerName.text.Length < 4)
                {
                    GameTipsManager.Instance.ShowGameTips("至少四个字符");
                    return;
                }
                LoginManager.Instance.playerName = txtPlayerName.text;
                LoginManager.Instance.RequestCreateRole();
            }
            else
            {
                LoginManager.Instance.RequestEnterGame();
            }
        }
        else if (go.Equals(btnAccount))
        {
            LoginManager.Instance.ShowLoginWin();
        }
    }

    private void OnClickProfession(GameObject go)
    {
        if (curSelectProfession)
        {
            if (curSelectProfession.Equals(go))
                return;
            curSelectProfession.transform.Find("select").gameObject.SetActive(false);
        }
        go.transform.Find("select").gameObject.SetActive(true);
        curSelectProfession = go;
        if (go.Equals(profession0))
        {
            LoginManager.Instance.professionId = 1;
        }
        else if (go.Equals(profession1))
        {
            LoginManager.Instance.professionId = 2;
        }
        else if (go.Equals(profession2))
        {
            LoginManager.Instance.professionId = 3;
        }
        else if (go.Equals(profession3))
        {

        }
        else if (go.Equals(profession4))
        {

        }
        if (null != go.GetComponent<UISelectPlayer>().PlayerInfo)
        {
            txtPlayerName.text = go.GetComponent<UISelectPlayer>().PlayerInfo.userName;
            txtPlayerName.GetComponent<UIInput>().enabled = false;
        }
        else
        {
            txtPlayerName.GetComponent<UIInput>().enabled = true;
            LoginManager.Instance.ShowProfessionDes(true);
        }
    }
    public void UpdatePlayerList()
    {
        List<PlayerLoginSimpleVO> playerList = LoginManager.Instance.LoginSimpleVOs;
        int i = 0, imax = (null != playerList) ? playerList.Count : 0;
        for (; i < imax; i++)
        {
            if (playerList[i].raceType == 1)
            {
                profession0.GetComponent<UISelectPlayer>().PlayerInfo = playerList[i];
            }
            else if (playerList[i].raceType == 2)
            {
                profession1.GetComponent<UISelectPlayer>().PlayerInfo = playerList[i];
            }
            else if (playerList[i].raceType == 3)
            {
                profession2.GetComponent<UISelectPlayer>().PlayerInfo = playerList[i];
            }
            else if (playerList[i].raceType == 4)
            {
                profession3.GetComponent<UISelectPlayer>().PlayerInfo = playerList[i];
            }
            else if (playerList[i].raceType == 5)
            {
                profession4.GetComponent<UISelectPlayer>().PlayerInfo = playerList[i];
            }
        }
    }
    void OnDisable()
    {
        UIEventListener.Get(profession0).onClick -= OnClickProfession;
        UIEventListener.Get(profession1).onClick -= OnClickProfession;
        UIEventListener.Get(profession2).onClick -= OnClickProfession;
        //UIEventListener.Get(profession3).onClick -= OnClickProfession;
        //UIEventListener.Get(profession4).onClick -= OnClickProfession;
        UIEventListener.Get(btnConfirm).onClick -= OnClickButton;
    }
}
