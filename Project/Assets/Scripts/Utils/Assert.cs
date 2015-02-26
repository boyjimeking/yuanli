using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class Assert {
	
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Should(bool comparison, string message="Assert failed!")
	{
		if(!comparison)
		{
			UnityEngine.Debug.LogError(message);
			UnityEngine.Debug.Break();
		}
	}
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Fail(string message = "Assert failed!")
    {
        UnityEngine.Debug.LogError(message);
        UnityEngine.Debug.Break();
    }
}
