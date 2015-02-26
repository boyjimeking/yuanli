using UnityEngine;
using System.Collections;
using com.pureland.proto;

public class UISearchPVPFrame : MonoBehaviour
{
    public UILabel txtJiFen;
    public UILabel txtSearchConsume;
    public GameObject btnRemote;
    public GameObject btnNear;
    public GameObject btnReturn;
    void OnEnable()
    {
        UIEventListener.Get(btnRemote).onClick += OnClickButton;
        UIEventListener.Get(btnNear).onClick += OnClickButton;
        UIEventListener.Get(btnReturn).onClick += OnClickButton;
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(btnReturn))
        {
            UIMananger.Instance.CloseWin("UISearchPanel");
        }
        else if (go.Equals(btnRemote))
        {
            GameWorld.Instance.ChangeLoading(WorldType.Battle, null, new[] { (int)FightSearchReq.SearchType.PVP_HIGH, -1 });
        }
        else if (go.Equals(btnNear))
        {
            GameWorld.Instance.ChangeLoading(WorldType.Battle, null, new[] { (int)FightSearchReq.SearchType.PVP_NORMAL, -1 });
        }
    }
    void OnDisable()
    {
        UIEventListener.Get(btnRemote).onClick -= OnClickButton;
        UIEventListener.Get(btnNear).onClick -= OnClickButton;
        UIEventListener.Get(btnReturn).onClick -= OnClickButton;
    }
}
