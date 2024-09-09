using Poc.Kafka.Results;
using Confluent.Kafka;

namespace Poc.Kafka.Managers;

internal interface IConsumerManagerCore<TKey, TValue> : IAsyncDisposable
{
    void ConfigureTopicsSubscription();
    void Subscribe(IEnumerable<string> topics);
    void Seek(TopicPartitionOffset topicPartitionOffset);
    ConsumeResult<TKey, TValue> ConsumeMessage(CancellationToken cancellationToken);
    Task ProcessMessageAsync(
       ConsumeResult<TKey, TValue> consumeResult,
       Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
       CancellationToken cancellationToken);
}
