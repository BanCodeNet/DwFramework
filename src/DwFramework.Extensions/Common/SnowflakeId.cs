namespace DwFramework.Extensions;

public record struct SnowflakeId
{
    public DateTime StartTime { get; init; }
    public DateTime GenerateTime { get; init; }
    public long WorkerId { get; init; }
    public long DataCenterId { get; init; }
    public long Sequence { get; init; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="timestampBits"></param>
    /// <param name="workerIdBits"></param>
    /// <param name="dataCenterIdBits"></param>
    /// <param name="sequenceBits"></param>
    /// <param name="startTime"></param>
    /// <param name="id"></param>
    public SnowflakeId(
        int timestampBits,
        int workerIdBits,
        int dataCenterIdBits,
        int sequenceBits,
        DateTime startTime,
        long id
    )
    {
        StartTime = startTime;
        var timestamp = id >> (workerIdBits + dataCenterIdBits + sequenceBits);
        GenerateTime = startTime.Add(TimeSpan.FromMilliseconds(timestamp));
        WorkerId = (id ^ (timestamp << (workerIdBits + dataCenterIdBits + sequenceBits))) >> (dataCenterIdBits + sequenceBits);
        DataCenterId = (id ^ (timestamp << (workerIdBits + dataCenterIdBits + sequenceBits)) ^ (WorkerId << (dataCenterIdBits + sequenceBits))) >> sequenceBits;
        Sequence = id ^ (timestamp << (workerIdBits + dataCenterIdBits + sequenceBits)) ^ (WorkerId << (dataCenterIdBits + sequenceBits)) ^ (DataCenterId << sequenceBits);
    }
}