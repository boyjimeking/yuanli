using UnityEngine;
using System.Collections;

/// <summary>
/// component 'BuildArrowView'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/BuildArrowView")]
public class BuildArrowView : MonoBehaviour
{
    public GameObject tl;
    public GameObject tr;
    public GameObject bl;
    public GameObject br;
    public void Init(int size)
    {
//        tl.transform.localPosition = new Vector3(-IsoHelper.TILE_HALF_WIDTH + IsoHelper.TILE_HALF_WIDTH * 0.5f * (size-1),IsoHelper.TILE_HALF_HEIGHT + IsoHelper.TILE_HALF_HEIGHT * 0.5f * (size-1));
//        bl.transform.localPosition = new Vector3(-IsoHelper.TILE_HALF_WIDTH + IsoHelper.TILE_HALF_WIDTH * 0.5f * (size-1),-IsoHelper.TILE_HALF_HEIGHT - IsoHelper.TILE_HALF_HEIGHT * 0.5f * (size-1));
//        tr.transform.localPosition = new Vector3(IsoHelper.TILE_HALF_WIDTH + IsoHelper.TILE_HALF_WIDTH * (size - 1) + IsoHelper.TILE_HALF_WIDTH * 0.5f * (size-1), IsoHelper.TILE_HALF_HEIGHT + IsoHelper.TILE_HALF_HEIGHT * (size - 1) - IsoHelper.TILE_HALF_HEIGHT * 0.5f * (size-1));
//        br.transform.localPosition = new Vector3(IsoHelper.TILE_HALF_WIDTH + IsoHelper.TILE_HALF_WIDTH * (size - 1) + IsoHelper.TILE_HALF_WIDTH * 0.5f * (size-1), -IsoHelper.TILE_HALF_HEIGHT - IsoHelper.TILE_HALF_HEIGHT * (size - 1) + IsoHelper.TILE_HALF_HEIGHT * 0.5f * (size-1));
        var center = size * 0.5f - 0.5f;
        var dist = size * 0.5f + 0.5f;
        tl.transform.localPosition = new Vector3(center,0,center + dist);
        tr.transform.localPosition = new Vector3(center+dist,0,center);
        bl.transform.localPosition = new Vector3(center - dist, 0, center);
        br.transform.localPosition = new Vector3(center,0,center - dist);

        IsoHelper.FaceToWorldCamera(tl.transform);
        IsoHelper.FaceToWorldCamera(bl.transform);
        IsoHelper.FaceToWorldCamera(tr.transform);
        IsoHelper.FaceToWorldCamera(br.transform);
    }
}
