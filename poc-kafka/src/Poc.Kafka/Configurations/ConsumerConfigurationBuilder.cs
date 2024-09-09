using Poc.Kafka.Configs;
using Poc.Kafka.Configs.Validators;

namespace Poc.Kafka.Configurations;

internal sealed class ConsumerConfigurationBuilder<TKey, TValue> : ConfigurationBuilderBase, IConsumerConfigurationBuilder<TKey, TValue>
{
    private readonly PocKafkaConsumerConfig _config = new();
    private PocKafkaSerializersConfig<TKey, TValue>? _serializersConfig;
    internal ConsumerConfigurationBuilder(
        string bootstrapServers,
        string? username = null,
        string? password = null) : base(
            bootstrapServers,
            username,
            password)
    { }

    public IConsumerConfigurationBuilder<TKey, TValue> Configure(
        Action<IPocKafkaConsumerConfig> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);

        configureAction(_config);

        SetBrokerConfig(_config);

        PocKafkaConsumerConfigValidator.Validate(_config);

        return this;
    }

    public IConsumerConfigurationBuilder<TKey, TValue> WithSerialization(
        Action<IDeserializationConfigurationBuilder<TKey, TValue>> configureAction)
    {
        var serializationConfigBuilder = new SerializationConfigurationBuilder<TKey, TValue>();
        configureAction(serializationConfigBuilder);

        _serializersConfig = serializationConfigBuilder.Build();
        return this;
    }

    internal ConsumerConfiguration<TKey, TValue> Build() => new()
    {
        ConsumerConfig = _config,
        SerializersConfig = _serializersConfig!,
    };
}