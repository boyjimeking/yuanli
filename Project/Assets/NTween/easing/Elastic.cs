using UnityEngine;

public class Elastic
{
    private const float _2PI = Mathf.PI * 2;
    public static NTweenEaseFunction EaseIn = easeIn;
    public static NTweenEaseFunction EaseOut = easeOut;
    public static NTweenEaseFunction EaseInOut = easeInOut;
    
    private static float easeIn (float t, float b, float c, float d)
    {
        return easeIn(t, b, c, d,0,0);
    }
    private static float easeOut (float t, float b, float c, float d)
    {
        return easeOut(t, b, c, d,0,0);
    }
    private static float easeInOut (float t, float b, float c, float d)
    {
        return easeInOut(t, b, c, d,0,0);
    }
    private static float easeIn (float t, float b, float c, float d,float a,float p)
    {
        float s;
		if (t==0f) return b;  if ((t/=d)==1f) return b+c;  if (p==0) p=d*.3f;
		if (a==0 || (c > 0f && a < c) || (c < 0f && a < -c)) { a=c; s = p/4f; }
		else s = p/_2PI * Mathf.Asin (c/a);
		return -(a*Mathf.Pow(2f,10f*(t-=1f)) * Mathf.Sin( (t*d-s)*_2PI/p )) + b;
    }
    private static float easeOut(float t, float b, float c, float d,float a,float p)
    {
        float s;
		if (t==0f) return b;  if ((t/=d)==1f) return b+c;  if (p==0) p=d*.3f;
		if (a==0 || (c > 0f && a < c) || (c < 0f && a < -c)) { a=c; s = p/4f; }
		else s = p/_2PI * Mathf.Asin (c/a);
		return (a*Mathf.Pow(2f,-10f*t) * Mathf.Sin( (t*d-s)*_2PI/p ) + c + b);
    }
    private static float easeInOut(float t, float b, float c, float d, float a, float p)
    {
        float s;
		if (t==0f) return b;  if ((t/=d*0.5f)==2f) return b+c;  if (p==0) p=d*(.3f*1.5f);
		if (a==0 || (c > 0f && a < c) || (c < 0f && a < -c)) { a=c; s = p/4f; }
		else s = p/_2PI * Mathf.Asin (c/a);
		if (t < 1f) return -.5f*(a*Mathf.Pow(2f,10f*(t-=1f)) * Mathf.Sin( (t*d-s)*_2PI/p )) + b;
		return a*Mathf.Pow(2f,-10f*(t-=1f)) * Mathf.Sin( (t*d-s)*_2PI/p )*.5f + c + b;
    }	
}
