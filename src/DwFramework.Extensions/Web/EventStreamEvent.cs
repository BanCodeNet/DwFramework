namespace DwFramework.Extensions;

public record class EventStreamEvent
{
    public string Id { get; init; }
    public string Event { get; init; }
    public int? Retry { get; init; }
    public IAsyncEnumerable<object> Data { get; init; }
}