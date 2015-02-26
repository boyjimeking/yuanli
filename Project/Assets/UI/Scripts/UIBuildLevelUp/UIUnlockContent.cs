using UnityEngine;
using System.Collections;

public class UIUnlockContent : MonoBehaviour
{
    public UILabel txtUnlockCount;
    //建筑模型
    private GameObject buildModel;
    public void SetUnlockInfo(int buildId, int count)
    {
        txtUnlockCount.text = "X" + count;
        buildModel = (GameObject)TileEntity.LoadAndCreate(ModelUtil.GetEntityModel(buildId));
        if (buildModel != null)
        {
            var view = buildModel.AddMissingComponent<EntityViewComponent>();
            view.Init(false);
            buildModel.transform.parent = this.transform;
            buildModel.SetLayerRecursively(LayerMask.NameToLayer("UILayerMiddle"));
            buildModel.transform.localScale = new Vector3(15, 15, 1);
            buildModel.transform.localPosition = new Vector3(10, -10, 0);
        }
    }
}
