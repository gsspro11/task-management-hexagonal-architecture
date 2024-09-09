using Poc.Kafka.Configurations;

namespace Poc.Kafka.Configurators;
internal interface IProducerConfigurator<TKey, TValue>
{
    void RegisterProducer(IProducerConfiguration<TKey, TValue> producerConfiguration);
}
