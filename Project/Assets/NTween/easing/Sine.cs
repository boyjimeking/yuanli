using System;
using UnityEngine;

public class Sine
{
    public static NTweenEaseFunction EaseIn = easeIn;
    public static NTweenEaseFunction EaseOut = easeOut;
    public static NTweenEaseFunction EaseInOut = easeInOut;
    private const float _HALF_PI = Mathf.PI * 0.5f;
    private static float easeIn(float t, float b, float c, float d)
    {
        return -c * Mathf.Cos(t / d * _HALF_PI) + c + b;
    }
    private static float easeOut(float t, float b, float c, float d)
    {
        return c * Mathf.Sin(t / d * _HALF_PI) + b;
	}
    private static float easeInOut(float t, float b, float c, float d)
    {
        return -c * 0.5f * (Mathf.Cos(Mathf.PI * t / d) - 1f) + b;
	}
}