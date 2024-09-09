namespace Poc.Kafka.Providers;

internal interface ITimeProvider
{
    DateTimeOffset Now { get; }
    DateTimeOffset UtcNow { get; }
}
