
using Poc.Kafka.Configs;

namespace Poc.Kafka.Configurations;

internal abstract class ConfigurationBuilderBase(
    string bootstrapServers,
    string? username = null,
    string? password = null)
{
    private readonly string _bootstrapServers = bootstrapServers;
    private readonly string? _username = username;
    private readonly string? _password = password;

    protected void SetBrokerConfig(PocKafkaConfigBase config)
    {
        config.SetBootstrapServers(_bootstrapServers);

        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_password))
            config.SetCredentials(_username, _password);
    }
}
