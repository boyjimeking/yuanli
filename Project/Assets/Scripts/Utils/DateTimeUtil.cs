
using System;
using Org.BouncyCastle.Utilities;

public class DateTimeUtil
{
    public static DateTime _1970 = new DateTime(1970, 1, 1, 0, 0, 0);

    public static int DateTimeToUnixTimestamp(DateTime dateTime)
    {
        TimeSpan span = (TimeSpan)(dateTime - _1970);
        return (int)span.TotalSeconds;
    }
    public static DateTime UnixTimestampToDateTime(long unixTimestamp)
    {
        return _1970.AddSeconds((double)unixTimestamp);
    }
    public static long DateTimeToUnixTimestampMS(DateTime dateTime)
    {
        TimeSpan span = (TimeSpan)(dateTime - _1970);
        return (long)span.TotalMilliseconds;
    }
    public static DateTime UnixTimestampMSToDateTime(long unixTimestamp)
    {
        return _1970.AddMilliseconds((double)unixTimestamp);
    }
    /// <summary>
    /// 获取友好时间显示(不显示为0的值)
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static string PrettyFormatTimeSpanNoZero(TimeSpan timeSpan)
    {
        var days = timeSpan.Days;
        var hours = timeSpan.Hours;
        var minutes = timeSpan.Minutes;
        var seconds = timeSpan.Seconds;
        if (days > 0)
        {
            return days + "天" + (hours > 0 ? (hours + "小时") : "");
        }
        if (hours > 0)
        {
            return hours + "小时" + (minutes > 0 ? (minutes + "分钟") : "");
        }
        if (minutes > 0)
        {
            return minutes + "分钟" + (seconds > 0 ? (seconds + "秒") : "");
        }
        return seconds + "秒";//TODO 本地化
    }
    /// <summary>
    /// 获取友好时间显示(显示为0的值)
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static string PrettyFormatTimeSpan(TimeSpan timeSpan)
    {
        var days = timeSpan.Days;
        var hours = timeSpan.Hours;
        var minutes = timeSpan.Minutes;
        var seconds = timeSpan.Seconds;
        if (days > 0)
        {
            return days + "天" + hours + "小时";
        }
        if (hours > 0)
        {
            return hours + "小时" + minutes + "分钟";
        }
        if (minutes > 0)
        {
            return minutes + "分钟" + seconds + "秒";
        }
        return seconds + "秒";//TODO 本地化
    }
    /// <summary>
    /// 1、带0 2、不带0
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string PrettyFormatTimeSeconds(int seconds, int type = 1)
    {
        if (1 == type)
            return PrettyFormatTimeSpan(new TimeSpan(0, 0, seconds));
        else if (2 == type)
            return PrettyFormatTimeSpanNoZero(new TimeSpan(0, 0, seconds));
        return "";
    }
}
