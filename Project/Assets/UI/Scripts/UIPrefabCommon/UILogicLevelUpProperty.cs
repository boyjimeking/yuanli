using UnityEngine;
using System.Collections;

public class UILogicLevelUpProperty : MonoBehaviour
{
    private UIControlProperty controlProperty;
    void OnEnable()
    {
        controlProperty = this.gameObject.GetComponent<UIControlProperty>();
        controlProperty.nextPropertyCon.SetActive(true);
    }
    public void SetPropertyIcon(string iconName)
    {
        controlProperty.iconProperty.spriteName = iconName;
    }
    public void SetPropertyInfo(int curProperty, int nextProperty, int maxProperty, string propertyDes)
    {
        if (nextProperty != -1)
        {
            controlProperty.txtProperty.text = propertyDes + curProperty + "+" + (nextProperty - curProperty);
        }
        controlProperty.curSlider.value = curProperty * 1.0f / maxProperty;
        controlProperty.nextSlider.value = nextProperty * 1.0f / maxProperty;
    }
}
