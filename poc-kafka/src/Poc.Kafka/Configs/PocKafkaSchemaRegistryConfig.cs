namespace Poc.Kafka.Configs;

internal sealed class PocKafkaSchemaRegistryConfig : PocKafkaCredentialsConfig, IPocKafkaSchemaRegistryConfig
{
    public string? Url { get; private set; }
    internal bool EnableUseBrokerCredentials { get; private set; } = false;
    public void SetUrl(string url) =>
        Url = url;
    public void UseBrokerCredentials() =>
        EnableUseBrokerCredentials = true;
}
