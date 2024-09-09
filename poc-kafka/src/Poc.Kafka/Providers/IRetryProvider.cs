using Confluent.Kafka;

namespace Poc.Kafka.Providers;

internal interface IRetryProvider
{
    bool IsRetryDelayExpired(Headers? headers = null);
    Task WaitBeforeNextRetryAsync(CancellationToken cancellationToken);
}
