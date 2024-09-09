using Confluent.Kafka;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Factories;

[ExcludeFromCodeCoverage]
internal static class KafkaTopicPartitionOffsetFactory
{
    internal static TopicPartitionOffset Create<TKey, TValue>(ConsumeResult<TKey, TValue> consumeResult) =>
        new(tp: consumeResult.TopicPartition, offset: consumeResult.Offset);
}
