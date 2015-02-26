using UnityEngine;
using System.Collections;

public class UIHonorWnd : UIBaseWnd
{
    public Transform hornorContentArea;
    public GameObject honorContent;
    public UILabel txtHonorCount;
    public UILabel txtHonorName;
    public UILabel txtHonorDes;
    public GameObject btnCollectReward;
    public GameObject btnChallengeFriend;
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = true;
        this.isLockScreen = true;
        this.layer = UIMananger.UILayer.UI_NORMAL_LAYER;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnCollectReward).onClick += OnClickButton;
        UIEventListener.Get(btnChallengeFriend).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnCollectReward))
        {

        }
        else if (go.Equals(btnChallengeFriend))
        {

        }
        else
        {

        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnCollectReward).onClick -= OnClickButton;
        UIEventListener.Get(btnChallengeFriend).onClick -= OnClickButton;
        for (int i = 0, imax = hornorContentArea.childCount; i < imax; i++)
        {
            GameObject tempObj = hornorContentArea.GetChild(i).gameObject;
            UIEventListener.Get(tempObj).onClick -= OnClickButton;
            GameObject.Destroy(tempObj);
        }
    }
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject tempObj = (GameObject)GameObject.Instantiate(honorContent, Vector3.zero, Quaternion.identity);
            tempObj.transform.parent = hornorContentArea;
            tempObj.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
            tempObj.transform.localPosition = new Vector3(-315 + Mathf.Floor(i / 2) * (tempObj.GetComponent<UISprite>().width + 10), 112 - (i % 2) * (tempObj.GetComponent<UISprite>().height + 10), 0);
            tempObj.SetActive(true);
            UIEventListener.Get(tempObj).onClick += OnClickButton;
        }
    }
}
