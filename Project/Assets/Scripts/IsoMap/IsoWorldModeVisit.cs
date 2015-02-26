
using UnityEngine;

public class IsoWorldModeVisit : IsoWorldMode
{
    private TileEntity currentBuilding;
    protected override void OnTap(Vector2 screenPosition)
    {
        var building = IsoMap.Instance.GetBuildingAtScreenPoint(screenPosition);
        var prevSelected = currentBuilding;
        if (currentBuilding != null)
        {
            currentBuilding.OnUnselected();
            currentBuilding = null;
        }
        if (prevSelected != building && building != null)
        {
            building.OnSelected();
            currentBuilding = building;
        }
    }
}