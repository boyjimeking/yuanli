using UnityEngine;
using System.Collections;
public class UIRepeatedButton : UIButton
{
    private bool isRepeated = true;
    protected override void OnPress(bool isPressed)
    {
        base.OnPress(isPressed);
        if (isPressed)
        {
            if (isRepeated)
                this.InvokeRepeating("OnRepeatedOperation", 0.4f, 0.3f);
        }
        else
        {
            this.CancelInvoke();
        }
    }
    protected override void OnDragOut()
    {
        if (isEnabled && (dragHighlight || UICamera.currentTouch.pressed == gameObject))
        {
            this.CancelInvoke();
            base.OnDragOut();
        }
    }
    private void OnRepeatedOperation()
    {
        base.OnClick();
        if (null != this.gameObject.GetComponent<UIPlayAnimation>())
            this.gameObject.GetComponent<UIPlayAnimation>().Play(true, false);
    }
    public bool IsRepeated
    {
        set
        {
            this.isRepeated = value;
        }
    }
}
