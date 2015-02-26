using UnityEngine;
using System.Collections;

public class UIZhuanChangWnd : UIBaseWnd
{
    protected override void Awake()
    {
        base.Awake();
        this.layer = UIMananger.UILayer.UI_ZHUANCHANG_LAYER;
    }
    public void Close()
    {
        StopAllCoroutines();
        var animator = GetComponentInChildren<Animator>();
        animator.SetBool("reverse", false);
    }

    public void Open()
    {
        var animator = GetComponentInChildren<Animator>();
        animator.SetBool("reverse", true);
        StartCoroutine(Coroutine_WaitToHide(animator));
    }

    private IEnumerator Coroutine_WaitToHide(Animator animator)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }
}
