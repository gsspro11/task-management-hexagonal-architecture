using Poc.Kafka.Results;

namespace Poc.Kafka.Managers;

internal interface IRetryConsumerManager<TKey, TValue> : IAsyncDisposable
{
    Task InitiateConsumeAsync(
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken);
}
