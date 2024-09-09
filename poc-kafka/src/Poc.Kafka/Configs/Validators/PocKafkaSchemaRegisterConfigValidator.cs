namespace Poc.Kafka.Configs.Validators;

internal static class PocKafkaSchemaRegisterConfigValidator
{
    internal static void Validate(PocKafkaSchemaRegistryConfig schemaRegistryConfig)
    {
        ArgumentNullException.ThrowIfNull(schemaRegistryConfig);
        ArgumentNullException.ThrowIfNull(schemaRegistryConfig.Url);

        PocKafkaCredentialsConfigValidator.Validate(schemaRegistryConfig);
    }
}