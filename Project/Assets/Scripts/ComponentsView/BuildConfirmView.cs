using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// component 'BuilderConfirmView'
/// ADD COMPONENT DESCRIPTION HERE
/// </summary>
[AddComponentMenu("Scripts/BuilderConfirmView")]
public class BuildConfirmView : MonoBehaviour
{
    public event Action<bool> OnConfirmEvent;
    public GameObject left;
    public GameObject right;

    public void Init(int size)
    {
        var offset = 2f;
        var height = size + 1;
        left.transform.localPosition = new Vector3(0, height, offset);
        right.transform.localPosition = new Vector3(offset, height, 0);
        IsoHelper.FaceToWorldCamera(left.transform);
        IsoHelper.FaceToWorldCamera(right.transform);
    }
    public void OnConfirm()
    {
        if (OnConfirmEvent != null)
        {
            OnConfirmEvent(true);
        }
    }

    public void OnCancel()
    {
        if (OnConfirmEvent != null)
        {
            OnConfirmEvent(false);
        }
    }
}
