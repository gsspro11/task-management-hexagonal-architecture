using Poc.Kafka.Configs;
using Confluent.SchemaRegistry;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Factories;

[ExcludeFromCodeCoverage]
internal static class KafkaSchemaRegistryFactory
{
    internal static ISchemaRegistryClient Create(PocKafkaSchemaRegistryConfig config)
    {
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = config.Url,
        };

        if (config.IsCredentialsProvided)
            schemaRegistryConfig.BasicAuthUserInfo = $"{config.Username}:{config.Password}";

        return new CachedSchemaRegistryClient(schemaRegistryConfig);
    }
}