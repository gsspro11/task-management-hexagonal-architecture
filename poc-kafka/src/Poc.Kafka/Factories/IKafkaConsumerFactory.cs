using Poc.Kafka.Configurations;
using Confluent.Kafka;

namespace Poc.Kafka.Factories;

internal interface IKafkaConsumerFactory
{
    IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(IConsumerConfiguration<TKey, TValue> consumerConfiguration);
}
