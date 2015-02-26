using UnityEngine;
using System.Collections;

/// <summary>
/// component 'BuildFenceView'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/BuildFenceView")]
public class BuildFenceView : MonoBehaviour
{

    public tk2dSpriteCollection spriteCollection;

    void Start()
    {
        Init(3);
    }
    public void Init(int size)
    {
        //left fence
        for (int i = 0; i < size; i++)
        {
            var go = new GameObject("fence");
            var sprite = go.AddComponent<tk2dSprite>();
            sprite.SetSprite(spriteCollection.spriteCollection, "buildFence");
            sprite.transform.parent = transform;
            sprite.transform.localPosition = new Vector3(i * 2,0,0);
            IsoHelper.FaceToWorldCamera(sprite.transform);
        }
        //right fence
        for (int i = 1; i < size; i++)
        {
            var go = new GameObject("fence");
            var sprite = go.AddComponent<tk2dSprite>();
            sprite.SetSprite(spriteCollection.spriteCollection, "buildFence");
            sprite.transform.parent = transform;
            sprite.transform.localPosition = new Vector3(0, 0, i * 2);
            IsoHelper.FaceToWorldCamera(sprite.transform);
        }
        //lines
        for (int i = 0; i < size-1; i++)
        {
            var go = new GameObject("fenceLineLeft");
            var sprite = go.AddComponent<tk2dSprite>();
            sprite.SetSprite(spriteCollection.spriteCollection, "buildFenceLeft");
            sprite.transform.parent = transform;
            sprite.transform.localPosition = new Vector3(0, 0,i*2);
            IsoHelper.FaceToWorldCamera(sprite.transform);

            var goRight = new GameObject("fenceLineRight");
            var spriteRight = goRight.AddComponent<tk2dSprite>();
            spriteRight.SetSprite(spriteCollection.spriteCollection, "buildFenceRight");
            spriteRight.transform.parent = transform;
            spriteRight.transform.localPosition = new Vector3(i*2, 0, 0);
            IsoHelper.FaceToWorldCamera(sprite.transform);
        }
    }
}
