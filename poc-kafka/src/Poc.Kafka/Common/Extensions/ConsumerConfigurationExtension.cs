using Poc.Kafka.Configs;
using Poc.Kafka.Configurations;

namespace Poc.Kafka.Common.Extensions;

internal static class ConsumerConfigurationExtension
{
    internal static IProducerConfiguration<TKey, TValue> ToProducerConfiguration<TKey, TValue>(
        this IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        return new ProducerConfiguration<TKey, TValue>
        {
            ProducerConfig = ConvertConsumerConfigToProducerConfig(consumerConfiguration.ConsumerConfig),
            SerializersConfig = consumerConfiguration.SerializersConfig,
        };
    }

    private static PocKafkaProducerConfig ConvertConsumerConfigToProducerConfig(PocKafkaConsumerConfig consumerConfig)
    {
        var producerConfig = new PocKafkaProducerConfig();
        producerConfig.SetBootstrapServers(consumerConfig.BootstrapServers!);
        producerConfig.SetIdempotenceEnabled();

        if (consumerConfig.IsCredentialsProvided)
            producerConfig.SetCredentials(consumerConfig.Username!, consumerConfig.Password!);

        return producerConfig;
    }
}