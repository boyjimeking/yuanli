using UnityEngine;
using System.Collections;

public class UILogicSoldierDes : MonoBehaviour
{
    private UIControlSoldierDes controlSoldierDes;
    void OnEnable()
    {
        controlSoldierDes = this.gameObject.GetComponent<UIControlSoldierDes>();
    }
    public void SetSoldierInfo(int soldierId)
    {
        EntityModel model = DataCenter.Instance.FindEntityModelById(soldierId);

    }
}
