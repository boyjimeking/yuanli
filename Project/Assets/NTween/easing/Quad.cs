using System;
using UnityEngine;

public class Quad
{
    public static NTweenEaseFunction EaseIn = easeIn;
    public static NTweenEaseFunction EaseOut = easeOut;
    public static NTweenEaseFunction EaseInOut = easeInOut;
    private static float easeIn(float t, float b, float c, float d)
    {
        return c * (t /= d) * t + b;
	}
    private static float easeOut(float t, float b, float c, float d)
    {
        return -c * (t /= d) * (t - 2f) + b;
    }
    private static float easeInOut(float t, float b, float c, float d)
    {
        if ((t /= d * 0.5f) < 1f) return c * 0.5f * t * t + b;
        return -c * 0.5f * ((--t) * (t - 2f) - 1f) + b;
	}
}