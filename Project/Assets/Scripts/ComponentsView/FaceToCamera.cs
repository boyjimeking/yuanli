using UnityEngine;
using System.Collections;

public class FaceToCamera : MonoBehaviour
{
    public float zOrder = 0;
    public void Start()
    {
        IsoHelper.FaceToWorldCamera(transform);
        IsoHelper.MoveAlongCamera(transform, zOrder);
    }
}
