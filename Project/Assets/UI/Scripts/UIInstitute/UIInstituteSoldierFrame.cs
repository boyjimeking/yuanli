using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class UIInstituteSoldierFrame : MonoBehaviour
{
    public GameObject btnReturn;
    //全部的兵种展示容器
    public GameObject allSoldierCon;
    //可选升级容
    public Transform soldierStoreArea;
    //可选升级物品
    public GameObject prefabSoldierItem;
    //兵种详细信息
    private GameObject soldierDes;
    //key:type value:GameObject
    private Dictionary<string, GameObject> dicSoldier = new Dictionary<string, GameObject>();
    void OnEnable()
    {
        UIEventListener.Get(btnReturn).onClick += OnClickButton;
        EventDispather.AddEventListener(GameEvents.SOLDIER_COUNT_CHANGE, UpdateInstituteSoldierInfo);
        EventDispather.AddEventListener(GameEvents.SOLDIER_UP, UpdateInstituteSoldierInfo);
    }
    void OnDisable()
    {
        UIEventListener.Get(btnReturn).onClick -= OnClickButton;
        EventDispather.RemoveEventListener(GameEvents.SOLDIER_COUNT_CHANGE, UpdateInstituteSoldierInfo);
        EventDispather.RemoveEventListener(GameEvents.SOLDIER_UP, UpdateInstituteSoldierInfo);
    }
    private void UpdateInstituteSoldierInfo(string eventType, object obj)
    {
        if (eventType == GameEvents.SOLDIER_COUNT_CHANGE)
        {
            dicSoldier[DataCenter.Instance.FindEntityModelById((int)obj).subType].GetComponent<UIInstituteItem>().SetSoldierCount();
        }
        else if (eventType == GameEvents.SOLDIER_UP)
        {
            UpdateSoldierFrame();
        }
    }
    public void UpdateSoldierFrame()
    {
        foreach (KeyValuePair<string, GameObject> keyValuePair in dicSoldier)
        {
            GameObject.Destroy(keyValuePair.Value);
        }
        dicSoldier.Clear();
        PlayerVO playerVO = DataCenter.Instance.Defender.player;
        int index = 0;
        foreach (var armyExpVo in playerVO.armyShop)
        {
            GameObject tempObj = (GameObject)GameObject.Instantiate(prefabSoldierItem, Vector3.zero, Quaternion.identity);
            tempObj.transform.parent = soldierStoreArea;
            tempObj.transform.localScale = Vector3.one;
            tempObj.transform.localPosition = new Vector3(-347 + Mathf.Floor(index / 2) * (tempObj.GetComponent<UISprite>().width + 20), 125 - (index % 2) * (tempObj.GetComponent<UISprite>().height + 80), 0);
            tempObj.SetActive(true);
            tempObj.GetComponent<UIDragScrollView>().scrollView = soldierStoreArea.GetComponent<UIScrollView>();
            tempObj.GetComponent<UIInstituteItem>().SetItemInfo(armyExpVo.cid);
            UIEventListener.Get(tempObj).onClick += OnClickButton;
            dicSoldier.Add(DataCenter.Instance.FindEntityModelById(armyExpVo.cid).subType, tempObj);
            index++;
        }
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnReturn))
        {
            allSoldierCon.SetActive(true);
            btnReturn.SetActive(false);
            if (soldierDes)
            {
                soldierDes.SetActive(false);
            }
        }
        else
        {
            allSoldierCon.SetActive(false);
            btnReturn.SetActive(true);
            if (null == soldierDes)
            {
                soldierDes = (GameObject)ResourceManager.Instance.LoadAndCreate("UI/PLG_Common/SoldierDes");
                soldierDes.transform.parent = this.transform;
                soldierDes.transform.localScale = Vector3.one;
            }
            soldierDes.SetActive(true);
            soldierDes.AddMissingComponent<UILogicSoldierDes>().SetSoldierInfo(go.GetComponent<UIInstituteItem>().itemId);
        }
    }
    public void ClearSoldierFrame()
    {
        foreach (KeyValuePair<string, GameObject> keyValuePair in dicSoldier)
        {
            GameObject.Destroy(keyValuePair.Value);
        }
        dicSoldier.Clear();
        if (soldierDes)
        {
            GameObject.Destroy(soldierDes);
        }
        UIEventListener.Get(btnReturn).onClick -= OnClickButton;
    }
}
