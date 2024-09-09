namespace Poc.Kafka.Test.CommonTests.Fakes;

public record AvroRecordFake
{
    public Guid Id { get; init; }
    public string? Type { get; init; }
    public DateTimeOffset Time { get; init; }

    public static AvroRecordFake Create() => new()
    {
        Id = Guid.NewGuid(),
        Type = "OrderCreated",
        Time = new DateTimeOffset(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc))
    };
}
