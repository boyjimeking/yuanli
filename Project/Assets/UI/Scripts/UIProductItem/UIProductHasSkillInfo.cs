using UnityEngine;
using System.Collections;

public class UIProductHasSkillInfo : MonoBehaviour
{
    public UISprite skillIcon;
    public UILabel txtSkillCount;
    public void SetHasSkillInfo(string iconName, int count)
    {
        skillIcon.spriteName = iconName;
        txtSkillCount.text = "X" + count;
    }
}
