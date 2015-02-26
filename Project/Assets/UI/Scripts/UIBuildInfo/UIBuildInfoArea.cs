using UnityEngine;
using System.Collections;

public class UIBuildInfoArea : MonoBehaviour
{
    //建筑的描述
    public UILabel txtBuildDes;
    //兵营或者是联盟的现有兵种容器
    public GameObject hasSoldierCon;
    public void SetBuildingInfo(TileEntity tileEntity)
    {
        if (EntityTypeUtil.IsBarracks(tileEntity.model))
        {

        }
        txtBuildDes.text = "这是个" + tileEntity.model.nameForView;
    }
}
