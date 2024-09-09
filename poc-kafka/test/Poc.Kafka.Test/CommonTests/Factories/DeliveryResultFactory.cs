using Confluent.Kafka;

namespace Poc.Kafka.Test.CommonTests.Factories;

internal static class DeliveryResultFactory<TKey, TValue>
{
    public static DeliveryResult<TKey, TValue> CreateDeliveryResult(string topic, Message<TKey, TValue> message) => new()
    {
        Status = PersistenceStatus.Persisted,
        TopicPartitionOffset = new TopicPartitionOffset(new TopicPartition(topic, new Partition(0)), new Offset(1)),
        Message = message
    };
}
