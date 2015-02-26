using UnityEngine;
using System.Collections;

public class CoroutineExtend {
	public static IEnumerator WaitForFrames(int frame)
	{
		while(frame-- > 0)
		{
			yield return 0;
		}
	}
}
