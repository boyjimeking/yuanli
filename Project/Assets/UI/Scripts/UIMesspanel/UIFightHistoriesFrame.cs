using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class UIFightHistoriesFrame : MonoBehaviour
{
    public int historyType = 0;
    public GameObject prefabMailContentFrame;
    void OnEnable()
    {
        EventDispather.AddEventListener(GameEvents.BATTLE_HISTORY_LOADED, OnUpDateMailFrame);
        OnUpDateMailFrame(null, null);
    }
    private void OnUpDateMailFrame(string eventType, object obj)
    {
        if (historyType == 0)
        {
            RefreshMail(prefabMailContentFrame, DataCenter.Instance.DefenseHistories);
        }
        else if (historyType == 1)
        {
            RefreshMail(prefabMailContentFrame, DataCenter.Instance.AttackHistories);
        }
    }
    private void RefreshMail(GameObject mailContent, List<BattleResultVO> histories)
    {
        if (histories == null)
            return;
        if (this.transform.childCount > 0)
            return;
        for (int i = 0; i < histories.Count; i++)
        {
            GameObject tempObj = (GameObject)GameObject.Instantiate(prefabMailContentFrame, Vector3.zero, Quaternion.identity);
            tempObj.transform.parent = this.transform;
            tempObj.transform.localScale = new Vector3(1, 1, 1);
            tempObj.transform.localPosition = new Vector3(0, 190 - i * (tempObj.GetComponent<UISprite>().height + 20));
            NGUITools.SetActive(tempObj, true);
            tempObj.GetComponent<UIMailContentFrame>().SetMailInfo(histories[i], historyType);
            tempObj.GetComponent<UIDragScrollView>().scrollView = this.gameObject.GetComponent<UIScrollView>();
        }
    }
    void OnDisable()
    {
        EventDispather.RemoveEventListener(GameEvents.BATTLE_HISTORY_LOADED, OnUpDateMailFrame);
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject.Destroy(this.transform.GetChild(i).gameObject);
        }
        UIScrollView.list.Remove(this.GetComponent<UIScrollView>());
        this.transform.localPosition = new Vector3(0, 6, 0);
        this.GetComponent<UIPanel>().clipOffset = new Vector2(0, 0);
    }
}
