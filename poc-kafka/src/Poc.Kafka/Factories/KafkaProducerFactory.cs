using Poc.Kafka.Common;
using Poc.Kafka.Common.Serdes;
using Poc.Kafka.Configs;
using Poc.Kafka.Configurations;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Factories;


[ExcludeFromCodeCoverage]
internal sealed class KafkaProducerFactory(ILogger<IPocKafkaPubSub> logger) : IKafkaProducerFactory
{
    private readonly ILogger<IPocKafkaPubSub> _logger = logger;

    public IProducer<TKey, TValue> CreateProducer<TKey, TValue>(IProducerConfiguration<TKey, TValue> producerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(producerConfiguration);
        ArgumentNullException.ThrowIfNull(producerConfiguration.ProducerConfig);

        var producerConfig = CreateProducerConfig(producerConfiguration.ProducerConfig);

        var builder = new ProducerBuilder<TKey, TValue>(producerConfig)
                .SetErrorHandler((_, error) => _logger.LogError("{Error}", error));

        ConfigureSerializers(builder, producerConfiguration.SerializersConfig);

        var producer = builder.Build();
        _logger.LogInformation("Producer created!");

        return producer;
    }

    private static ProducerConfig CreateProducerConfig(PocKafkaProducerConfig config)
    {
        var producerConfig = new ProducerConfig()
        {
            BootstrapServers = config.BootstrapServers,
            EnableIdempotence = config.EnableIdempotence,
            ApiVersionRequest = config.ApiVersionRequest,
        };

        ApplyOptionalProducerConfig(config, producerConfig);

        return producerConfig;
    }

    private static void ApplyOptionalProducerConfig(PocKafkaProducerConfig config, ProducerConfig producerConfig)
    {
        if (config.Acks.HasValue)
            producerConfig.Acks = config.Acks;

        if (config.MaxInFlight.HasValue)
            producerConfig.MaxInFlight = config.MaxInFlight;

        if (config.MessageSendMaxRetries.HasValue)
            producerConfig.MessageSendMaxRetries = config.MessageSendMaxRetries;

        if (!string.IsNullOrWhiteSpace(config.TransactionalId))
            producerConfig.TransactionalId = config.TransactionalId;

        if (config.TransactionTimeoutMs.HasValue)
            producerConfig.TransactionTimeoutMs = config.TransactionTimeoutMs;

        if (config.BatchSize.HasValue)
            producerConfig.BatchSize = config.BatchSize;

        if (config.BatchNumMessages.HasValue)
            producerConfig.BatchNumMessages = config.BatchNumMessages;

        if (config.LingerMs.HasValue)
            producerConfig.LingerMs = config.LingerMs;


        if (config.IsCredentialsProvided)
            ConfigureSecurity(producerConfig, config);
    }

    private static void ConfigureSecurity(ProducerConfig producerConfig, PocKafkaProducerConfig config)
    {
        producerConfig.SaslMechanism = SaslMechanism.ScramSha512;
        producerConfig.SaslUsername = config.Username;
        producerConfig.SaslPassword = config.Password;
        producerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
    }

    private static void ConfigureSerializers<TKey, TValue>(
        ProducerBuilder<TKey, TValue> builder,
        PocKafkaSerializersConfig<TKey, TValue>? serializersConfig = null)
    {
        if (serializersConfig?.KeyAsyncSerializer is not null)
            builder.SetKeySerializer(serializersConfig.KeyAsyncSerializer);
        else
            builder.SetKeySerializer(serializersConfig?.KeySerializer ?? new JsonSerializer<TKey>());

        if (serializersConfig?.ValueAsyncSerializer is not null)
            builder.SetValueSerializer(serializersConfig.ValueAsyncSerializer);
        else
            builder.SetValueSerializer(serializersConfig?.ValueSerializer ?? new JsonSerializer<TValue>());
    }
}
