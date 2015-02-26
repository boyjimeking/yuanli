using UnityEngine;
using System.Collections;
using com.pureland.proto;
using System;

public class UILogicSkillDes : MonoBehaviour
{
    private UIControlSkillDes controlSkillDes;
    private UIControlResourceIcon controlResRightOver;
    private UIControlResourceIcon controlResLevel;
    public event Action<int> OnClickRightOver;
    public event Action<int> OnClickLevelUp;
    private int skillId;
    void OnEnable()
    {
        controlSkillDes = this.gameObject.GetComponent<UIControlSkillDes>();
    }
    void OnDisable()
    {
        if (controlSkillDes)
        {
            UIEventListener.Get(controlSkillDes.btnRightOver).onClick -= OnClickButton;
            UIEventListener.Get(controlSkillDes.btnLevel).onClick -= OnClickButton;
        }
        OnClickRightOver = null;
        OnClickLevelUp = null;
    }
    public void SetSoldierInfo(int skillId, bool isLevel)
    {
        this.skillId = skillId;
        EntityModel model = DataCenter.Instance.FindEntityModelById(skillId);
        EntityModel nextModel = null;
        if (model.upgradeId != 0)
        {
            nextModel = DataCenter.Instance.FindEntityModelById(model.upgradeId);
        }
        if (isLevel)
        {
            controlSkillDes.rightOverCon.SetActive(true);
            controlResRightOver = controlSkillDes.rightOverConsume.GetComponent<UIControlResourceIcon>();
            controlResRightOver.txtResourceCount.text = GameDataAlgorithm.TimeToGem(nextModel.buildTime).ToString();
            UIEventListener.Get(controlSkillDes.btnRightOver).onClick += OnClickButton;
            controlResLevel = controlSkillDes.levelConsume.GetComponent<UIControlResourceIcon>();
            controlResLevel.iconResource.spriteName = nextModel.costResourceType.ToString();
            int myResCount = DataCenter.Instance.GetResource(nextModel.costResourceType);
            if (myResCount > nextModel.costResourceCount)
                controlResLevel.txtResourceCount.text = nextModel.costResourceCount.ToString();
            else
                controlResLevel.txtResourceCount.text = (myResCount - nextModel.costResourceCount).ToString();
            controlSkillDes.btnLevel.transform.Find("Animation/btnText").GetComponent<UILabel>().text = "升级:\n" + DateTimeUtil.PrettyFormatTimeSeconds(nextModel.buildTime);
            UIEventListener.Get(controlSkillDes.btnLevel).onClick += OnClickButton;
        }
    }

    private void OnClickButton(GameObject go)
    {
        if (go.Equals(controlSkillDes.btnRightOver))
        {
            if (null != OnClickRightOver)
                OnClickRightOver(skillId);
        }
        else if (go.Equals(controlSkillDes.btnLevel))
        {
            if (null != OnClickLevelUp)
                OnClickLevelUp(skillId);
        }
    }
}
