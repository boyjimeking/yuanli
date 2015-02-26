using System;
using UnityEngine;

public class Linear
{
    public static NTweenEaseFunction EaseIn = easeIn;
    public static NTweenEaseFunction EaseOut = easeOut;
    public static NTweenEaseFunction EaseInOut = easeInOut;
    private static float easeIn(float t, float b, float c, float d)
    {
        return c * t / d + b;
	}
    private static float easeOut(float t, float b, float c, float d)
    {
        return c * t / d + b;
    }
    private static float easeInOut(float t, float b, float c, float d)
    {
        return c * t / d + b;
	}
}