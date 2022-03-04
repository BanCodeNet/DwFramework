namespace DwFramework.Core.Generator;

public sealed class SnowflakeGenerator
{
    public record SnowflakeIdInfo
    {
        public long WorkerId { get; init; }
        public DateTime StartTime { get; init; }
        public long Timestamp { get; init; }
        public DateTime Time => DateTime.UnixEpoch.AddSeconds(Timestamp);
        public long WorkId { get; init; }
        public long Sequence { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        public SnowflakeIdInfo(long id, DateTime startTime)
        {
            if (id <= 0) throw new ExceptionBase(ExceptionType.Parameter, 0, "workerId必须大于0");
            WorkerId = id;
            StartTime = startTime;
            var timestamp = id >> (WORKER_ID_BITS + SEQUENCE_BITS);
            Timestamp = timestamp + (long)DateTime.UnixEpoch.GetTimeDiff(startTime);
            WorkId = (id ^ (timestamp << (WORKER_ID_BITS + SEQUENCE_BITS))) >> SEQUENCE_BITS;
            Sequence = id ^ (timestamp << (WORKER_ID_BITS + SEQUENCE_BITS) | WorkId << SEQUENCE_BITS);
        }
    }

    private readonly object _lock = new object();
    private long _currentTimestamp;
    private long _currentSequence;

    public const int TIMESTAMP_BITS = 41;
    public const int WORKER_ID_BITS = 10;
    public const int SEQUENCE_BITS = 12;
    public const long MAX_TIMESTAMP = 2L << TIMESTAMP_BITS;
    public const long MAX_WORKER_ID = (2L << WORKER_ID_BITS) - 1;
    public const long MAX_SEQUENCE = (2L << SEQUENCE_BITS) - 1;
    public long WorkerId { get; }
    public DateTime StartTime { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="workerId"></param>
    /// <param name="startTime"></param>
    public SnowflakeGenerator(long workerId, DateTime startTime)
    {
        if (workerId < 0 || workerId > MAX_WORKER_ID) throw new ExceptionBase(ExceptionType.Parameter, 0, "机器ID超过上限");
        WorkerId = workerId;
        StartTime = startTime;
    }

    /// <summary>
    /// 雪花算法
    /// </summary>
    /// <returns></returns>
    public long GenerateId()
    {
        lock (_lock)
        {
            _currentSequence++;
            if (_currentSequence > MAX_SEQUENCE) Thread.Sleep(1);
            var timestamp = StartTime.GetTimeDiff(DateTime.Now);
            if (timestamp > MAX_TIMESTAMP) throw new ExceptionBase(ExceptionType.Internal, 0, "时间戳容量不足,请调整StartTime");
            if (timestamp < _currentTimestamp) throw new ExceptionBase(ExceptionType.Internal, 0, "时间获取异常,请检查服务器时间");
            else if (timestamp > _currentTimestamp)
            {
                _currentTimestamp = (long)timestamp;
                _currentSequence = 0;
            }
            return _currentTimestamp << (WORKER_ID_BITS + SEQUENCE_BITS) | WorkerId << SEQUENCE_BITS | _currentSequence;
        }
    }

    /// <summary>
    /// 解析ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SnowflakeIdInfo DecodeId(long id) => new SnowflakeIdInfo(id, StartTime);
}