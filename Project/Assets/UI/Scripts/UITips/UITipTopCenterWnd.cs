using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITipTopCenterWnd : UIBaseWnd
{
    public GameObject baseText;
    private List<GameObject> listText = new List<GameObject>();
    protected override void Awake()
    {
        base.Awake();
        this.hasClose = false;
        this.layer = UIMananger.UILayer.UI_TIPS_LAYER;
        NGUITools.AdjustDepth(this.gameObject, UIMananger.UI_TIP);
    }
    public void ShowInfo(string tipStr)
    {
        GameObject text = (GameObject)GameObject.Instantiate(baseText, Vector3.zero, Quaternion.identity);
        text.transform.parent = this.transform;
        text.transform.localScale = new Vector3(1, 1, 1);
        text.SetActive(true);
        text.GetComponent<UILabel>().text = "[FF7B84]" + tipStr + "[-]";
        TweenAlpha tweenAlpha = text.AddComponent<TweenAlpha>();
        tweenAlpha.from = 1.0f;
        tweenAlpha.to = 0.0f;
        tweenAlpha.duration = 5.0f;
        tweenAlpha.delay = 1.0f;
        EventDelegate eventDelegate = new EventDelegate(this, "OnTweenAlphaOver");
        eventDelegate.oneShot = true;
        tweenAlpha.onFinished.Add(eventDelegate);
        tweenAlpha.PlayForward();
        listText.Insert(0, text);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        int index = 0;
        foreach (GameObject tempObj in listText)
        {
            tempObj.transform.localPosition = new Vector3(0, -105 + index * 35, 0);
            index++;
        }
    }
    private void OnTweenAlphaOver()
    {
        GameObject obj = listText[listText.Count - 1];
        listText.RemoveAt(listText.Count - 1);
        GameObject.DestroyImmediate(obj);
    }
}
