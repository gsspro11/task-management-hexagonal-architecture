namespace Poc.Kafka.Configs.Validators;

internal static class PocKafkaCredentialsConfigValidator
{
    internal static void Validate(PocKafkaCredentialsConfig cretentialsConfig)
    {
        if (cretentialsConfig.IsCredentialsProvided &&
            (string.IsNullOrWhiteSpace(cretentialsConfig.Username) || string.IsNullOrWhiteSpace(cretentialsConfig.Password)))
            throw new ArgumentException("Username and Password are required.");
    }
}
