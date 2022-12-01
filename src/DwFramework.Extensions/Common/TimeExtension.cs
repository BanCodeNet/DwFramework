namespace DwFramework.Extensions;

public static class TimeExtension
{
    /// <summary>
    /// 计算时间差
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="isMilliseconds"></param>
    /// <returns></returns>
    public static double GetTimeDifference(this DateTime startTime, DateTime endTime, bool isMilliseconds = false)
    {
        if (isMilliseconds) return (endTime - startTime).TotalMilliseconds;
        else return (endTime - startTime).TotalSeconds;
    }
}