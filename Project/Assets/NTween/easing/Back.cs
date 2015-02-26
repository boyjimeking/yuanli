public class Back
{
    public static NTweenEaseFunction EaseIn = easeIn;
    public static NTweenEaseFunction EaseOut = easeOut;
    public static NTweenEaseFunction EaseInOut = easeInOut;
    
    private static float easeIn (float t, float b, float c, float d)
    {
        float s = 1.70158f;
	    return c*(t/=d)*t*((s+1f)*t - s) + b;
    }
    private static float easeOut(float t, float b, float c, float d)
    {
        float s = 1.70158f;
	    return c*((t=t/d-1f)*t*((s+1)*t + s) + 1f) + b;
    }
    private static float easeInOut(float t, float b, float c, float d)
    {
        float s = 1.70158f;
	    if ((t/=d*0.5f) < 1f) return c*0.5f*(t*t*(((s*=(1.525f))+1f)*t - s)) + b;
	    return c/2f*((t-=2f)*t*(((s*=(1.525f))+1f)*t + s) + 2f) + b;
    }
}