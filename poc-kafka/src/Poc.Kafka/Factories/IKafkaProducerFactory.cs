using Poc.Kafka.Configurations;
using Confluent.Kafka;

namespace Poc.Kafka.Factories;

internal interface IKafkaProducerFactory
{
    IProducer<TKey, TValue> CreateProducer<TKey, TValue>(
        IProducerConfiguration<TKey, TValue> producerConfiguration);
}
