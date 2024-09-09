using Poc.Kafka.Results;

namespace Poc.Kafka.Managers;

internal interface IConsumerManager<TKey, TValue> : IAsyncDisposable
{
    Task InitiateConsumeAsync(
         Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
         CancellationToken cancellationToken);
}
