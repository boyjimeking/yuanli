using System;

public class ServerTime : Singleton<ServerTime>
{
    public long ServerTimeOffsetMS { get; private set; }

    /// <summary>
    /// 获取当前服务器时间
    /// </summary>
    /// <returns></returns>
    public long GetTimestamp(long offsetSeconds=0)
    {
        return DateTimeUtil.DateTimeToUnixTimestampMS(Now().AddSeconds(offsetSeconds));
    }
    
    /// <summary>
    /// 设置当前服务器时间
    /// </summary>
    /// <param name="serverUnixTimestampMS"></param>
    public void SetTimestamp(long serverUnixTimestampMS)
    {
        long timestampMS = DateTimeUtil.DateTimeToUnixTimestampMS(DateTime.UtcNow);
        ServerTimeOffsetMS = serverUnixTimestampMS - timestampMS;
    }
    public DateTime Now()
    {
        return LocalToServerTime(DateTime.UtcNow);
    }

    public DateTime LocalToServerTime(DateTime localTime)
    {
        return localTime.AddMilliseconds(ServerTimeOffsetMS);
    }

    public DateTime ServerToLocalTime(DateTime serverTime)
    {
        return serverTime.AddMilliseconds(-ServerTimeOffsetMS);
    }
}

