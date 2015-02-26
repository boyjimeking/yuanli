using UnityEngine;
using System.Collections;

public class RangeView : MonoBehaviour
{

    private Transform rangeView;
    private Transform blindRangeView;
    private Transform entityTransform;
    public void SetEntity(TileEntity entity)
    {
        gameObject.SetActive(true);
        entityTransform = entity.view.transform;
        SetRange(entity.model.range,entity.model.blindRange);
    }
    private void SetRange(float range,float blindRange)
    {
        if (rangeView == null)
        {
            rangeView = transform.FindChild("Range");
            rangeView.localPosition = IsoHelper.MoveAlongCamera(Vector3.zero, Constants.RANGE_Z_ORDER);
        }
        var rangeScale = range * 0.1f;
        rangeView.localScale = new Vector3(rangeScale, rangeScale, rangeScale);//素材占半径占10格子 r/10
        if (blindRange > 0)
        {
            if (blindRangeView == null)
            {
                blindRangeView = ((GameObject)Instantiate(rangeView.gameObject)).transform;
                blindRangeView.parent = transform;
                blindRangeView.localPosition = IsoHelper.MoveAlongCamera(Vector3.zero, Constants.BLIND_RANGE_Z_ORDER);
                var sprites = blindRangeView.GetComponentsInChildren<tk2dSprite>();
                foreach (var sprite in sprites)
                {
                    sprite.color = new Color(0.8f, 0.2f, 0.2f);
                }
            }
            blindRangeView.gameObject.SetActive(true);
            var blindRangeScale = blindRange * 0.1f;
            blindRangeView.localScale = new Vector3(blindRangeScale, blindRangeScale, blindRangeScale);
        }
        else
        {
            if (blindRangeView != null)
            {
                blindRangeView.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (entityTransform != null)
        {
            transform.position = entityTransform.position;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
