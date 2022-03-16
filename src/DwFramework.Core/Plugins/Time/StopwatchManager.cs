using System.Collections.Concurrent;

namespace DwFramework.Core.Time;

public static class StopwatchManager
{
    private sealed class Stopwatch
    {
        public string Tag { get; init; }
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="startTime"></param>
        public Stopwatch(string tag, DateTime startTime)
        {
            Tag = tag;
            StartTime = startTime;
        }

        /// <summary>
        /// 设置开始时间
        /// </summary>
        /// <param name="startTime"></param>
        public void SetStartTime(DateTime startTime) => StartTime = startTime;

        /// <summary>
        /// 获取总毫秒
        /// </summary>
        /// <returns></returns>
        public long GetTotalMilliseconds() => (long)(DateTime.UtcNow - StartTime).TotalMilliseconds;

        /// <summary>
        /// 获取总秒
        /// </summary>
        /// <returns></returns>
        public long GetTotalSeconds() => GetTotalMilliseconds() / 1000;
    }

    private static ConcurrentDictionary<string, Stopwatch> _stopwatches = new();

    /// <summary>
    /// 创建计时器
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="startTime"></param>
    /// <returns></returns>
    public static string Create(string? tag = null, DateTime? startTime = null)
    {
        tag ??= Guid.NewGuid().ToString();
        startTime ??= DateTime.UtcNow;
        _stopwatches[tag] = new Stopwatch(tag, startTime.Value);
        return tag;
    }

    /// <summary>
    /// 设置开始时间
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="startTime"></param>
    public static void SetStartTime(string tag, DateTime startTime)
    {
        if (!_stopwatches.ContainsKey(tag)) Create(tag, startTime);
        else _stopwatches[tag].SetStartTime(startTime);
    }

    /// <summary>
    /// 移除计时器
    /// </summary>
    /// <param name="tag"></param>
    public static void Remove(string tag)
    {
        if (!_stopwatches.ContainsKey(tag)) return;
        _stopwatches.TryRemove(tag, out _);
    }

    /// <summary>
    /// 获取总毫秒
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static long GetTotalMilliseconds(string tag)
    {
        if (!_stopwatches.ContainsKey(tag)) return 0;
        return _stopwatches[tag].GetTotalMilliseconds();
    }

    /// <summary>
    /// 获取总秒
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static long GetTotalSeconds(string tag)
    {
        if (!_stopwatches.ContainsKey(tag)) return 0;
        return _stopwatches[tag].GetTotalSeconds();
    }

    /// <summary>
    /// 执行并返回耗时
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static long ExecuteReturnMilliseconds(Action action)
    {
        var tag = Create();
        action?.Invoke();
        var ms = GetTotalMilliseconds(tag);
        Remove(tag);
        return ms;
    }

    /// <summary>
    /// 执行并返回耗时
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<long> ExecuteReturnMillisecondsAsync(Func<Task> action)
    {
        var tag = Create();
        await action.Invoke();
        var ms = GetTotalMilliseconds(tag);
        Remove(tag);
        return ms;
    }

    /// <summary>
    /// 执行并返回耗时
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static long ExecuteReturnSeconds(Action action)
    {
        var tag = Create();
        action.Invoke();
        var sec = GetTotalSeconds(tag);
        Remove(tag);
        return sec;
    }

    /// <summary>
    /// 执行并返回耗时
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<long> ExecuteReturnSecondsAsync(Func<Task> action)
    {
        var tag = Create();
        await action.Invoke();
        var sec = GetTotalSeconds(tag);
        Remove(tag);
        return sec;
    }
}