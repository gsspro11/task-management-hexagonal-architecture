using Poc.Kafka.Configs;

namespace Poc.Kafka.Configurations;

internal interface IProducerConfiguration<TKey, TValue>
{
    PocKafkaProducerConfig ProducerConfig { get; }
    PocKafkaSerializersConfig<TKey, TValue>? SerializersConfig { get; }
}