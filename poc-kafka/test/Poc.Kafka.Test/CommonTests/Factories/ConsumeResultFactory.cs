using Confluent.Kafka;

namespace Poc.Kafka.Test.CommonTests.Factories;

internal static class ConsumeResultFactory<TKey, TValue>
{
    public static ConsumeResult<TKey, TValue> CreateConsumeResult(
        string topic,
        Message<TKey, TValue> message = default!,
        bool isPartitionEof = false) => new()
        {
            IsPartitionEOF = isPartitionEof,
            TopicPartitionOffset = new TopicPartitionOffset(topic, new Partition(1), new Offset(1)),
            Message = message
        };
}

