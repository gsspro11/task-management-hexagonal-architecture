using Poc.Kafka.Configurations;

namespace Poc.Kafka.Configurators;

internal interface IConsumerConfigurator<TKey, TValue>
{
    void RegisterConsumer(IConsumerConfiguration<TKey, TValue> consumerConfiguration);
}
