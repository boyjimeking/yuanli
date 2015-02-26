using UnityEngine;
using System.Collections;

public class DelayAnimation : MonoBehaviour
{
    public float delay;
	// Use this for initialization
	void Start () {
	    if (delay > 0)
	    {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<tk2dSpriteAnimator>().enabled = false;
	        StartCoroutine(Coroutine_Delay());
	    }
	}

    private IEnumerator Coroutine_Delay()
    {
        yield return new WaitForSeconds(delay);
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<tk2dSpriteAnimator>().enabled = true;
    }
}
