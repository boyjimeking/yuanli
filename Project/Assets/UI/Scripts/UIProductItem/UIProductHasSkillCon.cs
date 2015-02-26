using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System.Collections.Generic;

public class UIProductHasSkillCon : MonoBehaviour
{
    public GameObject prefabHasSkill;
    public void UpdateHasSkillFrame()
    {
        Clear();
        List<SkillVO> skills = DataCenter.Instance.Defender.skills;
        for (int i = 0, imax = skills.Count; i < imax; i++)
        {
            GameObject tempObj = (GameObject)GameObject.Instantiate(prefabHasSkill, Vector3.zero, Quaternion.identity);
            tempObj.transform.parent = this.transform;
            tempObj.transform.localPosition = new Vector3(-368 + i * (tempObj.GetComponent<UISprite>().width + 20), 0, 0);
            tempObj.transform.localScale = Vector3.one;
            tempObj.SetActive(true);
            EntityModel model = DataCenter.Instance.FindEntityModelById(skills[i].cid);
            if (null == model)
            {
                GameTipsManager.Instance.ShowGameDevelopTips("找不到EntityModel中id为" + skills[i].cid + "的数据");
            }
            tempObj.GetComponent<UIProductHasSkillInfo>().SetHasSkillInfo(ResourceUtil.GetItemIconByModel(model), skills[i].amount);
        }
    }
    public void Clear()
    {
        for (int i = 0, imax = this.transform.childCount; i < imax; i++)
        {
            GameObject.Destroy(this.transform.GetChild(i).gameObject);
        }
    }
}
