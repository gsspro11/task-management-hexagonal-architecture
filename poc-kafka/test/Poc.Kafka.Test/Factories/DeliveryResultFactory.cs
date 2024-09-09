using Confluent.Kafka;

namespace Poc.Kafka.Test.Factories;

internal static class DeliveryResultFactory<TKey, TValue>
{
    public static DeliveryResult<TKey, TValue> CreateDeliveryResult(string topic, TKey key, TValue value) => new()
    {
        TopicPartitionOffset = new TopicPartitionOffset(new TopicPartition(topic, new Partition(0)), new Offset(1)),
        Message = new Message<TKey, TValue> { Key = key, Value = value }
    };
}
