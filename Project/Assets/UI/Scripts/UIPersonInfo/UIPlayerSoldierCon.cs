using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPlayerSoldierCon : MonoBehaviour
{
    public Transform soldierCon;
    public void SetSoldierData(List<int> armys)
    {
        armys = PlayerManager.Instance.GetSortedArmys(armys);
        for (int i = 0; i < armys.Count; i++)
        {
            GameObject tempObj = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/FightItem");
            tempObj.transform.parent = this.soldierCon;
            tempObj.transform.localScale = Vector3.one;
            tempObj.transform.localPosition = new Vector3(-349 + i * (tempObj.GetComponent<UISprite>().width + 10), 0, 0);
            tempObj.SetActive(true);
            tempObj.GetComponent<UIDragScrollView>().scrollView = soldierCon.GetComponent<UIScrollView>();
            tempObj.AddMissingComponent<UIPlayerItemInfo>().SetItemInfo(armys[i]);
        }
    }
}
