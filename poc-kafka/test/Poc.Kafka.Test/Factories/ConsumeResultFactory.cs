using Confluent.Kafka;

namespace Poc.Kafka.Test.Factories;

internal static class ConsumeResultFactory<TKey, TValue>
{
    public static ConsumeResult<TKey, TValue> CreateConsumeResult(
        string topic,
        bool isPartitionEOF = false,
        Message<TKey, TValue>? message = null) => new()
        {
            IsPartitionEOF = isPartitionEOF,
            TopicPartitionOffset = new TopicPartitionOffset(topic, new Partition(1), new Offset(1)),
            Message = message
        };
}

