using Poc.Kafka.Common;
using Poc.Kafka.Common.Serdes;
using Poc.Kafka.Configs;
using Poc.Kafka.Configurations;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Factories;

[ExcludeFromCodeCoverage]
internal sealed class KafkaConsumerFactory(ILogger<IPocKafkaPubSub> logger) : IKafkaConsumerFactory
{
    private readonly ILogger<IPocKafkaPubSub> _logger = logger;

    public IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>(IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(consumerConfiguration);
        ArgumentNullException.ThrowIfNull(consumerConfiguration.ConsumerConfig);
        
        var consumerConfig = CreateConsumerConfig(consumerConfiguration.ConsumerConfig);

        var builder = new ConsumerBuilder<TKey, TValue>(consumerConfig)
                        .SetErrorHandler((consumer, error) => _logger.LogError("{Error}", error));

        ConfigureSerializers(builder, consumerConfiguration.SerializersConfig);

        var consumer = builder.Build();
        _logger.LogInformation("Consumer created!");

        return consumer;
    }

    private static ConsumerConfig CreateConsumerConfig(PocKafkaConsumerConfig config)
    {
        var consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = config.BootstrapServers,
            ApiVersionRequest = config.ApiVersionRequest,
            GroupId = config.GroupId,
            EnableAutoCommit = config.EnableAutoCommit,
            EnablePartitionEof = config.EnablePartitionEof
        };

        ApplyOptionalConsumerConfig(config, consumerConfig);

        return consumerConfig;
    }

    private static void ApplyOptionalConsumerConfig(PocKafkaConsumerConfig config, ConsumerConfig consumerConfig)
    {
        if (config.StatisticsIntervalMs.HasValue)
            consumerConfig.StatisticsIntervalMs = config.StatisticsIntervalMs;

        if (config.SessionTimeoutMs.HasValue)
            consumerConfig.SessionTimeoutMs = config.SessionTimeoutMs;

        if (config.HeartbeatIntervalMs.HasValue)
            consumerConfig.HeartbeatIntervalMs = config.HeartbeatIntervalMs;

        if (config.MaxPollIntervalMs.HasValue)
            consumerConfig.MaxPollIntervalMs = config.MaxPollIntervalMs;

        if (config.MaxPollIntervalMs.HasValue)
            consumerConfig.MaxPollIntervalMs = config.MaxPollIntervalMs;

        if (config.AutoOffsetReset.HasValue)
            consumerConfig.AutoOffsetReset = config.AutoOffsetReset;

        if (config.FetchWaitMaxMs.HasValue)
            consumerConfig.FetchWaitMaxMs = config.FetchWaitMaxMs;

        if (config.FetchMinBytes.HasValue)
            consumerConfig.FetchMinBytes = config.FetchMinBytes;

        if (config.FetchMaxBytes.HasValue)
            consumerConfig.FetchMaxBytes = config.FetchMaxBytes;

        if (config.MaxPartitionFetchBytes.HasValue)
            consumerConfig.MaxPartitionFetchBytes = config.MaxPartitionFetchBytes;

        if (config.PartitionAssignmentStrategy.HasValue)
            consumerConfig.PartitionAssignmentStrategy = config.PartitionAssignmentStrategy;

        if (config.IsCredentialsProvided)
            ConfigureSecurity(consumerConfig, config);
    }

    private static void ConfigureSecurity(ConsumerConfig consumerConfig, PocKafkaConsumerConfig config)
    {
        consumerConfig.SaslMechanism = SaslMechanism.ScramSha512;
        consumerConfig.SaslUsername = config.Username;
        consumerConfig.SaslPassword = config.Password;
        consumerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
    }

    private void ConfigureSerializers<TKey, TValue>(
        ConsumerBuilder<TKey, TValue> builder,
        PocKafkaSerializersConfig<TKey, TValue>? serializersConfig = null)
    {
        builder
            .SetKeyDeserializer(serializersConfig?.KeyDeserializer ?? new JsonDeserializer<TKey>(_logger))
            .SetValueDeserializer(serializersConfig?.ValueDeserializer ?? new JsonDeserializer<TValue>(_logger));
    }
}
