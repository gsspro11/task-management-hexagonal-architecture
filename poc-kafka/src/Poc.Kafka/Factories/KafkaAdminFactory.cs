using Poc.Kafka.Configs;
using Confluent.Kafka;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Factories;

[ExcludeFromCodeCoverage]
internal static class KafkaAdminFactory
{
    internal static IAdminClient CreateAdminClient(PocKafkaAdminClientConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var adminClientConfig = new AdminClientConfig
        {
            ClientId = "",
            BootstrapServers = config.BootstrapServers,
        };

        if (config.IsCredentialsProvided)
        {
            adminClientConfig.SaslMechanism = SaslMechanism.ScramSha512;
            adminClientConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
            adminClientConfig.SaslUsername = config.Username;
            adminClientConfig.SaslPassword = config.Password;
        }

        return new AdminClientBuilder(adminClientConfig).Build();
    }
}
