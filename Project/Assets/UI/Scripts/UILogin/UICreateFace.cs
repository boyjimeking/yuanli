using UnityEngine;
using System.Collections;

public class UICreateFace : MonoBehaviour
{
    public UILabel txtPlayerName;
    public GameObject btnConfirm;
    public GameObject btnComplete;
    public GameObject headIcon;
    private GameObject curSelectIcon;
    public GameObject btnReturn;
    void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            GameObject tempObj = (GameObject)GameObject.Instantiate(headIcon, Vector3.zero, Quaternion.identity);
            tempObj.transform.parent = this.transform;
            tempObj.transform.localScale = new Vector3(1, 1, 1);
            tempObj.transform.localPosition = new Vector3(-322 + (i % 6) * (tempObj.GetComponent<UISprite>().width + 15), 100 - Mathf.Floor(i / 6) * (tempObj.GetComponent<UISprite>().height + 30));
            tempObj.SetActive(true);
            tempObj.GetComponent<UISprite>().color = Color.black;
            tempObj.transform.Find("personIcon").GetComponent<UISprite>().spriteName = "UI_Create_face_0" + (i + 1);
            UIEventListener.Get(tempObj).onClick += OnClickHeadIcon;
            if (i == 0)
            {
                OnClickHeadIcon(tempObj);
            }
        }
    }

    private void OnClickHeadIcon(GameObject go)
    {
        if (curSelectIcon)
        {
            if (go.Equals(curSelectIcon))
            {
                return;
            }
            curSelectIcon.GetComponent<UISprite>().color = Color.black;
        }
        go.GetComponent<UISprite>().color = Color.white;
        curSelectIcon = go;
    }
    void OnEnable()
    {
        UIEventListener.Get(btnConfirm).onClick += OnClickButton;
        UIEventListener.Get(btnComplete).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnConfirm))
        {
            if (txtPlayerName.text == "")
                return;
            if (null == curSelectIcon)
                return;
            UIMananger.Instance.CloseWin("UIStartPanel");
        }
    }
    void OnDisable()
    {
        UIEventListener.Get(btnConfirm).onClick -= OnClickButton;
        UIEventListener.Get(btnComplete).onClick -= OnClickButton;
    }
}
