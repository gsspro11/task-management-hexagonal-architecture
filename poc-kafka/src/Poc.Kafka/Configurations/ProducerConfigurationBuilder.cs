using Poc.Kafka.Configs;
using Poc.Kafka.Configs.Validators;

namespace Poc.Kafka.Configurations;

internal sealed class ProducerConfigurationBuilder<TKey, TValue> : 
    ConfigurationBuilderBase, IProducerConfigurationBuilder<TKey, TValue>
{
    private readonly PocKafkaProducerConfig _config = new();
    private PocKafkaSerializersConfig<TKey, TValue>? _serializersConfig;

    internal ProducerConfigurationBuilder(
        string bootstrapServers,
        string? username = null,
        string? password = null) : base(
            bootstrapServers,
            username,
            password)
    { }

    public IProducerConfigurationBuilder<TKey, TValue> Configure(
        Action<IPocKafkaProducerConfig> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);

        configureAction(_config);

        SetBrokerConfig(_config);

        PocKafkaProducerConfigValidator.Validate(_config);

        return this;
    }

    public IProducerConfigurationBuilder<TKey, TValue> WithSerialization(
        Action<ISerializationConfigurationBuilder<TKey, TValue>> configureAction)
    {
        var serializationConfigBuilder = new SerializationConfigurationBuilder<TKey, TValue>();
        configureAction(serializationConfigBuilder);

        _serializersConfig = serializationConfigBuilder.Build();
        return this;
    }

    internal ProducerConfiguration<TKey, TValue> Build() => new()
    {
        ProducerConfig = _config,
        SerializersConfig = _serializersConfig,
    };
}