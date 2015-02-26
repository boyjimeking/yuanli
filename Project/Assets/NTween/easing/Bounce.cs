public class Bounce {
    public static NTweenEaseFunction EaseIn = easeIn;
    public static NTweenEaseFunction EaseOut = easeOut;
    public static NTweenEaseFunction EaseInOut = easeInOut;
    private static float easeIn(float t, float b, float c, float d)
    {
		return c - easeOut(d-t, 0f, c, d) + b;
	}
    private static float easeOut(float t, float b, float c, float d)
    {
        if ((t /= d) < (1f / 2.75f))
        {
            return c * (7.5625f * t * t) + b;
        }
        else if (t < (2f / 2.75f))
        {
            return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + b;
        }
        else if (t < (2.5f / 2.75f))
        {
            return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + b;
        }
        else
        {
            return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + b;
        }
    }
    private static float easeInOut(float t, float b, float c, float d)
    {
		if (t < d*0.5f) return easeIn (t*2f, 0f, c, d) * .5f + b;
		else return easeOut (t*2f-d, 0f, c, d) * .5f + c*.5f + b;
	}
}