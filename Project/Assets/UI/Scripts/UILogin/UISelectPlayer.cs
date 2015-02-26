using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.pureland.proto;

public class UISelectPlayer : MonoBehaviour
{
    //角色信息
    public GameObject roleInfoCon;
    //玩家名字
    public UILabel txtPlayerName;
    //玩家等级
    public UILabel txtPlayerLevel;
    //选择框
    public GameObject selectIcon;
    //玩家信息数据
    private PlayerLoginSimpleVO playerInfo;
    public bool IsSelect
    {
        set
        {
            selectIcon.SetActive(value);
        }
    }
    public PlayerLoginSimpleVO PlayerInfo
    {
        set
        {
            playerInfo = value;
            roleInfoCon.SetActive(true);
            txtPlayerName.text = playerInfo.userName;
            txtPlayerLevel.text = playerInfo.userLevel.ToString();
        }
        get
        {
            return this.playerInfo;
        }
    }
}
