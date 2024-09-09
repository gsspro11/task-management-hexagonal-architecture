using Poc.Kafka.Configs;

namespace Poc.Kafka.Configurations;

internal interface IConsumerConfiguration<TKey, TValue>
{
    PocKafkaConsumerConfig ConsumerConfig { get; }
    PocKafkaSerializersConfig<TKey, TValue>? SerializersConfig { get; }
}
