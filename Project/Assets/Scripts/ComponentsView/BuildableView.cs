using UnityEngine;
using System.Collections;

/// <summary>
/// component 'BuildableView'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/BuildableView")]
public class BuildableView : MonoBehaviour
{
    public GameObject smallTilePrefab;

    public void Init(int size)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var tile = Instantiate(smallTilePrefab, IsoHelper.GridToPosition(i, j), Quaternion.identity) as GameObject;

                tile.transform.position = IsoHelper.GridToPosition(i, j);
                var localPos = tile.transform.localPosition;
                tile.transform.parent = transform;
                tile.transform.localPosition = localPos;

                IsoHelper.FaceToWorldCamera(tile.transform);
            }
        }
    }

    public void SetBuildable(bool buildable)
    {
        Color drawColor = buildable ? Color.green : Color.red;

        var sprites = GetComponentsInChildren<tk2dSprite>();
        foreach (var sprite in sprites)
        {
            sprite.color = drawColor;
        }
    }
}
