using System;
using UnityEngine;

public class Expo
{
    public static NTweenEaseFunction EaseIn = easeIn;
    public static NTweenEaseFunction EaseOut = easeOut;
    public static NTweenEaseFunction EaseInOut = easeInOut;
    private static float easeIn(float t, float b, float c, float d)
    {
        return (t == 0f) ? b : c * Mathf.Pow(2f, 10f * (t / d - 1f)) + b - c * 0.001f;
    }
    private static float easeOut(float t, float b, float c, float d)
    {
        return (t == d) ? b + c : c * (-Mathf.Pow(2f, -10f * t / d) + 1f) + b;
	}
    private static float easeInOut(float t, float b, float c, float d)
    {
        if (t == 0f) return b;
        if (t == d) return b + c;
        if ((t /= d * 0.5f) < 1f) return c * 0.5f * Mathf.Pow(2f, 10f * (t - 1f)) + b;
        return c * 0.5f * (-Mathf.Pow(2f, -10f * --t) + 2f) + b;
	}
}