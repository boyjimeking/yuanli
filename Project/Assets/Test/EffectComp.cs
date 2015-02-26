using UnityEngine;
using System.Collections;

public class EffectComp : MonoBehaviour {
    public float delay;
    public float life;

    /// <summary>
    /// 用该函数代替OnEnable（因为OnEnable会在gameObject.SetActive(xxx);时被调用（导致错乱）
    /// </summary>
    public void Init()
    {
        CoroutineHelper.Run(Coroutine_Delay());
    }

    private IEnumerator Coroutine_Delay()
    {
        if (delay > 0)
        {
            gameObject.SetActive(false);
            yield return new WaitForSeconds(delay);
        }
        gameObject.SetActive(true);
        if (life > 0)
        {
            yield return new WaitForSeconds(life);
            gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    void OnDestroy()
    {
        Debug.Log("OnDestroy：" + gameObject.name);
    }
#endif  //  UNITY_EDITOR

}
