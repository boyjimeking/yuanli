using System;
using UnityEngine;

public class Circ {
    public static NTweenEaseFunction EaseIn = easeIn;
    public static NTweenEaseFunction EaseOut = easeOut;
    public static NTweenEaseFunction EaseInOut = easeInOut;
    private static float easeIn(float t, float b, float c, float d)
    {
        return -c * (Mathf.Sqrt(1f - (t /= d) * t) - 1f) + b;
	}
    private static float easeOut(float t, float b, float c, float d)
    {
        return c * Mathf.Sqrt(1f - (t = t / d - 1f) * t) + b;
    }
    private static float easeInOut(float t, float b, float c, float d)
    {
        if ((t /= d * 0.5f) < 1f) return -c * 0.5f * (Mathf.Sqrt(1f - t * t) - 1f) + b;
        return c * 0.5f * (Mathf.Sqrt(1f - (t -= 2f) * t) + 1f) + b;
	}
}