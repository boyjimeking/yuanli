using UnityEngine;
using System.Collections;

public class ChangeAnimation : MonoBehaviour
{
    public float changeTime = 1f;
    private float lastChangeTime = 0;
    private int currentClipIndex;
    private tk2dSpriteAnimator animator;
	// Use this for initialization
	void Start ()
	{
	    animator = GetComponent<tk2dSpriteAnimator>();
	    StartCoroutine(LateStart());
	}

    private IEnumerator LateStart()
    {
        yield return 0;
        for (int i = 0; i < animator.Library.clips.Length; i++)
        {
            if (animator.Library.clips[i] == animator.CurrentClip)
            {
                currentClipIndex = i;
                break;
            }
        }
    }

    // Update is called once per frame
	void Update ()
	{
	    lastChangeTime += Time.deltaTime;
	    if (lastChangeTime > changeTime)
	    {
	        lastChangeTime -= changeTime;
	        var i = ++currentClipIndex % animator.Library.clips.Length;
	        animator.Play(animator.Library.clips[i]);
	    }
	}
}
