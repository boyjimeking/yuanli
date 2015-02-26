using System;
using UnityEngine;
using System.Collections;

public class EffectTrigger : MonoBehaviour
{
    public float delay;
    public event Action<EffectTrigger> TriggeredEvent;
    public event Action<EffectTrigger> CompleteEvent;
    private float duration;

    void OnEnable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        var effectsComp = GetComponentsInChildren<EffectComp>();
        duration = 0;
        foreach (var effectComp in effectsComp)
        {
            duration = Mathf.Max(duration, effectComp.delay + effectComp.life);
        }
        CoroutineHelper.Run(Coroutine_Delay());

        //  init sub
        foreach (var effectComp in effectsComp)
            effectComp.Init();
    }

    private void Trigger()
    {
        if (TriggeredEvent != null)
        {
            TriggeredEvent(this);
        }
    }

    private void Complete()
    {
        if (CompleteEvent != null)
        {
            CompleteEvent(this);
        }
    }
    private IEnumerator Coroutine_Delay()
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        Trigger();
        yield return new WaitForSeconds(duration - delay);//已经等待了delay的咏唱时间
        Complete();
    }
}
