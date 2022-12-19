namespace DwFramework.Extensions;

public sealed class SnowflakeGenerator
{
    public static int TIMESTAMP_BITS { get; } = 41;
    public static int WORKER_ID_BITS { get; } = 5;
    public static int DATA_CENTER_ID_BITS { get; } = 5;
    public static int SEQUENCE_BITS { get; } = 12;
    public static long MAX_TIMESTAMP { get; } = 2L << TIMESTAMP_BITS;
    public static long MAX_WORKER_ID { get; } = (2L << WORKER_ID_BITS) - 1;
    public static long MAX_DATA_CENTER_ID { get; } = (2L << DATA_CENTER_ID_BITS) - 1;
    public static long MAX_SEQUENCE { get; } = (2L << SEQUENCE_BITS) - 1;
    public long WorkerId { get; init; }
    public long DataCenterId { get; init; }
    public DateTime StartTime { get; init; }

    private static object _lock { get; } = new object();
    private long _lastTimestamp { get; set; }
    private long _lastSequence { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="workerId"></param>
    /// <param name="dataCenterId"></param>
    public SnowflakeGenerator(DateTime startTime, long workerId, long dataCenterId)
    {
        if (workerId < 0 || workerId > MAX_WORKER_ID)
            throw new ArgumentException("invalid work id");
        if (dataCenterId < 0 || dataCenterId > MAX_DATA_CENTER_ID)
            throw new ArgumentException("invalid data center id");
        StartTime = startTime;
        _lastTimestamp = (long)StartTime.GetTimeDifference(DateTime.Now);
        WorkerId = workerId;
        DataCenterId = dataCenterId;
    }

    /// <summary>
    /// 雪花算法
    /// </summary>
    /// <returns></returns>
    public long GenerateId()
    {
        lock (_lock)
        {
            var currentTimestamp = (long)StartTime.GetTimeDifference(DateTime.Now, true);
            if (currentTimestamp > MAX_TIMESTAMP)
                throw new SystemException("the capacity of timestamp has be undercapacity");
            else if (currentTimestamp > _lastTimestamp)
            {
                _lastTimestamp = currentTimestamp;
                _lastSequence = 0;
            }
            _lastSequence++;
            if (_lastSequence > MAX_SEQUENCE) return GenerateId();
            return _lastTimestamp << (WORKER_ID_BITS + DATA_CENTER_ID_BITS + SEQUENCE_BITS)
                | WorkerId << (DATA_CENTER_ID_BITS + SEQUENCE_BITS)
                | DataCenterId << SEQUENCE_BITS
                | _lastSequence;
        }
    }

    /// <summary>
    /// 解析ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SnowflakeId DecodeId(long id)
    {
        return new SnowflakeId(
            TIMESTAMP_BITS,
            WORKER_ID_BITS,
            DATA_CENTER_ID_BITS,
            SEQUENCE_BITS,
            StartTime,
            id
        );
    }
}